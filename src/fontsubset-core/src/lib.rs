mod harfbuzz;

use std::{
    collections::BTreeSet,
    fmt, fs,
    path::{Path, PathBuf},
};

use anyhow::{Context, Result, bail};
use harfbuzz::HarfBuzz;
use regex::Regex;
use ttf_parser::{Face, OutlineBuilder};
use walkdir::WalkDir;

pub const DEFAULT_FILE_REGEX: &str = r"(?i)\.(txt|lua|asset)$";

#[derive(Clone, Debug)]
pub struct SubsetRequest {
    pub input: PathBuf,
    pub output: PathBuf,
    pub chars_dir: Option<PathBuf>,
    pub file_regex: String,
    pub literal_text: Option<String>,
    pub retain_ascii: bool,
    pub strip_hints: bool,
    pub drop_layout: bool,
}

#[derive(Clone, Debug)]
pub struct SubsetResult {
    pub outline: OutlineKind,
    pub engine: String,
    pub matched_files: usize,
    pub requested_characters: usize,
    pub supported_characters: usize,
    pub original_glyphs: u16,
    pub subset_glyphs: u16,
    pub original_size: usize,
    pub subset_size: usize,
    pub output: PathBuf,
}

impl SubsetResult {
    pub fn reduction_percent(&self) -> f64 {
        reduction_percent(self.original_size, self.subset_size)
    }
}

#[derive(Clone, Copy, Debug, Eq, PartialEq)]
pub enum OutlineKind {
    TrueType,
    Cff,
    Cff2,
}

impl OutlineKind {
    fn from_face(face: &Face<'_>) -> Result<Self> {
        let tables = face.tables();
        if tables.glyf.is_some() {
            Ok(Self::TrueType)
        } else if tables.cff.is_some() {
            Ok(Self::Cff)
        } else if tables.cff2.is_some() {
            Ok(Self::Cff2)
        } else {
            bail!(
                "unsupported outline format: expected TrueType glyf, OpenType CFF, or OpenType CFF2"
            )
        }
    }

    pub fn label(self) -> &'static str {
        match self {
            Self::TrueType => "TrueType (glyf)",
            Self::Cff => "PostScript (CFF)",
            Self::Cff2 => "PostScript variable (CFF2)",
        }
    }
}

impl fmt::Display for OutlineKind {
    fn fmt(&self, formatter: &mut fmt::Formatter<'_>) -> fmt::Result {
        formatter.write_str(self.label())
    }
}

#[derive(Debug)]
struct CollectedCharacters {
    characters: BTreeSet<char>,
    matched_files: usize,
}

pub fn subset(request: &SubsetRequest) -> Result<SubsetResult> {
    validate_font_extension(&request.input, "input")?;
    validate_font_extension(&request.output, "output")?;

    let collected = collect_characters(request)?;
    if collected.characters.is_empty() {
        bail!(
            "the retained character set is empty; select a character directory, enter text, or retain ASCII"
        );
    }

    let input_bytes = fs::read(&request.input)
        .with_context(|| format!("failed to read input font {}", request.input.display()))?;
    let input_face = parse_face(&input_bytes, "input font")?;
    let input_outline = OutlineKind::from_face(&input_face)?;
    let original_glyphs = input_face.number_of_glyphs();
    let expected_characters: BTreeSet<char> = collected
        .characters
        .iter()
        .copied()
        .filter(|character| input_face.glyph_index(*character).is_some())
        .collect();

    if expected_characters.is_empty() {
        bail!("none of the requested characters are present in the input font");
    }

    let harfbuzz = HarfBuzz::new();
    let output_bytes = harfbuzz
        .subset_font(
            &input_bytes,
            collected.characters.iter().copied(),
            request.strip_hints,
            request.drop_layout,
        )
        .context("font subsetting failed")?;

    let output_face = parse_face(&output_bytes, "subsetter output")?;
    let output_outline = OutlineKind::from_face(&output_face)?;
    if output_outline != input_outline {
        bail!(
            "outline format changed unexpectedly from {} to {}",
            input_outline,
            output_outline
        );
    }

    validate_cmap(&output_face, &expected_characters)?;
    validate_outlines(&input_face, &output_face, &expected_characters)?;

    if let Some(parent) = request
        .output
        .parent()
        .filter(|path| !path.as_os_str().is_empty())
    {
        fs::create_dir_all(parent)
            .with_context(|| format!("failed to create output directory {}", parent.display()))?;
    }
    fs::write(&request.output, &output_bytes)
        .with_context(|| format!("failed to write output font {}", request.output.display()))?;

    Ok(SubsetResult {
        outline: input_outline,
        engine: harfbuzz.display_name(),
        matched_files: collected.matched_files,
        requested_characters: collected.characters.len(),
        supported_characters: expected_characters.len(),
        original_glyphs,
        subset_glyphs: output_face.number_of_glyphs(),
        original_size: input_bytes.len(),
        subset_size: output_bytes.len(),
        output: request.output.clone(),
    })
}

fn parse_face<'a>(bytes: &'a [u8], description: &str) -> Result<Face<'a>> {
    Face::parse(bytes, 0)
        .map_err(|error| anyhow::anyhow!("failed to parse {description}: {error:?}"))
}

fn collect_characters(request: &SubsetRequest) -> Result<CollectedCharacters> {
    let mut characters = BTreeSet::new();
    let mut matched_files = 0;

    if let Some(text) = &request.literal_text {
        characters.extend(text.chars());
    }
    if request.retain_ascii {
        characters.extend('\u{20}'..='\u{7e}');
    }

    if let Some(directory) = &request.chars_dir {
        if !directory.is_dir() {
            bail!(
                "character source directory does not exist: {}",
                directory.display()
            );
        }

        let regex = Regex::new(&request.file_regex)
            .with_context(|| format!("invalid file regex {:?}", request.file_regex))?;
        for entry in WalkDir::new(directory).sort_by_file_name() {
            let entry = entry.with_context(|| {
                format!(
                    "failed while walking character directory {}",
                    directory.display()
                )
            })?;
            if !entry.file_type().is_file()
                || !regex.is_match(entry.path().to_string_lossy().as_ref())
            {
                continue;
            }

            let text = fs::read_to_string(entry.path()).with_context(|| {
                format!(
                    "failed to read character source {} as UTF-8",
                    entry.path().display()
                )
            })?;
            characters.extend(text.chars());
            matched_files += 1;
        }
    }

    Ok(CollectedCharacters {
        characters,
        matched_files,
    })
}

fn validate_cmap(face: &Face<'_>, expected: &BTreeSet<char>) -> Result<()> {
    let actual = unicode_cmap_characters(face);
    if actual == *expected {
        return Ok(());
    }

    let missing = expected.difference(&actual).count();
    let unexpected = actual.difference(expected).count();
    bail!(
        "subset cmap validation failed: {missing} requested characters are missing and {unexpected} unexpected characters remain"
    )
}

fn unicode_cmap_characters(face: &Face<'_>) -> BTreeSet<char> {
    let mut characters = BTreeSet::new();
    let Some(cmap) = face.tables().cmap else {
        return characters;
    };

    for subtable in cmap
        .subtables
        .into_iter()
        .filter(|table| table.is_unicode())
    {
        subtable.codepoints(|codepoint| {
            if let Some(character) = char::from_u32(codepoint)
                && face.glyph_index(character).is_some()
            {
                characters.insert(character);
            }
        });
    }
    characters
}

fn validate_outlines(
    input: &Face<'_>,
    output: &Face<'_>,
    characters: &BTreeSet<char>,
) -> Result<()> {
    let missing = characters
        .iter()
        .filter(|character| has_outline(input, **character) && !has_outline(output, **character))
        .count();
    if missing != 0 {
        bail!("subset validation failed: {missing} glyph outlines were lost");
    }
    Ok(())
}

fn validate_font_extension(path: &Path, description: &str) -> Result<()> {
    let extension = path
        .extension()
        .and_then(|value| value.to_str())
        .unwrap_or_default();
    if !extension.eq_ignore_ascii_case("ttf") && !extension.eq_ignore_ascii_case("otf") {
        bail!(
            "{description} font must use a .ttf or .otf extension: {}",
            path.display()
        );
    }
    Ok(())
}

fn reduction_percent(original: usize, subset: usize) -> f64 {
    if original == 0 || subset >= original {
        0.0
    } else {
        (original - subset) as f64 * 100.0 / original as f64
    }
}

fn has_outline(face: &Face<'_>, character: char) -> bool {
    let Some(glyph_id) = face.glyph_index(character) else {
        return false;
    };
    face.outline_glyph(glyph_id, &mut OutlineProbe).is_some()
}

struct OutlineProbe;

impl OutlineBuilder for OutlineProbe {
    fn move_to(&mut self, _x: f32, _y: f32) {}
    fn line_to(&mut self, _x: f32, _y: f32) {}
    fn quad_to(&mut self, _x1: f32, _y1: f32, _x: f32, _y: f32) {}
    fn curve_to(&mut self, _x1: f32, _y1: f32, _x2: f32, _y2: f32, _x: f32, _y: f32) {}
    fn close(&mut self) {}
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn reduction_is_reported_as_a_percentage() {
        assert_eq!(reduction_percent(1000, 250), 75.0);
        assert_eq!(reduction_percent(1000, 1000), 0.0);
        assert_eq!(reduction_percent(0, 0), 0.0);
    }

    #[test]
    fn recognizes_supported_extensions_case_insensitively() {
        assert!(validate_font_extension(Path::new("font.ttf"), "input").is_ok());
        assert!(validate_font_extension(Path::new("font.OTF"), "input").is_ok());
        assert!(validate_font_extension(Path::new("font.pfb"), "input").is_err());
    }
}

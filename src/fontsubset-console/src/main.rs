use std::{path::PathBuf, process::ExitCode};

use anyhow::Result;
use clap::Parser;
use fontsubset_core::{DEFAULT_FILE_REGEX, SubsetRequest, subset};

#[derive(Debug, Parser)]
#[command(
    name = "fontsubset-console",
    version,
    about = "Subset TTF or OTF fonts with TrueType, CFF, or CFF2 outlines"
)]
struct Cli {
    /// Recursively collect characters from matching files in this directory.
    #[arg(
        short = 'c',
        long = "chars-dir",
        visible_alias = "charsfile",
        value_name = "DIR"
    )]
    chars_dir: Option<PathBuf>,

    /// Regex matched against each full file path under --chars-dir.
    #[arg(short = 'r', long = "regex", default_value = DEFAULT_FILE_REGEX)]
    file_regex: String,

    /// Add literal text directly to the retained character set.
    #[arg(long, value_name = "TEXT")]
    text: Option<String>,

    /// Retain visible ASCII characters U+0020 through U+007E.
    #[arg(short = 'a', long = "ascii")]
    retain_ascii: bool,

    /// Remove TrueType hint instructions and hint tables.
    #[arg(short = 's', long = "strip-hints", visible_alias = "strip")]
    strip_hints: bool,

    /// Drop GSUB, GPOS, and GDEF. This can break shaping and ligatures.
    #[arg(long)]
    drop_layout: bool,

    /// Source .ttf or .otf font.
    #[arg(value_name = "INPUT_FONT")]
    input: PathBuf,

    /// Destination .ttf or .otf font.
    #[arg(value_name = "OUTPUT_FONT")]
    output: PathBuf,
}

fn main() -> ExitCode {
    match run(Cli::parse()) {
        Ok(()) => ExitCode::SUCCESS,
        Err(error) => {
            eprintln!("Error: {error:#}");
            ExitCode::FAILURE
        }
    }
}

fn run(cli: Cli) -> Result<()> {
    let result = subset(&SubsetRequest {
        input: cli.input,
        output: cli.output,
        chars_dir: cli.chars_dir,
        file_regex: cli.file_regex,
        literal_text: cli.text,
        retain_ascii: cli.retain_ascii,
        strip_hints: cli.strip_hints,
        drop_layout: cli.drop_layout,
    })?;

    println!("Subset succeeded");
    println!("  Engine: {}", result.engine);
    println!("  Outline: {}", result.outline);
    println!("  Matched files: {}", result.matched_files);
    println!(
        "  Characters: {} requested, {} present in source",
        result.requested_characters, result.supported_characters
    );
    println!(
        "  Glyphs: {} -> {}",
        result.original_glyphs, result.subset_glyphs
    );
    println!(
        "  Size: {} -> {} bytes ({:.2}% smaller)",
        result.original_size,
        result.subset_size,
        result.reduction_percent()
    );
    println!("  Output: {}", result.output.display());

    Ok(())
}

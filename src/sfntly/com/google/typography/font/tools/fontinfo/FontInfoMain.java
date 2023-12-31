// Copyright 2012 Google Inc. All Rights Reserved.

namespace com.google.typography.font.tools.fontinfo;








/**
 * This is the main class for the command-line version of the font info tool
 *
 * @author Han-Wen Yeh
 *
 */
public class FontInfoMain {
  private static readonly String PROGRAM_NAME = "java -jar fontinfo.jar";

  public static void main(String[] args) {
    CommandOptions options = new CommandOptions();
    JCommander commander = null;
    try {
      commander = new JCommander(options, args);
    } catch (ParameterException e) {
      Debug.WriteLine(e.getMessage());
      commander = new JCommander(options, "--help");
    }

    // Display help
    if (options.help) {
      commander.setProgramName(PROGRAM_NAME);
      commander.usage();
      return;
    }

    // No font loaded
    if (options.files.size() != 1) {
      Debug.WriteLine(
          "Please specify a single font. Try '" + PROGRAM_NAME + " --help' for more information.");
      return;
    }

    // Default option
    if (!(options.metrics || options.general || options.cmap || options.chars || options.blocks
        || options.scripts || options.glyphs || options.all)) {
      options.general = true;
    }

    // Obtain file name
    String fileName = options.files.get(0);

    // Load font
    Font[] fonts = null;
    try {
      fonts = FontUtils.getFonts(fileName);
    } catch (IOException e) {
      Debug.WriteLine("Unable to load font " + fileName);
      return;
    }

    for (int i = 0; i < fonts.length; i++) {
      Font font = fonts[i];

      if (fonts.length > 1 && !options.csv) {
        Debug.WriteLine("==== Information for font index " + i + " ====\n");
      }

      // Print general information
      if (options.general || options.all) {
        if (options.csv) {
          Debug.WriteLine(String.Format("sfnt version: %s", FontInfo.sfntVersion(font)));
          Debug.WriteLine();
          Debug.WriteLine("Font Tables");
          Debug.WriteLine(
              prependDataAndBuildCsv(FontInfo.listTables(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
          Debug.WriteLine("Name Table Entries:");
          Debug.WriteLine(
              prependDataAndBuildCsv(FontInfo.listNameEntries(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine(String.Format("sfnt version: %s", FontInfo.sfntVersion(font)));
          Debug.WriteLine();
          Debug.WriteLine("Font Tables:");
          FontInfo.listTables(font).prettyPrint();
          Debug.WriteLine();
          Debug.WriteLine("Name Table Entries:");
          FontInfo.listNameEntries(font).prettyPrint();
          Debug.WriteLine();
        }
      }

      // Print metrics
      if (options.metrics || options.all) {
        if (options.csv) {
          Debug.WriteLine("Font Metrics:");
          Debug.WriteLine(
              prependDataAndBuildCsv(FontInfo.listFontMetrics(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine("Font Metrics:");
          FontInfo.listFontMetrics(font).prettyPrint();
          Debug.WriteLine();
        }
      }

      // Print glyph metrics
      if (options.metrics || options.glyphs || options.all) {
        if (options.csv) {
          Debug.WriteLine("Glyph Metrics:");
          Debug.WriteLine(prependDataAndBuildCsv(
              FontInfo.listGlyphDimensionBounds(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine("Glyph Metrics:");
          FontInfo.listGlyphDimensionBounds(font).prettyPrint();
          Debug.WriteLine();
        }
      }

      // Print cmap list
      if (options.cmap || options.all) {
        if (options.csv) {
          Debug.WriteLine("Cmaps in the font:");
          Debug.WriteLine(
              prependDataAndBuildCsv(FontInfo.listCmaps(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine("Cmaps in the font:");
          FontInfo.listCmaps(font).prettyPrint();
          Debug.WriteLine();
        }
      }

      // Print blocks
      if (options.blocks || options.all) {
        if (options.csv) {
          Debug.WriteLine("Unicode block coverage:");
          Debug.WriteLine(prependDataAndBuildCsv(
              FontInfo.listCharBlockCoverage(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine("Unicode block coverage:");
          FontInfo.listCharBlockCoverage(font).prettyPrint();
          Debug.WriteLine();
        }
      }

      // Print scripts
      if (options.scripts || options.all) {
        if (options.csv) {
          Debug.WriteLine("Unicode script coverage:");
          Debug.WriteLine(prependDataAndBuildCsv(
              FontInfo.listScriptCoverage(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
          if (options.detailed) {
            Debug.WriteLine("Uncovered code points in partially-covered scripts:");
            Debug.WriteLine(prependDataAndBuildCsv(
                FontInfo.listCharsNeededToCoverScript(font).csvStringArray(), fileName, i));
            Debug.WriteLine();
          }
        } else {
          Debug.WriteLine("Unicode script coverage:");
          FontInfo.listScriptCoverage(font).prettyPrint();
          Debug.WriteLine();
          if (options.detailed) {
            Debug.WriteLine("Uncovered code points in partially-covered scripts:");
            FontInfo.listCharsNeededToCoverScript(font).prettyPrint();
            Debug.WriteLine();
          }
        }
      }

      // Print char list
      if (options.chars || options.all) {
        if (options.csv) {
          Debug.WriteLine("Characters with valid glyphs:");
          Debug.WriteLine(
              prependDataAndBuildCsv(FontInfo.listChars(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine("Characters with valid glyphs:");
          FontInfo.listChars(font).prettyPrint();
          Debug.WriteLine();
          Debug.WriteLine(String.format(
              "Total number of characters with valid glyphs: %d", FontInfo.numChars(font)));
          Debug.WriteLine();
        }
      }

      // Print glyph information
      if (options.glyphs || options.all) {
        DataDisplayTable unmappedGlyphs = FontInfo.listUnmappedGlyphs(font);
        if (options.csv) {
          Debug.WriteLine(String.Format("Total hinting size: %s", FontInfo.hintingSize(font)));
          Debug.WriteLine(String.format(
              "Number of unmapped glyphs: %d / %d", unmappedGlyphs.getNumRows(),
              FontInfo.numGlyphs(font)));
          Debug.WriteLine();
          if (options.detailed) {
            Debug.WriteLine("Unmapped glyphs:");
            Debug.WriteLine(
                prependDataAndBuildCsv(unmappedGlyphs.csvStringArray(), fileName, i));
            Debug.WriteLine();
          }
          Debug.WriteLine("Subglyphs used by characters in the font:");
          Debug.WriteLine(prependDataAndBuildCsv(
              FontInfo.listSubglyphFrequency(font).csvStringArray(), fileName, i));
          Debug.WriteLine();
        } else {
          Debug.WriteLine(String.Format("Total hinting size: %s", FontInfo.hintingSize(font)));
          Debug.WriteLine(String.format(
              "Number of unmapped glyphs: %d / %d", unmappedGlyphs.getNumRows(),
              FontInfo.numGlyphs(font)));
          Debug.WriteLine();
          if (options.detailed) {
            Debug.WriteLine("Unmapped glyphs:");
            unmappedGlyphs.prettyPrint();
            Debug.WriteLine();
          }
          Debug.WriteLine("Subglyphs used by characters in the font:");
          FontInfo.listSubglyphFrequency(font).prettyPrint();
          Debug.WriteLine();
        }
      }
    }
  }

  private static String prependDataAndBuildCsv(String[] arr, String fontName, int fontIndex) {
    StringBuilder output = new StringBuilder("Font,font index,").append(arr[0]).append('\n');
    for (int i = 1; i < arr.length; i++) {
      String row = arr[i];
      output.append(fontName)
          .append(',')
          .append("font index ")
          .append(fontIndex)
          .append(',')
          .append(row)
          .append('\n');
    }
    return output.ToString();
  }
}

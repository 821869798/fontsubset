namespace com.google.typography.font.sfntly.table.opentype.testing;








public class TestLanguagesForFonts {
  private static readonly String FONTS_ROOT = "/usr/local/google/home/cibu/sfntly/fonts";
  private static readonly String WORDS_DIR = "/usr/local/google/home/cibu/sfntly/adv_layout/data/testdata/wiki_words";
  private static readonly String OUTPUT_FILE = "/tmp/font-languages.txt";

  private static readonly FontLanguages fontLanguages = new FontLanguages(availableLangs(WORDS_DIR));

  public static void main(String[] args)  {
    List<File> fontFiles = FontLoader.getFontFiles(FONTS_ROOT);
    PrintWriter writer = new PrintWriter(OUTPUT_FILE);
    foreach(File fontFile in fontFiles) {
      writer.print(fontFile.getPath());
      ISet<String> langs = fontLanguages.get(FontLoader.getFont(fontFile));
      if (langs.isEmpty()) {
        langs.add("en");
      }
      foreach(String lang in langs) {
        writer.print("," + lang);
      }
      writer.println();
    }
    writer.close();
  }

  private static List<String> availableLangs(String wordsDir) {
    List<String> langs = new ArrayList<String>();
    File[] wordFiles = new File(wordsDir).listFiles();
    foreach(File file in wordFiles) {
      String lang = file.getName();
      if (lang.startsWith(".")) {
        continue;
      }
      langs.add(lang);
    }
    return langs;
  }
}

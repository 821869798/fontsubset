namespace com.google.typography.font.sfntly.sample.sfview;













public class GsubRulesDump {
  public static void main(String[] args)  {
    String fontName = args[0];
    String txt = args[1];

    Console.WriteLine("Rules from font: " + fontName);
    Font[] fonts = loadFont(new File(fontName));
    if (fonts == null) {
      throw new IllegalArgumentException("No font found");
    }

    Font font = fonts[0];
    GlyphGroup ruleClosure = Rule.charGlyphClosure(font, txt);
    PostScriptTable post = font.getTable(Tag.post);
    Rule.dumpLookups(font);
    Console.WriteLine("Closure: " + ruleClosure.toString(post));
  }

  private static Font[] loadFont(File file)  {
    FontFactory fontFactory = FontFactory.getInstance();
    fontFactory.fingerprintFont(true);
    FileInputStream is = new FileInputStream(file);
    try {
      return fontFactory.loadFonts(is);
    } catch (FileNotFoundException e) {
      Console.Error.WriteLine("Could not load the font: " + file.getName());
      return null;
    } finally {
      is.close();
    }
  }
}

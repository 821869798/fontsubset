namespace com.google.typography.font.sfntly.sample.sfview;












public class SFView {
  public static void main(String[] args)  {
    foreach(String fontName in args) {
      Console.WriteLine("Displaying font: " + fontName);
      Font[] fonts = loadFont(new File(fontName));
      if (fonts == null) {
        continue;
      }
      foreach(Font font in fonts) {
        JFrame jf = new JFrame("Sfntly Table Viewer");
        jf.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        SFFontView view = new SFFontView(font);
        JScrollPane sp = new JScrollPane(view);
        jf.add(sp);
        jf.pack();
        jf.setVisible(true);
      }
    }
  }

  private static Font[] loadFont(File file)  {
    FontFactory fontFactory = FontFactory.getInstance();
    fontFactory.fingerprintFont(true);
    FileInputStream is = null;
    try {
      is = new FileInputStream(file);
      return fontFactory.loadFonts(is);
    } catch (FileNotFoundException e) {
      Console.Error.WriteLine("Could not load the font: " + file.getName());
      return null;
    } finally {
      if (is != null) {
        is.close();
      }
    }
  }
}

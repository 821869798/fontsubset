namespace com.google.typography.font.sfntly.sample.sfview;













public class HtmlViewer {
//  private static readonly String fileName = "/home/build/google3/googledata/third_party/" +
//      "fonts/ascender/arial.ttf";

  public static void main(String[] args)  {

    Font[] fonts = loadFont(new File(args[0]));
    GSubTable gsub = fonts[0].getTable(Tag.GSUB);
    tag(gsub, args[1]);

  }
  public static void tag(GSubTable gsub, String outFileName) throws FileNotFoundException, UnsupportedEncodingException {
    PrintWriter writer = new PrintWriter(outFileName, "UTF-8");
    writer.println("<html>");
    writer.println("  <head>");
    writer.println("    <link href=special.css rel=stylesheet type=text/css>");
    writer.println("  </head>");
    writer.println("  <body>");
//    writer.println(gsub.scriptList().toHtml());
//    writer.println(gsub.featureList().toHtml());
//    writer.println(gsub.lookupList().toHtml());
    writer.println("  </body>");
    writer.println("</html>");
    writer.close();
  }

  public static Font[] loadFont(File file)  {
    FontFactory fontFactory = FontFactory.getInstance();
    fontFactory.fingerprintFont(true);
    FileInputStream is = null;
    try {
      is = new FileInputStream(file);
      return fontFactory.loadFonts(is);
    } finally {
      if (is != null) {
        is.close();
      }
    }
  }
}

namespace com.google.typography.font.sfntly.table.opentype.testing;











public class FontLoader
{
    public static List<FileInfo> getFontFiles(String fontDir)
    {
        List<FileInfo> fontFiles = new ArrayList<FileInfo>();
        getFontFiles(fontFiles, new DirectoryInfo(fontDir), "", true);
        return fontFiles;
    }

    public static Font getFont(FileInfo fontFile)
    {
        Font[] fonts = load(fontFile);
        if (fonts == null)
        {
            throw new IllegalArgumentException("No font found");
        }
        return fonts[0];
    }

    private static void getFontFiles(
        List<FileInfo> fonts, DirectoryInfo dir, String startFrom, boolean foundStart)
    {
        FileInfo[] files = dir.listFiles();
        foreach (File file in files)
        {
            if (file.getName().endsWith(".ttf"))
            {
                if (foundStart || startFrom.endsWith(file.getName()))
                {
                    foundStart = true;
                    fonts.add(file);
                }
            }
            if (file.isDirectory())
            {
                getFontFiles(fonts, file, startFrom, foundStart);
            }
        }
    }

    private static Font[] load(FileInfo file)
    {
        FontFactory fontFactory = FontFactory.getInstance();
        fontFactory.fingerprintFont(true);
        FileInputStream @is = new FileInputStream(file);
        try
        {
            return fontFactory.loadFonts(@is);
        }
        catch (FileNotFoundException e)
        {
            Console.Error.WriteLine("Could not load the font : " + file.getName());
            return null;
        }
        finally
        {
            @is.close();
        }
    }
}
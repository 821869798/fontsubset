using com.google.typography.font.sfntly;
using com.google.typography.font.sfntly.table.truetype;
using com.google.typography.font.tools.sfnttool;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace sfntly.Subsetter;

public static class SubsetterUtil
{
    public static async Task<SubsetterResult> StartSubset(string inputFont, string outputFile, string charsSetPath, string charsfileMatch, bool containAscii, bool stripTable)
    {

        SubsetterResult result = new SubsetterResult();
        try
        {
            var inputFile = new FileInfo(inputFont);

            string parentOutput = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(parentOutput))
            {
                Directory.CreateDirectory(parentOutput);
            }

            var outFile = new FileInfo(outputFile);

            var sfntTool = new SfntTool();

            var uniqueChars = new ConcurrentDictionary<char, byte>();
            if (containAscii)
            {
                for (int i = 31; i < 128; i++)
                {
                    uniqueChars.TryAdd((char)i, 0);
                }
            }

            var regex = new Regex(charsfileMatch);
            var files = Directory.GetFiles(charsSetPath, "*", SearchOption.AllDirectories).Where(path => regex.IsMatch(path));
            var tasks = files.Select(async file =>
            {
                string text = await File.ReadAllTextAsync(file);
                foreach (var c in text)
                {
                    uniqueChars.TryAdd(c, 0);
                }
            });

            await Task.WhenAll(tasks);

            string uniqueCharsString = new string(uniqueChars.Keys.ToArray());

            sfntTool.subsetString = uniqueCharsString;

            sfntTool.strip = stripTable;

            var sfntInfo = sfntTool.subsetFontFile(inputFile, outFile, 1);

            if (sfntInfo.UnSupportedFont)
            {
                result.Exception = new Exception("\nFailed to subset font,only supports TrueType font files.\nPlease use software to convert PostScript fonts to TrueType fonts");
                return result;
            }

            LocaTable loca = sfntInfo.OriginFont.getTable<LocaTable>(Tag.loca);
            result.OriginCharactersCount = loca.numGlyphs();
            loca = sfntInfo.ModifiedFont.getTable<LocaTable>(Tag.loca);
            result.SubsetCharactersCount = loca.numGlyphs();
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }



}


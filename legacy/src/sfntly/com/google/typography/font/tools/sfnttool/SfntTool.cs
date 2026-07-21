/*
 * Copyright 2011 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using com.google.typography.font.sfntly;
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.core;
using com.google.typography.font.sfntly.table.truetype;
using com.google.typography.font.tools.conversion.eot;
using com.google.typography.font.tools.conversion.woff;
using com.google.typography.font.tools.subsetter;
using System.Diagnostics;

namespace com.google.typography.font.tools.sfnttool;





















/**
 * @author Raph Levien
 */
public class SfntTool
{
    public boolean strip = false;
    public String subsetString = null;
    public boolean woff = false;
    public boolean eot = false;
    public boolean mtx = false;

    public static void Main(String[] args)
    {
        SfntTool tool = new SfntTool();
        FileInfo fontFile = null;
        FileInfo outputFile = null;
        boolean bench = false;
        int nIters = 1;


        for (int i = 0; i < args.Length; i++)
        {
            String option = null;
            if (args[i][0] == '-')
            {
                option = args[i].Substring(1);
            }

            if (option != null)
            {
                if (option.Equals("help") || option.Equals("?"))
                {
                    printUsage();
                    return;
                }
                else if (option.Equals("b") || option.Equals("bench"))
                {
                    nIters = 10000;
                }
                else if (option.Equals("h") || option.Equals("hints"))
                {
                    tool.strip = true;
                }
                else if (option.Equals("s") || option.Equals("string"))
                {
                    tool.subsetString = args[i + 1];
                    i++;
                }
                else if (option.Equals("w") || option.Equals("woff"))
                {
                    tool.woff = true;
                }
                else if (option.Equals("e") || option.Equals("eot"))
                {
                    tool.eot = true;
                }
                else if (option.Equals("x") || option.Equals("mtx"))
                {
                    tool.mtx = true;
                }
                else
                {
                    printUsage();
                    return;
                }
            }
            else
            {
                if (fontFile == null)
                {
                    fontFile = new FileInfo(args[i]);
                }
                else
                {
                    outputFile = new FileInfo(args[i]);
                    break;
                }
            }
        }

        if (tool.woff && tool.eot)
        {
            Debug.WriteLine("WOFF and EOT options are mutually exclusive");
            return;
        }

        if (fontFile != null && outputFile != null)
        {
            var resultInfo = tool.subsetFontFile(fontFile, outputFile, nIters);
            if (resultInfo.UnSupportedFont)
            {
                Console.WriteLine("Failed to subset font,only supports TrueType font files.\nPlease use software to convert PostScript fonts to TrueType fonts");
            }

        }
        else
        {
            printUsage();
        }
    }

    private static void printUsage()
    {
        Console.WriteLine("Subset [-?|-h|-help] [-b] [-s string] fontfile outfile");
        Console.WriteLine("Prototype font subsetter");
        Console.WriteLine("\t-?,-help\tprint this help information");
        Console.WriteLine("\t-s,-string\t String to subset");
        Console.WriteLine("\t-b,-bench\t Benchmark (run 10000 iterations)");
        Console.WriteLine("\t-h,-hints\t Strip hints");
        Console.WriteLine("\t-w,-woff\t Output WOFF format");
        Console.WriteLine("\t-e,-eot\t Output EOT format");
        Console.WriteLine("\t-x,-mtx\t Enable Microtype Express compression for EOT format");
    }

    public SfntInfo subsetFontFile(FileInfo fontFile, FileInfo outputFile, int nIters)
    {
        FontFactory fontFactory = FontFactory.getInstance();
        FileInputStream fis = null;
        SfntInfo sfntInfo = new SfntInfo();
        try
        {
            fis = fontFile.OpenRead();
            byte[] fontBytes = new byte[(int)fontFile.Length];
            fis.read(fontBytes);
            Font[] fontArray = null;
            fontArray = fontFactory.loadFonts(fontBytes);
            Font font = fontArray[0];
            IList<CMapTable.CMapId> cmapIds = new List<CMapTable.CMapId>();
            cmapIds.Add(CMapTable.CMapId.WINDOWS_BMP);
            sfntInfo.OriginFont = font;

            LocaTable locaTable = font.getTable<LocaTable>(Tag.loca);
            GlyphTable glyfTable = font.getTable<GlyphTable>(Tag.glyf);
            if (locaTable == null || glyfTable == null)
            {
                sfntInfo.UnSupportedFont = true;
                return sfntInfo;
            }

            for (int i = 0; i < nIters; i++)
            {
                Font newFont = font;
                if (subsetString != null)
                {
                    subsetter.Subsetter subsetter = new RenumberingSubsetter(newFont, fontFactory);
                    subsetter.setCMaps(cmapIds, 1);
                    IList<Integer> glyphs = GlyphCoverage.getGlyphCoverage(font, subsetString);
                    subsetter.setGlyphs(glyphs);
                    ISet<Integer> removeTables = new HashSet<Integer>();
                    // Most of the following are valid tables, but we don't renumber them yet, so strip
                    removeTables.Add(Tag.GDEF);
                    removeTables.Add(Tag.GPOS);
                    removeTables.Add(Tag.GSUB);
                    removeTables.Add(Tag.kern);
                    removeTables.Add(Tag.hdmx);
                    removeTables.Add(Tag.vmtx);
                    removeTables.Add(Tag.VDMX);
                    removeTables.Add(Tag.LTSH);
                    removeTables.Add(Tag.DSIG);
                    removeTables.Add(Tag.vhea);
                    // AAT tables, not yet defined in sfntly Tag class
                    removeTables.Add(Tag.intValue(new char[] { 'm', 'o', 'r', 't' }));
                    removeTables.Add(Tag.intValue(new char[] { 'm', 'o', 'r', 'x' }));
                    subsetter.setRemoveTables(removeTables);
                    newFont = subsetter.subset().build();
                }
                if (strip)
                {
                    subsetter.Subsetter hintStripper = new HintStripper(newFont, fontFactory);
                    ISet<Integer> removeTables = new HashSet<Integer>();
                    removeTables.Add(Tag.fpgm);
                    removeTables.Add(Tag.prep);
                    removeTables.Add(Tag.cvt);
                    removeTables.Add(Tag.hdmx);
                    removeTables.Add(Tag.VDMX);
                    removeTables.Add(Tag.LTSH);
                    removeTables.Add(Tag.DSIG);
                    removeTables.Add(Tag.vhea);
                    hintStripper.setRemoveTables(removeTables);
                    newFont = hintStripper.subset().build();
                }

                sfntInfo.ModifiedFont = newFont;

                using (FileOutputStream fos = outputFile.OpenWrite())
                {
                    if (woff)
                    {
                        WritableFontData woffData = new WoffWriter().convert(newFont);
                        woffData.copyTo(fos);
                    }
                    else if (eot)
                    {
                        WritableFontData eotData = new EOTWriter(mtx).convert(newFont);
                        eotData.copyTo(fos);
                    }
                    else
                    {
                        fontFactory.serializeFont(newFont, fos);
                    }
                }
            }
        }
        finally
        {
            if (fis != null)
            {
                fis.close();
            }
        }

        return sfntInfo;
    }
}
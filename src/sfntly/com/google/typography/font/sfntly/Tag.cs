/*
 * Copyright 2010 Google Inc. All Rights Reserved.
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

namespace com.google.typography.font.sfntly;


/**
 * Font identification tags used for tables, features, etc.
 *
 * Tag names are consistent with the OpenType and sfnt specs.
 *
 * @author Stuart Gill
 */
public sealed class Tag
{
    public static readonly int ttcf = Tag.intValue(new char[] { 't', 't', 'c', 'f' });

    /***********************************************************************************
     *
     * Table Type Tags
     *
     ***********************************************************************************/

    // required tables
    public static readonly int cmap = Tag.intValue(new char[] { 'c', 'm', 'a', 'p' });
    public static readonly int head = Tag.intValue(new char[] { 'h', 'e', 'a', 'd' });
    public static readonly int hhea = Tag.intValue(new char[] { 'h', 'h', 'e', 'a' });
    public static readonly int hmtx = Tag.intValue(new char[] { 'h', 'm', 't', 'x' });
    public static readonly int maxp = Tag.intValue(new char[] { 'm', 'a', 'x', 'p' });
    public static readonly int name = Tag.intValue(new char[] { 'n', 'a', 'm', 'e' });
    public static readonly int OS_2 = Tag.intValue(new char[] { 'O', 'S', '/', '2' });
    public static readonly int post = Tag.intValue(new char[] { 'p', 'o', 's', 't' });

    // truetype outline tables
    public static readonly int cvt = Tag.intValue(new char[] { 'c', 'v', 't', ' ' });
    public static readonly int fpgm = Tag.intValue(new char[] { 'f', 'p', 'g', 'm' });
    public static readonly int glyf = Tag.intValue(new char[] { 'g', 'l', 'y', 'f' });
    public static readonly int loca = Tag.intValue(new char[] { 'l', 'o', 'c', 'a' });
    public static readonly int prep = Tag.intValue(new char[] { 'p', 'r', 'e', 'p' });

    // postscript outline tables
    public static readonly int CFF = Tag.intValue(new char[] { 'C', 'F', 'F', ' ' });
    public static readonly int VORG = Tag.intValue(new char[] { 'V', 'O', 'R', 'G' });

    // opentype bitmap glyph outlines
    public static readonly int EBDT = Tag.intValue(new char[] { 'E', 'B', 'D', 'T' });
    public static readonly int EBLC = Tag.intValue(new char[] { 'E', 'B', 'L', 'C' });
    public static readonly int EBSC = Tag.intValue(new char[] { 'E', 'B', 'S', 'C' });

    // advanced typographic features
    public static readonly int BASE = Tag.intValue(new char[] { 'B', 'A', 'S', 'E' });
    public static readonly int GDEF = Tag.intValue(new char[] { 'G', 'D', 'E', 'F' });
    public static readonly int GPOS = Tag.intValue(new char[] { 'G', 'P', 'O', 'S' });
    public static readonly int GSUB = Tag.intValue(new char[] { 'G', 'S', 'U', 'B' });
    public static readonly int JSTF = Tag.intValue(new char[] { 'J', 'S', 'T', 'F' });

    // other
    public static readonly int DSIG = Tag.intValue(new char[] { 'D', 'S', 'I', 'G' });
    public static readonly int gasp = Tag.intValue(new char[] { 'g', 'a', 's', 'p' });
    public static readonly int hdmx = Tag.intValue(new char[] { 'h', 'd', 'm', 'x' });
    public static readonly int kern = Tag.intValue(new char[] { 'k', 'e', 'r', 'n' });
    public static readonly int LTSH = Tag.intValue(new char[] { 'L', 'T', 'S', 'H' });
    public static readonly int PCLT = Tag.intValue(new char[] { 'P', 'C', 'L', 'T' });
    public static readonly int VDMX = Tag.intValue(new char[] { 'V', 'D', 'M', 'X' });
    public static readonly int vhea = Tag.intValue(new char[] { 'v', 'h', 'e', 'a' });
    public static readonly int vmtx = Tag.intValue(new char[] { 'v', 'm', 't', 'x' });

    // AAT Tables
    // TODO(stuartg): some tables may be missing from this list
    public static readonly int bsln = Tag.intValue(new char[] { 'b', 's', 'l', 'n' });
    public static readonly int feat = Tag.intValue(new char[] { 'f', 'e', 'a', 't' });
    public static readonly int lcar = Tag.intValue(new char[] { 'l', 'c', 'a', 'r' });
    public static readonly int morx = Tag.intValue(new char[] { 'm', 'o', 'r', 'x' });
    public static readonly int opbd = Tag.intValue(new char[] { 'o', 'p', 'b', 'd' });
    public static readonly int prop = Tag.intValue(new char[] { 'p', 'r', 'o', 'p' });

    // Graphite tables
    public static readonly int Feat = Tag.intValue(new char[] { 'F', 'e', 'a', 't' });
    public static readonly int Glat = Tag.intValue(new char[] { 'G', 'l', 'a', 't' });
    public static readonly int Gloc = Tag.intValue(new char[] { 'G', 'l', 'o', 'c' });
    public static readonly int Sile = Tag.intValue(new char[] { 'S', 'i', 'l', 'e' });
    public static readonly int Silf = Tag.intValue(new char[] { 'S', 'i', 'l', 'f' });

    // truetype bitmap font tables
    public static readonly int bhed = Tag.intValue(new char[] { 'b', 'h', 'e', 'd' });
    public static readonly int bdat = Tag.intValue(new char[] { 'b', 'd', 'a', 't' });
    public static readonly int bloc = Tag.intValue(new char[] { 'b', 'l', 'o', 'c' });

    private Tag()
    {
        // Prevent construction.
    }

    public static int intValue(char[] tag)
    {
        return ((byte)tag[0]) << 24 | ((byte)tag[1]) << 16 | ((byte)tag[2]) << 8 | ((byte)tag[3]);
    }
    public static int intValue(byte[] tag)
    {
        return tag[0] << 24 | tag[1] << 16 | tag[2] << 8 | tag[3];
    }

    public static byte[] byteValue(int tag)
    {
        byte[] b = new byte[4];
        b[0] = (byte)(0xff & (tag >> 24));
        b[1] = (byte)(0xff & (tag >> 16));
        b[2] = (byte)(0xff & (tag >> 8));
        b[3] = (byte)(0xff & tag);
        return b;
    }

    public static String stringValue(int tag)
    {
        return Encoding.ASCII.GetString(Tag.byteValue(tag));
    }

    public static int intValue(String s)
    {
        byte[] b = Encoding.ASCII.GetBytes(s);
        return intValue(b);
    }

    /**
     * Determines whether the tag is that for the header table.
     * @param tag table tag
     * @return true if the tag represents the font header table
     */
    public static boolean isHeaderTable(int tag)
    {
        if (tag == Tag.head || tag == Tag.bhed)
        {
            return true;
        }
        return false;
    }
}
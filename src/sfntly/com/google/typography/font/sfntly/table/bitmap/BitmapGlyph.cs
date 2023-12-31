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

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.bitmap;






/**
 * @author Stuart Gill
 *
 */
public abstract class BitmapGlyph : SubTable
{

    public enum Offset
    {
        // header
        version = (0),

        smallGlyphMetricsLength = (5),
        bigGlyphMetricsLength = (8),
        // format 1
        glyphFormat1_imageData = (smallGlyphMetricsLength),

        // format 2
        glyphFormat2_imageData = (smallGlyphMetricsLength),

        // format 3

        // format 4

        // format 5
        glyphFormat5_imageData = (0),

        // format 6
        glyphFormat6_imageData = (bigGlyphMetricsLength),

        // format 7
        glyphFormat7_imageData = (bigGlyphMetricsLength),

        // format 8
        glyphFormat8_numComponents = ((int)Offset.smallGlyphMetricsLength + 1),
        glyphFormat8_componentArray = (glyphFormat8_numComponents
            + FontData.DataSize.USHORT),

        // format 9
        glyphFormat9_numComponents = ((int)Offset.bigGlyphMetricsLength),
        glyphFormat9_componentArray = (glyphFormat9_numComponents
            + FontData.DataSize.USHORT),


        // ebdtComponent
        ebdtComponentLength = (FontData.DataSize.USHORT + 2 * FontData.DataSize.CHAR),
        ebdtComponent_glyphCode = (0),
        ebdtComponent_xOffset = (2),
        ebdtComponent_yOffset = (3)/*;
    
    public final int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    private int _format;

    public static BitmapGlyph createGlyph(ReadableFontData data, int format)
    {
        BitmapGlyph glyph = null;
        var builder = BitmapGlyph.createGlyphBuilder(data, format);
        if (builder != null)
        {
            glyph = builder.build();
        }
        return glyph;
    }

    public BitmapGlyph(ReadableFontData data, int format) : base(data)
    {
        this._format = format;
    }

    public BitmapGlyph(ReadableFontData data, int offset, int length, int format) : base(data, offset, length)
    {
        this._format = format;
    }

    public int format()
    {
        return this._format;
    }

    public static IBuilder<BitmapGlyph> createGlyphBuilder(
        ReadableFontData data, int format)
    {
        switch (format)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                return SimpleBitmapGlyph.createBuilder(data, format);
            case 8:
            case 9:
                return CompositeBitmapGlyph.createBuilder(data, format);
        }
        return null;
    }

    new public interface IBuilder<out TBitmapGlyph> : SubTable.IBuilder<TBitmapGlyph> where TBitmapGlyph : BitmapGlyph
    {
        int format();
    }

    new protected abstract class Builder<TBitmapGlyph> : SubTable.Builder<TBitmapGlyph>, IBuilder<TBitmapGlyph> where TBitmapGlyph : BitmapGlyph
    {

        private readonly int _format;


        public Builder(WritableFontData data, int format) : base(data)
        {
            this._format = format;
        }

        public Builder(ReadableFontData data, int format) : base(data)
        {
            this._format = format;
        }

        public int format()
        {
            return this._format;
        }

        public override void subDataSet()
        {
            // NOP
        }

        public override int subDataSizeToSerialize()
        {
            return this.internalReadData().length();
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return this.internalReadData().copyTo(newData);
        }
    }

    public override String ToString()
    {
        return "BitmapGlyph [format=" + _format + ", data = " + base.ToString() + "]";
    }
}
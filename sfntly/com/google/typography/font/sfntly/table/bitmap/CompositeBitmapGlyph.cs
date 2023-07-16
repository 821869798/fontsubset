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
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;

namespace com.google.typography.font.sfntly.table.bitmap;




/**
 * @author Stuart Gill
 *
 */
public class CompositeBitmapGlyph : BitmapGlyph
{

    public sealed class Component
    {
        private readonly int _glyphCode;
        private int _xOffset;
        private int _yOffset;

        public Component(int glyphCode, int xOffset, int yOffset)
        {
            this._glyphCode = glyphCode;
            this._xOffset = xOffset;
            this._yOffset = yOffset;
        }

        public int glyphCode()
        {
            return this._glyphCode;
        }

        public int xOffset()
        {
            return this._xOffset;
        }

        public int yOffset()
        {
            return this._yOffset;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + _glyphCode;
            return result;
        }

        public override boolean Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Component))
            {
                return false;
            }
            Component other = (Component)obj;
            if (glyphCode != other.glyphCode)
            {
                return false;
            }
            return true;
        }
    }

    private int numComponentsOffset;
    private int componentArrayOffset;

    public CompositeBitmapGlyph(ReadableFontData data, int format) : base(data, format)
    {
        initialize(format);
    }

    /**
     * Initializes the public state from the data.
     *
     * @param format the glyph format
     */
    private void initialize(int format)
    {
        if (format == 8)
        {
            this.numComponentsOffset = (int)Offset.glyphFormat8_numComponents;
            this.componentArrayOffset = (int)Offset.glyphFormat8_componentArray;
        }
        else if (format == 9)
        {
            this.numComponentsOffset = (int)Offset.glyphFormat9_numComponents;
            this.componentArrayOffset = (int)Offset.glyphFormat9_componentArray;
        }
        else
        {
            throw new IllegalStateException(
                "Attempt to create a Composite Bitmap Glyph with a non-composite format.");
        }
    }

    public int numComponents()
    {
        return this._data.readUShort(this.numComponentsOffset);
    }

    public Component component(int componentNum)
    {
        int componentOffset =
            this.componentArrayOffset + componentNum * (int)Offset.ebdtComponentLength;
        return new Component(
            this._data.readUShort(componentOffset + (int)Offset.ebdtComponent_glyphCode),
            this._data.readChar(componentOffset + (int)Offset.ebdtComponent_xOffset),
            this._data.readChar(componentOffset + (int)Offset.ebdtComponent_yOffset));
    }

    public static IBuilder createBuilder(WritableFontData data, int format)
    {
        return new Builder(data, format);
    }

    public static IBuilder createBuilder(ReadableFontData data, int format)
    {
        return new Builder(data, format);
    }

    public interface IBuilder : BitmapGlyph.IBuilder<CompositeBitmapGlyph>
    {

    }

    private class Builder : BitmapGlyph.Builder<CompositeBitmapGlyph>, IBuilder
    {

        public Builder(WritableFontData data, int format) : base(data, format)
        {
        }

        public Builder(ReadableFontData data, int format) : base(data, format)
        {
        }

        public override CompositeBitmapGlyph subBuildTable(ReadableFontData data)
        {
            return new CompositeBitmapGlyph(data, this.format());
        }
    }
}
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
 */
public class SmallGlyphMetrics : GlyphMetrics
{

    public enum Offset
    {
        metricsLength = (5), height = (0), width = (1), BearingX = (2), BearingY = (3), Advance = (4)/*;

     readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    private SmallGlyphMetrics(ReadableFontData data) : base(data)
    {
    }

    public int height()
    {
        return this._data.readByte((int)Offset.height);
    }

    public int width()
    {
        return this._data.readByte((int)Offset.width);
    }

    public int bearingX()
    {
        return this._data.readChar((int)Offset.BearingX);
    }

    public int bearingY()
    {
        return this._data.readChar((int)Offset.BearingY);
    }

    public int advance()
    {
        return this._data.readByte((int)Offset.Advance);
    }

    protected class Builder : GlyphMetrics.Builder<SmallGlyphMetrics>
    {

        /**
         * Constructor.
         *
         * @param data the data for the builder
         */
        public Builder(WritableFontData data) : base(data)
        {
        }

        /**
         * Constructor.
         *
         * @param data the data for the builder
         */
        public Builder(ReadableFontData data) : base(data)
        {
        }

        public int height()
        {
            return this.internalReadData().readByte((int)Offset.height);
        }

        public void setHeight(byte height)
        {
            this.internalWriteData().writeByte((int)Offset.height, height);
        }

        public int width()
        {
            return this.internalReadData().readByte((int)Offset.width);
        }

        public void setWidth(byte width)
        {
            this.internalWriteData().writeByte((int)Offset.width, width);
        }

        public int bearingX()
        {
            return this.internalReadData().readChar((int)Offset.BearingX);
        }

        public void setBearingX(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.BearingX, bearing);
        }

        public int bearingY()
        {
            return this.internalReadData().readChar((int)Offset.BearingY);
        }

        public void setBearingY(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.BearingY, bearing);
        }

        public int advance()
        {
            return this.internalReadData().readByte((int)Offset.Advance);
        }

        public void setAdvance(byte advance)
        {
            this.internalWriteData().writeByte((int)Offset.Advance, advance);
        }

        public override SmallGlyphMetrics subBuildTable(ReadableFontData data)
        {
            return new SmallGlyphMetrics(data);
        }

        public override void subDataSet()
        {
            // NOP
        }

        public override int subDataSizeToSerialize()
        {
            return 0;
        }

        public override boolean subReadyToSerialize()
        {
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return this.data().copyTo(newData);
        }
    }
}
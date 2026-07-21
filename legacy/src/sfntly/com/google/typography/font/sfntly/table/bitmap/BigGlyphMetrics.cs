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
public class BigGlyphMetrics : GlyphMetrics
{

    public enum Offset
    {
        metricsLength = (8),

        height = (0),
        width = (1),
        horiBearingX = (2),
        horiBearingY = (3),
        horiAdvance = (4),
        vertBearingX = (5),
        vertBearingY = (6),
        vertAdvance = (7)/*;

     readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    /**
     * @param data
     */
    public BigGlyphMetrics(ReadableFontData data) : base(data)
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

    public int horiBearingX()
    {
        return this._data.readChar((int)Offset.horiBearingX);
    }

    public int horiBearingY()
    {
        return this._data.readChar((int)Offset.horiBearingY);
    }

    public int horiAdvance()
    {
        return this._data.readByte((int)Offset.horiAdvance);
    }

    public int vertBearingX()
    {
        return this._data.readChar((int)Offset.vertBearingX);
    }

    public int vertBearingY()
    {
        return this._data.readChar((int)Offset.vertBearingY);
    }

    public int vertAdvance()
    {
        return this._data.readByte((int)Offset.vertAdvance);
    }

    public static IBuilder createBuilder()
    {
        WritableFontData data = WritableFontData.createWritableFontData((int)Offset.metricsLength);
        return new Builder(data);
    }


    public static IBuilder createBuilder(WritableFontData data)
    {
        return new Builder(data);
    }


    public static IBuilder createBuilder(ReadableFontData data)
    {
        return new Builder(data);
    }

    public interface IBuilder : GlyphMetrics.IBuilder<BigGlyphMetrics>
    {

    }

    protected class Builder : GlyphMetrics.Builder<BigGlyphMetrics>, IBuilder
    {


        /**
         * Constructor.
         *
         * @param data
         */
        public Builder(WritableFontData data) : base(data)
        {
        }

        /**
         * Constructor.
         *
         * @param data
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

        public int horiBearingX()
        {
            return this.internalReadData().readChar((int)Offset.horiBearingX);
        }

        public void setHoriBearingX(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.horiBearingX, bearing);
        }

        public int horiBearingY()
        {
            return this.internalReadData().readChar((int)Offset.horiBearingY);
        }

        public void setHoriBearingY(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.horiBearingY, bearing);
        }

        public int horiAdvance()
        {
            return this.internalReadData().readByte((int)Offset.horiAdvance);
        }

        public void setHoriAdvance(byte advance)
        {
            this.internalWriteData().writeByte((int)Offset.horiAdvance, advance);
        }

        public int vertBearingX()
        {
            return this.internalReadData().readChar((int)Offset.vertBearingX);
        }

        public void setVertBearingX(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.vertBearingX, bearing);
        }

        public int vertBearingY()
        {
            return this.internalReadData().readChar((int)Offset.vertBearingY);
        }

        public void setVertBearingY(byte bearing)
        {
            this.internalWriteData().writeChar((int)Offset.vertBearingY, bearing);
        }

        public int vertAdvance()
        {
            return this.internalReadData().readByte((int)Offset.vertAdvance);
        }

        public void setVertAdvance(byte advance)
        {
            this.internalWriteData().writeByte((int)Offset.vertAdvance, advance);
        }

        public override BigGlyphMetrics subBuildTable(ReadableFontData data)
        {
            return new BigGlyphMetrics(data);
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
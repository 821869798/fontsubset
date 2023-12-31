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
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;












/**
 * Format 5 Index Subtable Entry.
 * 
 * @author Stuart Gill
 * 
 */
public sealed class IndexSubTableFormat5 : IndexSubTable
{
    private readonly int _imageSize;

    private IndexSubTableFormat5(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
    {
        this._imageSize = this._data.readULongAsInt((int)Offset.indexSubTable5_imageSize);
    }

    private static int numGlyphs(ReadableFontData data, int tableOffset)
    {
        int numGlyphs = data.readULongAsInt(tableOffset + (int)Offset.indexSubTable5_numGlyphs);
        return numGlyphs;
    }

    public int imageSize()
    {
        return this._data.readULongAsInt((int)Offset.indexSubTable5_imageSize);
    }

    public BigGlyphMetrics bigMetrics()
    {
        return new BigGlyphMetrics(this._data.slice(
            (int)Offset.indexSubTable5_bigGlyphMetrics, (int)BigGlyphMetrics.Offset.metricsLength));
    }

    public override int numGlyphs()
    {
        return IndexSubTableFormat5.numGlyphs(this._data, 0);
    }

    public override int glyphStartOffset(int glyphId)
    {
        this.checkGlyphRange(glyphId);
        int loca =
            this.readFontData().searchUShort((int)Offset.indexSubTable5_glyphArray,
                (int)FontData.DataSize.USHORT, this.numGlyphs(), glyphId);
        if (loca == -1)
        {
            return loca;
        }
        return loca * this._imageSize;
    }

    public override int glyphLength(int glyphId)
    {
        this.checkGlyphRange(glyphId);
        return this._imageSize;
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(
        ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
    {
        int length = Builder.dataLength(data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
        return new Builder(data.slice(indexSubTableOffset, length), firstGlyphIndex, lastGlyphIndex);
    }

    public static IBuilder createBuilder(
        WritableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
    {
        int length = Builder.dataLength(data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
        return new Builder(data.slice(indexSubTableOffset, length), firstGlyphIndex, lastGlyphIndex);
    }

    public interface IBuilder : IndexSubTable.IBuilder<IndexSubTableFormat5>
    {

    }

    private sealed class Builder : IndexSubTable.Builder<IndexSubTableFormat5>, IBuilder
    {
        private List<Integer> _glyphArray;
        private BigGlyphMetrics.IBuilder metrics;

        public static int dataLength(
            ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
        {
            int numGlyphs = IndexSubTableFormat5.numGlyphs(data, indexSubTableOffset);
            return (int)Offset.indexSubTable5_glyphArray + numGlyphs * (int)FontData.DataSize.USHORT;
        }

        public Builder() : base((int)Offset.indexSubTable5_builderDataSize, Format.FORMAT_5)
        {
            this.metrics = BigGlyphMetrics.createBuilder();
        }

        public Builder(WritableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public Builder(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public int imageSize()
        {
            return this.internalReadData().readULongAsInt((int)Offset.indexSubTable5_imageSize);
        }

        public void setImageSize(int imageSize)
        {
            this.internalWriteData().writeULong((int)Offset.indexSubTable5_imageSize, imageSize);
        }

        public BigGlyphMetrics.IBuilder bigMetrics()
        {
            if (this.metrics == null)
            {
                WritableFontData data =
                    this.internalWriteData().slice((int)Offset.indexSubTable5_bigGlyphMetrics,
                        (int)BigGlyphMetrics.Offset.metricsLength);
                this.metrics = BigGlyphMetrics.createBuilder(data);
                this.setModelChanged();
            }
            return this.metrics;
        }

        public override int numGlyphs()
        {
            return this.getGlyphArray().Count;
        }

        public override int glyphLength(int glyphId)
        {
            return this.imageSize();
        }

        public override int glyphStartOffset(int glyphId)
        {
            this.checkGlyphRange(glyphId);
            var glyphArray = this.getGlyphArray();

            int loca = glyphArray.BinarySearch(glyphId);
            if (loca < 0)
            {
                return -1;
            }
            return loca * this.imageSize();
        }

        public IList<Integer> glyphArray()
        {
            return this.getGlyphArray();
        }

        private List<Integer> getGlyphArray()
        {
            if (this._glyphArray == null)
            {
                this.initialize(base.internalReadData());
                base.setModelChanged();
            }
            return this._glyphArray;
        }

        private void initialize(ReadableFontData data)
        {
            if (this._glyphArray == null)
            {
                this._glyphArray = new List<Integer>();
            }
            else
            {
                this._glyphArray.Clear();
            }

            if (data != null)
            {
                int numGlyphs = IndexSubTableFormat5.numGlyphs(data, 0);
                for (int i = 0; i < numGlyphs; i++)
                {
                    this._glyphArray.Add(data.readUShort(
                        (int)Offset.indexSubTable5_glyphArray + i * (int)FontData.DataSize.USHORT));
                }
            }
        }

        public void setGlyphArray(List<Integer> array)
        {
            this._glyphArray = array;
            this.setModelChanged();
        }
        /*
        private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo>
        {
            private int offsetIndex;

            public BitmapGlyphInfoIterator()
            {
            }

            public override boolean hasNext()
            {
                if (this.offsetIndex < IndexSubTableFormat5.Builder.@this.getGlyphArray().size())
                {
                    return true;
                }
                return false;
            }

            public override BitmapGlyphInfo next()
            {
                if (!hasNext())
                {
                    throw new NoSuchElementException("No more characters to iterate.");
                }
                BitmapGlyphInfo info = new BitmapGlyphInfo(
                    IndexSubTableFormat5.Builder.@this.getGlyphArray().get(this.offsetIndex),
                    IndexSubTableFormat5.Builder.@this.imageDataOffset(),
                        this.offsetIndex * IndexSubTableFormat5.Builder.@this.imageSize(),
                        IndexSubTableFormat5.Builder.@this.imageSize(),
                    IndexSubTableFormat5.Builder.@this.imageFormat());
                this.offsetIndex++;
                return info;
            }

            public override void remove()
            {
                throw new UnsupportedOperationException("Unable to remove a glyph info.");
            }
        }*/

        public override IEnumerator<BitmapGlyphInfo> GetEnumerator()
        {
            return getGlyphArray()
                .Select((x, index) => new BitmapGlyphInfo(
                    x,
                    imageDataOffset(),
                    index * imageSize(),
                    imageSize(),
                    imageFormat()
                    )
                )
                .GetEnumerator();
            // return new BitmapGlyphInfoIterator();
        }

        public override void revert()
        {
            base.revert();
            this._glyphArray = null;
        }

        public override IndexSubTableFormat5 subBuildTable(ReadableFontData data)
        {
            return new IndexSubTableFormat5(data, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.glyphArray == null)
            {
                return this.internalReadData().length();
            }
            return (int)Offset.indexSubTable5_builderDataSize + this._glyphArray.Count
                * (int)FontData.DataSize.USHORT;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.glyphArray != null)
            {
                return true;
            }
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = base.serializeIndexSubHeader(newData);
            if (!this.modelChanged())
            {
                size +=
                    this.internalReadData().slice((int)Offset.indexSubTable5_imageSize)
                        .copyTo(newData.slice((int)Offset.indexSubTable5_imageSize));
            }
            else
            {
                size += newData.writeULong((int)Offset.indexSubTable5_imageSize, this.imageSize());
                size += this.bigMetrics().subSerialize(newData.slice(size));
                size += newData.writeULong(size, this._glyphArray.Count);
                foreach (Integer glyphId in this._glyphArray)
                {
                    size += newData.writeUShort(size, glyphId);
                }
            }
            return size;
        }
    }
}

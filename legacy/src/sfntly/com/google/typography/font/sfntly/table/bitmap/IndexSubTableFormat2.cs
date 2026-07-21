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
 * Format 2 Index Subtable Entry.
 * 
 * @author Stuart Gill
 * 
 */
public sealed class IndexSubTableFormat2 : IndexSubTable
{
    private readonly int _imageSize;

    private IndexSubTableFormat2(ReadableFontData data, int first, int last) : base(data, first, last)
    {
        this._imageSize = this._data.readULongAsInt((int)Offset.indexSubTable2_imageSize);
    }

    public int imageSize()
    {
        return this._data.readULongAsInt((int)Offset.indexSubTable2_imageSize);
    }

    public BigGlyphMetrics bigMetrics()
    {
        return new BigGlyphMetrics(this._data.slice((int)Offset.indexSubTable2_bigGlyphMetrics,
            (int)BigGlyphMetrics.Offset.metricsLength));
    }

    public override int numGlyphs()
    {
        return this.lastGlyphIndex() - this.firstGlyphIndex() + 1;
    }

    public override int glyphStartOffset(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
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

    public interface IBuilder : IndexSubTable.IBuilder<IndexSubTableFormat2>
    {

    }

    private sealed class Builder : IndexSubTable.Builder<IndexSubTableFormat2>, IBuilder
    {

        private BigGlyphMetrics.IBuilder metrics;

        public static int dataLength(
            ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
        {
            return (int)Offset.indexSubTable2Length;
        }

        public Builder() : base((int)Offset.indexSubTable2_builderDataSize, Format.FORMAT_2)
        {
            this.metrics = BigGlyphMetrics.createBuilder();
        }

        public Builder(WritableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public Builder(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public override int numGlyphs()
        {
            return this.lastGlyphIndex() - this.firstGlyphIndex() + 1;
        }

        public override int glyphStartOffset(int glyphId)
        {
            int loca = base.checkGlyphRange(glyphId);
            return loca * this.imageSize();
        }

        public override int glyphLength(int glyphId)
        {
            base.checkGlyphRange(glyphId);
            return this.imageSize();
        }

        public int imageSize()
        {
            return this.internalReadData().readULongAsInt((int)Offset.indexSubTable2_imageSize);
        }

        public void setImageSize(int imageSize)
        {
            this.internalWriteData().writeULong((int)Offset.indexSubTable2_imageSize, imageSize);
        }

        public BigGlyphMetrics.IBuilder bigMetrics()
        {
            if (this.metrics == null)
            {
                WritableFontData data =
                    this.internalWriteData().slice((int)Offset.indexSubTable2_bigGlyphMetrics,
                        (int)BigGlyphMetrics.Offset.metricsLength);
                this.metrics = BigGlyphMetrics.createBuilder(data);
            }
            return this.metrics;
        }

        /*private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo>
        {
            private int glyphId;

            public BitmapGlyphInfoIterator()
            {
                this.glyphId = IndexSubTableFormat2.Builder.@this.firstGlyphIndex();
            }

            public override boolean hasNext()
            {
                if (this.glyphId <= IndexSubTableFormat2.Builder.@this.lastGlyphIndex())
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
                BitmapGlyphInfo info =
                    new BitmapGlyphInfo(this.glyphId, IndexSubTableFormat2.Builder.@this.imageDataOffset(),
                        IndexSubTableFormat2.Builder.@this.glyphStartOffset(this.glyphId),
                        IndexSubTableFormat2.Builder.@this.glyphLength(this.glyphId),
                        IndexSubTableFormat2.Builder.@this.imageFormat());
                this.glyphId++;
                return info;
            }

            public override void remove()
            {
                throw new UnsupportedOperationException("Unable to remove a glyph info.");
            }
        }*/

        public override IEnumerator<BitmapGlyphInfo> GetEnumerator()
        {
            return Enumerable
                .Range(firstGlyphIndex(), lastGlyphIndex() - firstGlyphIndex() + 1)
                .Select(glyphId => new BitmapGlyphInfo(glyphId, imageDataOffset(), glyphStartOffset(glyphId), glyphLength(glyphId), imageFormat()))
                .GetEnumerator();
            // return new BitmapGlyphInfoIterator();
        }

        public override IndexSubTableFormat2 subBuildTable(ReadableFontData data)
        {
            return new IndexSubTableFormat2(data, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            return (int)Offset.indexSubTable2Length;
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = base.serializeIndexSubHeader(newData);
            if (this.metrics == null)
            {
                size += this.internalReadData().slice(size).copyTo(newData.slice(size));
            }
            else
            {
                size += newData.writeLong((int)Offset.indexSubTable2_imageSize, this.imageSize());
                size += this.metrics.subSerialize(newData.slice(size));
            }
            return size;
        }
    }
}
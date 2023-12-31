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
using System.Linq;
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;











/**
 * Format 1 Index Subtable Entry.
 *
 * @author Stuart Gill
 *
 */
public sealed class IndexSubTableFormat1 : IndexSubTable
{
    private IndexSubTableFormat1(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
    {
    }

    public override int numGlyphs()
    {
        return this.lastGlyphIndex() - this.firstGlyphIndex() + 1;
    }

    public override int glyphStartOffset(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
        return this.loca(loca);
    }

    public override int glyphLength(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
        return this.loca(loca + 1) - this.loca(loca);
    }

    private int loca(int loca)
    {
        return this.imageDataOffset() + this._data.readULongAsInt(
            (int)Offset.indexSubTable1_offsetArray + loca * (int)FontData.DataSize.ULONG);
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

    public interface IBuilder : IndexSubTable.IBuilder<IndexSubTableFormat1>
    {

    }

    private sealed class Builder : IndexSubTable.Builder<IndexSubTableFormat1>, IBuilder
    {
        private IList<Integer> _offsetArray;

        public static int dataLength(
            ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
        {
            return (int)Offset.indexSubHeaderLength + (lastGlyphIndex - firstGlyphIndex + 1 + 1)
                * (int)FontData.DataSize.ULONG;
        }

        public Builder() : base((int)Offset.indexSubTable1_builderDataSize, Format.FORMAT_1)
        {
        }

        public Builder(WritableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public Builder(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public override int numGlyphs()
        {
            return this.getOffsetArray().Count - 1;
        }

        public override int glyphLength(int glyphId)
        {
            int loca = this.checkGlyphRange(glyphId);
            IList<Integer> offsetArray = this.getOffsetArray();
            return offsetArray.get(loca + 1) - offsetArray.get(loca);
        }

        public override int glyphStartOffset(int glyphId)
        {
            int loca = this.checkGlyphRange(glyphId);
            IList<Integer> offsetArray = this.getOffsetArray();
            return offsetArray.get(loca);
        }

        public IList<Integer> offsetArray()
        {
            return this.getOffsetArray();
        }

        private IList<Integer> getOffsetArray()
        {
            if (this.offsetArray == null)
            {
                this.initialize(this.internalReadData());
                this.setModelChanged();
            }
            return this._offsetArray;
        }

        private void initialize(ReadableFontData data)
        {
            if (this._offsetArray == null)
            {
                this._offsetArray = new List<Integer>();
            }
            else
            {
                this._offsetArray.Clear();
            }

            if (data != null)
            {
                int numOffsets = (this.lastGlyphIndex() - this.firstGlyphIndex() + 1) + 1;
                for (int i = 0; i < numOffsets; i++)
                {
                    this._offsetArray.Add(data.readULongAsInt(
                        (int)Offset.indexSubTable1_offsetArray + i * (int)FontData.DataSize.ULONG));
                }
            }
        }

        public void setOffsetArray(IList<Integer> array)
        {
            this._offsetArray = array;
            this.setModelChanged();
        }

        /*private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo>
        {
            private int glyphId;

            public BitmapGlyphInfoIterator()
            {
                this.glyphId = IndexSubTableFormat1.Builder.@this.firstGlyphIndex();
            }

            public override boolean hasNext()
            {
                if (this.glyphId <= IndexSubTableFormat1.Builder.@this.lastGlyphIndex())
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
                    new BitmapGlyphInfo(this.glyphId, IndexSubTableFormat1.Builder.@this.imageDataOffset(),
                        IndexSubTableFormat1.Builder.@this.glyphStartOffset(this.glyphId),
                        IndexSubTableFormat1.Builder.@this.glyphLength(this.glyphId),
                        IndexSubTableFormat1.Builder.@this.imageFormat());
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
            //return new BitmapGlyphInfoIterator();
        }

        public override void revert()
        {
            base.revert();
            this._offsetArray = null;
        }

        public override IndexSubTableFormat1 subBuildTable(ReadableFontData data)
        {
            return new IndexSubTableFormat1(data, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.offsetArray == null)
            {
                return this.internalReadData().length();
            }
            return (int)Offset.indexSubHeaderLength + this._offsetArray.Count
                * (int)FontData.DataSize.ULONG;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.offsetArray != null)
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
                size += this.internalReadData().slice((int)Offset.indexSubTable1_offsetArray).copyTo(
                    newData.slice((int)Offset.indexSubTable1_offsetArray));
            }
            else
            {
                foreach (Integer loca in this._offsetArray)
                {
                    size += newData.writeULong(size, loca);
                }
            }
            return size;
        }
    }
}
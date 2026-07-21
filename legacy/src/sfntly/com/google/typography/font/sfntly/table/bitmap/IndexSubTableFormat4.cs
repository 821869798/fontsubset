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
using System.Reflection.Metadata;
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;












/**
 * Format 4 Index Subtable Entry.
 * 
 * @author Stuart Gill
 * 
 */
public sealed class IndexSubTableFormat4 : IndexSubTable
{
    private IndexSubTableFormat4(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
    {
    }

    private static int numGlyphs(ReadableFontData data, int tableOffset)
    {
        int numGlyphs = data.readULongAsInt(tableOffset + (int)Offset.indexSubTable4_numGlyphs);
        return numGlyphs;
    }

    public override int numGlyphs()
    {
        return IndexSubTableFormat4.numGlyphs(this._data, 0);
    }

    public override int glyphStartOffset(int glyphId)
    {
        this.checkGlyphRange(glyphId);
        int pairIndex = this.findCodeOffsetPair(glyphId);
        if (pairIndex < 0)
        {
            return -1;
        }
        return this._data.readUShort(
            (int)Offset.indexSubTable4_glyphArray + pairIndex * (int)Offset.codeOffsetPairLength
                + (int)Offset.codeOffsetPair_offset);
    }

    public override int glyphLength(int glyphId)
    {
        this.checkGlyphRange(glyphId);
        int pairIndex = this.findCodeOffsetPair(glyphId);
        if (pairIndex < 0)
        {
            return -1;
        }
        return (this._data.readUShort((int)Offset.indexSubTable4_glyphArray + (pairIndex + 1)
            * (int)Offset.codeOffsetPairLength + (int)Offset.codeOffsetPair_offset))
            - this._data.readUShort((int)Offset.indexSubTable4_glyphArray + (pairIndex)
                * (int)Offset.codeOffsetPairLength + (int)Offset.codeOffsetPair_offset);
    }

    public int findCodeOffsetPair(int glyphId)
    {
        return this._data.searchUShort((int)Offset.indexSubTable4_glyphArray,
            (int)Offset.codeOffsetPairLength, this.numGlyphs(), glyphId);
    }

    public class CodeOffsetPair
    {
        public int _glyphCode;
        public int _offset;

        public CodeOffsetPair(int glyphCode, int offset)
        {
            this._glyphCode = glyphCode;
            this._offset = offset;
        }

        public int glyphCode()
        {
            return this._glyphCode;
        }

        public int offset()
        {
            return this._offset;
        }
    }

    public sealed class CodeOffsetPairBuilder : CodeOffsetPair
    {
        public CodeOffsetPairBuilder(int glyphCode, int offset) : base(glyphCode, offset)
        {
        }

        public void setGlyphCode(int glyphCode)
        {
            this._glyphCode = glyphCode;
        }

        public void set(int offset)
        {
            this._offset = offset;
        }
    }

    private sealed class CodeOffsetPairGlyphCodeComparator : IComparer<CodeOffsetPair>
    {
        public CodeOffsetPairGlyphCodeComparator()
        {
            // Prevent construction.
        }

        public int Compare(CodeOffsetPair p1, CodeOffsetPair p2)
        {
            return p1._glyphCode - p2._glyphCode;
        }
    }
    public static readonly IComparer<CodeOffsetPair> CodeOffsetPairComparatorByGlyphCode =
        new CodeOffsetPairGlyphCodeComparator();

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

    public interface IBuilder : IndexSubTable.IBuilder<IndexSubTableFormat4>
    {

    }

    private sealed class Builder : IndexSubTable.Builder<IndexSubTableFormat4>, IBuilder
    {
        private IList<CodeOffsetPairBuilder> offsetPairArray;

        public static int dataLength(
            ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
        {
            int numGlyphs = IndexSubTableFormat4.numGlyphs(data, indexSubTableOffset);
            return (int)Offset.indexSubTable4_glyphArray + numGlyphs
                * (int)Offset.indexSubTable4_codeOffsetPairLength;
        }

        public Builder() : base((int)Offset.indexSubTable4_builderDataSize, Format.FORMAT_4)
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
            this.checkGlyphRange(glyphId);
            int pairIndex = this.findCodeOffsetPair(glyphId);
            if (pairIndex == -1)
            {
                return -1;
            }
            return this.getOffsetArray().get(pairIndex + 1).offset()
                - this.getOffsetArray().get(pairIndex).offset();
        }

        public override int glyphStartOffset(int glyphId)
        {
            this.checkGlyphRange(glyphId);
            int pairIndex = this.findCodeOffsetPair(glyphId);
            if (pairIndex == -1)
            {
                return -1;
            }
            return this.getOffsetArray().get(pairIndex).offset();
        }

        public IList<CodeOffsetPairBuilder> offsetArray()
        {
            return this.getOffsetArray();
        }

        private IList<CodeOffsetPairBuilder> getOffsetArray()
        {
            if (this.offsetPairArray == null)
            {
                this.initialize(base.internalReadData());
                base.setModelChanged();
            }
            return this.offsetPairArray;
        }

        private void initialize(ReadableFontData data)
        {
            if (this.offsetPairArray == null)
            {
                this.offsetPairArray = new List<CodeOffsetPairBuilder>();
            }
            else
            {
                this.offsetPairArray.Clear();
            }

            if (data != null)
            {
                int numPairs = IndexSubTableFormat4.numGlyphs(data, 0) + 1;
                int offset = (int)Offset.indexSubTable4_glyphArray;
                for (int i = 0; i < numPairs; i++)
                {
                    int glyphCode =
                        data.readUShort(offset + (int)Offset.indexSubTable4_codeOffsetPair_glyphCode);
                    int glyphOffset =
                        data.readUShort(offset + (int)Offset.indexSubTable4_codeOffsetPair_offset);
                    offset += (int)Offset.indexSubTable4_codeOffsetPairLength;
                    CodeOffsetPairBuilder pairBuilder = new CodeOffsetPairBuilder(glyphCode, glyphOffset);
                    this.offsetPairArray.Add(pairBuilder);
                }
            }
        }

        private int findCodeOffsetPair(int glyphId)
        {
            IList<CodeOffsetPairBuilder> pairList = this.getOffsetArray();
            int location = 0;
            int bottom = 0;
            int top = pairList.Count;
            while (top != bottom)
            {
                location = (top + bottom) / 2;
                CodeOffsetPairBuilder pair = pairList.get(location);
                if (glyphId < pair.glyphCode())
                {
                    // location is below current location
                    top = location;
                }
                else if (glyphId > pair.glyphCode())
                {
                    // location is above current location
                    bottom = location + 1;
                }
                else
                {
                    return location;
                }
            }
            return -1;
        }
        public void setOffsetArray(IList<CodeOffsetPairBuilder> array) {
          this.offsetPairArray = array;
          this.setModelChanged();
        }

        /*
        private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo> {
          private int codeOffsetPairIndex;

          public BitmapGlyphInfoIterator() {
          }

          public override boolean hasNext() {
            if (this.codeOffsetPairIndex
                < IndexSubTableFormat4.Builder.@this.getOffsetArray().size() - 1) {
              return true;
            }
            return false;
          }

          public override BitmapGlyphInfo next() {
            if (!hasNext()) {
              throw new NoSuchElementException("No more characters to iterate.");
            }
            List<CodeOffsetPairBuilder> offsetArray =
                IndexSubTableFormat4.Builder.@this.getOffsetArray();
            CodeOffsetPair pair =
                offsetArray.get(this.codeOffsetPairIndex);
            BitmapGlyphInfo info = new BitmapGlyphInfo(pair.glyphCode(),
                IndexSubTableFormat4.Builder.@this.imageDataOffset(), pair.offset(),
                offsetArray.get(this.codeOffsetPairIndex + 1).offset() - pair.offset(),
                IndexSubTableFormat4.Builder.@this.imageFormat());
            this.codeOffsetPairIndex++;
            return info;
          }

          public override void remove() {
            throw new UnsupportedOperationException("Unable to remove a glyph info.");
          }
        }*/

        public override IEnumerator<BitmapGlyphInfo> GetEnumerator()
        {
            var offsetArray = getOffsetArray();

            return Enumerable.Range(0, offsetArray.Count - 1)
                .Select(index => (pair: offsetArray[index], next: offsetArray[index + 1]))
                .Select(x => new BitmapGlyphInfo(
                    x.pair.glyphCode(),
                    imageDataOffset(),
                    x.pair.offset(),
                    x.next.offset() - x.pair.offset(),
                    imageFormat()
                    )
                )
                .GetEnumerator();
            //return new BitmapGlyphInfoIterator();
        }

        public override void revert()
        {
            base.revert();
            this.offsetPairArray = null;
        }

        public override IndexSubTableFormat4 subBuildTable(ReadableFontData data)
        {
            return new IndexSubTableFormat4(data, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.offsetPairArray == null)
            {
                return this.internalReadData().length();
            }
            return (int)Offset.indexSubHeaderLength + (int)FontData.DataSize.ULONG
                + this.offsetPairArray.Count * (int)Offset.indexSubTable4_codeOffsetPairLength;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.offsetPairArray != null)
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
                size += this.internalReadData().slice((int)Offset.indexSubTable4_numGlyphs).copyTo(
                    newData.slice((int)Offset.indexSubTable4_numGlyphs));
            }
            else
            {

                size += newData.writeLong(size, this.offsetPairArray.Count - 1);
                foreach (CodeOffsetPair pair in this.offsetPairArray)
                {
                    size += newData.writeUShort(size, pair.glyphCode());
                    size += newData.writeUShort(size, pair.offset());
                }
            }
            return size;
        }
    }
}
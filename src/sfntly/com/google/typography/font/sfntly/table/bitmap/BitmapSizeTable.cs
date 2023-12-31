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
using com.google.typography.font.sfntly.math;
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;















public sealed class BitmapSizeTable : SubTable
{
    // binary search would be faster but many fonts have index subtables that
    // aren't sorted
    private static readonly boolean USE_BINARY_SEARCH = false;

    private readonly Object indexSubTablesLock = new Object();
    private volatile IList<IndexSubTable> indexSubTables = null;

    public BitmapSizeTable(ReadableFontData data, ReadableFontData masterData) : base(data, masterData)
    {
    }

    public int indexSubTableArrayOffset()
    {
        return this._data.readULongAsInt((int)Offset.bitmapSizeTable_indexSubTableArrayOffset);
    }

    public int indexTableSize()
    {
        return this._data.readULongAsInt((int)Offset.bitmapSizeTable_indexTableSize);
    }

    private static int numberOfIndexSubTables(ReadableFontData data, int tableOffset)
    {
        return data.readULongAsInt(tableOffset + (int)Offset.bitmapSizeTable_numberOfIndexSubTables);
    }

    public int numberOfIndexSubTables()
    {
        return BitmapSizeTable.numberOfIndexSubTables(this._data, 0);
    }

    public int colorRef()
    {
        return this._data.readULongAsInt((int)Offset.bitmapSizeTable_colorRef);
    }

    // TODO(stuartg): implement later
    public void /* SBitLineMetrics */hori()
    {
        // NOP
    }

    // TODO(stuartg): implement later
    public void /* SBitLineMetrics */vert()
    {
        // NOP
    }

    public int startGlyphIndex()
    {
        return this._data.readUShort((int)Offset.bitmapSizeTable_startGlyphIndex);
    }

    public int endGlyphIndex()
    {
        return this._data.readUShort((int)Offset.bitmapSizeTable_endGlyphIndex);
    }

    public int ppemX()
    {
        return this._data.readByte((int)Offset.bitmapSizeTable_ppemX);
    }

    public int ppemY()
    {
        return this._data.readByte((int)Offset.bitmapSizeTable_ppemY);
    }

    public int bitDepth()
    {
        return this._data.readByte((int)Offset.bitmapSizeTable_bitDepth);
    }

    public int flagsAsInt()
    {
        return this._data.readChar((int)Offset.bitmapSizeTable_flags);
    }

    public IndexSubTable indexSubTable(int index)
    {
        IList<IndexSubTable> subTableList = getIndexSubTableList();
        return subTableList.get(index);
    }

    public BitmapGlyphInfo glyphInfo(int glyphId)
    {
        IndexSubTable subTable = searchIndexSubTables(glyphId);
        if (subTable == null)
        {
            return null;
        }
        return subTable.glyphInfo(glyphId);
    }

    public int glyphOffset(int glyphId)
    {
        IndexSubTable subTable = searchIndexSubTables(glyphId);
        if (subTable == null)
        {
            return -1;
        }
        return subTable.glyphOffset(glyphId);
    }

    public int glyphLength(int glyphId)
    {
        IndexSubTable subTable = searchIndexSubTables(glyphId);
        if (subTable == null)
        {
            return -1;
        }
        return subTable.glyphLength(glyphId);
    }

    public int glyphFormat(int glyphId)
    {
        IndexSubTable subTable = searchIndexSubTables(glyphId);
        if (subTable == null)
        {
            return -1;
        }
        return subTable.imageFormat();
    }

    private IndexSubTable searchIndexSubTables(int glyphId)
    {
        // would be faster to binary search but too many size tables don't have
        // sorted subtables
        if (USE_BINARY_SEARCH)
        {
            return binarySearchIndexSubTables(glyphId);
        }
        return linearSearchIndexSubTables(glyphId);
    }

    private IndexSubTable linearSearchIndexSubTables(int glyphId)
    {
        foreach (IndexSubTable subTable in this.getIndexSubTableList())
        {
            if (subTable.firstGlyphIndex() <= glyphId && subTable.lastGlyphIndex() >= glyphId)
            {
                return subTable;
            }
        }
        return null;
    }

    private IndexSubTable binarySearchIndexSubTables(int glyphId)
    {
        IList<IndexSubTable> subTableList = getIndexSubTableList();
        int index = 0;
        int bottom = 0;
        int top = subTableList.Count;
        while (top != bottom)
        {
            index = (top + bottom) / 2;
            IndexSubTable subTable = subTableList.get(index);
            if (glyphId < subTable.firstGlyphIndex())
            {
                // location below current location
                top = index;
            }
            else
            {
                if (glyphId <= subTable.lastGlyphIndex())
                {
                    return subTable;
                }
                // location is above the current location
                bottom = index + 1;
            }
        }
        return null;
    }

    private IndexSubTable createIndexSubTable(int index)
    {
        return IndexSubTable.createIndexSubTable(
            this.masterReadData(), this.indexSubTableArrayOffset(), index);
    }

    private IList<IndexSubTable> getIndexSubTableList()
    {
        if (this.indexSubTables == null)
        {
            lock (this.indexSubTablesLock)
            {
                if (this.indexSubTables == null)
                {
                    IList<IndexSubTable> subTables =
                        new List<IndexSubTable>(this.numberOfIndexSubTables());
                    for (int i = 0; i < this.numberOfIndexSubTables(); i++)
                    {
                        subTables.Add(this.createIndexSubTable(i));
                    }
                    this.indexSubTables = subTables;
                }
            }
        }
        return this.indexSubTables;
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder("BitmapSizeTable: ");
        IList<IndexSubTable> indexSubTableList = this.getIndexSubTableList();
        sb.Append("[s=0x");
        sb.Append(NumberHelper.toHexString(this.startGlyphIndex()));
        sb.Append(", e=0x");
        sb.Append(NumberHelper.toHexString(this.endGlyphIndex()));
        sb.Append(", ppemx=");
        sb.Append(this.ppemX());
        sb.Append(", index subtables count=");
        sb.Append(this.numberOfIndexSubTables());
        sb.Append("]");
        for (int index = 0; index < indexSubTableList.Count; index++)
        {
            sb.Append("\n\t");
            sb.Append(index);
            sb.Append(": ");
            sb.Append(indexSubTableList.get(index));
            sb.Append(", ");
        }
        sb.Append("\n");
        return sb.ToString();
    }

    public static IBuilder createBuilder(WritableFontData data, ReadableFontData masterData)
    {
        return new Builder(data, masterData);
    }

    public static IBuilder createBuilder(ReadableFontData data, ReadableFontData masterData)
    {
        return new Builder(data, masterData);
    }

    public interface IBuilder : SubTable.IBuilder<BitmapSizeTable>
    {
        IDictionary<int, BitmapGlyphInfo> generateLocaMap();
        IList<IndexSubTable.IBuilder<IndexSubTable>> indexSubTableBuilders();
        void setIndexSubTableArrayOffset(int offset);
        void setIndexTableSize(int size);
    }

    private sealed class Builder : SubTable.Builder<BitmapSizeTable>, IBuilder
    {
        IList<IndexSubTable.IBuilder<IndexSubTable>> indexSubTables;

        public Builder(WritableFontData data, ReadableFontData masterData) : base(data, masterData)
        {
        }

        public Builder(ReadableFontData data, ReadableFontData masterData) : base(data, masterData)
        {
        }

        /**
         * Gets the subtable array offset as set in the original table as read from
         * the font file. This value cannot be explicitly set and will be generated
         * during table building.
         *
         * @return the subtable array offset
         */
        public int indexSubTableArrayOffset()
        {
            return this.internalReadData().readULongAsInt(
                (int)Offset.bitmapSizeTable_indexSubTableArrayOffset);
        }

        /**
         * Sets the subtable array offset. This is used only during the building
         * process when the objects are being serialized.
         *
         * @param (int)Offset.the to the index subtable array
         */
        public void setIndexSubTableArrayOffset(int offset)
        {
            this.internalWriteData().writeULong(
                (int)Offset.bitmapSizeTable_indexSubTableArrayOffset, offset);
        }

        /**
         * Gets the subtable array size as set in the original table as read from
         * the font file. This value cannot be explicitly set and will be generated
         * during table building.
         *
         * @return the subtable array size
         */
        public int indexTableSize()
        {
            return this.internalReadData().readULongAsInt((int)Offset.bitmapSizeTable_indexTableSize);
        }

        /**
         * Sets the subtable size. This is used only during the building process
         * when the objects are being serialized.
         *
         * @param size the offset to the index subtable array
         */
        public void setIndexTableSize(int size)
        {
            this.internalWriteData().writeULong((int)Offset.bitmapSizeTable_indexTableSize, size);
        }

        public int numberOfIndexSubTables()
        {
            return this.getIndexSubTableBuilders().Count;
        }

        private void setNumberOfIndexSubTables(int numberOfIndexSubTables)
        {
            this.internalWriteData().writeULong(
                (int)Offset.bitmapSizeTable_numberOfIndexSubTables, numberOfIndexSubTables);
        }

        public int colorRef()
        {
            return this.internalReadData().readULongAsInt((int)Offset.bitmapSizeTable_colorRef);
        }

        // TODO(stuartg): implement later
        public void /* SBitLineMetrics */hori()
        {
            // NOP
        }

        // TODO(stuartg): implement later
        public void /* SBitLineMetrics */vert()
        {
            // NOP
        }

        public int startGlyphIndex()
        {
            return this.internalReadData().readUShort((int)Offset.bitmapSizeTable_startGlyphIndex);
        }

        public int endGlyphIndex()
        {
            return this.internalReadData().readUShort((int)Offset.bitmapSizeTable_endGlyphIndex);
        }

        public int ppemX()
        {
            return this.internalReadData().readByte((int)Offset.bitmapSizeTable_ppemX);
        }

        public int ppemY()
        {
            return this.internalReadData().readByte((int)Offset.bitmapSizeTable_ppemY);
        }

        public int bitDepth()
        {
            return this.internalReadData().readByte((int)Offset.bitmapSizeTable_bitDepth);
        }

        public int flagsAsInt()
        {
            return this.internalReadData().readChar((int)Offset.bitmapSizeTable_flags);
        }

        public IndexSubTable.IBuilder<IndexSubTable> indexSubTableBuilder(int index)
        {
            var subTableList = getIndexSubTableBuilders();
            return subTableList.get(index);
        }

        public BitmapGlyphInfo glyphInfo(int glyphId)
        {
            var subTable = searchIndexSubTables(glyphId);
            if (subTable == null)
            {
                return null;
            }
            return subTable.glyphInfo(glyphId);
        }

        public int glyphOffset(int glyphId)
        {
            var subTable = searchIndexSubTables(glyphId);
            if (subTable == null)
            {
                return -1;
            }
            return subTable.glyphOffset(glyphId);
        }

        public int glyphLength(int glyphId)
        {
            var subTable = searchIndexSubTables(glyphId);
            if (subTable == null)
            {
                return -1;
            }
            return subTable.glyphLength(glyphId);
        }

        public int glyphFormat(int glyphId)
        {
            var subTable = searchIndexSubTables(glyphId);
            if (subTable == null)
            {
                return -1;
            }
            return subTable.imageFormat();
        }

        public IList<IndexSubTable.IBuilder<IndexSubTable>> indexSubTableBuilders()
        {
            return this.getIndexSubTableBuilders();
        }

        //private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo>
        //{
        //    IEnumerator<IndexSubTable.Builder<IndexSubTable>> subTableIter;
        //    IEnumerator<BitmapGlyphInfo> subTableGlyphInfoIter;

        //    public BitmapGlyphInfoIterator()
        //    {
        //        this.subTableIter = BitmapSizeTable.Builder.@this.getIndexSubTableBuilders().GetEnumerator();
        //    }

        //    public boolean MoveNext()
        //    {
        //        if (this.subTableGlyphInfoIter != null && this.subTableGlyphInfoIter.MoveNext())
        //        {
        //            return true;
        //        }
        //        while (subTableIter.MoveNext())
        //        {
        //            IndexSubTable.Builder<IndexSubTable> indexSubTable = this.subTableIter.Current;
        //            this.subTableGlyphInfoIter = indexSubTable.GetEnumerator();
        //            if (this.subTableGlyphInfoIter.MoveNext())
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }

        //    public override BitmapGlyphInfo next()
        //    {
        //        return this.subTableGlyphInfoIter.Current;
        //    }

        //    public override void remove()
        //    {
        //        throw new UnsupportedOperationException("Unable to remove a glyph info.");
        //    }
        //}

        public IEnumerator<BitmapGlyphInfo> GetEnumerator()
        {
            return getIndexSubTableBuilders().SelectMany(x => x.GetEnumerator().BoxEnumerator()).GetEnumerator();
            // return new BitmapGlyphInfoIterator();
        }

        public void revert()
        {
            this.indexSubTables = null;
            this.setModelChanged(false);
        }

        public IDictionary<Integer, BitmapGlyphInfo> generateLocaMap()
        {
            IDictionary<Integer, BitmapGlyphInfo> locaMap = new Dictionary<Integer, BitmapGlyphInfo>();
            var iter = this.GetEnumerator();
            while (iter.MoveNext())
            {
                BitmapGlyphInfo info = iter.Current;
                locaMap.put(info.glyphId(), info);
            }
            return locaMap;
        }

        private IndexSubTable.IBuilder<IndexSubTable> searchIndexSubTables(int glyphId)
        {
            // would be faster to binary search but too many size tables don't have
            // sorted subtables
            if (USE_BINARY_SEARCH)
            {
                return binarySearchIndexSubTables(glyphId);
            }
            return linearSearchIndexSubTables(glyphId);
        }

        private IndexSubTable.IBuilder<IndexSubTable> linearSearchIndexSubTables(int glyphId)
        {
            var subTableList =
                getIndexSubTableBuilders();
            foreach (var subTable in subTableList)
            {
                if (subTable.firstGlyphIndex() <= glyphId && subTable.lastGlyphIndex() >= glyphId)
                {
                    return subTable;
                }
            }
            return null;
        }

        private IndexSubTable.IBuilder<IndexSubTable> binarySearchIndexSubTables(int glyphId)
        {
            var subTableList = getIndexSubTableBuilders();
            int index = 0;
            int bottom = 0;
            int top = subTableList.Count;
            while (top != bottom)
            {
                index = (top + bottom) / 2;
                var subTable = subTableList.get(index);
                if (glyphId < subTable.firstGlyphIndex())
                {
                    // location below current location
                    top = index;
                }
                else
                {
                    if (glyphId <= subTable.lastGlyphIndex())
                    {
                        return subTable;
                    }
                    // location is above the current location
                    bottom = index + 1;
                }
            }
            return null;
        }

        private IList<IndexSubTable.IBuilder<IndexSubTable>> getIndexSubTableBuilders()
        {
            if (this.indexSubTables == null)
            {
                this.initialize(this.internalReadData());
                this.setModelChanged();
            }
            return this.indexSubTables;
        }

        private void initialize(ReadableFontData data)
        {
            if (this.indexSubTables == null)
            {
                this.indexSubTables = new List<IndexSubTable.IBuilder<IndexSubTable>>();
            }
            else
            {
                this.indexSubTables.Clear();
            }
            if (data != null)
            {
                int numberOfIndexSubTables = BitmapSizeTable.numberOfIndexSubTables(data, 0);
                for (int i = 0; i < numberOfIndexSubTables; i++)
                {
                    this.indexSubTables.Add(this.createIndexSubTableBuilder(i));
                }
            }
        }

        private IndexSubTable.IBuilder<IndexSubTable> createIndexSubTableBuilder(int index)
        {
            return IndexSubTable.createBuilder(
                this.masterReadData(), this.indexSubTableArrayOffset(), index);
        }

        public override BitmapSizeTable subBuildTable(ReadableFontData data)
        {
            return new BitmapSizeTable(data, this.masterReadData());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.indexSubTableBuilders() == null)
            {
                return 0;
            }
            int size = (int)Offset.bitmapSizeTableLength;
            boolean variable = false;
            foreach (IndexSubTable.Builder<IndexSubTable> subTableBuilder in this.indexSubTables)
            {
                size += (int)Offset.indexSubTableEntryLength;
                int subTableSize = subTableBuilder.subDataSizeToSerialize();
                int padding =
                    FontMath.paddingRequired(Math.Abs(subTableSize), (int)FontData.DataSize.ULONG);
                variable = subTableSize > 0 ? variable : true;
                size += Math.Abs(subTableSize) + padding;
            }
            return variable ? -size : size;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.indexSubTableBuilders() == null)
            {
                return false;
            }
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            this.setNumberOfIndexSubTables(this.indexSubTableBuilders().Count);
            int size = this.internalReadData().copyTo(newData);
            return size;
        }
    }
}
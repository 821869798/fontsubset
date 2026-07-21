/*
 * Copyright 2011 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.math;
using System.Diagnostics;

namespace com.google.typography.font.sfntly.table.bitmap;













/**
 * @author Stuart Gill
 *
 */
public class EblcTable : SubTableContainerTable
{
    private static readonly boolean DEBUG = false;

    public static readonly int NOTDEF = -1;

    public enum Offset
    {
        // header
        version = (0),
        numSizes = (4),
        headerLength = (numSizes + FontData.DataSize.ULONG),

        // bitmapSizeTable
        bitmapSizeTableArrayStart = (headerLength),
        bitmapSizeTableLength = (48),
        bitmapSizeTable_indexSubTableArrayOffset = (0),
        bitmapSizeTable_indexTableSize = (4),
        bitmapSizeTable_numberOfIndexSubTables = (8),
        bitmapSizeTable_colorRef = (12),
        bitmapSizeTable_hori = (16),
        bitmapSizeTable_vert = (28),
        bitmapSizeTable_startGlyphIndex = (40),
        bitmapSizeTable_endGlyphIndex = (42),
        bitmapSizeTable_ppemX = (44),
        bitmapSizeTable_ppemY = (45),
        bitmapSizeTable_bitDepth = (46),
        bitmapSizeTable_flags = (47),

        // sbitLineMetrics
        sbitLineMetricsLength = (12),
        sbitLineMetrics_ascender = (0),
        sbitLineMetrics_descender = (1),
        sbitLineMetrics_widthMax = (2),
        sbitLineMetrics_caretSlopeNumerator = (3),
        sbitLineMetrics__caretSlopeDenominator = (4),
        sbitLineMetrics_caretOffset = (5),
        sbitLineMetrics_minOriginSB = (6),
        sbitLineMetrics_minAdvanceSB = (7),
        sbitLineMetrics_maxBeforeBL = (8),
        sbitLineMetrics_minAfterBL = (9),
        sbitLineMetrics_pad1 = (10),
        sbitLineMetrics_pad2 = (11),

        // indexSubTable
        indexSubTableEntryLength = (8),
        indexSubTableEntry_firstGlyphIndex = (0),
        indexSubTableEntry_lastGlyphIndex = (2),
        indexSubTableEntry_additionalOffsetToIndexSubtable = (4),

        // indexSubHeader
        indexSubHeaderLength = (8),
        indexSubHeader_indexFormat = (0),
        indexSubHeader_imageFormat = (2),
        indexSubHeader_imageDataOffset = (4),

        // indexSubTable - all offset relative to the subtable start

        // indexSubTable1
        indexSubTable1_offsetArray = (indexSubHeaderLength),
        indexSubTable1_builderDataSize = (indexSubHeaderLength),

        // indexSubTable2
        indexSubTable2Length = (indexSubHeaderLength + FontData.DataSize.ULONG + BitmapGlyph.Offset.bigGlyphMetricsLength),
        indexSubTable2_imageSize = (indexSubHeaderLength),
        indexSubTable2_bigGlyphMetrics = (indexSubTable2_imageSize + FontData.DataSize.ULONG),
        indexSubTable2_builderDataSize = (indexSubTable2_bigGlyphMetrics + BigGlyphMetrics.Offset.metricsLength),

        // indexSubTable3
        indexSubTable3_offsetArray = (indexSubHeaderLength),
        indexSubTable3_builderDataSize = (indexSubTable3_offsetArray),

        // indexSubTable4
        indexSubTable4_numGlyphs = (indexSubHeaderLength),
        indexSubTable4_glyphArray = (indexSubTable4_numGlyphs + FontData.DataSize.ULONG),
        indexSubTable4_codeOffsetPairLength = (2 * FontData.DataSize.USHORT),
        indexSubTable4_codeOffsetPair_glyphCode = (0),
        indexSubTable4_codeOffsetPair_offset = (FontData.DataSize.USHORT),
        indexSubTable4_builderDataSize = (indexSubTable4_glyphArray),

        // indexSubTable5
        indexSubTable5_imageSize = (indexSubHeaderLength),
        indexSubTable5_bigGlyphMetrics = (indexSubTable5_imageSize + FontData.DataSize.ULONG),
        indexSubTable5_numGlyphs = (indexSubTable5_bigGlyphMetrics + BitmapGlyph.Offset.bigGlyphMetricsLength),
        indexSubTable5_glyphArray = (indexSubTable5_numGlyphs + FontData.DataSize.ULONG),
        indexSubTable5_builderDataSize = (indexSubTable5_glyphArray),

        // codeOffsetPair
        codeOffsetPairLength = (2 * FontData.DataSize.USHORT),
        codeOffsetPair_glyphCode = (0),
        codeOffsetPair_offset = (FontData.DataSize.USHORT)/*;


     readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    /**
     * Lock on all operations that will affect the value of the bitmapSizeTable.
     */
    private readonly Object bitmapSizeTableLock = new Object();
    private volatile IList<BitmapSizeTable> _bitmapSizeTable;

    /**
     * @param header
     * @param data
     */
    public EblcTable(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public int version()
    {
        return this._data.readFixed((int)Offset.version);
    }

    public int numSizes()
    {
        return this._data.readULongAsInt((int)Offset.numSizes);
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder(base.ToString());
        sb.Append("\nnum sizes = ");
        sb.Append(this.numSizes());
        sb.Append("\n");
        for (int i = 0; i < this.numSizes(); i++)
        {
            sb.Append(i);
            sb.Append(": ");
            BitmapSizeTable size = this.bitmapSizeTable(i);
            sb.Append(size.ToString());
        }
        return sb.ToString();
    }

    public BitmapSizeTable bitmapSizeTable(int index)
    {
        if (index < 0 || index > this.numSizes())
        {
            throw new IndexOutOfBoundsException("Size table index is outside of the range of tables.");
        }
        IList<BitmapSizeTable> bitmapSizeTableList = getBitmapSizeTableList();
        return bitmapSizeTableList.get(index);
    }

    private IList<BitmapSizeTable> getBitmapSizeTableList()
    {
        if (this.bitmapSizeTable == null)
        {
            lock (this.bitmapSizeTableLock)
            {
                if (this.bitmapSizeTable == null)
                {
                    this._bitmapSizeTable = createBitmapSizeTable(this._data, this.numSizes());
                }
            }
        }
        return this._bitmapSizeTable;
    }

    private static IList<BitmapSizeTable> createBitmapSizeTable(ReadableFontData data, int numSizes)
    {
        IList<BitmapSizeTable> bitmapSizeTable = new List<BitmapSizeTable>();
        for (int i = 0; i < numSizes; i++)
        {
            var sizeBuilder =
                BitmapSizeTable.createBuilder(data.slice(
                    (int)Offset.bitmapSizeTableArrayStart + i * (int)Offset.bitmapSizeTableLength,
                    (int)Offset.bitmapSizeTableLength), data);
            BitmapSizeTable size = sizeBuilder.build();
            bitmapSizeTable.Add(size);
        }
        return Collections.unmodifiableList(bitmapSizeTable);
    }

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, ReadableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : SubTableContainerTable.IBuilder<EblcTable>
    {

    }

    private sealed class Builder : SubTableContainerTable.Builder<EblcTable>, IBuilder
    {
        private readonly int version = 0x00020000; // TODO(user) constant/enum
        private IList<BitmapSizeTable.IBuilder> sizeTableBuilders;

        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public IList<BitmapSizeTable.IBuilder> bitmapSizeBuilders()
        {
            return this.getSizeList();
        }

        public void revert()
        {
            this.sizeTableBuilders = null;
            this.setModelChanged(false);
        }

        /**
         * Generates the loca list for the EBDT table. The list is intended to be
         * used by the EBDT to allow it to parse the glyph data and generate glyph
         * objects. After returning from this method the list belongs to the caller.
         * The list entries are in the same order as the size table builders are at
         * the time of this call.
         *
         * @return the list of loca maps with one for each size table builder
         */
        public IList<IDictionary<Integer, BitmapGlyphInfo>> generateLocaList()
        {
            var sizeBuilderList = this.getSizeList();

            IList<IDictionary<Integer, BitmapGlyphInfo>> locaList =
                new List<IDictionary<Integer, BitmapGlyphInfo>>(sizeBuilderList.Count);
            int sizeIndex = 0;
            foreach (var sizeBuilder in sizeBuilderList)
            {
                if (DEBUG)
                {
                    Debug.WriteLine("size table = %d%n", sizeIndex++);
                }
                IDictionary<Integer, BitmapGlyphInfo> locaMap = sizeBuilder.generateLocaMap();
                locaList.Add(locaMap);
            }

            return locaList;
        }

        private IList<BitmapSizeTable.IBuilder> getSizeList()
        {
            if (this.sizeTableBuilders == null)
            {
                this.sizeTableBuilders = this.initialize(this.internalReadData());
                base.setModelChanged();
            }
            return this.sizeTableBuilders;
        }

        private IList<BitmapSizeTable.IBuilder> initialize(ReadableFontData data)
        {
            IList<BitmapSizeTable.IBuilder> sizeBuilders = new List<BitmapSizeTable.IBuilder>();

            if (data != null)
            {
                int numSizes = data.readULongAsInt((int)Offset.numSizes);
                for (int i = 0; i < numSizes; i++)
                {
                    var sizeBuilder = BitmapSizeTable.createBuilder(
                            data.slice((int)Offset.bitmapSizeTableArrayStart + i
                                * (int)Offset.bitmapSizeTableLength, (int)Offset.bitmapSizeTableLength),
                            data);
                    sizeBuilders.Add(sizeBuilder);
                }
            }
            return sizeBuilders;
        }

        public override EblcTable subBuildTable(ReadableFontData data)
        {
            return new EblcTable(this.header(), data);
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.sizeTableBuilders == null)
            {
                return 0;
            }
            int size = (int)Offset.headerLength;
            boolean variable = false;
            int sizeIndex = 0;
            foreach (var sizeBuilder in this.sizeTableBuilders)
            {
                int sizeBuilderSize = sizeBuilder.subDataSizeToSerialize();
                if (DEBUG)
                {
                    Debug.WriteLine("sizeIndex = 0x%x, sizeBuilderSize = 0x%x%n", sizeIndex++,
                        sizeBuilderSize);
                }
                variable = sizeBuilderSize > 0 ? variable : true;
                size += Math.Abs(sizeBuilderSize);
            }
            return variable ? -size : size;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.sizeTableBuilders == null)
            {
                return false;
            }
            foreach (var sizeBuilder in this.sizeTableBuilders)
            {
                if (!sizeBuilder.subReadyToSerialize())
                {
                    return false;
                }
            }
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            // header
            int size = newData.writeFixed(0, this.version);
            size += newData.writeULong(size, this.sizeTableBuilders.Count);

            // calculate the offsets

            // offset to the start of the size table array
            int sizeTableStartOffset = size;
            // walking offset in the size table array
            int sizeTableOffset = sizeTableStartOffset;

            // offset to the start of the whole index subtable block
            int subTableBlockStartOffset =
                sizeTableOffset + this.sizeTableBuilders.Count * (int)Offset.bitmapSizeTableLength;
            // walking offset in the index subtable
            // points to the start of the current subtable block
            int currentSubTableBlockStartOffset = subTableBlockStartOffset;

            int sizeIndex = 0;
            foreach (var sizeBuilder in this.sizeTableBuilders)
            {
                sizeBuilder.setIndexSubTableArrayOffset(currentSubTableBlockStartOffset);
                var indexSubTableBuilderList = sizeBuilder.indexSubTableBuilders();

                // walking offset within the current subTable array
                int indexSubTableArrayOffset = currentSubTableBlockStartOffset;
                // walking offset within the subTable entries
                int indexSubTableOffset = indexSubTableArrayOffset + indexSubTableBuilderList.Count
                    * (int)Offset.indexSubHeaderLength;

                if (DEBUG)
                {
                    Debug.WriteLine(
                        "size %d: sizeTable = %x, current subTable Block = %x, index subTable Start = %x%n",
                        sizeIndex, sizeTableOffset, currentSubTableBlockStartOffset, indexSubTableOffset);
                    sizeIndex++;
                }
                int subTableIndex = 0;

                foreach (var indexSubTableBuilder in indexSubTableBuilderList)
                {
                    if (DEBUG)
                    {
                        Debug.WriteLine("\tsubTableIndex %d: format = %x, ", subTableIndex,
                            indexSubTableBuilder.indexFormat());
                        Debug.WriteLine("indexSubTableArrayOffset = %x, indexSubTableOffset = %x%n",
                            indexSubTableArrayOffset, indexSubTableOffset);
                        subTableIndex++;
                    }
                    // array entry
                    indexSubTableArrayOffset +=
                        newData.writeUShort(indexSubTableArrayOffset, indexSubTableBuilder.firstGlyphIndex());
                    indexSubTableArrayOffset +=
                        newData.writeUShort(indexSubTableArrayOffset, indexSubTableBuilder.lastGlyphIndex());
                    indexSubTableArrayOffset += newData.writeULong(
                        indexSubTableArrayOffset, indexSubTableOffset - currentSubTableBlockStartOffset);

                    // index sub table
                    int currentSubTableSize =
                        indexSubTableBuilder.subSerialize(newData.slice(indexSubTableOffset));
                    int padding =
                        FontMath.paddingRequired(currentSubTableSize, (int)FontData.DataSize.ULONG);
                    if (DEBUG)
                    {
                        Debug.WriteLine(
                            "\t\tsubTableSize = %x, padding = %x%n", currentSubTableSize, padding);
                    }

                    indexSubTableOffset += currentSubTableSize;
                    indexSubTableOffset += newData.writePadding(indexSubTableOffset, padding);
                }

                // serialize size table
                sizeBuilder.setIndexTableSize(indexSubTableOffset - currentSubTableBlockStartOffset);
                sizeTableOffset += sizeBuilder.subSerialize(newData.slice(sizeTableOffset));

                currentSubTableBlockStartOffset = indexSubTableOffset;
            }
            return size + currentSubTableBlockStartOffset;
        }
    }
}
/*
 * Copyright 2010 Google Inc. All Rights Reserved.
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

namespace com.google.typography.font.sfntly.table.core;

using static com.google.typography.font.sfntly.Font;
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table;
using static com.google.typography.font.sfntly.table.core.CMap;

/**
 * A CMap table.
 *
 * @author Stuart Gill
 */
public sealed class CMapTable : SubTableContainerTable {

    /**
     * The .notdef glyph.
     */
    public static readonly int NOTDEF = 0;

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    public enum Offset : int {
        version = (0),
        numTables = (2),
        encodingRecordStart = (4),

        // offsets relative to the encoding record
        encodingRecordPlatformId = (0),
        encodingRecordEncodingId = (2),
        encodingRecordOffset = (4),
        encodingRecordSize = (8),

        format = (0),

        // Format 0: Byte encoding table
        format0Format = (0),
        format0Length = (2),
        format0Language = (4),
        format0GlyphIdArray = (6),

        // Format 2: High-byte mapping through table
        format2Format = (0),
        format2Length = (2),
        format2Language = (4),
        format2SubHeaderKeys = (6),
        format2SubHeaders = (518),
        // offset relative to the subHeader structure
        format2SubHeader_firstCode = (0),
        format2SubHeader_entryCount = (2),
        format2SubHeader_idDelta = (4),
        format2SubHeader_idRangeOffset = (6),
        format2SubHeader_structLength = (8),

        // Format 4: Segment mapping to delta values
        format4Format = (0),
        format4Length = (2),
        format4Language = (4),
        format4SegCountX2 = (6),
        format4SearchRange = (8),
        format4EntrySelector = (10),
        format4RangeShift = (12),
        format4EndCount = (14),
        format4FixedSize = (16),

        // format 6: Trimmed table mapping
        format6Format = (0),
        format6Length = (2),
        format6Language = (4),
        format6FirstCode = (6),
        format6EntryCount = (8),
        format6GlyphIdArray = (10),

        // Format 8: mixed 16-bit and 32-bit coverage
        format8Format = (0),
        format8Length = (4),
        format8Language = (8),
        format8Is32 = (12),
        format8nGroups = (8204),
        format8Groups = (8208),
        // ofset relative to the group structure
        format8Group_startCharCode = (0),
        format8Group_endCharCode = (4),
        format8Group_startGlyphId = (8),
        format8Group_structLength = (12),

        // Format 10: Trimmed array
        format10Format = (0),
        format10Length = (4),
        format10Language = (8),
        format10StartCharCode = (12),
        format10NumChars = (16),
        format10Glyphs = (20),

        // Format 12: Segmented coverage
        format12Format = (0),
        format12Length = (4),
        format12Language = (8),
        format12nGroups = (12),
        format12Groups = (16),
        format12Groups_structLength = (12),
        // offsets within the group structure
        format12_startCharCode = (0),
        format12_endCharCode = (4),
        format12_startGlyphId = (8),

        // Format 13: Last Resort Font
        format13Format = (0),
        format13Length = (4),
        format13Language = (8),
        format13nGroups = (12),
        format13Groups = (16),
        format13Groups_structLength = (12),
        // offsets within the group structure
        format13_startCharCode = (0),
        format13_endCharCode = (4),
        format13_glyphId = (8),

        // TODO: finish support for format 14
        // Format 14: Unicode Variation Sequences
        format14Format = (0),
        format14Length = (2),
    }

    public sealed class CMapId : IComparable<CMapId> {

        public static readonly CMapId WINDOWS_BMP =
            CMapId.getInstance((int)PlatformId.Windows, (int)WindowsEncodingId.UnicodeUCS2);
        public static readonly CMapId WINDOWS_UCS4 =
            CMapId.getInstance((int)PlatformId.Windows, (int)WindowsEncodingId.UnicodeUCS4);
        public static readonly CMapId MAC_ROMAN =
            CMapId.getInstance((int)PlatformId.Macintosh, (int)MacintoshEncodingId.Roman);

        public static CMapId getInstance(int platformId, int encodingId) {
            return new CMapId(platformId, encodingId);
        }

        private readonly int _platformId;
        private readonly int _encodingId;

        private CMapId(int platformId, int encodingId) {
            this._platformId = platformId;
            this._encodingId = encodingId;
        }

        public int platformId() {
            return this._platformId;
        }

        public int encodingId() {
            return this._encodingId;
        }

        public override boolean Equals(Object obj) {
            if (obj == this) {
                return true;
            }
            if (!(obj is CMapId)) {
                return false;
            }
            CMapId otherKey = (CMapId)obj;
            if ((otherKey._platformId == this._platformId) && (otherKey._encodingId == this._encodingId)) {
                return true;
            }
            return false;
        }

        public override int GetHashCode() {
            return this._platformId << 8 | this._encodingId;
        }

        public int CompareTo(CMapId o) {
            return this.GetHashCode() - o.GetHashCode();
        }

        public override String ToString() {
            StringBuilder b = new StringBuilder();
            b.Append("pid = ");
            b.Append(this._platformId);
            b.Append(", eid = ");
            b.Append(this._encodingId);
            return b.ToString();
        }
    }

    /**
     * Constructor.
     *
     * @param header header for the table
     * @param data data for the table
     */
    private CMapTable(Header header, ReadableFontData data) :base(header, data) {

    }

    /**
     * Get the table version.
     *
     * @return table version
     */
    public int version() {
        return this._data.readUShort((int)Offset.version);
    }

    /**
     * Gets the number of cmaps within the CMap table.
     *
     * @return the number of cmaps
     */
    public int numCMaps() {
        return this._data.readUShort((int)Offset.numTables);
    }

    /**
     * Returns the index of the cmap with the given CMapId in the table or -1 if a cmap with the
     * CMapId does not exist in the table.
     *
     * @param id the id of the cmap to get the index for; this value cannot be null
     * @return the index of the cmap in the table or -1 if the cmap with the CMapId does not exist in
     *         the table
     */
    // TODO Modify the iterator to be index-based and used here
    public int getCmapIndex(CMapId id) {
        for (int index = 0; index < numCMaps(); index++) {
            if (id.Equals(cmapId(index))) {
                return index;
            }
        }

        return -1;
    }

    /**
     * Gets the offset in the table data for the encoding record for the cmap with
     * the given index. The offset is from the beginning of the table.
     *
     * @param index the index of the cmap
     * @return offset in the table data
     */
    private static int offsetForEncodingRecord(int index) {
        return (int)Offset.encodingRecordStart + index * (int)Offset.encodingRecordSize;
    }

    /**
     * Gets the cmap id for the cmap with the given index.
     *
     * @param index the index of the cmap
     * @return the cmap id
     */
    public CMapId cmapId(int index) {
        return CMapId.getInstance(platformId(index), encodingId(index));
    }

    /**
     * Gets the platform id for the cmap with the given index.
     *
     * @param index the index of the cmap
     * @return the platform id
     */
    public int platformId(int index) {
        return this._data.readUShort(
            (int)Offset.encodingRecordPlatformId + CMapTable.offsetForEncodingRecord(index));
    }

    /**
     * Gets the encoding id for the cmap with the given index.
     *
     * @param index the index of the cmap
     * @return the encoding id
     */
    public int encodingId(int index) {
        return this._data.readUShort(
            (int)Offset.encodingRecordEncodingId + CMapTable.offsetForEncodingRecord(index));
    }

    /**
     * Gets the offset in the table data for the cmap table with the given index.
     * The offset is from the beginning of the table.
     *
     * @param index the index of the cmap
     * @return the offset in the table data
     */
    public int offset(int index) {
        return this._data.readULongAsInt(
            (int)(int)Offset.encodingRecordOffset + CMapTable.offsetForEncodingRecord(index));
    }

    /**
     * Gets an iterator over all of the cmaps within this CMapTable.
     */
    public IEnumerator<CMap> GetEnumerator() {
        // return new CMapIterator();
        return iterator(_ => true);
    }

    /**
     * Gets an iterator over the cmaps within this CMap table using the provided
     * filter to select the cmaps returned.
     *
     * @param filter the filter
     * @return iterator over cmaps
     */
    public IEnumerator<CMap> iterator(CMapFilter filter) {
        return iterator(filter.accept);
    }

    public IEnumerator<CMap> iterator(Func<CMapId, bool> filter)
    {
        // return new CMapIterator(filter);
        return Enumerable
            .Range(0, numCMaps())
            .Where(index => filter(cmapId(index)))
            .Select(index => cmap(index))
            .GetEnumerator();
    }

    public override String ToString() {
        StringBuilder sb = new StringBuilder(base.ToString());
        sb.Append(" = { ");
        for (int i = 0; i < this.numCMaps(); i++) {
            CMap cmap;
            try {
                cmap = this.cmap(i);
            } catch (IOException e) {
                continue;
            }
            sb.Append("[0x");
            sb.Append(NumberHelper.toHexString(this.offset(i)));
            sb.Append(" = ");
            sb.Append(cmap);
            if (i < this.numCMaps() - 1) {
                sb.Append("], ");
            } else {
                sb.Append("]");
            }
        }
        sb.Append(" }");
        return sb.ToString();
    }

    /**
     * A filter on cmaps.
     */
    public interface CMapFilter {
        /**
         * Test on whether the cmap is acceptable or not.
         *
         * @param cmapId the id of the cmap
         * @return true if the cmap is acceptable; false otherwise
         */
        boolean accept(CMapId cmapId);
    }
    /*
    private class CMapIterator : IEnumerator<CMap> {
        private int tableIndex = 0;
        private CMapFilter filter;

        private CMapIterator() {
            // no filter - iterate over all cmap subtables
        }

        private CMapIterator(CMapFilter filter) {
            this.filter = filter;
        }

        public override boolean hasNext() {
            if (this.filter == null) {
                if (this.tableIndex < numCMaps()) {
                    return true;
                }
                return false;
            }
            for (; this.tableIndex < numCMaps(); this.tableIndex++) {
                if (filter.accept(cmapId(this.tableIndex))) {
                    return true;
                }
            }
            return false;
        }

        public override CMap next() {
            if (!hasNext()) {
                throw new NoSuchElementException();
            }
            try {
                return cmap(this.tableIndex++);
            } catch (IOException e) {
                NoSuchElementException newException =
                    new NoSuchElementException("Error during the creation of the CMap.");
                newException.initCause(e);
                throw newException;
            }
        }

        public override void remove() {
            throw new UnsupportedOperationException("Cannot remove a CMap table from an existing font.");
        }
    }
    */
    /**
     * Gets the cmap for the given index.
     *
     * @param index the index of the cmap
     * @return the cmap at the index
     * @
     */
    public CMap cmap(int index) {
        var builder = CMapTable.cmapBuilder(this.readFontData(), index);
        return builder.build();
    }

    /**
     * Gets the cmap with the given ids if it exists.
     *
     * @param platformId the platform id
     * @param encodingId the encoding id
     * @return the cmap if it exists; null otherwise
     */
    public CMap cmap(int platformId, int encodingId) {
        return cmap(CMapId.getInstance(platformId, encodingId));
    }

    public CMap cmap(CMapId cmapId) {
        /*Iterator<CMap> cmapIter = this.iterator(new CMapFilter() {
      public override boolean accept(CMapId foundCMapId) {
        if (cmapId.equals(foundCMapId)) {
          return true;
        }
        return false;
      }
    })*/;
        // can only be one cmap for each set of ids

        var cmapIter = this.iterator(foundCMapId => cmapId.Equals(foundCMapId));

        if (cmapIter.MoveNext()) {
            return cmapIter.Current;
        }
        return null;
    }


    /**
     * Static factory method to create a cmap subtable builder.
     *
     * @param data the data for the whole cmap table
     * @param index the index of the cmap subtable within the table
     * @return the cmap subtable requested if it exists; null otherwise
     */
    public static CMap.IBuilder<CMap> cmapBuilder(ReadableFontData data, int index)
    {
        if (index < 0 || index > Builder.numCMaps(data))
        {
            throw new IndexOutOfBoundsException(
                "CMap table is outside the bounds of the known tables.");
        }

        // read from encoding records
        int platformId = data.readUShort(
            (int)Offset.encodingRecordPlatformId + CMapTable.offsetForEncodingRecord(index));
        int encodingId = data.readUShort(
            (int)Offset.encodingRecordEncodingId + CMapTable.offsetForEncodingRecord(index));
        int offset = data.readULongAsInt(
            (int)(int)Offset.encodingRecordOffset + CMapTable.offsetForEncodingRecord(index));
        CMapId cmapId = CMapId.getInstance(platformId, encodingId);

        var builder = CMap.getBuilder(data, offset, cmapId);
        return builder;
    }

    /**
     * Creates a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : SubTableContainerTable.IBuilder<CMapTable>
    {
        CMap.IBuilder<CMap> newCMapBuilder(CMapId cmapId, ReadableFontData data);

        CMap.IBuilder<CMap> newCMapBuilder(CMapId cmapId, CMapFormat cmapFormat);

        CMap.IBuilder<CMap> cmapBuilder(CMapId cmapId);
    }

    /**
     * CMap Table Builder.
     *
     */
    private class Builder : SubTableContainerTable.Builder<CMapTable>, IBuilder
    {

        private int _version = 0; // TODO(stuartg): make a CMapTable constant
        private IDictionary<CMapId, CMap.IBuilder<CMap>> cmapBuilders;


        /**
         * Constructor.
         *
         * @param header the table header
         * @param data the writable data for the table
         */
        public Builder(Header header, WritableFontData data) : base(header, data) {

        }

        /**
         * Constructor. This constructor will try to maintain the data as readable
         * but if editing operations are attempted then a writable copy will be made
         * the readable data will be discarded.
         *
         * @param header the table header
         * @param data the readable data for the table
         */
        public Builder(Header header, ReadableFontData data) : base(header, data) {

        }

        public override void subDataSet() {
            this.cmapBuilders = null;
            base.setModelChanged(false);
        }

        private void initialize(ReadableFontData data) {
            this.cmapBuilders = new /*TreeMap*/ Dictionary<CMapId, CMap.IBuilder<CMap>>();

            int numCMaps = Builder.numCMaps(data);
            for (int i = 0; i < numCMaps; i++) {
                var cmapBuilder = CMapTable.cmapBuilder(data, i);
                cmapBuilders.put(cmapBuilder.cmapId(), cmapBuilder);
            }
        }

        private IDictionary<CMapId, CMap.IBuilder<CMap>> getCMapBuilders() {
            if (this.cmapBuilders != null) {
                return this.cmapBuilders;
            }
            this.initialize(this.internalReadData());
            this.setModelChanged();

            return this.cmapBuilders;
        }

        public static int numCMaps(ReadableFontData data) {
            if (data == null) {
                return 0;
            }
            return data.readUShort((int)Offset.numTables);
        }

        public int numCMaps() {
            return this.getCMapBuilders().Count;
        }

        public override int subDataSizeToSerialize() {
            if (this.cmapBuilders == null || this.cmapBuilders.Count == 0) {
                return 0;
            }

            boolean variable = false;
            int size = (int)CMapTable.Offset.encodingRecordStart + this.cmapBuilders.Count
            * (int)CMapTable.Offset.encodingRecordSize;

            // calculate size of each table
            foreach (var b in this.cmapBuilders.values()) {
                int cmapSize = b.subDataSizeToSerialize();
                size += Math.Abs(cmapSize);
                variable |= cmapSize <= 0;
            }
            return variable ? -size : size;
        }

        public override boolean subReadyToSerialize() {
            if (this.cmapBuilders == null) {
                return false;
            }
            // check each table
            foreach (var b in this.cmapBuilders.values()) {
                if (!b.subReadyToSerialize()) {
                    return false;
                }
            }
            return true;
        }

        public override int subSerialize(WritableFontData newData) {
            int size = newData.writeUShort((int)CMapTable.Offset.version, this.version());
            size += newData.writeUShort((int)CMapTable.Offset.numTables, this.cmapBuilders.Count);

            int indexOffset = size;
            size += this.cmapBuilders.Count * (int)CMapTable.Offset.encodingRecordSize;
            foreach (var b in this.cmapBuilders.values()) {
                // header entry
                indexOffset += newData.writeUShort(indexOffset, b.platformId());
                indexOffset += newData.writeUShort(indexOffset, b.encodingId());
                indexOffset += newData.writeULong(indexOffset, size);

                // cmap
                size += b.subSerialize(newData.slice(size));
            }
            return size;
        }

        public override CMapTable subBuildTable(ReadableFontData data) {
            return new CMapTable(this.header(), data);
        }

        // public building API

        public IEnumerator<CMap.IBuilder<CMap>> GetEnumerator() {
            return this.getCMapBuilders().values().GetEnumerator();
        }

        public int version() {
            return this._version;
        }

        public void setVersion(int version) {
            this._version = version;
        }

        /**
         * Gets a new cmap builder for this cmap table. The new cmap builder will be
         * for the cmap id specified and initialized with the data given. The data
         * will be copied and the original data will not be modified.
         *
         * @param cmapId the id for the new cmap builder
         * @param data the data to copy for the new cmap builder
         * @return a new cmap builder initialized with the cmap id and a copy of the
         *         data
         * @
         */
        public CMap.IBuilder<CMap> newCMapBuilder(CMapId cmapId, ReadableFontData data)
        {
            WritableFontData wfd = WritableFontData.createWritableFontData(data.size());
            data.copyTo(wfd);
            var builder = CMap.getBuilder(wfd, 0, cmapId);
            var cmapBuilders = this.getCMapBuilders();
            cmapBuilders.put(cmapId, builder);
            return builder;
        }

        public CMap.IBuilder<CMap> newCMapBuilder(CMapId cmapId, CMapFormat cmapFormat) {
            var builder = CMap.getBuilder(cmapFormat, cmapId);
            var cmapBuilders = this.getCMapBuilders();
            cmapBuilders.put(cmapId, builder);
            return builder;
        }

        public CMap.IBuilder<CMap> cmapBuilder(CMapId cmapId) {
            var cmapBuilders = this.getCMapBuilders();
            return cmapBuilders.get(cmapId);
        }

    }
}
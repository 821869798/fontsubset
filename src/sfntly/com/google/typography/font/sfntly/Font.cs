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

namespace com.google.typography.font.sfntly;

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.math;
using com.google.typography.font.sfntly.table;
using com.google.typography.font.sfntly.table.core;
using com.google.typography.font.sfntly.table.truetype;
using System.Diagnostics;


/**
 * An sfnt container font object. This object is immutable and thread safe. To
 * construct one use an instance of {@link Font.Builder}.
 *
 * @author Stuart Gill
 */
public class Font
{

    //private static readonly Logger logger =
    //  Logger.getLogger(typeof(Font).getCanonicalName());

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    private enum Offset
    {
        // Offsets within the main directory
        sfntVersion = (0),
        numTables = (4),
        searchRange = (6),
        entrySelector = (8),
        rangeShift = (10),
        tableRecordBegin = (12),
        sfntHeaderSize = (12),

        // Offsets within a specific table record
        tableTag = (0),
        tableCheckSum = (4),
        tableOffset = (8),
        tableLength = (12),
        tableRecordSize = (16),
    }

    /**
     * Ordering of tables for different font types.
     */
    private readonly static IList<Integer> CFF_TABLE_ORDERING;
    private readonly static IList<Integer> TRUE_TYPE_TABLE_ORDERING;
    static Font()
    {
        Integer[] cffArray = new Integer[] {Tag.head,
        Tag.hhea,
        Tag.maxp,
        Tag.OS_2,
        Tag.name,
        Tag.cmap,
        Tag.post,
        Tag.CFF};
        var cffList = new List<Integer>(cffArray.Length);
        cffList.AddRange(cffArray);
        CFF_TABLE_ORDERING = Collections.unmodifiableList(cffList);

        Integer[] ttArray = new Integer[] {Tag.head,
        Tag.hhea,
        Tag.maxp,
        Tag.OS_2,
        Tag.hmtx,
        Tag.LTSH,
        Tag.VDMX,
        Tag.hdmx,
        Tag.cmap,
        Tag.fpgm,
        Tag.prep,
        Tag.cvt,
        Tag.loca,
        Tag.glyf,
        Tag.kern,
        Tag.name,
        Tag.post,
        Tag.gasp,
        Tag.PCLT,
        Tag.DSIG};
        var ttList = new List<Integer>(ttArray.Length);
        ttList.AddRange(ttArray);
        TRUE_TYPE_TABLE_ORDERING = Collections.unmodifiableList(ttList);
    }

    /**
     * Platform ids. These are used in a number of places within the font whenever
     * the platform needs to be specified.
     *
     * @see NameTable
     * @see CMapTable
     */
    public enum PlatformId
    {
        Unknown = (-1), Unicode = (0), Macintosh = (1), ISO = (2), Windows = (3), Custom = (4)/*;

    private readonly int _value;

    private PlatformId(int value) {
      this._value = value;
    }

    public int value() {
      return this._value;
    }

    public boolean equals(int value) {
      return value == this._value;
    }

    public static PlatformId valueOf(int value) {
      foreach (PlatformId platform in PlatformId.values()) {
        if (platform.equals(value)) {
          return platform;
        }
      }
      return Unknown;
    }*/
    }

    /**
     * Unicode encoding ids. These are used in a number of places within the font
     * whenever character encodings need to be specified.
     *
     * @see NameTable
     * @see CMapTable
     */
    public enum UnicodeEncodingId
    {
        // Unicode Platform Encodings
        Unknown = (-1),
        Unicode1_0 = (0),
        Unicode1_1 = (1),
        ISO10646 = (2),
        Unicode2_0_BMP = (3),
        Unicode2_0 = (4),
        UnicodeVariationSequences = (5)/*;

        private readonly int _value;

        private UnicodeEncodingId(int value)
        {
            this._value = value;
        }

        public int value()
        {
            return this._value;
        }

        public boolean equals(int value)
        {
            return value == this._value;
        }

        public static UnicodeEncodingId valueOf(int value)
        {
            foreach (UnicodeEncodingId encoding in UnicodeEncodingId.values())
            {
                if (encoding.equals(value))
                {
                    return encoding;
                }
            }
            return Unknown;
        }*/
    }

    /**
     * Windows encoding ids. These are used in a number of places within the font
     * whenever character encodings need to be specified.
     *
     * @see NameTable
     * @see CMapTable
     */
    public enum WindowsEncodingId
    {
        // Windows Platform Encodings
        Unknown = (-1),
        Symbol = (0),
        UnicodeUCS2 = (1),
        ShiftJIS = (2),
        PRC = (3),
        Big5 = (4),
        Wansung = (5),
        Johab = (6),
        UnicodeUCS4 = (10)/*;

    private readonly int _value;

    private WindowsEncodingId(int value) {
      this._value = value;
    }

    public int value() {
      return this._value;
    }

    public boolean equals(int value) {
      return value == this._value;
    }

    public static WindowsEncodingId valueOf(int value) {
      foreach (WindowsEncodingId encoding in WindowsEncodingId.values()) {
        if (encoding.equals(value)) {
          return encoding;
        }
      }
      return Unknown;
    }*/
    }

    /**
     * Macintosh encoding ids. These are used in a number of places within the
     * font whenever character encodings need to be specified.
     *
     * @see NameTable
     * @see CMapTable
     */
    public enum MacintoshEncodingId
    {
        // Macintosh Platform Encodings
        Unknown = (-1),
        Roman = (0),
        Japanese = (1),
        ChineseTraditional = (2),
        Korean = (3),
        Arabic = (4),
        Hebrew = (5),
        Greek = (6),
        Russian = (7),
        RSymbol = (8),
        Devanagari = (9),
        Gurmukhi = (10),
        Gujarati = (11),
        Oriya = (12),
        Bengali = (13),
        Tamil = (14),
        Telugu = (15),
        Kannada = (16),
        Malayalam = (17),
        Sinhalese = (18),
        Burmese = (19),
        Khmer = (20),
        Thai = (21),
        Laotian = (22),
        Georgian = (23),
        Armenian = (24),
        ChineseSimplified = (25),
        Tibetan = (26),
        Mongolian = (27),
        Geez = (28),
        Slavic = (29),
        Vietnamese = (30),
        Sindhi = (31),
        Uninterpreted = (32)/*;

    private readonly int _value;

    private MacintoshEncodingId(int value) {
      this._value = value;
    }

    public int value() {
      return this._value;
    }

    public boolean equals(int value) {
      return value == this._value;
    }

    public static MacintoshEncodingId valueOf(int value) {
      foreach (MacintoshEncodingId encoding in MacintoshEncodingId.values()) {
        if (encoding.equals(value)) {
          return encoding;
        }
      }
      return Unknown;
    }*/
    }

    public static readonly int SFNTVERSION_1 = Fixed1616.@fixed(1, 0);

    private readonly int _sfntVersion;
    private readonly byte[] _digest;
    private long _checksum;

    private IDictionary<Integer, Table> _tables; // these get set in the builder

    /**
     * Constructor.
     *
     * @param sfntVersion the sfnt version
     * @param digest the computed digest for the font; null if digest was not
     *        computed
     */
    private Font(int sfntVersion, byte[] digest)
    {
        this._sfntVersion = sfntVersion;
        this._digest = digest;
    }

    /**
     * Gets the sfnt version set in the sfnt wrapper of the font.
     *
     * @return the sfnt version
     */
    public int sfntVersion()
    {
        return this._sfntVersion;
    }

    /**
     * Gets a copy of the fonts digest that was created when the font was read. If
     * no digest was set at creation time then the return result will be null.
     *
     * @return a copy of the digest array or <code>null</code> if one wasn't set
     *         at creation time
     */
    public byte[] digest()
    {
        if (this._digest == null)
        {
            return null;
        }
        return Arrays.copyOf(this._digest, this._digest.Length);
    }

    /**
     * Get the checksum for this font.
     *
     * @return the font checksum
     */
    public long checksum()
    {
        return this._checksum;
    }

    /**
     * Get the number of tables in this font.
     *
     * @return the number of tables
     */
    public int numTables()
    {
        return this._tables.Count;
    }

    /**
     * Get an iterator over all the tables in the font.
     *
     * @return a table iterator
     */
    public IEnumerator<Table> GetEnumerator()
    {
        return this._tables.values().GetEnumerator();
    }

    /**
     * Does the font have a particular table.
     *
     * @param tag the table identifier
     * @return true if the table is in the font; false otherwise
     */
    public boolean hasTable(int tag)
    {
        return this._tables.containsKey(tag);
    }

    /**
     * Get the table in this font with the specified id.
     *
     * @param <T> the type of the table
     * @param tag the identifier of the table
     * @return the table specified if it exists; null otherwise
     */
    public T getTable<T>(int tag) where T : Table
    {
        return (T)this._tables.get(tag);
    }

    /**
     * Get a map of the tables in this font accessed by table tag.
     *
     * @return an unmodifiable view of the tables in this font
     */
    public IDictionary<Integer, Table> tableMap()
    {
        return Collections.unmodifiableMap(this._tables);
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("digest = ");
        byte[] digest = this.digest();
        if (digest != null)
        {
            for (int i = 0; i < digest.Length; i++)
            {
                int d = 0xff & digest[i];
                if (d < 0x10)
                {
                    sb.Append("0");
                }
                sb.Append(NumberHelper.toHexString(d));
            }
        }
        sb.Append("\n[");
        sb.Append(Fixed1616.toString(_sfntVersion));
        sb.Append(", ");
        sb.Append(this.numTables());
        sb.Append("]\n");
        var iter = this.GetEnumerator();
        while (iter.MoveNext())
        {
            FontDataTable table = iter.Current;
            sb.Append("\t");
            sb.Append(table);
            sb.Append("\n");
        }
        return sb.ToString();
    }

    /**
     * Serialize the font to the output stream.
     *
     * @param os the destination for the font serialization
     * @param tableOrdering the table ordering to apply
     * @
     */
    public void serialize(OutputStream os, IList<Integer> tableOrdering)
    {
        IList<Integer> finalTableOrdering = this.generateTableOrdering(tableOrdering);
        IList<Header> tableRecords = buildTableHeadersForSerialization(finalTableOrdering);
        FontOutputStream fos = new FontOutputStream(os);
        this.serializeHeader(fos, tableRecords);
        this.serializeTables(fos, tableRecords);
        fos.Flush();
    }

    /**
     * Build the table headers to be used for serialization. These headers will be
     * filled out with the data required for serialization. The headers will be
     * sorted in the order specified and only those specified will have headers
     * generated.
     *
     * @param tableOrdering the tables to generate headers for and the order to
     *        sort them
     * @return a list of table headers ready for serialization
     */
    private IList<Header> buildTableHeadersForSerialization(IList<Integer> tableOrdering)
    {
        IList<Integer> finalTableOrdering = this.generateTableOrdering(tableOrdering);

        IList<Header> tableHeaders = new List<Header>(this.numTables());
        int tableOffset =
            (int)Offset.tableRecordBegin + this.numTables() * (int)Offset.tableRecordSize;
        foreach (Integer tag in finalTableOrdering)
        {
            Table table = this._tables.get(tag);
            if (table != null)
            {
                tableHeaders.Add(new Header(
                    tag, table.calculatedChecksum(), tableOffset, table.header().length()));
                // write on boundary of 4 bytes
                tableOffset += (table.dataLength() + 3) & ~3;
            }
        }
        return tableHeaders;
    }

    /**
     * Searialize the headers.
     *
     * @param fos the destination stream for the headers
     * @param tableHeaders the headers to serialize
     * @
     */
    private void serializeHeader(FontOutputStream fos, IList<Header> tableHeaders)
    {
        fos.writeFixed(this._sfntVersion);
        fos.writeUShort(tableHeaders.Count);
        int log2OfMaxPowerOf2 = FontMath.log2(tableHeaders.Count);
        int searchRange = 2 << (log2OfMaxPowerOf2 - 1 + 4);
        fos.writeUShort(searchRange);
        fos.writeUShort(log2OfMaxPowerOf2);
        fos.writeUShort((tableHeaders.Count * 16) - searchRange);

        var sortedHeaders = new List<Header>(tableHeaders);

        sortedHeaders.Sort(Header.COMPARATOR_BY_TAG);

        foreach (Header record in sortedHeaders)
        {
            fos.writeULong(record.tag());
            fos.writeULong(record.checksum());
            fos.writeULong(record.offset());
            fos.writeULong(record.length());
        }
    }

    /**
     * Serialize the tables.
     *
     * @param fos the destination stream for the headers
     * @param tableHeaders the headers for the tables to serialize
     * @
     */
    private void serializeTables(FontOutputStream fos, IList<Header> tableHeaders)
    {

        foreach (Header record in tableHeaders)
        {
            Table table = this.getTable<Table>(record.tag());
            if (table == null)
            {
                throw new IOException("Table out of sync with font header.");
            }
            int tableSize = table.serialize(fos);
            int fillerSize = ((tableSize + 3) & ~3) - tableSize;
            for (int i = 0; i < fillerSize; i++)
            {
                fos.write(0);
            }
        }
    }

    /**
     * Generate the full table ordering to used for serialization. The full
     * ordering uses the partial ordering as a seed and then adds all remaining
     * tables in the font in an undefined order.
     *
     * @param defaultTableOrdering the partial ordering to be used as a seed for
     *        the full ordering
     * @return the full ordering for serialization
     */
    private IList<Integer> generateTableOrdering(IList<Integer> defaultTableOrdering)
    {
        IList<Integer> tableOrdering = new List<Integer>(this._tables.Count);
        if (defaultTableOrdering == null)
        {
            defaultTableOrdering = this.defaultTableOrdering();
        }

        var tablesInFont = new SortedSet<Integer>(this._tables.keySet());

        // add all the default ordering
        foreach (Integer tag in defaultTableOrdering)
        {
            if (this.hasTable(tag))
            {
                tableOrdering.Add(tag);
                tablesInFont.Remove(tag);
            }
        }

        // add all the rest
        foreach (Integer tag in tablesInFont)
        {
            tableOrdering.Add(tag);
        }

        return tableOrdering;
    }

    /**
     * Get the default table ordering based on the type of the font.
     *
     * @return the default table ordering
     */
    private IList<Integer> defaultTableOrdering()
    {
        if (this.hasTable(Tag.CFF))
        {
            return Font.CFF_TABLE_ORDERING;
        }
        return Font.TRUE_TYPE_TABLE_ORDERING;
    }

    /**
     * A builder for a font object. The builder allows the for the creation of
     * immutable {@link Font} objects. The builder is a one use non-thread safe
     * object and cnce the {@link Font} object has been created it is no longer
     * usable. To create a further {@link Font} object new builder will be
     * required.
     *
     * @author Stuart Gill
     *
     */
    public sealed class Builder
    {

        private IDictionary<Integer, Table.IBuilder<Table>> tableBuilders;
        private FontFactory factory;
        private int sfntVersion = SFNTVERSION_1;
        private int numTables;
        private int searchRange;
        private int entrySelector;
        private int rangeShift;
        private IDictionary<Header, WritableFontData> dataBlocks;
        private byte[] digest;

        private Builder(FontFactory factory)
        {
            this.factory = factory;
            this.tableBuilders = new Dictionary<Integer, Table.IBuilder<Table>>();
        }

        private void loadFont(InputStream @is)
        {
            if (@is == null)
            {
                throw new IOException("No input stream for font.");
            }
            FontInputStream fontIS = null;
            try
            {
                fontIS = new FontInputStream(@is);
                SortedSet<Header> records = readHeader(fontIS);
                this.dataBlocks = loadTableData(records, fontIS);
                this.tableBuilders = buildAllTableBuilders(this.dataBlocks);
            }
            finally
            {
                fontIS.close();
            }
        }

        private void loadFont(WritableFontData wfd, int offsetToOffsetTable)
        {
            if (wfd == null)
            {
                throw new IOException("No data for font.");
            }
            SortedSet<Header> records = readHeader(wfd, offsetToOffsetTable);
            this.dataBlocks = loadTableData(records, wfd);
            this.tableBuilders = buildAllTableBuilders(this.dataBlocks);
        }

        public static Builder getOTFBuilder(FontFactory factory, InputStream @is)
        {
            Builder builder = new Builder(factory);
            builder.loadFont(@is);
            return builder;
        }

        public static Builder getOTFBuilder(
        FontFactory factory, WritableFontData wfd, int offsetToOffsetTable)
        {
            Builder builder = new Builder(factory);
            builder.loadFont(wfd, offsetToOffsetTable);
            return builder;
        }

        public static Builder getOTFBuilder(FontFactory factory)
        {
            return new Builder(factory);
        }

        /**
         * Get the font factory that created this font builder.
         *
         * @return the font factory
         */
        public FontFactory getFontFactory()
        {
            return this.factory;
        }

        /**
         * Is the font ready to build?
         *
         * @return true if ready to build; false otherwise
         */
        public boolean readyToBuild()
        {
            // just read in data with no manipulation
            if (this.tableBuilders == null && this.dataBlocks != null && this.dataBlocks.Count > 0)
            {
                return true;
            }

            foreach (Table.Builder<Table> tableBuilder in this.tableBuilders.values())
            {
                if (tableBuilder.readyToBuild() == false)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Build the {@link Font}. After this call this builder will no longer be
         * usable.
         *
         * @return a {@link Font}
         */
        public Font build()
        {
            IDictionary<Integer, Table> tables = null;

            Font font = new Font(this.sfntVersion, this.digest);

            if (this.tableBuilders.Count > 0)
            {
                tables = buildTablesFromBuilders(font, this.tableBuilders);
            }
            font._tables = tables;
            this.tableBuilders = null;
            this.dataBlocks = null;
            return font;
        }

        /**
         * Set a unique fingerprint for the font object.
         *
         * @param digest a unique identifier for the font
         */
        public void setDigest(byte[] digest)
        {
            this.digest = digest;
        }

        /**
         * Clear all table builders.
         */
        public void clearTableBuilders()
        {
            this.tableBuilders.Clear();
        }

        /**
         * Does this font builder have the specified table builder.
         *
         * @param tag the table builder tag
         * @return true if there is a builder for that table; false otherwise
         */
        public boolean hasTableBuilder(int tag)
        {
            return this.tableBuilders.containsKey(tag);
        }

        /**
         * Get the table builder for the given tag. If there is no builder for that
         * tag then return a null.
         *
         * @param tag the table builder tag
         * @return the builder for the tag; null if there is no builder for that tag
         */
        public Table.IBuilder<Table> getTableBuilder(int tag)
        {
            var builder = this.tableBuilders.get(tag);
            return builder;
        }

        /**
         * Creates a new empty table builder for the table type given by the table
         * id tag.
         *
         *  This new table will be added to the font and will replace any existing
         * builder for that table.
         *
         * @param tag
         * @return new empty table of the type specified by tag; if tag is not known
         *         then a generic OpenTypeTable is returned
         */
        public Table.IBuilder<Table> newTableBuilder(int tag)
        {
            Header header = new Header(tag);
            var builder = Table.Builder<Table>.getBuilder(header, null);
            this.tableBuilders.put(header.tag(), builder);

            return builder;
        }

        /**
         * Creates a new table builder for the table type given by the table id tag.
         * It makes a copy of the data provided and uses that copy for the table.
         *
         *  This new table has been added to the font and will replace any existing
         * builder for that table.
         *
         * @param tag
         * @param srcData
         * @return new empty table of the type specified by tag; if tag is not known
         *         then a generic OpenTypeTable is returned
         */
        public Table.IBuilder<Table> newTableBuilder(int tag, ReadableFontData srcData)
        {
            WritableFontData data;
            data = WritableFontData.createWritableFontData(srcData.length());
            // TODO(stuartg): take over original data instead?
            srcData.copyTo(data);

            Header header = new Header(tag, data.length());
            var builder = Table.Builder<Table>.getBuilder(header, data);

            this.tableBuilders.put(tag, builder);

            return builder;
        }

        /**
         * Get a map of the table builders in this font builder accessed by table
         * tag.
         *
         * @return an unmodifiable view of the table builders in this font builder
         */
        public IDictionary<Integer, Table.IBuilder<Table>> tableBuilderMap()
        {
            return Collections.unmodifiableMap(this.tableBuilders);
        }

        /**
         * Remove the specified table builder from the font builder.
         *
         * @param tag the table builder to remove
         * @return the table builder removed
         */
        public Table.IBuilder<Table> removeTableBuilder(int tag)
        {
            return this.tableBuilders.remove(tag);
        }

        /**
         * Get the number of table builders in the font builder.
         *
         * @return the number of table builders
         */
        public int tableBuilderCount()
        {
            return this.tableBuilders.Count;
        }

        private int sfntWrapperSize()
        {
            return (int)Offset.sfntHeaderSize +
            ((int)Offset.tableRecordSize * this.tableBuilders.Count);
        }

        private IDictionary<Integer, Table.IBuilder<Table>> buildAllTableBuilders(
            IDictionary<Header, WritableFontData> tableData)
        {
            var builderMap =
              new Dictionary<Integer, Table.IBuilder<Table>>();
            var records = tableData.keySet();
            foreach (Header record in records)
            {
                var builder = getTableBuilder(record, tableData.get(record));
                builderMap.put(record.tag(), builder);
            }
            interRelateBuilders(builderMap);
            return builderMap;
        }

        private Table.IBuilder<Table> getTableBuilder(Header header, WritableFontData data)
        {
            var builder = Table.Builder<Table>.getBuilder(header, data);
            return builder;
        }

        private static IDictionary<Integer, Table> buildTablesFromBuilders(Font font, IDictionary<Integer, Table.IBuilder<Table>> builderMap)
        {
            IDictionary<Integer, Table> tableMap = new Dictionary<Integer, Table>();

            interRelateBuilders(builderMap);

            long fontChecksum = 0;
            boolean tablesChanged = false;
            FontHeaderTable.IBuilder headerTableBuilder = null;

            // now build all the tables
            foreach (var builder in builderMap.values())
            {
                Table table = null;
                if (Tag.isHeaderTable(builder.header().tag()))
                {
                    headerTableBuilder = (FontHeaderTable.IBuilder)builder;
                    continue;
                }
                if (builder.readyToBuild())
                {
                    tablesChanged |= builder.changed();
                    table = builder.build();
                }
                if (table == null)
                {
                    throw new RuntimeException("Unable to build table - " + builder);
                }
                long tableChecksum = table.calculatedChecksum();
                fontChecksum += tableChecksum;
                tableMap.put(table.header().tag(), table);
            }

            // now fix up the header table
            Table headerTable = null;
            if (headerTableBuilder != null)
            {
                if (tablesChanged)
                {
                    headerTableBuilder.setFontChecksum(fontChecksum);
                }
                if (headerTableBuilder.readyToBuild())
                {
                    tablesChanged |= headerTableBuilder.changed();
                    headerTable = headerTableBuilder.build();
                }
                if (headerTable == null)
                {
                    throw new RuntimeException("Unable to build table - " + headerTableBuilder);
                }
                fontChecksum += headerTable.calculatedChecksum();
                tableMap.put(headerTable.header().tag(), headerTable);
            }

            font._checksum = fontChecksum & 0xffffffffL;
            return tableMap;
        }

        private static void
        interRelateBuilders(IDictionary<Integer, Table.IBuilder<Table>> builderMap)
        {
            var headerTableBuilder =
              (FontHeaderTable.IBuilder)builderMap.get(Tag.head);
            var horizontalHeaderBuilder =
              (HorizontalHeaderTable.IBuilder)builderMap.get(Tag.hhea);
            var maxProfileBuilder =
              (MaximumProfileTable.IBuilder)builderMap.get(Tag.maxp);
                var locaTableBuilder =
              (LocaTable.IBuilder)builderMap.get(Tag.loca);
            var horizontalMetricsBuilder =
              (HorizontalMetricsTable.IBuilder)builderMap.get(Tag.hmtx);
            var hdmxTableBuilder =
              (HorizontalDeviceMetricsTable.IBuilder)builderMap.get(Tag.hdmx);

            // set the inter table data required to build certain tables
            if (horizontalMetricsBuilder != null)
            {
                if (maxProfileBuilder != null)
                {
                    horizontalMetricsBuilder.setNumGlyphs(maxProfileBuilder.numGlyphs());
                }
                if (horizontalHeaderBuilder != null)
                {
                    horizontalMetricsBuilder.setNumberOfHMetrics(
                        horizontalHeaderBuilder.numberOfHMetrics());
                }
            }

            if (locaTableBuilder != null)
            {
                if (maxProfileBuilder != null)
                {
                    locaTableBuilder.setNumGlyphs(maxProfileBuilder.numGlyphs());
                }
                if (headerTableBuilder != null)
                {
                    locaTableBuilder.setFormatVersion(headerTableBuilder.indexToLocFormat());
                }
            }

            if (hdmxTableBuilder != null)
            {
                if (maxProfileBuilder != null)
                {
                    hdmxTableBuilder.setNumGlyphs(maxProfileBuilder.numGlyphs());
                }
            }
        }

        private SortedSet<Header> readHeader(FontInputStream @is)
        {
            var records =
                new SortedSet<Header>(Header.COMPARATOR_BY_OFFSET);

            this.sfntVersion = @is.readFixed();
            this.numTables = @is.readUShort();
            this.searchRange = @is.readUShort();
            this.entrySelector = @is.readUShort();
            this.rangeShift = @is.readUShort();

            for (int tableNumber = 0; tableNumber < this.numTables; tableNumber++)
            {
                Header table = new Header(@is.readULongAsInt(), // safe since the tag is ASCII
                    @is.readULong(),         // checksum
                    @is.readULongAsInt(),    // offset
                    @is.readULongAsInt());   // length
                records.Add(table);
            }
            return records;
        }

        private IDictionary<Header, WritableFontData> loadTableData(
            SortedSet<Header> headers, FontInputStream @is)
        {
            IDictionary<Header, WritableFontData> tableData =
                new Dictionary<Header, WritableFontData>(headers.Count);
            Debug.WriteLine("########  Reading Table Data");
            foreach (Header tableHeader in headers)
            {
                @is.skip(tableHeader.offset() - @is.position());
                Debug.WriteLine("\t" + tableHeader);
                Debug.WriteLine("\t\tStream Position = " + NumberHelper.toHexString((int)@is.position()));
                // don't close this or the whole stream is gone
                FontInputStream tableIS = new FontInputStream(@is, tableHeader.length());
                // TODO(stuartg): start tracking bad tables and other errors
                WritableFontData data = WritableFontData.createWritableFontData(tableHeader.length());
                data.copyFrom(tableIS, tableHeader.length());
                tableData.put(tableHeader, data);
            }
            return tableData;
        }

        private SortedSet<Header> readHeader(ReadableFontData fd, int offset)
        {
            SortedSet<Header> records =
                new SortedSet<Header>(Header.COMPARATOR_BY_OFFSET);

            this.sfntVersion = fd.readFixed(offset + (int)Offset.sfntVersion);
            this.numTables = fd.readUShort(offset + (int)Offset.numTables);
            this.searchRange = fd.readUShort(offset + (int)Offset.searchRange);
            this.entrySelector = fd.readUShort(offset + (int)Offset.entrySelector);
            this.rangeShift = fd.readUShort(offset + (int)Offset.rangeShift);

            int tableOffset = offset + (int)Offset.tableRecordBegin;
            for (int tableNumber = 0;
            tableNumber < this.numTables;
            tableNumber++, tableOffset += (int)Offset.tableRecordSize)
            {
                Header table =
                    // safe since the tag is ASCII
                    new Header(fd.readULongAsInt(tableOffset + (int)Offset.tableTag),
                        fd.readULong(tableOffset + (int)Offset.tableCheckSum), // checksum
                    fd.readULongAsInt(tableOffset + (int)(int)Offset.tableOffset), // offset
                    fd.readULongAsInt(tableOffset + (int)Offset.tableLength)); // length
                records.Add(table);
            }
            return records;
        }

        private IDictionary<Header, WritableFontData> loadTableData(
            SortedSet<Header> headers, WritableFontData fd)
        {
            IDictionary<Header, WritableFontData> tableData =
                new Dictionary<Header, WritableFontData>(headers.Count);
            Debug.WriteLine("########  Reading Table Data");
            foreach (Header tableHeader in headers)
            {
                WritableFontData data = fd.slice(tableHeader.offset(), tableHeader.length());
                tableData.put(tableHeader, data);
            }
            return tableData;
        }
    }
}
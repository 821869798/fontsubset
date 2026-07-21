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

using com.google.typography.font.sfntly.data;
using System.Reflection.Metadata;

namespace com.google.typography.font.sfntly.table.core;










/**
 * A Font Header table.
 * 
 * @author Stuart Gill
 */
public sealed class FontHeaderTable : Table
{

    /**
     * Checksum adjustment base value. To compute the checksum adjustment: 
     * 1) set it to 0; 2) sum the entire font as ULONG, 3) then store 0xB1B0AFBA - sum.
     */
    public static readonly long CHECKSUM_ADJUSTMENT_BASE = 0xB1B0AFBAL;

    /**
     * Magic number value stored in the magic number field.
     */
    public static readonly long MAGIC_NUMBER = 0x5F0F3CF5L;

    /**
     * The ranges to use for checksum calculation.
     */
    private static readonly int[] CHECKSUM_RANGES =
      new int[] { 0, (int)Offset.checkSumAdjustment, (int)Offset.magicNumber };

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    private enum Offset
    {
        tableVersion = (0),
        fontRevision = (4),
        checkSumAdjustment = (8),
        magicNumber = (12),
        flags = (16),
        unitsPerEm = (18),
        created = (20),
        modified = (28),
        xMin = (36),
        yMin = (38),
        xMax = (40),
        yMax = (42),
        macStyle = (44),
        lowestRecPPEM = (46),
        fontDirectionHint = (48),
        indexToLocFormat = (50),
        glyphDataFormat = (52)
    }

    /**
     * Constructor.
     *
     * @param header the table header
     * @param data the readable data for the table
     */
    private FontHeaderTable(Header header, ReadableFontData data) : base(header, data)
    {
        data.setCheckSumRanges(0, (int)Offset.checkSumAdjustment, (int)Offset.magicNumber);
    }

    /**
     * Get the table version.
     *
     * @return the table version
     */
    public int tableVersion()
    {
        return this._data.readFixed((int)Offset.tableVersion);
    }

    /**
     * Get the font revision.
     *
     * @return the font revision
     */
    public int fontRevision()
    {
        return this._data.readFixed((int)Offset.fontRevision);
    }

    /**
     * Get the checksum adjustment. To compute: set it to 0, sum the entire font
     * as ULONG, then store 0xB1B0AFBA - sum.
     *
     * @return checksum adjustment
     */
    public long checkSumAdjustment()
    {
        return this._data.readULong((int)Offset.checkSumAdjustment);
    }

    /**
     * Get the magic number. Set to 0x5F0F3CF5.
     *
     * @return the magic number
     */
    public long magicNumber()
    {
        return this._data.readULong((int)Offset.magicNumber);
    }

    /**
     * Flag values in the font header table.
     *
     */
    public sealed class Flags : ClassEnumBase<Flags>
    {
        public static readonly Flags
        BaselineAtY0,
        LeftSidebearingAtX0,
        InstructionsDependOnPointSize,
        ForcePPEMToInteger,
        InstructionsAlterAdvanceWidth,
        //Apple Flags
        Apple_Vertical,
        Apple_Zero,
        Apple_RequiresLayout,
        Apple_GXMetamorphosis,
        Apple_StrongRTL,
        Apple_IndicRearrangement,

        FontDataLossless,
        FontConverted,
        OptimizedForClearType,
        Reserved14,
        Reserved15;

        public int mask()
        {
            return 1 << this.ordinal();
        }

        public static EnumSet<Flags> asSet(int value)
        {
            EnumSet<Flags> set = EnumSet.noneOf<Flags>();
            foreach (Flags flag in Flags.values())
            {
                if ((value & flag.mask()) == flag.mask())
                {
                    set.Add(flag);
                }
            }
            return set;
        }

        public static int value(EnumSet<Flags> set)
        {
            int value = 0;
            foreach (Flags flag in set)
            {
                value |= flag.mask();
            }
            return value;
        }

        public static int cleanValue(EnumSet<Flags> set)
        {
            EnumSet<Flags> clean = EnumSet.copyOf(set);
            clean.Remove(Flags.Reserved14);
            clean.Remove(Flags.Reserved15);
            return value(clean);
        }
    }

    /**
     * Get the flags as an int value.
     *
     * @return the flags
     */
    public int flagsAsInt()
    {
        return this._data.readUShort((int)Offset.flags);
    }

    /**
     * Get the flags as an enum set.
     *
     * @return the enum set of the flags
     */
    public EnumSet<Flags> flags()
    {
        return Flags.asSet(this.flagsAsInt());
    }

    /**
     * Get the units per em.
     *
     * @return the units per em
     */
    public int unitsPerEm()
    {
        return this._data.readUShort((int)Offset.unitsPerEm);
    }

    /**
     * Get the created date. Number of seconds since 12:00 midnight, January 1,
     * 1904. 64-bit integer.
     *
     * @return created date
     */
    public long created()
    {
        return this._data.readDateTimeAsLong((int)Offset.created);
    }

    /**
     * Get the modified date. Number of seconds since 12:00 midnight, January 1,
     * 1904. 64-bit integer.
     *
     * @return created date
     */
    public long modified()
    {
        return this._data.readDateTimeAsLong((int)Offset.modified);
    }

    /**
     * Get the x min. For all glyph bounding boxes.
     *
     * @return the x min
     */
    public int xMin()
    {
        return this._data.readShort((int)Offset.xMin);
    }

    /**
     * Get the y min. For all glyph bounding boxes.
     *
     * @return the y min
     */
    public int yMin()
    {
        return this._data.readShort((int)Offset.yMin);
    }

    /**
     * Get the x max. For all glyph bounding boxes.
     *
     * @return the xmax
     */
    public int xMax()
    {
        return this._data.readShort((int)Offset.xMax);
    }

    /**
     * Get the y max. For all glyph bounding boxes.
     *
     * @return the ymax
     */
    public int yMax()
    {
        return this._data.readShort((int)Offset.yMax);
    }

    /**
     * Mac style bits set in the font header table.
     *
     */
    public sealed class MacStyle : ClassEnumBase<MacStyle>
    {
        public static readonly MacStyle
        Bold = new(),
        Italic = new(),
        Underline = new(),
        Outline = new(),
        Shadow = new(),
        Condensed = new(),
        Extended = new(),
        Reserved7 = new(),
        Reserved8 = new(),
        Reserved9 = new(),
        Reserved10 = new(),
        Reserved11 = new(),
        Reserved12 = new(),
        Reserved13 = new(),
        Reserved14 = new(),
        Reserved15 = new();


    public int mask() {
      return 1 << this.ordinal();
    }

    public static EnumSet<MacStyle> asSet(int value) {
      EnumSet<MacStyle> set = EnumSet.noneOf<MacStyle>();
      foreach(MacStyle style in MacStyle.values()) {
        if ((value & style.mask()) == style.mask()) {
          set.Add(style);
        }
      }
      return set;
    }

    public static int value(EnumSet<MacStyle> set) {
      int value = 0;
      foreach(MacStyle style in set) {
        value |= style.mask();
      }
      return value;
    }

    public static int cleanValue(EnumSet<MacStyle> set) {
      EnumSet<MacStyle> clean = EnumSet.copyOf(set);
      clean.RemoveAll(reserved);
      return value(clean);
    }

    private static readonly EnumSet<MacStyle> reserved = 
      EnumSet.range(MacStyle.Reserved7, MacStyle.Reserved15);
    }

    /**
     * Get the Mac style bits as an int.
     *
     * @return the Mac style bits
     */
    public int macStyleAsInt()
    {
        return this._data.readUShort((int)Offset.macStyle);
    }

    /**
     * Get the Mac style bits as an enum set.
     *
     * @return the Mac style bits
     */
    public EnumSet<MacStyle> macStyle()
    {
        return MacStyle.asSet(this.macStyleAsInt());
    }

    public int lowestRecPPEM()
    {
        return this._data.readUShort((int)Offset.lowestRecPPEM);
    }

    /**
     * Font direction hint values in the font header table.
     *
     */
    public enum FontDirectionHint
    {
        FullyMixed = (0),
        OnlyStrongLTR = (1),
        StrongLTRAndNeutral = (2),
        OnlyStrongRTL = (-1),
        StrongRTLAndNeutral = (-2)/*;

    private readonly int value;

    private FontDirectionHint(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static FontDirectionHint valueOf(int value) {
      foreach(FontDirectionHint hint in FontDirectionHint.values()) {
        if (hint.equals(value)) {
          return hint;
        }
      }
      return null;
    }*/
    }

    public int fontDirectionHintAsInt()
    {
        return this._data.readShort((int)Offset.fontDirectionHint);
    }

    public FontDirectionHint fontDirectionHint()
    {
        return (FontDirectionHint)(this.fontDirectionHintAsInt());
    }

    /**
     * The index to location format used in the LocaTable.
     *
     * @see LocaTable
     */
    public enum IndexToLocFormat
    {
        shortOffset = (0),
        longOffset = (1)/*;

    private readonly int value;
    
    private IndexToLocFormat(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static IndexToLocFormat valueOf(int value) {
      foreach(IndexToLocFormat format in IndexToLocFormat.values()) {
        if (format.equals(value)) {
          return format;
        }
      }
      return null;
    }*/
    }

    public int indexToLocFormatAsInt()
    {
        return this._data.readShort((int)Offset.indexToLocFormat);
    }

    public IndexToLocFormat indexToLocFormat()
    {
        return (IndexToLocFormat)(this.indexToLocFormatAsInt());
    }

    public int glyphdataFormat()
    {
        return this._data.readShort((int)Offset.glyphDataFormat);
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

    public interface IBuilder : ITableBasedTableBuilder<FontHeaderTable>
    {
        IndexToLocFormat indexToLocFormat();
        void setFontChecksum(long checksum);
    }

    private class Builder : TableBasedTableBuilder<FontHeaderTable>, IBuilder
    {
        private boolean fontChecksumSet = false;
        private long fontChecksum = 0;


        public Builder(Header header, WritableFontData data) : base(header, data)
        {
            data.setCheckSumRanges(0, (int)Offset.checkSumAdjustment, (int)Offset.magicNumber);
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
            data.setCheckSumRanges(FontHeaderTable.CHECKSUM_RANGES);
        }

        public override boolean subReadyToSerialize()
        {
            if (this.dataChanged())
            {
                ReadableFontData data = this.internalReadData();
                data.setCheckSumRanges(FontHeaderTable.CHECKSUM_RANGES);
            }
            if (this.fontChecksumSet)
            {
                ReadableFontData data = this.internalReadData();
                data.setCheckSumRanges(FontHeaderTable.CHECKSUM_RANGES);
                long checksumAdjustment =
                  FontHeaderTable.CHECKSUM_ADJUSTMENT_BASE - (this.fontChecksum + data.checksum());
                this.setCheckSumAdjustment(checksumAdjustment);
            }
            return base.subReadyToSerialize();
        }

        public override FontHeaderTable subBuildTable(ReadableFontData data)
        {
            return new FontHeaderTable(this.header(), data);
        }

        /**
         * Sets the font checksum to be used when calculating the the checksum
         * adjustment for the header table during build time.
         * 
         * The font checksum is the sum value of all tables but the font header
         * table. If the font checksum has been set then further setting will be
         * ignored until the font check sum has been cleared with
         * {@link #clearFontChecksum()}. Most users will never need to set this. It
         * is used when the font is being built. If set by a client it can interfere
         * with that process.
         * 
         * @param checksum
         *          the font checksum
         */
        public void setFontChecksum(long checksum)
        {
            if (this.fontChecksumSet)
            {
                return;
            }
            this.fontChecksumSet = true;
            this.fontChecksum = checksum;
        }

        /**
         * Clears the font checksum to be used when calculating the the checksum
         * adjustment for the header table during build time.
         * 
         * The font checksum is the sum value of all tables but the font header
         * table. If the font checksum has been set then further setting will be
         * ignored until the font check sum has been cleared.
         * 
         */
        public void clearFontChecksum()
        {
            this.fontChecksumSet = false;
        }

        public int tableVersion()
        {
            return this.table().tableVersion();
        }

        public void setTableVersion(int version)
        {
            this.internalWriteData().writeFixed((int)Offset.tableVersion, version);
        }

        public int fontRevision()
        {
            return this.table().fontRevision();
        }

        public void setFontRevision(int revision)
        {
            this.internalWriteData().writeFixed((int)Offset.fontRevision, revision);
        }

        public long checkSumAdjustment()
        {
            return this.table().checkSumAdjustment();
        }

        public void setCheckSumAdjustment(long adjustment)
        {
            this.internalWriteData().writeULong((int)Offset.checkSumAdjustment, adjustment);
        }

        public long magicNumber()
        {
            return this.table().magicNumber();
        }

        public void setMagicNumber(long magicNumber)
        {
            this.internalWriteData().writeULong((int)Offset.magicNumber, magicNumber);
        }

        public int flagsAsInt()
        {
            return this.table().flagsAsInt();
        }

        public EnumSet<Flags> flags()
        {
            return this.table().flags();
        }

        public void setFlagsAsInt(int flags)
        {
            this.internalWriteData().writeUShort((int)Offset.flags, flags);
        }

        public void setFlags(EnumSet<Flags> flags)
        {
            setFlagsAsInt(Flags.cleanValue(flags));
        }

        public int unitsPerEm()
        {
            return this.table().unitsPerEm();
        }

        public void setUnitsPerEm(int units)
        {
            this.internalWriteData().writeUShort((int)Offset.unitsPerEm, units);
        }

        public long created()
        {
            return this.table().created();
        }

        public void setCreated(long date)
        {
            this.internalWriteData().writeDateTime((int)Offset.created, date);
        }

        public long modified()
        {
            return this.table().modified();
        }

        public void setModified(long date)
        {
            this.internalWriteData().writeDateTime((int)Offset.modified, date);
        }

        public int xMin()
        {
            return this.table().xMin();
        }

        public void setXMin(int xmin)
        {
            this.internalWriteData().writeShort((int)Offset.xMin, xmin);
        }

        public int yMin()
        {
            return this.table().yMin();
        }

        public void setYMin(int ymin)
        {
            this.internalWriteData().writeShort((int)Offset.yMin, ymin);
        }

        public int xMax()
        {
            return this.table().xMax();
        }

        public void setXMax(int xmax)
        {
            this.internalWriteData().writeShort((int)Offset.xMax, xmax);
        }

        public int yMax()
        {
            return this.table().yMax();
        }

        public void setYMax(int ymax)
        {
            this.internalWriteData().writeShort((int)Offset.yMax, ymax);
        }

        public int macStyleAsInt()
        {
            return this.table().macStyleAsInt();
        }

        public void setMacStyleAsInt(int style)
        {
            this.internalWriteData().writeUShort((int)Offset.macStyle, style);
        }

        public EnumSet<MacStyle> macStyle()
        {
            return this.table().macStyle();
        }

        public void macStyle(EnumSet<MacStyle> style)
        {
            this.setMacStyleAsInt(MacStyle.cleanValue(style));
        }

        public int lowestRecPPEM()
        {
            return this.table().lowestRecPPEM();
        }

        public void setLowestRecPPEM(int size)
        {
            this.internalWriteData().writeUShort((int)Offset.lowestRecPPEM, size);
        }

        public int fontDirectionHintAsInt()
        {
            return this.table().fontDirectionHintAsInt();
        }

        public void setFontDirectionHintAsInt(int hint)
        {
            this.internalWriteData().writeShort((int)Offset.fontDirectionHint, hint);
        }

        public FontDirectionHint fontDirectionHint()
        {
            return this.table().fontDirectionHint();
        }

        public void setFontDirectionHint(FontDirectionHint hint)
        {
            this.setFontDirectionHintAsInt((int)hint);
        }

        public int indexToLocFormatAsInt()
        {
            return this.table().indexToLocFormatAsInt();
        }

        public void setIndexToLocFormatAsInt(int format)
        {
            this.internalWriteData().writeShort((int)Offset.indexToLocFormat, format);
        }

        public IndexToLocFormat indexToLocFormat()
        {
            return this.table().indexToLocFormat();
        }

        public void setIndexToLocFormat(IndexToLocFormat format)
        {
            this.setIndexToLocFormatAsInt((int)format);
        }

        public int glyphdataFormat()
        {
            return this.table().glyphdataFormat();
        }

        public void setGlyphdataFormat(int format)
        {
            this.internalWriteData().writeShort((int)Offset.glyphDataFormat, format);
        }
    }
}
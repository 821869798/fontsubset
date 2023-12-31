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

namespace com.google.typography.font.sfntly.table.core;










/**
 * An OS/2 table - 'OS/2'.
 *
 * @author Stuart Gill
 */
public sealed class OS2Table : Table
{

    /**
     * Offsets to specific elements in the underlying data. These offsets are
     * relative to the start of the table or the start of sub-blocks within the
     * table.
     */
    private enum Offset
    {
        version = (0),
        xAvgCharWidth = (2),
        usWeightClass = (4),
        usWidthClass = (6),
        fsType = (8),
        ySubscriptXSize = (10),
        ySubscriptYSize = (12),
        ySubscriptXOffset = (14),
        ySubscriptYOffset = (16),
        ySuperscriptXSize = (18),
        ySuperscriptYSize = (20),
        ySuperscriptXOffset = (22),
        ySuperscriptYOffset = (24),
        yStrikeoutSize = (26),
        yStrikeoutPosition = (28),
        sFamilyClass = (30),
        panose = (32),
        panoseLength = (10), // length of panose bytes
        ulUnicodeRange1 = (42),
        ulUnicodeRange2 = (46),
        ulUnicodeRange3 = (50),
        ulUnicodeRange4 = (54),
        achVendId = (58),
        achVendIdLength = (4), // length of ach vend id bytes
        fsSelection = (62),
        usFirstCharIndex = (64),
        usLastCharIndex = (66),
        sTypoAscender = (68),
        sTypoDescender = (70),
        sTypoLineGap = (72),
        usWinAscent = (74),
        usWinDescent = (76),
        ulCodePageRange1 = (78),
        ulCodePageRange2 = (82),
        sxHeight = (86),
        sCapHeight = (88),
        usDefaultChar = (90),
        usBreakChar = (92),
        usMaxContext = (94),
        /*
private readonly int offset;

private (int)Offset.int) {
  this.offset = offset;
}*/
    }

    private OS2Table(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public int tableVersion()
    {
        return this._data.readUShort((int)Offset.version);
    }

    public int xAvgCharWidth()
    {
        return this._data.readShort((int)Offset.xAvgCharWidth);
    }

    public enum WeightClass
    {
        Thin = (100),
        ExtraLight = (200),
        UltraLight = (200),
        Light = (300),
        Normal = (400),
        Regular = (400),
        Medium = (500),
        SemiBold = (600),
        DemiBold = (600),
        Bold = (700),
        ExtraBold = (800),
        UltraBold = (800),
        Black = (900),
        Heavy = (900),
        /*
    private readonly int value;

    private WeightClass(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static WeightClass valueOf(int value) {
      foreach(WeightClass weight in WeightClass.values()) {
        if (weight.equals(value)) {
          return weight;
        }
      }
      return null;
    }*/
    }

    public int usWeightClass()
    {
        return this._data.readUShort((int)Offset.usWeightClass);
    }

    public enum WidthClass
    {
        UltraCondensed = (1),
        ExtraCondensed = (2),
        Condensed = (3),
        SemiCondensed = (4),
        Medium = (5),
        Normal = (5),
        SemiExpanded = (6),
        Expanded = (7),
        ExtraExpanded = (8),
        UltraExpanded = (9)/*;

    private readonly int value;

    private WidthClass(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static WeightClass valueOf(int value) {
      foreach(WeightClass weight in WeightClass.values()) {
        if (weight.equals(value)) {
          return weight;
        }
      }
      return null;
    }*/
    }

    public int usWidthClass()
    {
        return this._data.readUShort((int)Offset.usWidthClass);
    }

    /**
     * Flags to indicate the embedding licensing rights for a font.
     */
    public sealed class EmbeddingFlags : ClassEnumBase<EmbeddingFlags>
    {
        public static readonly EmbeddingFlags
        Reserved0 = new(),
        RestrictedLicenseEmbedding = new(),
        PreviewAndPrintEmbedding = new(),
        EditableEmbedding = new(),
        Reserved4 = new(),
        Reserved5 = new(),
        Reserved6 = new(),
        Reserved7 = new(),
        NoSubsetting = new(),
        BitmapEmbeddingOnly = new(),
        Reserved10 = new(),
        Reserved11 = new(),
        Reserved12 = new(),
        Reserved13 = new(),
        Reserved14 = new(),
        Reserved15 = new();

        /**
         * Returns the bit mask corresponding to this embedding flag.
         *
         * @return the bit mask corresponding to this embedding flag
         */
        public int mask()
        {
            return 1 << this.ordinal();
        }

        /**
         * Generates an EnumSet\<EmbeddingFlags\> representation of the supplied
         * unsigned short.
         *
         * @param value the unsigned short
         * @return a new EnumSet\<EmbeddingFlags\>
         */
        public static EnumSet<EmbeddingFlags> asSet(int value)
        {
            EnumSet<EmbeddingFlags> set = EnumSet.noneOf<EmbeddingFlags>();
            foreach (EmbeddingFlags flag in EmbeddingFlags.values())
            {
                if ((value & flag.mask()) == flag.mask())
                {
                    set.Add(flag);
                }
            }
            return set;
        }

        /**
         * Generates an unsigned short representation of the provided
         * EnumSet\<EmbeddingFlags\>.
         *
         * @param flagSet the set of flags
         * @return the unsigned short representation of the provided flagSet
         */
        public static int asUShort(EnumSet<EmbeddingFlags> flagSet)
        {
            int flags = 0;
            foreach (EmbeddingFlags flag in flagSet)
            {
                flags |= flag.mask();
            }
            return flags;
        }

        /**
         * Takes an EnumSet\<EmbeddingFlags\> representation of the fsType and
         * returns whether or not the fsType is Installable Embedding. The fsType is
         * Installable Editing iff none of the fsType bits are set.
         *
         * @param flagSet the set of flags
         * @return true if the font has InstallableEmbedding
         */
        public static boolean isInstallableEditing(EnumSet<EmbeddingFlags> flagSet)
        {
            return flagSet.isEmpty();
        }

        /**
         * Takes the unsigned short representation of the fsType and returns whether
         * or not the fsType is Installable Embedding. The fsType is Installable
         * Editing iff none of the fsType bits are set.
         *
         * @param value the value to check
         * @return true if the font has InstallableEmbedding
         */
        public static boolean isInstallableEditing(int value)
        {
            return value == 0;
        }
    }

    public EnumSet<EmbeddingFlags> fsType()
    {
        return EmbeddingFlags.asSet(this.fsTypeAsInt());
    }

    public int fsTypeAsInt()
    {
        return this._data.readUShort((int)Offset.fsType);
    }

    public int ySubscriptXSize()
    {
        return this._data.readShort((int)Offset.ySubscriptXSize);
    }

    public int ySubscriptYSize()
    {
        return this._data.readShort((int)Offset.ySubscriptYSize);
    }

    public int ySubscriptXOffset()
    {
        return this._data.readShort((int)(int)Offset.ySubscriptXOffset);
    }

    public int ySubscriptYOffset()
    {
        return this._data.readShort((int)(int)Offset.ySubscriptYOffset);
    }

    public int ySuperscriptXSize()
    {
        return this._data.readShort((int)Offset.ySuperscriptXSize);
    }

    public int ySuperscriptYSize()
    {
        return this._data.readShort((int)Offset.ySuperscriptYSize);
    }

    public int ySuperscriptXOffset()
    {
        return this._data.readShort((int)Offset.ySuperscriptXOffset);
    }

    public int ySuperscriptYOffset()
    {
        return this._data.readShort((int)Offset.ySuperscriptYOffset);
    }

    public int yStrikeoutSize()
    {
        return this._data.readShort((int)Offset.yStrikeoutSize);
    }

    public int yStrikeoutPosition()
    {
        return this._data.readShort((int)Offset.yStrikeoutPosition);
    }

    // TODO(stuartg): IBM family enum?
    public int sFamilyClass()
    {
        return this._data.readShort((int)Offset.sFamilyClass);
    }

    // TODO(stuartg): panose class? individual getters for the panose values?
    public byte[] panose()
    {
        byte[] panose = new byte[10];
        this._data.readBytes((int)Offset.panose, panose, 0, panose.Length);
        return panose;
    }

    public long ulUnicodeRange1()
    {
        return this._data.readULong((int)Offset.ulUnicodeRange1);
    }

    public long ulUnicodeRange2()
    {
        return this._data.readULong((int)Offset.ulUnicodeRange2);
    }

    public long ulUnicodeRange3()
    {
        return this._data.readULong((int)Offset.ulUnicodeRange3);
    }

    public long ulUnicodeRange4()
    {
        return this._data.readULong((int)Offset.ulUnicodeRange4);
    }

    public sealed class UnicodeRange : ClassEnumBase<UnicodeRange>
    {
        public static readonly UnicodeRange
        // Do NOT reorder. This enum relies on the ordering of the data matching the
        // ordinal numbers of the properties
        BasicLatin,
        Latin1Supplement,
        LatinExtendedA,
        LatinExtendedB,
        IPAExtensions,
        SpacingModifierLetters,
        CombiningDiacriticalMarks,
        GreekAndCoptic,
        Coptic,
        Cyrillic,
        Armenian,
        Hebrew,
        Vai,
        Arabic,
        NKo,
        Devanagari,
        Bengali,
        Gurmukhi,
        Gujarati,
        Oriya,
        Tamil,
        Telugu,
        Kannada,
        Malayalam,
        Thai,
        Lao,
        Georgian,
        Balinese,
        HangulJamo,
        LatinExtendedAdditional,
        GreekExtended,
        GeneralPunctuation,
        SuperscriptsAndSubscripts,
        CurrencySymbols,
        NumberForms,
        Arrows,
        MathematicalOperators,
        MiscTechnical,
        ControlPictures,
        OCR,
        EnclosedAlphanumerics,
        BoxDrawing,
        BlockElements,
        GeometricShapes,
        MiscSymbols,
        Dingbats,
        CJKSymbolsAndPunctuation,
        Hiragana,
        Katakana,
        Bopomofo,
        HangulCompatibilityJamo,
        Phagspa,
        EnclosedCJKLettersAndMonths,
        CJKCompatibility,
        HangulSyllables,
        NonPlane0,
        Phoenician,
        CJKUnifiedIdeographs,
        PrivateUseAreaPlane0,
        CJKStrokes,
        AlphabeticPresentationForms,
        ArabicPresentationFormsA,
        CombiningHalfMarks,
        VerticalForms,
        SmallFormVariants,
        ArabicPresentationFormsB,
        HalfwidthAndFullwidthForms,
        Specials,
        Tibetan,
        Syriac,
        Thaana,
        Sinhala,
        Myanmar,
        Ethiopic,
        Cherokee,
        UnifiedCanadianAboriginalSyllabics,
        Ogham,
        Runic,
        Khmer,
        Mongolian,
        BraillePatterns,
        YiSyllables,
        Tagalog,
        OldItalic,
        Gothic,
        Deseret,
        MusicalSymbols,
        MathematicalAlphanumericSymbols,
        PrivateUsePlane15And16,
        VariationSelectors,
        Tags,
        Limbu,
        TaiLe,
        NewTaiLue,
        Buginese,
        Glagolitic,
        Tifnagh,
        YijingHexagramSymbols,
        SylotiNagari,
        LinearB,
        AncientGreekNumbers,
        Ugaritic,
        OldPersian,
        Shavian,
        Osmanya,
        CypriotSyllabary,
        Kharoshthi,
        TaiXuanJingSymbols,
        Cuneiform,
        CountingRodNumerals,
        Sudanese,
        Lepcha,
        OlChiki,
        Saurashtra,
        KayahLi,
        Rejang,
        Charm,
        AncientSymbols,
        PhaistosDisc,
        Carian,
        DominoTiles,
        Reserved123,
        Reserved124,
        Reserved125,
        Reserved126,
        Reserved127;

        public static UnicodeRange range(int bit)
        {
            if (bit > UnicodeRange.values().Length)
            {
                return null;
            }
            return UnicodeRange.values()[bit];
        }

        public static EnumSet<UnicodeRange> asSet(long range1, long range2, long range3, long range4)
        {
            EnumSet<UnicodeRange> set = EnumSet.noneOf<UnicodeRange>();
            long[] range = { range1, range2, range3, range4 };
            int rangeBit = 0;
            int rangeIndex = -1;
            foreach (UnicodeRange ur in UnicodeRange.values())
            {
                if (ur.ordinal() % 32 == 0)
                {
                    rangeBit = 0;
                    rangeIndex++;
                }
                else
                {
                    rangeBit++;
                }
                if ((range[rangeIndex] & 1 << rangeBit) == 1 << rangeBit)
                {
                    set.Add(ur);
                }
            }
            return set;
        }

        public static long[] asArray(EnumSet<UnicodeRange> rangeSet)
        {
            long[] range = new long[4];
            IEnumerator<UnicodeRange> iter = rangeSet.GetEnumerator();
            while (iter.MoveNext())
            {
                UnicodeRange ur = iter.Current;
                int urSegment = ur.ordinal() / 32;
                long urFlag = 1 << (ur.ordinal() % 32);
                range[urSegment] |= urFlag;
            }
            return range;
        }
    }

    public EnumSet<UnicodeRange> ulUnicodeRange()
    {
        return UnicodeRange.asSet(
            this.ulUnicodeRange1(), this.ulUnicodeRange2(),
            this.ulUnicodeRange3(), this.ulUnicodeRange4());
    }

    public byte[] achVendId()
    {
        byte[] b = new byte[4];
        this._data.readBytes((int)Offset.achVendId, b, 0, b.Length);
        return b;
    }

    public int fsSelectionAsInt()
    {
        return this._data.readUShort((int)Offset.fsSelection);
    }

    public sealed class FsSelection : ClassEnumBase<FsSelection>
    {
        public static readonly FsSelection
        ITALIC = new(),
        UNDERSCORE = new(),
        NEGATIVE = new(),
        OUTLINED = new(),
        STRIKEOUT = new(),
        BOLD = new(),
        REGULAR = new(),
        USE_TYPO_METRICS = new(),
        WWS = new(),
        OBLIQUE = new();

        public int mask()
        {
            return 1 << this.ordinal();
        }

        public static EnumSet<FsSelection> asSet(int value)
        {
            EnumSet<FsSelection> set = EnumSet.noneOf<FsSelection>();
            foreach (FsSelection selection in FsSelection.values())
            {
                if ((value & selection.mask()) == selection.mask())
                {
                    set.Add(selection);
                }
            }
            return set;
        }

        public static int asInt(EnumSet<FsSelection> fsSelectionSet)
        {
            int value = 0;
            IEnumerator<FsSelection> iter = fsSelectionSet.GetEnumerator();
            while (iter.MoveNext())
            {
                FsSelection fsSelection = iter.Current;
                value |= fsSelection.mask();
            }
            return value;
        }
    }

    public EnumSet<FsSelection> fsSelection()
    {
        return FsSelection.asSet(this.fsSelectionAsInt());
    }

    public int usFirstCharIndex()
    {
        return this._data.readUShort((int)Offset.usFirstCharIndex);
    }

    public int usLastCharIndex()
    {
        return this._data.readUShort((int)Offset.usLastCharIndex);
    }

    public int sTypoAscender()
    {
        return this._data.readShort((int)Offset.sTypoAscender);
    }

    public int sTypoDescender()
    {
        return this._data.readShort((int)Offset.sTypoDescender);
    }

    public int sTypoLineGap()
    {
        return this._data.readShort((int)Offset.sTypoLineGap);
    }

    public int usWinAscent()
    {
        return this._data.readUShort((int)Offset.usWinAscent);
    }

    public int usWinDescent()
    {
        return this._data.readUShort((int)Offset.usWinDescent);
    }

    public long ulCodePageRange1()
    {
        return this._data.readULong((int)Offset.ulCodePageRange1);
    }

    public long ulCodePageRange2()
    {
        return this._data.readULong((int)Offset.ulCodePageRange2);
    }

    public sealed class CodePageRange : ClassEnumBase<CodePageRange>
    {
        public static readonly CodePageRange
        Latin1_1252,
        Latin2_1250,
        Cyrillic_1251,
        Greek_1253,
        Turkish_1254,
        Hebrew_1255,
        Arabic_1256,
        WindowsBaltic_1257,
        Vietnamese_1258,
        AlternateANSI9,
        AlternateANSI10,
        AlternateANSI11,
        AlternateANSI12,
        AlternateANSI13,
        AlternateANSI14,
        AlternateANSI15,
        Thai_874,
        JapanJIS_932,
        ChineseSimplified_936,
        KoreanWansung_949,
        ChineseTraditional_950,
        KoreanJohab_1361,
        AlternateANSI22,
        AlternateANSI23,
        AlternateANSI24,
        AlternateANSI25,
        AlternateANSI26,
        AlternateANSI27,
        AlternateANSI28,
        MacintoshCharacterSet,
        OEMCharacterSet,
        SymbolCharacterSet,
        ReservedForOEM32,
        ReservedForOEM33,
        ReservedForOEM34,
        ReservedForOEM35,
        ReservedForOEM36,
        ReservedForOEM37,
        ReservedForOEM38,
        ReservedForOEM39,
        ReservedForOEM40,
        ReservedForOEM41,
        ReservedForOEM42,
        ReservedForOEM43,
        ReservedForOEM44,
        ReservedForOEM45,
        ReservedForOEM46,
        ReservedForOEM47,
        IBMGreek_869,
        MSDOSRussion_866,
        MSDOSNordic_865,
        Arabic_864,
        MSDOSCanadianFrench_863,
        Hebrew_862,
        MSDOSIcelandic_861,
        MSDOSPortugese_860,
        IBMTurkish_857,
        IBMCyrillic_855,
        Latin2_852,
        MSDOSBaltic_775,
        Greek_737,
        Arabic_708,
        Latin1_850,
        US_437;

        public static UnicodeRange range(int bit)
        {
            if (bit > UnicodeRange.values().Length)
            {
                return null;
            }
            return UnicodeRange.values()[bit];
        }

        public static EnumSet<CodePageRange> asSet(long range1, long range2)
        {
            EnumSet<CodePageRange> set = EnumSet.noneOf<CodePageRange>();
            long[] range = { range1, range2 };
            int rangeBit = 0;
            int rangeIndex = -1;
            foreach (CodePageRange cpr in CodePageRange.values())
            {
                if (cpr.ordinal() % 32 == 0)
                {
                    rangeBit = 0;
                    rangeIndex++;
                }
                else
                {
                    rangeBit++;
                }
                if ((range[rangeIndex] & 1 << rangeBit) == 1 << rangeBit)
                {
                    set.Add(cpr);
                }
            }
            return set;
        }

        public static long[] asArray(EnumSet<CodePageRange> rangeSet)
        {
            long[] range = new long[4];
            IEnumerator<CodePageRange> iter = rangeSet.GetEnumerator();
            while (iter.MoveNext())
            {
                CodePageRange ur = iter.Current;
                int urSegment = ur.ordinal() / 32;
                long urFlag = 1 << (ur.ordinal() % 32);
                range[urSegment] |= urFlag;
            }
            return range;
        }
    }

    public EnumSet<CodePageRange> ulCodePageRange()
    {
        return CodePageRange.asSet(this.ulCodePageRange1(), this.ulCodePageRange1());
    }

    public int sxHeight()
    {
        return this._data.readShort((int)Offset.sxHeight);
    }

    public int sCapHeight()
    {
        return this._data.readShort((int)Offset.sCapHeight);
    }

    public int usDefaultChar()
    {
        return this._data.readUShort((int)Offset.usDefaultChar);
    }

    public int usBreakChar()
    {
        return this._data.readUShort((int)Offset.usBreakChar);
    }

    public int usMaxContext()
    {
        return this._data.readUShort((int)Offset.usMaxContext);
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

    public interface IBuilder : ITableBasedTableBuilder<OS2Table>
    {

    }

    /**
     * A builder for the OS/2 table = 'OS/2'.
     *
     */
    private sealed class Builder : TableBasedTableBuilder<OS2Table>, IBuilder
    {

        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public override OS2Table subBuildTable(ReadableFontData data)
        {
            return new OS2Table(this.header(), data);
        }

        public int tableVersion()
        {
            return this.internalReadData().readUShort((int)Offset.version);
        }

        public void setTableVersion(int version)
        {
            this.internalWriteData().writeUShort((int)Offset.version, version);
        }

        public int xAvgCharWidth()
        {
            return this.internalReadData().readShort((int)Offset.xAvgCharWidth);
        }

        public void setXAvgCharWidth(int width)
        {
            this.internalWriteData().writeShort((int)Offset.xAvgCharWidth, width);
        }

        public int usWeightClass()
        {
            return this.internalReadData().readUShort((int)Offset.usWeightClass);
        }

        public void setUsWeightClass(int weight)
        {
            this.internalWriteData().writeUShort((int)Offset.usWeightClass, weight);
        }

        public int usWidthClass()
        {
            return this.internalReadData().readUShort((int)Offset.usWidthClass);
        }

        public void setUsWidthClass(int width)
        {
            this.internalWriteData().writeUShort((int)Offset.usWidthClass, width);
        }

        public EnumSet<EmbeddingFlags> fsType()
        {
            return EmbeddingFlags.asSet(this.fsTypeAsInt());
        }

        public int fsTypeAsInt()
        {
            return this.internalReadData().readUShort((int)Offset.fsType);
        }

        public void setFsType(EnumSet<EmbeddingFlags> flagSet)
        {
            this.setFsType(EmbeddingFlags.asUShort(flagSet));
        }

        public void setFsType(int fsType)
        {
            this.internalWriteData().writeUShort((int)Offset.fsType, fsType);
        }

        public int ySubscriptXSize()
        {
            return this.internalReadData().readShort((int)Offset.ySubscriptXSize);
        }

        public void setYSubscriptXSize(int size)
        {
            this.internalWriteData().writeShort((int)Offset.ySubscriptXSize, size);
        }

        public int ySubscriptYSize()
        {
            return this.internalReadData().readShort((int)Offset.ySubscriptYSize);
        }

        public void setYSubscriptYSize(int size)
        {
            this.internalWriteData().writeShort((int)Offset.ySubscriptYSize, size);
        }

        public int ySubscriptXOffset()
        {
            return this.internalReadData().readShort((int)(int)Offset.ySubscriptXOffset);
        }

        public void setYSubscriptX(int offset)
        {
            this.internalWriteData().writeShort((int)Offset.ySubscriptXOffset, offset);
        }

        public int ySubscriptYOffset()
        {
            return this.internalReadData().readShort((int)Offset.ySubscriptYOffset);
        }

        public void setYSubscriptY(int offset)
        {
            this.internalWriteData().writeShort((int)(int)Offset.ySubscriptYOffset, offset);
        }

        public int ySuperscriptXSize()
        {
            return this.internalReadData().readShort((int)Offset.ySuperscriptXSize);
        }

        public void setYSuperscriptXSize(int size)
        {
            this.internalWriteData().writeShort((int)Offset.ySuperscriptXSize, size);
        }

        public int ySuperscriptYSize()
        {
            return this.internalReadData().readShort((int)Offset.ySuperscriptYSize);
        }

        public void setYSuperscriptYSize(int size)
        {
            this.internalWriteData().writeShort((int)Offset.ySuperscriptYSize, size);
        }

        public int ySuperscriptXOffset()
        {
            return this.internalReadData().readShort((int)Offset.ySuperscriptXOffset);
        }

        public void setYSuperscriptX(int offset)
        {
            this.internalWriteData().writeShort((int)Offset.ySuperscriptXOffset, offset);
        }

        public int ySuperscriptYOffset()
        {
            return this.internalReadData().readShort((int)Offset.ySuperscriptYOffset);
        }

        public void setYSuperscriptY(int offset)
        {
            this.internalWriteData().writeShort((int)Offset.ySuperscriptYOffset, offset);
        }

        public int yStrikeoutSize()
        {
            return this.internalReadData().readShort((int)Offset.yStrikeoutSize);
        }

        public void setYStrikeoutSize(int size)
        {
            this.internalWriteData().writeShort((int)Offset.yStrikeoutSize, size);
        }

        public int yStrikeoutPosition()
        {
            return this.internalReadData().readShort((int)Offset.yStrikeoutPosition);
        }

        public void setYStrikeoutPosition(int position)
        {
            this.internalWriteData().writeShort((int)Offset.yStrikeoutPosition, position);
        }

        public int sFamilyClass()
        {
            return this.internalReadData().readShort((int)Offset.sFamilyClass);
        }

        public void setSFamilyClass(int family)
        {
            this.internalWriteData().writeShort((int)Offset.sFamilyClass, family);
        }

        public byte[] panose()
        {
            byte[] panose = new byte[(int)Offset.panoseLength];
            this.internalReadData().readBytes((int)Offset.panose, panose, 0, panose.Length);
            return panose;
        }

        public void setPanose(byte[] panose)
        {
            if (panose.Length != (int)Offset.panoseLength)
            {
                throw new IllegalArgumentException("Panose bytes must be exactly 10 in length.");
            }
            this.internalWriteData().writeBytes((int)Offset.panose, panose, 0, panose.Length);
        }

        public long ulUnicodeRange1()
        {
            return this.internalReadData().readULong((int)Offset.ulUnicodeRange1);
        }

        public void setUlUnicodeRange1(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulUnicodeRange1, range);
        }

        public long ulUnicodeRange2()
        {
            return this.internalReadData().readULong((int)Offset.ulUnicodeRange2);
        }

        public void setUlUnicodeRange2(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulUnicodeRange2, range);
        }

        public long ulUnicodeRange3()
        {
            return this.internalReadData().readULong((int)Offset.ulUnicodeRange3);
        }

        public void setUlUnicodeRange3(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulUnicodeRange3, range);
        }

        public long ulUnicodeRange4()
        {
            return this.internalReadData().readULong((int)Offset.ulUnicodeRange4);
        }

        public void setUlUnicodeRange4(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulUnicodeRange4, range);
        }

        public EnumSet<UnicodeRange> ulUnicodeRange()
        {
            return UnicodeRange.asSet(this.ulUnicodeRange1(), this.ulUnicodeRange2(),
                this.ulUnicodeRange3(), this.ulUnicodeRange4());
        }

        public void setUlUnicodeRange(EnumSet<UnicodeRange> rangeSet)
        {
            long[] range = UnicodeRange.asArray(rangeSet);
            this.setUlUnicodeRange1(range[0]);
            this.setUlUnicodeRange2(range[1]);
            this.setUlUnicodeRange3(range[2]);
            this.setUlUnicodeRange4(range[3]);
        }

        public byte[] achVendId()
        {
            byte[] b = new byte[(int)Offset.achVendIdLength];
            this.internalReadData().readBytes((int)Offset.achVendId, b, 0, b.Length);
            return b;
        }

        /**
         * Sets the achVendId field.
         *
         *  This field is 4 bytes in length and only the first 4 bytes of the byte
         * array will be written. If the byte array is less than 4 bytes it will be
         * padded out with space characters (0x20).
         *
         * @param b ach Vendor Id
         */
        public void setAchVendId(byte[] b)
        {
            this.internalWriteData().writeBytesPad(
                (int)Offset.achVendId, b, 0, (int)Offset.achVendIdLength, (byte)' ');
        }

        public int fsSelectionAsInt()
        {
            return this.internalReadData().readUShort((int)Offset.fsSelection);
        }

        public void setFsSelection(int fsSelection)
        {
            this.internalWriteData().writeUShort((int)Offset.fsSelection, fsSelection);
        }

        public void fsSelection(EnumSet<FsSelection> fsSelection)
        {
            this.setFsSelection(FsSelection.asInt(fsSelection));
        }

        public int usFirstCharIndex()
        {
            return this.internalReadData().readUShort((int)Offset.usFirstCharIndex);
        }

        public void setUsFirstCharIndex(int firstIndex)
        {
            this.internalWriteData().writeUShort((int)Offset.usFirstCharIndex, firstIndex);
        }

        public int usLastCharIndex()
        {
            return this.internalReadData().readUShort((int)Offset.usLastCharIndex);
        }

        public void setUsLastCharIndex(int lastIndex)
        {
            this.internalWriteData().writeUShort((int)Offset.usLastCharIndex, lastIndex);
        }

        public int sTypoAscender()
        {
            return this.internalReadData().readShort((int)Offset.sTypoAscender);
        }

        public void setSTypoAscender(int ascender)
        {
            this.internalWriteData().writeShort((int)Offset.sTypoAscender, ascender);
        }

        public int sTypoDescender()
        {
            return this.internalReadData().readShort((int)Offset.sTypoDescender);
        }

        public void setSTypoDescender(int descender)
        {
            this.internalWriteData().writeShort((int)Offset.sTypoDescender, descender);
        }

        public int sTypoLineGap()
        {
            return this.internalReadData().readShort((int)Offset.sTypoLineGap);
        }

        public void setSTypoLineGap(int lineGap)
        {
            this.internalWriteData().writeShort((int)Offset.sTypoLineGap, lineGap);
        }

        public int usWinAscent()
        {
            return this.internalReadData().readUShort((int)Offset.usWinAscent);
        }

        public void setUsWinAscent(int ascent)
        {
            this.internalWriteData().writeUShort((int)Offset.usWinAscent, ascent);
        }

        public int usWinDescent()
        {
            return this.internalReadData().readUShort((int)Offset.usWinDescent);
        }

        public void setUsWinDescent(int descent)
        {
            this.internalWriteData().writeUShort((int)Offset.usWinAscent, descent);
        }

        public long ulCodePageRange1()
        {
            return this.internalReadData().readULong((int)Offset.ulCodePageRange1);
        }

        public void setUlCodePageRange1(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulCodePageRange1, range);
        }

        public long ulCodePageRange2()
        {
            return this.internalReadData().readULong((int)Offset.ulCodePageRange2);
        }

        public void setUlCodePageRange2(long range)
        {
            this.internalWriteData().writeULong((int)Offset.ulCodePageRange2, range);
        }

        public EnumSet<CodePageRange> ulCodePageRange()
        {
            return CodePageRange.asSet(this.ulCodePageRange1(), this.ulCodePageRange2());
        }

        public void setUlCodePageRange(EnumSet<CodePageRange> rangeSet)
        {
            long[] range = CodePageRange.asArray(rangeSet);
            this.setUlCodePageRange1(range[0]);
            this.setUlCodePageRange2(range[1]);
        }

        public int sxHeight()
        {
            return this.internalReadData().readShort((int)Offset.sxHeight);
        }

        public void setSxHeight(int height)
        {
            this.internalWriteData().writeShort((int)Offset.sxHeight, height);
        }

        public int sCapHeight()
        {
            return this.internalReadData().readShort((int)Offset.sCapHeight);
        }

        public void setSCapHeight(int height)
        {
            this.internalWriteData().writeShort((int)Offset.sCapHeight, height);
        }

        public int usDefaultChar()
        {
            return this.internalReadData().readUShort((int)Offset.usDefaultChar);
        }

        public void setUsDefaultChar(int defaultChar)
        {
            this.internalWriteData().writeUShort((int)Offset.usDefaultChar, defaultChar);
        }

        public int usBreakChar()
        {
            return this.internalReadData().readUShort((int)Offset.usBreakChar);
        }

        public void setUsBreakChar(int breakChar)
        {
            this.internalWriteData().writeUShort((int)Offset.usBreakChar, breakChar);
        }

        public int usMaxContext()
        {
            return this.internalReadData().readUShort((int)Offset.usMaxContext);
        }

        public void setUsMaxContext(int maxContext)
        {
            this.internalWriteData().writeUShort((int)Offset.usMaxContext, maxContext);
        }
    }
}
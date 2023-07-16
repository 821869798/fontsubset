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
using static com.google.typography.font.sfntly.Font;

namespace com.google.typography.font.sfntly.table.core;
























// TODO(stuartg): support format 1 name tables
/**
 * A Name table.
 *
 * @author Stuart Gill
 */
public sealed class NameTable : SubTableContainerTable
{

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    public enum Offset
    {
        format = (0),
        count = (2),
        stringOffset = (4),
        nameRecordStart = (6),

        // format 1 - offset from the end of the name records
        langTagCount = (0),
        langTagRecord = (2),

        nameRecordSize = (12),
        // Name Records
        nameRecordPlatformId = (0),
        nameRecordEncodingId = (2),
        nameRecordLanguageId = (4),
        nameRecordNameId = (6),
        nameRecordStringLength = (8),
        nameRecordStringOffset = (10)/*;

    private readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    public enum NameId
    {
        Unknown = (-1),
        CopyrightNotice = (0),
        FontFamilyName = (1),
        FontSubfamilyName = (2),
        UniqueFontIdentifier = (3),
        FullFontName = (4),
        VersionString = (5),
        PostscriptName = (6),
        Trademark = (7),
        ManufacturerName = (8),
        Designer = (9),
        Description = (10),
        VendorURL = (11),
        DesignerURL = (12),
        LicenseDescription = (13),
        LicenseInfoURL = (14),
        Reserved15 = (15),
        PreferredFamily = (16),
        PreferredSubfamily = (17),
        CompatibleFullName = (18),
        SampleText = (19),
        PostscriptCID = (20),
        WWSFamilyName = (21),
        WWSSubfamilyName = (22)/*;

    private readonly int value;

    private NameId(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static NameId valueOf(int value) {
      foreach(NameId name in NameId.values()) {
        if (name.equals(value)) {
          return name;
        }
      }
      return Unknown;
    }*/
    }

    public enum UnicodeLanguageId
    {
        // Unicode Language IDs (platform ID = 0)
        Unknown = (-1), All = (0)/*;

    private readonly int value;

    private UnicodeLanguageId(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static UnicodeLanguageId valueOf(int value) {
      foreach(UnicodeLanguageId language in UnicodeLanguageId.values()) {
        if (language.equals(value)) {
          return language;
        }
      }
      return Unknown;
    }*/
    }

    /**
     * Macinstosh Language IDs (platform ID = 1)
     *
     */
    public enum MacintoshLanguageId
    {
        Unknown = (-1),
        English = (0),
        French = (1),
        German = (2),
        Italian = (3),
        Dutch = (4),
        Swedish = (5),
        Spanish = (6),
        Danish = (7),
        Portuguese = (8),
        Norwegian = (9),
        Hebrew = (10),
        Japanese = (11),
        Arabic = (12),
        Finnish = (13),
        Greek = (14),
        Icelandic = (15),
        Maltese = (16),
        Turkish = (17),
        Croatian = (18),
        Chinese_Traditional = (19),
        Urdu = (20),
        Hindi = (21),
        Thai = (22),
        Korean = (23),
        Lithuanian = (24),
        Polish = (25),
        Hungarian = (26),
        Estonian = (27),
        Latvian = (28),
        Sami = (29),
        Faroese = (30),
        FarsiPersian = (31),
        Russian = (32),
        Chinese_Simplified = (33),
        Flemish = (34),
        IrishGaelic = (35),
        Albanian = (36),
        Romanian = (37),
        Czech = (38),
        Slovak = (39),
        Slovenian = (40),
        Yiddish = (41),
        Serbian = (42),
        Macedonian = (43),
        Bulgarian = (44),
        Ukrainian = (45),
        Byelorussian = (46),
        Uzbek = (47),
        Kazakh = (48),
        Azerbaijani_Cyrillic = (49),
        Azerbaijani_Arabic = (50),
        Armenian = (51),
        Georgian = (52),
        Moldavian = (53),
        Kirghiz = (54),
        Tajiki = (55),
        Turkmen = (56),
        Mongolian_Mongolian = (57),
        Mongolian_Cyrillic = (58),
        Pashto = (59),
        Kurdish = (60),
        Kashmiri = (61),
        Sindhi = (62),
        Tibetan = (63),
        Nepali = (64),
        Sanskrit = (65),
        Marathi = (66),
        Bengali = (67),
        Assamese = (68),
        Gujarati = (69),
        Punjabi = (70),
        Oriya = (71),
        Malayalam = (72),
        Kannada = (73),
        Tamil = (74),
        Telugu = (75),
        Sinhalese = (76),
        Burmese = (77),
        Khmer = (78),
        Lao = (79),
        Vietnamese = (80),
        Indonesian = (81),
        Tagalong = (82),
        Malay_Roman = (83),
        Malay_Arabic = (84),
        Amharic = (85),
        Tigrinya = (86),
        Galla = (87),
        Somali = (88),
        Swahili = (89),
        KinyarwandaRuanda = (90),
        Rundi = (91),
        NyanjaChewa = (92),
        Malagasy = (93),
        Esperanto = (94),
        Welsh = (128),
        Basque = (129),
        Catalan = (130),
        Latin = (131),
        Quenchua = (132),
        Guarani = (133),
        Aymara = (134),
        Tatar = (135),
        Uighur = (136),
        Dzongkha = (137),
        Javanese_Roman = (138),
        Sundanese_Roman = (139),
        Galician = (140),
        Afrikaans = (141),
        Breton = (142),
        Inuktitut = (143),
        ScottishGaelic = (144),
        ManxGaelic = (145),
        IrishGaelic_WithDotAbove = (146),
        Tongan = (147),
        Greek_Polytonic = (148),
        Greenlandic = (149),
        Azerbaijani_Roman = (150)/*,

    private readonly int value;

    private MacintoshLanguageId(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static MacintoshLanguageId valueOf(int value) {
      foreach(MacintoshLanguageId language in MacintoshLanguageId.values()) {
        if (language.equals(value)) {
          return language;
        }
      }
      return Unknown;
    }*/
    }

    /**
     * Windows Language IDs (platform ID = 3)
     */
    public enum WindowsLanguageId
    {
        Unknown = (-1),
        Afrikaans_SouthAfrica = (0x0436),
        Albanian_Albania = (0x041C),
        Alsatian_France = (0x0484),
        Amharic_Ethiopia = (0x045E),
        Arabic_Algeria = (0x1401),
        Arabic_Bahrain = (0x3C01),
        Arabic_Egypt = (0x0C01),
        Arabic_Iraq = (0x0801),
        Arabic_Jordan = (0x2C01),
        Arabic_Kuwait = (0x3401),
        Arabic_Lebanon = (0x3001),
        Arabic_Libya = (0x1001),
        Arabic_Morocco = (0x1801),
        Arabic_Oman = (0x2001),
        Arabic_Qatar = (0x4001),
        Arabic_SaudiArabia = (0x0401),
        Arabic_Syria = (0x2801),
        Arabic_Tunisia = (0x1C01),
        Arabic_UAE = (0x3801),
        Arabic_Yemen = (0x2401),
        Armenian_Armenia = (0x042B),
        Assamese_India = (0x044D),
        Azeri_Cyrillic_Azerbaijan = (0x082C),
        Azeri_Latin_Azerbaijan = (0x042C),
        Bashkir_Russia = (0x046D),
        Basque_Basque = (0x042D),
        Belarusian_Belarus = (0x0423),
        Bengali_Bangladesh = (0x0845),
        Bengali_India = (0x0445),
        Bosnian_Cyrillic_BosniaAndHerzegovina = (0x201A),
        Bosnian_Latin_BosniaAndHerzegovina = (0x141A),
        Breton_France = (0x047E),
        Bulgarian_Bulgaria = (0x0402),
        Catalan_Catalan = (0x0403),
        Chinese_HongKongSAR = (0x0C04),
        Chinese_MacaoSAR = (0x1404),
        Chinese_PeoplesRepublicOfChina = (0x0804),
        Chinese_Singapore = (0x1004),
        Chinese_Taiwan = (0x0404),
        Corsican_France = (0x0483),
        Croatian_Croatia = (0x041A),
        Croatian_Latin_BosniaAndHerzegovina = (0x101A),
        Czech_CzechRepublic = (0x0405),
        Danish_Denmark = (0x0406),
        Dari_Afghanistan = (0x048C),
        Divehi_Maldives = (0x0465),
        Dutch_Belgium = (0x0813),
        Dutch_Netherlands = (0x0413),
        English_Australia = (0x0C09),
        English_Belize = (0x2809),
        English_Canada = (0x1009),
        English_Caribbean = (0x2409),
        English_India = (0x4009),
        English_Ireland = (0x1809),
        English_Jamaica = (0x2009),
        English_Malaysia = (0x4409),
        English_NewZealand = (0x1409),
        English_RepublicOfThePhilippines = (0x3409),
        English_Singapore = (0x4809),
        English_SouthAfrica = (0x1C09),
        English_TrinidadAndTobago = (0x2C09),
        English_UnitedKingdom = (0x0809),
        English_UnitedStates = (0x0409),
        English_Zimbabwe = (0x3009),
        Estonian_Estonia = (0x0425),
        Faroese_FaroeIslands = (0x0438),
        Filipino_Philippines = (0x0464),
        Finnish_Finland = (0x040B),
        French_Belgium = (0x080C),
        French_Canada = (0x0C0C),
        French_France = (0x040C),
        French_Luxembourg = (0x140c),
        French_PrincipalityOfMonoco = (0x180C),
        French_Switzerland = (0x100C),
        Frisian_Netherlands = (0x0462),
        Galician_Galician = (0x0456),
        Georgian_Georgia = (0x0437),
        German_Austria = (0x0C07),
        German_Germany = (0x0407),
        German_Liechtenstein = (0x1407),
        German_Luxembourg = (0x1007),
        German_Switzerland = (0x0807),
        Greek_Greece = (0x0408),
        Greenlandic_Greenland = (0x046F),
        Gujarati_India = (0x0447),
        Hausa_Latin_Nigeria = (0x0468),
        Hebrew_Israel = (0x040D),
        Hindi_India = (0x0439),
        Hungarian_Hungary = (0x040E),
        Icelandic_Iceland = (0x040F),
        Igbo_Nigeria = (0x0470),
        Indonesian_Indonesia = (0x0421),
        Inuktitut_Canada = (0x045D),
        Inuktitut_Latin_Canada = (0x085D),
        Irish_Ireland = (0x083C),
        isiXhosa_SouthAfrica = (0x0434),
        isiZulu_SouthAfrica = (0x0435),
        Italian_Italy = (0x0410),
        Italian_Switzerland = (0x0810),
        Japanese_Japan = (0x0411),
        Kannada_India = (0x044B),
        Kazakh_Kazakhstan = (0x043F),
        Khmer_Cambodia = (0x0453),
        Kiche_Guatemala = (0x0486),
        Kinyarwanda_Rwanda = (0x0487),
        Kiswahili_Kenya = (0x0441),
        Konkani_India = (0x0457),
        Korean_Korea = (0x0412),
        Kyrgyz_Kyrgyzstan = (0x0440),
        Lao_LaoPDR = (0x0454),
        Latvian_Latvia = (0x0426),
        Lithuanian_Lithuania = (0x0427),
        LowerSorbian_Germany = (0x082E),
        Luxembourgish_Luxembourg = (0x046E),
        Macedonian_FYROM_FormerYugoslavRepublicOfMacedonia = (0x042F),
        Malay_BruneiDarussalam = (0x083E),
        Malay_Malaysia = (0x043E),
        Malayalam_India = (0x044C),
        Maltese_Malta = (0x043A),
        Maori_NewZealand = (0x0481),
        Mapudungun_Chile = (0x047A),
        Marathi_India = (0x044E),
        Mohawk_Mohawk = (0x047C),
        Mongolian_Cyrillic_Mongolia = (0x0450),
        Mongolian_Traditional_PeoplesRepublicOfChina = (0x0850),
        Nepali_Nepal = (0x0461),
        Norwegian_Bokmal_Norway = (0x0414),
        Norwegian_Nynorsk_Norway = (0x0814),
        Occitan_France = (0x0482),
        Oriya_India = (0x0448),
        Pashto_Afghanistan = (0x0463),
        Polish_Poland = (0x0415),
        Portuguese_Brazil = (0x0416),
        Portuguese_Portugal = (0x0816),
        Punjabi_India = (0x0446),
        Quechua_Bolivia = (0x046B),
        Quechua_Ecuador = (0x086B),
        Quechua_Peru = (0x0C6B),
        Romanian_Romania = (0x0418),
        Romansh_Switzerland = (0x0417),
        Russian_Russia = (0x0419),
        Sami_Inari_Finland = (0x243B),
        Sami_Lule_Norway = (0x103B),
        Sami_Lule_Sweden = (0x143B),
        Sami_Northern_Finland = (0x0C3B),
        Sami_Northern_Norway = (0x043B),
        Sami_Northern_Sweden = (0x083B),
        Sami_Skolt_Finland = (0x203B),
        Sami_Southern_Norway = (0x183B),
        Sami_Southern_Sweden = (0x1C3B),
        Sanskrit_India = (0x044F),
        Serbian_Cyrillic_BosniaAndHerzegovina = (0x1C1A),
        Serbian_Cyrillic_Serbia = (0x0C1A),
        Serbian_Latin_BosniaAndHerzegovina = (0x181A),
        Serbian_Latin_Serbia = (0x081A),
        SesothoSaLeboa_SouthAfrica = (0x046C),
        Setswana_SouthAfrica = (0x0432),
        Sinhala_SriLanka = (0x045B),
        Slovak_Slovakia = (0x041B),
        Slovenian_Slovenia = (0x0424),
        Spanish_Argentina = (0x2C0A),
        Spanish_Bolivia = (0x400A),
        Spanish_Chile = (0x340A),
        Spanish_Colombia = (0x240A),
        Spanish_CostaRica = (0x140A),
        Spanish_DominicanRepublic = (0x1C0A),
        Spanish_Ecuador = (0x300A),
        Spanish_ElSalvador = (0x440A),
        Spanish_Guatemala = (0x100A),
        Spanish_Honduras = (0x480A),
        Spanish_Mexico = (0x080A),
        Spanish_Nicaragua = (0x4C0A),
        Spanish_Panama = (0x180A),
        Spanish_Paraguay = (0x3C0A),
        Spanish_Peru = (0x280A),
        Spanish_PuertoRico = (0x500A),
        Spanish_ModernSort_Spain = (0x0C0A),
        Spanish_TraditionalSort_Spain = (0x040A),
        Spanish_UnitedStates = (0x540A),
        Spanish_Uruguay = (0x380A),
        Spanish_Venezuela = (0x200A),
        Sweden_Finland = (0x081D),
        Swedish_Sweden = (0x041D),
        Syriac_Syria = (0x045A),
        Tajik_Cyrillic_Tajikistan = (0x0428),
        Tamazight_Latin_Algeria = (0x085F),
        Tamil_India = (0x0449),
        Tatar_Russia = (0x0444),
        Telugu_India = (0x044A),
        Thai_Thailand = (0x041E),
        Tibetan_PRC = (0x0451),
        Turkish_Turkey = (0x041F),
        Turkmen_Turkmenistan = (0x0442),
        Uighur_PRC = (0x0480),
        Ukrainian_Ukraine = (0x0422),
        UpperSorbian_Germany = (0x042E),
        Urdu_IslamicRepublicOfPakistan = (0x0420),
        Uzbek_Cyrillic_Uzbekistan = (0x0843),
        Uzbek_Latin_Uzbekistan = (0x0443),
        Vietnamese_Vietnam = (0x042A),
        Welsh_UnitedKingdom = (0x0452),
        Wolof_Senegal = (0x0448),
        Yakut_Russia = (0x0485),
        Yi_PRC = (0x0478),
        Yoruba_Nigeria = (0x046A)/*,

    private readonly int value;

    private WindowsLanguageId(int value) {
      this.value = value;
    }

    public int value() {
      return this.value;
    }

    public boolean equals(int value) {
      return value == this.value;
    }

    public static WindowsLanguageId valueOf(int value) {
      foreach(WindowsLanguageId language in WindowsLanguageId.values()) {
        if (language.equals(value)) {
          return language;
        }
      }
      return Unknown;
    }*/
    }

    private NameTable(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public int format()
    {
        return this._data.readUShort((int)Offset.format);
    }

    /**
     * Get the number of names in the name table.
     * @return the number of names
     */
    public int nameCount()
    {
        return this._data.readUShort((int)Offset.count);
    }

    /**
     * Get the offset to the string data in the name table.
     * @return the string offset
     */
    private int stringOffset()
    {
        return this._data.readUShort((int)Offset.stringOffset);
    }

    /**
     * Get the offset for the given name record.
     * @param index the index of the name record
     * @return the offset of the name record
     */
    private int offsetForNameRecord(int index)
    {
        return (int)Offset.nameRecordStart + index * (int)Offset.nameRecordSize;
    }

    /**
     * Get the platform id for the given name record.
     *
     * @param index the index of the name record
     * @return the platform id
     * @see PlatformId
     */
    public int platformId(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordPlatformId + this.offsetForNameRecord(index));
    }

    /**
     * Get the encoding id for the given name record.
     *
     * @param index the index of the name record
     * @return the encoding id
     *
     * @see MacintoshEncodingId
     * @see WindowsEncodingId
     * @see UnicodeEncodingId
     */
    public int encodingId(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordEncodingId + this.offsetForNameRecord(index));
    }

    /**
     * Get the language id for the given name record.
     * @param index the index of the name record
     * @return the language id
     */
    public int languageId(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordLanguageId + this.offsetForNameRecord(index));
    }

    /**
     * Get the name id for given name record.
     * @param index the index of the name record
     * @return the name id
     */
    public int nameId(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordNameId + this.offsetForNameRecord(index));
    }

    /**
     * Get the length of the string data for the given name record.
     * @param index the index of the name record
     * @return the length of the string data in bytes
     */
    private int nameLength(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordStringLength + this.offsetForNameRecord(index));
    }

    /**
     * Get the offset of the string data for the given name record.
     * @param index the index of the name record
     * @return the offset of the string data from the start of the table
     */
    private int nameOffset(int index)
    {
        return this._data.readUShort(
            (int)Offset.nameRecordStringOffset +
            this.offsetForNameRecord(index)) + this.stringOffset();
    }

    /**
     * Get the name as bytes for the given name record.
     * @param index the index of the name record
     * @return the bytes for the name
     */
    public byte[] nameAsBytes(int index)
    {
        int length = this.nameLength(index);
        byte[] b = new byte[length];
        this._data.readBytes(this.nameOffset(index), b, 0, length);
        return b;
    }

    /**
     * Get the name as bytes for the specified name. If there is no entry for the requested name
     * then <code>null</code> is returned.
     * @param platformId the platform id
     * @param encodingId the encoding id
     * @param languageId the language id
     * @param nameId the name id
     * @return the bytes for the name
     */
    public byte[] nameAsBytes(int platformId, int encodingId, int languageId, int nameId)
    {
        NameEntry entry = this.nameEntry(platformId, encodingId, languageId, nameId);
        if (entry != null)
        {
            return entry.nameAsBytes();
        }
        return null;
    }

    /**
     * Get the name as a String for the given name record. If there is no encoding conversion
     * available for the name record then a best attempt String will be returned.
     * @param index the index of the name record
     * @return the name
     */
    public String name(int index)
    {
        return convertFromNameBytes(
            this.nameAsBytes(index), this.platformId(index), this.encodingId(index));
    }

    /**
     * Get the name as a String for the specified name. If there is no entry for the requested name
     * then <code>null</code> is returned. If there is no encoding conversion
     * available for the name then a best attempt String will be returned.
     * @param platformId the platform id
     * @param encodingId the encoding id
     * @param languageId the language id
     * @param nameId the name id
     * @return the name
     */
    public String name(int platformId, int encodingId, int languageId, int nameId)
    {
        NameEntry entry = this.nameEntry(platformId, encodingId, languageId, nameId);
        if (entry != null)
        {
            return entry.name();
        }
        return null;
    }

    /**
     * Get the name entry record for the given name entry.
     * @param index the index of the name record
     * @return the name entry
     */
    public NameEntry nameEntry(int index)
    {
        return new NameEntry(
            this.platformId(index), this.encodingId(index), this.languageId(index),
            this.nameId(index), this.nameAsBytes(index));
    }

    /**
     * Get the name entry record for the specified name. If there is no entry for the requested name
     * then <code>null</code> is returned.
     * @param platformId the platform id
     * @param encodingId the encoding id
     * @param languageId the language id
     * @param nameId the name id
     * @return the name entry
     */
    public NameEntry nameEntry(int platformId, int encodingId, int languageId, int nameId)
    {

        /*Iterator<NameEntry> nameEntryIter = this.iterator(new NameEntryFilter() {
          public override boolean accept(int pid, int eid, int lid, int nid) {
            if (pid == platformId && eid == encodingId && lid == languageId && nid == nameId) {
              return true;
            }
            return false;
          }
        });*/

        var nameEntryIter = this.iterator((int pid, int eid, int lid, int nid) => pid == platformId && eid == encodingId && lid == languageId && nid == nameId);

        // can only be one name for each set of ids
        if (nameEntryIter.MoveNext())
        {
            return nameEntryIter.Current;
        }
        return null;
    }

    /**
     * Get all the name entry records.
     * @return the set of all name entry records
     */
    public HashSet<NameEntry> names()
    {
        var nameSet = new HashSet<NameEntry>(this.nameCount());
        foreach (NameEntry entry in this)
        {
            nameSet.Add(entry);
        }
        return nameSet;
    }

    public class NameEntryId : IComparable<NameEntryId>
    {
        /* @see Font.PlatformId
         */
        public int _platformId;
        /* @see Font.UnicodeEncodingId
         * @see Font.MacintoshEncodingId
         * @see Font.WindowsEncodingId
         */
        public int _encodingId;
        /* @see NameTable.UnicodeLanguageId
         * @see NameTable.MacintoshLanguageId
         * @see NameTable.WindowsLanguageId
         */
        public int _languageId;
        /* @see NameTable.NameId
         */
        public int _nameId;

        /**
         * @param platformId
         * @param encodingId
         * @param languageId
         * @param nameId
         */
        public NameEntryId(int platformId, int encodingId, int languageId, int nameId)
        {
            this._platformId = platformId;
            this._encodingId = encodingId;
            this._languageId = languageId;
            this._nameId = nameId;
        }

        /**
         * Get the platform id.
         *
         * @return the platform id
         */
        public int getPlatformId()
        {
            return this._platformId;
        }

        /**
         * Get the encoding id.
         *
         * @return the encoding id
         */
        public int getEncodingId()
        {
            return this._encodingId;
        }

        /**
         * Get the language id.
         *
         * @return the language id
         */
        public int getLanguageId()
        {
            return this._languageId;
        }

        /**
         * Get the name id.
         *
         * @return the name id
         */
        public int getNameId()
        {
            return this._nameId;
        }

        public override boolean Equals(Object obj)
        {
            if (!(obj is NameEntryId))
            {
                return false;
            }
            NameEntryId other = (NameEntryId)obj;
            return (this._encodingId == other._encodingId) && (this._languageId == other._languageId)
                && (this._platformId == other._platformId) && (this._nameId == other._nameId);
        }

        public override int GetHashCode()
        {
            /*
             * - this takes advantage of the sizes of the various entries and the fact
             * that the ranges of their values have an almost zero probability of ever
             * changing - this allows us to generate a unique hash at low cost - if
             * the ranges do change then we will potentially generate non-unique hash
             * values which is a common result
             */
            return ((this._encodingId & 0x3f) << 26) | ((this._nameId & 0x3f) << 16)
                | ((this._platformId & 0x0f) << 12) | (this._languageId & 0xff);
        }

        /**
         * Name entries are sorted by platform id, encoding id, language id, and
         * name id in order of decreasing importance.
         *
         * @return less than zero if this entry is less than the other; greater than
         *         zero if this entry is greater than the other; and zero if they
         *         are equal
         *
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public int CompareTo(NameEntryId o)
        {
            if (this._platformId != o._platformId)
            {
                return this._platformId - o._platformId;
            }
            if (this._encodingId != o._encodingId)
            {
                return this._encodingId - o._encodingId;
            }
            if (this._languageId != o._languageId)
            {
                return this._languageId - o._languageId;
            }
            return this._nameId - o._nameId;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P=");
            sb.Append((PlatformId)(this._platformId));
            sb.Append(", E=0x");
            sb.Append(NumberHelper.toHexString(this._encodingId));
            sb.Append(", L=0x");
            sb.Append(NumberHelper.toHexString(this._languageId));
            sb.Append(", N=");

            NameId nameId = (NameId)(this._nameId);
            if (Enum.IsDefined(typeof(NameId),nameId))
            {
                sb.Append((NameId)(this._nameId));
            }
            else
            {
                sb.Append("0x");
                sb.Append(NumberHelper.toHexString(this._nameId));
            }
            return sb.ToString();
        }
    }

    /**
     * Class to represent a name entry in the name table.
     *
     */
    public class NameEntry
    {
        public NameEntryId nameEntryId;
        public int length;
        public byte[] nameBytes;

        public NameEntry()
        {
        }

        public NameEntry(NameEntryId nameEntryId, byte[] nameBytes)
        {
            this.nameEntryId = nameEntryId;
            this.nameBytes = nameBytes;
        }

        public NameEntry(
            int platformId, int encodingId, int languageId, int nameId, byte[] nameBytes) : this(new NameEntryId(platformId, encodingId, languageId, nameId), nameBytes)
        {
        }

        public NameEntryId getNameEntryId()
        {
            return this.nameEntryId;
        }

        /**
         * Get the platform id.
         * @return the platform id
         */
        public int platformId()
        {
            return this.nameEntryId.getPlatformId();
        }

        /**
         * Get the encoding id.
         * @return the encoding id
         */
        public int encodingId()
        {
            return this.nameEntryId.getEncodingId();
        }

        /**
         * Get the language id.
         * @return the language id
         */
        public int languageId()
        {
            return this.nameEntryId.getLanguageId();
        }

        /**
         * Get the name id.
         * @return the name id
         */
        public int nameId()
        {
            return this.nameEntryId.getNameId();
        }

        /**
         * Get the bytes for name.
         * @return the name bytes
         */
        public byte[] nameAsBytes()
        {
            return this.nameBytes;
        }

        /**
         * Get the name as a String. If there is no encoding conversion
         * available for the name bytes then a best attempt String will be returned.
         * @return the name
         */
        public String name()
        {
            return NameTable.convertFromNameBytes(this.nameBytes, this.platformId(), this.encodingId());
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(this.nameEntryId);
            sb.Append(", \"");
            String name = this.name();
            sb.Append(this.name());
            sb.Append("\"]");
            return sb.ToString();
        }

        public override boolean Equals(Object obj)
        {
            if (!(obj is NameEntry))
            {
                return false;
            }
            NameEntry other = (NameEntry)obj;
            if (!this.nameEntryId.Equals(other.nameEntryId))
            {
                return false;
            }
            if (this.nameBytes.Length != other.nameBytes.Length)
            {
                return false;
            }
            for (int i = 0; i < this.nameBytes.Length; i++)
            {
                if (this.nameBytes[i] != other.nameBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = this.nameEntryId.GetHashCode();
            for (int i = 0; i < this.nameBytes.Length; i += 4)
            {
                for (int j = 0; j < 4 && j + i < this.nameBytes.Length; j++)
                {
                    hash |= this.nameBytes[j] << j * 8;
                }
            }
            return hash;
        }
    }

    public class NameEntryBuilder : NameEntry
    {

        /**
         * Constructor.
         */
        public NameEntryBuilder() : base()
        {
        }

        public NameEntryBuilder(NameEntryId nameEntryId, byte[] nameBytes) : base(nameEntryId, nameBytes)
        {
        }

        public NameEntryBuilder(NameEntryId nameEntryId) : this(nameEntryId, null)
        {
        }

        public NameEntryBuilder(NameEntry nameEntry) : this(nameEntry.getNameEntryId(), nameEntry.nameAsBytes())
        {
        }

        public void setName(String name)
        {
            if (name == null)
            {
                this.nameBytes = new byte[0];
                return;
            }
            this.nameBytes = NameTable.convertToNameBytes(
                name, this.nameEntryId.getPlatformId(), this.nameEntryId.getEncodingId());
        }

        public void setName(byte[] nameBytes)
        {
            this.nameBytes = Arrays.copyOf(nameBytes, nameBytes.Length);
        }

        public void setName(byte[] nameBytes, int offset, int length)
        {
            this.nameBytes = Arrays.copyOfRange(nameBytes, offset, offset + length);
        }
    }

    ///**
    // * An interface for a filter to use with the name entry iterator. This allows
    // * name entries to be iterated and only those acceptable to the filter will be returned.
    // */
    //public interface NameEntryFilter {
    //  /**
    //   * Callback to determine if a name entry is acceptable.
    //   * @param platformId platform id
    //   * @param encodingId encoding id
    //   * @param languageId language id
    //   * @param nameId name id
    //   * @return true if the name entry is acceptable; false otherwise
    //   */
    //  boolean accept(int platformId, int encodingId, int languageId, int nameId);
    //}

    public delegate bool NameEntryFilter(int platformId, int encodingId, int languageId, int nameId);


    /*
  public class NameEntryIterator : IEnumerator<NameEntry> {
    private int nameIndex = 0;
    private NameEntryFilter filter = null;

    private NameEntryIterator() {
      // no filter - iterate all name entries
    }

    private NameEntryIterator(NameEntryFilter filter) {
      this.filter = filter;
    }

    public override boolean hasNext() {
      if (this.filter == null) {
        if (this.nameIndex < nameCount()) {
          return true;
        }
        return false;
      }
      for (; this.nameIndex < nameCount(); this.nameIndex++) {
        if (filter.accept(
            platformId(this.nameIndex), encodingId(this.nameIndex),
            languageId(this.nameIndex), nameId(this.nameIndex))) {
          return true;
        }
      }
      return false;
    }

    public override NameEntry next() {
      if (!hasNext()) {
        throw new NoSuchElementException();
      }
      return nameEntry(this.nameIndex++);
    }

    public override void remove() {
      throw new UnsupportedOperationException("Cannot remove a CMap table from an existing font.");
    }
  }*/

    public IEnumerator<NameEntry> GetEnumerator()
    {
        //return new NameEntryIterator();
        return iterator(null);
    }

    /**
     * Get an iterator over name entries in the name table.
     * @param filter a filter to select acceptable name entries
     * @return an iterator over name entries
     */
    public IEnumerator<NameEntry> iterator(NameEntryFilter filter)
    {
        //return new NameEntryIterator(filter);
        return Enumerable.Range(0, nameCount())
                .WhereIf(filter != null, (nameIndex) => filter(platformId(nameIndex), encodingId(nameIndex), languageId(nameIndex), nameId(nameIndex)))
                .Select(nameIndex => nameEntry(nameIndex))
                .GetEnumerator();
    }

    // TODO(stuartg): do this in the encoding enums
    private static String getEncodingName(int platformId, int encodingId)
    {
        String encodingName = null;
        switch ((PlatformId)platformId)
        {
            case PlatformId.Unicode:
                encodingName = "UTF-16BE";
                break;
            case PlatformId.Macintosh:
                switch ((MacintoshEncodingId)encodingId)
                {
                    case MacintoshEncodingId.Roman:
                        encodingName = "MacRoman";
                        break;
                    case MacintoshEncodingId.Japanese:
                        encodingName = "Shift_JIS";
                        break;
                    case MacintoshEncodingId.ChineseTraditional:
                        encodingName = "Big5";
                        break;
                    case MacintoshEncodingId.Korean:
                        encodingName = "EUC-KR";
                        break;
                    case MacintoshEncodingId.Arabic:
                        encodingName = "MacArabic";
                        break;
                    case MacintoshEncodingId.Hebrew:
                        encodingName = "MacHebrew";
                        break;
                    case MacintoshEncodingId.Greek:
                        encodingName = "MacGreek";
                        break;
                    case MacintoshEncodingId.Russian:
                        encodingName = "MacCyrillic";
                        break;
                    case MacintoshEncodingId.RSymbol:
                        encodingName = "MacSymbol";
                        break;
                    case MacintoshEncodingId.Devanagari:
                        break;
                    case MacintoshEncodingId.Gurmukhi:
                        break;
                    case MacintoshEncodingId.Gujarati:
                        break;
                    case MacintoshEncodingId.Oriya:
                        break;
                    case MacintoshEncodingId.Bengali:
                        break;
                    case MacintoshEncodingId.Tamil:
                        break;
                    case MacintoshEncodingId.Telugu:
                        break;
                    case MacintoshEncodingId.Kannada:
                        break;
                    case MacintoshEncodingId.Malayalam:
                        break;
                    case MacintoshEncodingId.Sinhalese:
                        break;
                    case MacintoshEncodingId.Burmese:
                        break;
                    case MacintoshEncodingId.Khmer:
                        break;
                    case MacintoshEncodingId.Thai:
                        encodingName = "MacThai";
                        break;
                    case MacintoshEncodingId.Laotian:
                        break;
                    case MacintoshEncodingId.Georgian:
                        // TODO: ??? is it?
                        encodingName = "MacCyrillic";
                        break;
                    case MacintoshEncodingId.Armenian:
                        break;
                    case MacintoshEncodingId.ChineseSimplified:
                        encodingName = "EUC-CN";
                        break;
                    case MacintoshEncodingId.Tibetan:
                        break;
                    case MacintoshEncodingId.Mongolian:
                        // TODO: ??? is it?
                        encodingName = "MacCyrillic";
                        break;
                    case MacintoshEncodingId.Geez:
                        break;
                    case MacintoshEncodingId.Slavic:
                        // TODO: ??? is it?
                        encodingName = "MacCyrillic";
                        break;
                    case MacintoshEncodingId.Vietnamese:
                        break;
                    case MacintoshEncodingId.Sindhi:
                        break;
                    case MacintoshEncodingId.Uninterpreted:
                        break;
                }
                break;
            case PlatformId.ISO:
                break;
            case PlatformId.Windows:
                switch ((WindowsEncodingId)(encodingId))
                {
                    case WindowsEncodingId.Symbol:
                        encodingName = "UTF-16BE";
                        break;
                    case WindowsEncodingId.UnicodeUCS2:
                        encodingName = "UTF-16BE";
                        break;
                    case WindowsEncodingId.ShiftJIS:
                        encodingName = "windows-932";
                        break;
                    case WindowsEncodingId.PRC:
                        encodingName = "windows-936";
                        break;
                    case WindowsEncodingId.Big5:
                        encodingName = "windows-950";
                        break;
                    case WindowsEncodingId.Wansung:
                        encodingName = "windows-949";
                        break;
                    case WindowsEncodingId.Johab:
                        encodingName = "ms1361";
                        break;
                    case WindowsEncodingId.UnicodeUCS4:
                        encodingName = "UCS-4";
                        break;
                }
                break;
            case PlatformId.Custom:
                break;
            default:
                break;
        }
        return encodingName;
    }

    // TODO: caching of charsets?
    private static Charset getCharset(int platformId, int encodingId)
    {
        String encodingName = NameTable.getEncodingName(platformId, encodingId);
        if (encodingName == null)
        {
            return null;
        }
        Charset charset = Charset.GetEncoding(encodingName);
        return charset;
    }

    // TODO(stuartg):
    // do the conversion by hand to detect conversion failures (i.e. no character in the encoding)
    private static byte[] convertToNameBytes(String name, int platformId, int encodingId)
    {
        Charset cs = NameTable.getCharset(platformId, encodingId);
        if (cs == null)
        {
            return null;
        }
        var bb = cs.GetBytes(name);
        return bb;
    }

    public static String convertFromNameBytes(byte[] nameBytes, int platformId, int encodingId)
    {
        Charset cs = NameTable.getCharset(platformId, encodingId);
        if (cs == null)
        {
            return NumberHelper.toHexString(platformId);
        }
        var cb = cs.GetString(nameBytes);
        return cb;
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

    public interface IBuilder : SubTableContainerTable.IBuilder<NameTable>
    {
        void revertNames();
        int builderCount();
        void clear();
        boolean has(int platformId, int encodingId, int languageId, int nameId);
        NameEntryBuilder nameBuilder(int platformId, int encodingId, int languageId, int nameId);
        boolean remove(int platformId, int encodingId, int languageId, int nameId);
    }
    private class Builder : SubTableContainerTable.Builder<NameTable>, IBuilder
    {

        private IDictionary<NameEntryId, NameEntryBuilder> nameEntryMap;


        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        private void initialize(ReadableFontData data)
        {
            this.nameEntryMap = new Dictionary<NameEntryId, NameEntryBuilder>();

            if (data != null)
            {
                NameTable table = new NameTable(this.header(), data);

                var nameIter = table.GetEnumerator();
                while (nameIter.MoveNext())
                {
                    NameEntry nameEntry = nameIter.Current;
                    NameEntryBuilder nameEntryBuilder = new NameEntryBuilder(nameEntry);
                    this.nameEntryMap.put(nameEntryBuilder.getNameEntryId(), nameEntryBuilder);
                }
            }
        }

        private IDictionary<NameEntryId, NameEntryBuilder> getNameBuilders()
        {
            if (this.nameEntryMap == null)
            {
                this.initialize(base.internalReadData());
            }
            base.setModelChanged();
            return this.nameEntryMap;
        }

        /**
         * Revert the name builders for the name table to the last version that came
         * from data.
         */
        public void revertNames()
        {
            this.nameEntryMap = null;
            this.setModelChanged(false);
        }

        public int builderCount()
        {
            return this.getNameBuilders().Count;
        }

        /**
         * Clear the name builders for the name table.
         */
        public void clear()
        {
            this.getNameBuilders().Clear();
        }

        public boolean has(int platformId, int encodingId, int languageId, int nameId)
        {
            NameEntryId probe = new NameEntryId(platformId, encodingId, languageId, nameId);
            return this.getNameBuilders().containsKey(probe);
        }

        public NameEntryBuilder nameBuilder(int platformId, int encodingId, int languageId, int nameId)
        {
            NameEntryId probe = new NameEntryId(platformId, encodingId, languageId, nameId);
            NameEntryBuilder builder = this.getNameBuilders().get(probe);
            if (builder == null)
            {
                builder = new NameEntryBuilder(probe);
                this.getNameBuilders().put(probe, builder);
            }
            return builder;
        }

        public boolean remove(int platformId, int encodingId, int languageId, int nameId)
        {
            NameEntryId probe = new NameEntryId(platformId, encodingId, languageId, nameId);
            return (this.getNameBuilders().remove(probe) != null);
        }

        // subclass API implementation

        public override NameTable subBuildTable(ReadableFontData data)
        {
            return new NameTable(this.header(), data);
        }

        public override void subDataSet()
        {
            this.nameEntryMap = null;
            base.setModelChanged(false);
        }

        public override int subDataSizeToSerialize()
        {
            if (this.nameEntryMap == null || this.nameEntryMap.Count == 0)
            {
                return 0;
            }

            int size = (int)NameTable.Offset.nameRecordStart + this.nameEntryMap.Count
                * (int)NameTable.Offset.nameRecordSize;
            foreach (var entry in this.nameEntryMap.entrySet())
            {
                size += entry.getValue().nameAsBytes().Length;
            }
            return size;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.nameEntryMap == null || this.nameEntryMap.Count == 0)
            {
                return false;
            }
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int stringTableStartOffset =
                (int)NameTable.Offset.nameRecordStart + this.nameEntryMap.Count
                    * (int)NameTable.Offset.nameRecordSize;

            // header
            newData.writeUShort((int)NameTable.Offset.format, 0);
            newData.writeUShort((int)NameTable.Offset.count, this.nameEntryMap.Count);
            newData.writeUShort((int)NameTable.Offset.stringOffset, stringTableStartOffset);
            int nameRecordOffset = (int)NameTable.Offset.nameRecordStart;
            int stringOffset = 0;
            foreach (var entry in this.nameEntryMap.entrySet())
            {
                // lookup table
                newData.writeUShort(nameRecordOffset + (int)NameTable.Offset.nameRecordPlatformId,
                    entry.getKey().getPlatformId());
                newData.writeUShort(nameRecordOffset + (int)NameTable.Offset.nameRecordEncodingId,
                    entry.getKey().getEncodingId());
                newData.writeUShort(nameRecordOffset + (int)NameTable.Offset.nameRecordLanguageId,
                    entry.getKey().getLanguageId());
                newData.writeUShort(nameRecordOffset + (int)NameTable.Offset.nameRecordNameId,
                    entry.getKey().getNameId());
                newData.writeUShort(nameRecordOffset + (int)NameTable.Offset.nameRecordStringLength,
                    entry.getValue().nameAsBytes().Length);
                newData.writeUShort(
                    nameRecordOffset + (int)NameTable.Offset.nameRecordStringOffset, stringOffset);
                nameRecordOffset += (int)NameTable.Offset.nameRecordSize;
                // string table
                byte[] nameBytes = entry.getValue().nameAsBytes();
                if (nameBytes.Length > 0)
                {
                    stringOffset += newData.writeBytes(
                        stringOffset + stringTableStartOffset, entry.getValue().nameAsBytes());
                }
            }
            return stringOffset + stringTableStartOffset;
        }
    }
}
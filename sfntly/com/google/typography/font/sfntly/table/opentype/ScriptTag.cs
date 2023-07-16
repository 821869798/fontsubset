// Copyright 2012 Google Inc. All Rights Reserved.

namespace com.google.typography.font.sfntly.table.opentype;



/**
 * @author dougfelt@google.com (Doug Felt)
 */
public sealed class ScriptTag : ClassEnumBase<ScriptTag>
{
    public static readonly ScriptTag
  arab = new("Arabic"),
  armn = new("Armenian"),
  avst = new("Avestan"),
  bali = new("Balinese"),
  bamu = new("Bamum"),
  batk = new("Batak"),
  beng = new("Bengali"),
  bng2 = new("Bengali v.2"),
  bopo = new("Bopomofo"),
  brai = new("Braille"),
  brah = new("Brahmi"),
  bugi = new("Buginese"),
  buhd = new("Buhid"),
  byzm = new("Byzantine Music"),
  cans = new("Canadian Syllabics"),
  cari = new("Carian"),
  cakm = new("Chakma"),
  cham = new("Cham"),
  cher = new("Cherokee"),
  hani = new("CJK Ideographic"),
  copt = new("Coptic"),
  cprt = new("Cypriot Syllabary"),
  cyrl = new("Cyrillic"),
  DFLT = new("Default"),
  dsrt = new("Deseret"),
  deva = new("Devanagari"),
  dev2 = new("Devanagari v.2"),
  egyp = new("Egyptian heiroglyphs"),
  ethi = new("Ethiopic"),
  geor = new("Georgian"),
  glag = new("Glagolitic"),
  goth = new("Gothic"),
  grek = new("Greek"),
  gujr = new("Gujarati"),
  gjr2 = new("Gujarati v.2"),
  guru = new("Gurmukhi"),
  gur2 = new("Gurmukhi v.2"),
  hang = new("Hangul"),
  jamo = new("Hangul Jamo"),
  hano = new("Hanunoo"),
  hebr = new("Hebrew"),
  kana = new("Hiragana or Katakana"),
  armi = new("Imperial Aramaic"),
  phli = new("Inscriptional Pahlavi"),
  prti = new("Inscriptional Parthian"),
  java = new("Javanese"),
  kthi = new("Kaithi"),
  knda = new("Kannada"),
  knd2 = new("Kannada v.2"),
  kali = new("Kayah Li"),
  khar = new("Kharosthi"),
  khmr = new("Khmer"),
  lao = new("Lao"),
  latn = new("Latin"),
  lepc = new("Lepcha"),
  limb = new("Limbu"),
  linb = new("Linear B"),
  lisu = new("Lisu (Fraser)"),
  lyci = new("Lycian"),
  lydi = new("Lydian"),
  mlym = new("Malayalam"),
  mlm2 = new("Malayalam v.2"),
  mly2 = new("Malayalam v.2 alt"),
  mand = new("Mandaic, Mandaean"),
  math = new("Mathematical Alphanumeric Symbols"),
  mtei = new("Meitei Mayek (Meithei, Meetei)"),
  merc = new("Meroitic Cursive"),
  mero = new("Meroitic Hieroglyphs"),
  mong = new("Mongolian"),
  musc = new("Musical Symbols"),
  musi = new("Musical Symbols Alt"),
  mymr = new("Myanmar"),
  mym2 = new("Myanmar v.2"),
  talu = new("New Tai Lue"),
  nko = new("N'Ko"),
  ogam = new("Ogham"),
  olck = new("Ol Chiki"),
  ital = new("Old Italic"),
  xpeo = new("Old Persian Cuneiform"),
  sarb = new("Old South Arabian"),
  orkh = new("Old Turkic, Orkhon Runic"),
  orya = new("Odia (formerly Oriya)"),
  ory2 = new("Odia v.2 (formerly Oriya v.2)"),
  osma = new("Osmanya"),
  phag = new("Phags-pa"),
  phnx = new("Phoenician"),
  rjng = new("Rejang"),
  runr = new("Runic"),
  samr = new("Samaritan"),
  saur = new("Saurashtra"),
  shrd = new("Sharada"),
  shaw = new("Shavian"),
  sinh = new("Sinhala"),
  sora = new("Sora Sompeng"),
  xsux = new("Sumero-Akkadian Cuneiform"),
  sund = new("Sundanese"),
  sylo = new("Syloti Nagri"),
  syrc = new("Syriac"),
  tglg = new("Tagalog"),
  tagb = new("Tagbanwa"),
  tale = new("Tai Le"),
  lana = new("Tai Tham (Lanna)"),
  tavt = new("Tai Viet"),
  takr = new("Takri"),
  taml = new("Tamil"),
  tml2 = new("Tamil v.2"),
  telu = new("Telugu"),
  tel2 = new("Telugu v.2"),
  thaa = new("Thaana"),
  thai = new("Thai"),
  tibt = new("Tibetan"),
  tfng = new("Tifinagh"),
  ugar = new("Ugaritic Cuneiform"),
  vai = new("Vai"),
  yi = new("Yi");

    private ScriptTag(String description)
    {
        String tag = name();
        while (tag.Length < 4)
        {
            tag = tag + ' ';
        }
        this._tag = Tag.intValue(tag);
        this._description = description;
    }

    public int tag()
    {
        return _tag;
    }

    public String description()
    {
        return _description;
    }

    private readonly int _tag;
    private readonly String _description;

    public static ScriptTag fromTag(int tag)
    {
        foreach (ScriptTag script in ScriptTag.values())
        {
            if (script._tag == tag)
            {
                return script;
            }
        }
        throw new IllegalArgumentException(Tag.stringValue(tag));
    }
}
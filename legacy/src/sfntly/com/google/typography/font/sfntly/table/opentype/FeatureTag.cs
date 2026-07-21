// Copyright 2012 Google Inc. All Rights Reserved.

using System.Xml.Linq;

namespace com.google.typography.font.sfntly.table.opentype;






/**
 * @author dougfelt@google.com (Doug Felt)
 */
sealed class FeatureTag : ClassEnumBase<FeatureTag>
{
    public static readonly FeatureTag
  aalt = new("Access All Alternates"),
  abvf = new("Above-base Forms"),
  abvm = new("Above-base Mark Positioning"),
  abvs = new("Above-base Substitutions"),
  afrc = new("Alternative Fractions"),
  akhn = new("Akhands"),
  blwf = new("Below-base Forms"),
  blwm = new("Below-base Mark Positioning"),
  blws = new("Below-base Substitutions"),
  calt = new("Contextual Alternates"),
  // Note, 'case' collides with a reserved word in java,
  // so the enum constant has a trailing underscore
  case_ = new("case", "Case-Sensitive Forms"),
  ccmp = new("Glyph Composition / Decomposition"),
  cfar = new("Conjunct Form After Ro"),
  cjct = new("Conjunct Forms"),
  clig = new("Contextual Ligatures"),
  cpct = new("Centered CJK Punctuation"),
  cpsp = new("Capital Spacing"),
  cswh = new("Contextual Swash"),
  curs = new("Cursive Positioning"),
  cv01 = new("Character Variants 1"),
  cv02 = new("Character Variants 2"),
  cv03 = new("Character Variants 3"),
  cv04 = new("Character Variants 4"),
  cv05 = new("Character Variants 5"),
  cv06 = new("Character Variants 6"),
  cv07 = new("Character Variants 7"),
  cv08 = new("Character Variants 8"),
  cv09 = new("Character Variants 9"),
  cv10 = new("Character Variants 10"),
  // continues to cv99, omitted here
  c2pc = new("Petite Capitals From Capitals"),
  c2sc = new("Small Capitals From Capitals"),
  dist = new("Distances"),
  dlig = new("Discretionary Ligatures"),
  dnom = new("Denominators"),
  expt = new("Expert Forms"),
  falt = new("Final Glyph on Line Alternates"),
  fin2 = new("Terminal Forms #2"),
  fin3 = new("Terminal Forms #3"),
  fina = new("Terminal Forms"),
  frac = new("Fractions"),
  fwid = new("Full Widths"),
  half = new("Half Forms"),
  haln = new("Halant Forms"),
  halt = new("Alternate Half Widths"),
  hist = new("Historical Forms"),
  hkna = new("Horizontal Kana Alternates"),
  hlig = new("Historical Ligatures"),
  hngl = new("Hangul"),
  hojo = new("Hojo Kanji Forms (JIS X 0212-1990 Kanji Forms)"),
  hwid = new("Half Widths"),
  init = new("Initial Forms"),
  isol = new("Isolated Forms"),
  ital = new("Italics"),
  jalt = new("Justification Alternates"),
  jp78 = new("JIS78 Forms"),
  jp83 = new("JIS83 Forms"),
  jp90 = new("JIS90 Forms"),
  jp04 = new("JIS2004 Forms"),
  kern = new("Kerning"),
  lfbd = new("Left Bounds"),
  liga = new("Standard Ligatures"),
  ljmo = new("Leading Jamo Forms"),
  lnum = new("Lining Figures"),
  locl = new("Localized Forms"),
  ltra = new("Left-to-right alternates"),
  ltrm = new("Left-to-right mirrored forms"),
  mark = new("Mark Positioning"),
  med2 = new("Medial Forms #2"),
  medi = new("Medial Forms"),
  mgrk = new("Mathematical Greek"),
  mkmk = new("Mark to Mark Positioning"),
  mset = new("Mark Positioning via Substitution"),
  nalt = new("Alternate Annotation Forms"),
  nlck = new("NLC Kanji Forms"),
  nukt = new("Nukta Forms"),
  numr = new("Numerators"),
  onum = new("Oldstyle Figures"),
  opbd = new("Optical Bounds"),
  ordn = new("Ordinals"),
  ornm = new("Ornaments"),
  palt = new("Proportional Alternate Widths"),
  pcap = new("Petite Capitals"),
  pkna = new("Proportional Kana"),
  pnum = new("Proportional Figures"),
  pref = new("Pre-Base Forms"),
  pres = new("Pre-base Substitutions"),
  pstf = new("Post-base Forms"),
  psts = new("Post-base Substitutions"),
  pwid = new("Proportional Widths"),
  qwid = new("Quarter Widths"),
  rand = new("Randomize"),
  rkrf = new("Rakar Forms"),
  rlig = new("Required Ligatures"),
  rphf = new("Reph Forms"),
  rtbd = new("Right Bounds"),
  rtla = new("Right-to-left alternates"),
  rtlm = new("Right-to-left mirrored forms"),
  ruby = new("Ruby Notation Forms"),
  salt = new("Stylistic Alternates"),
  sinf = new("Scientific Inferiors"),
  size = new("Optical size"),
  smcp = new("Small Capitals"),
  smpl = new("Simplified Forms"),
  ss01 = new("Stylistic Set 1"),
  ss02 = new("Stylistic Set 2"),
  ss03 = new("Stylistic Set 3"),
  ss04 = new("Stylistic Set 4"),
  ss05 = new("Stylistic Set 5"),
  ss06 = new("Stylistic Set 6"),
  ss07 = new("Stylistic Set 7"),
  ss08 = new("Stylistic Set 8"),
  ss09 = new("Stylistic Set 9"),
  ss10 = new("Stylistic Set 10"),
  ss11 = new("Stylistic Set 11"),
  ss12 = new("Stylistic Set 12"),
  ss13 = new("Stylistic Set 13"),
  ss14 = new("Stylistic Set 14"),
  ss15 = new("Stylistic Set 15"),
  ss16 = new("Stylistic Set 16"),
  ss17 = new("Stylistic Set 17"),
  ss18 = new("Stylistic Set 18"),
  ss19 = new("Stylistic Set 19"),
  ss20 = new("Stylistic Set 20"),
  subs = new("Subscript"),
  sups = new("Superscript"),
  swsh = new("Swash"),
  titl = new("Titling"),
  tjmo = new("Trailing Jamo Forms"),
  tnam = new("Traditional Name Forms"),
  tnum = new("Tabular Figures"),
  trad = new("Traditional Forms"),
  twid = new("Third Widths"),
  unic = new("Unicase"),
  valt = new("Alternate Vertical Metrics"),
  vatu = new("Vattu Variants"),
  vert = new("Vertical Writing"),
  vhal = new("Alternate Vertical Half Metrics"),
  vjmo = new("Vowel Jamo Forms"),
  vkna = new("Vertical Kana Alternates"),
  vkrn = new("Vertical Kerning"),
  vpal = new("Proportional Alternate Vertical Metrics"),
  vrt2 = new("Vertical Alternates and Rotation"),
  zero = new("Slashed Zero");

    private static IDictionary<Integer, FeatureTag> tagMap;
    private static readonly object TYPE_LOCK = new object();

    private FeatureTag(String _name)
    {
        this._tag = Tag.intValue(name());
        this._name = _name;
    }

    private FeatureTag(String tagName, String name)
    {
        this._tag = Tag.intValue(tagName);
        this._name = name;
    }

    public static FeatureTag forTagValue(int value)
    {
        lock (TYPE_LOCK)
        {
            if (tagMap == null)
            {
                IDictionary<Integer, FeatureTag> map = new Dictionary<Integer, FeatureTag>();
                foreach (FeatureTag tag in values())
                {
                    map.put(tag.tag(), tag);
                }
                tagMap = map;
            }
            return tagMap.get(value);
        }
    }

    private int tag()
    {
        return _tag;
    }

    public String longName()
    {
        return _name;
    }

    private readonly int _tag;
    private readonly String _name;
}
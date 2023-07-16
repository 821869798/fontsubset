using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public class GlyphClassList : NumRecordList {
  private GlyphClassList(WritableFontData data) :base(data) {
  }

  private GlyphClassList(ReadableFontData data) :base(data) {
  }

  private GlyphClassList(ReadableFontData data, int countDecrement) :base(data, countDecrement) {
  }

  private GlyphClassList(
      ReadableFontData data, int countDecrement, int countOffset, int valuesOffset) :base(data, countDecrement, countOffset, valuesOffset) {
  }

  public GlyphClassList(NumRecordList other) :base(other) {
  }

  public static int sizeOfListOfCount(int count) {
    return RecordList<Record>.DATA_OFFSET + count * NumRecord.RECORD_SIZE;
  }
}

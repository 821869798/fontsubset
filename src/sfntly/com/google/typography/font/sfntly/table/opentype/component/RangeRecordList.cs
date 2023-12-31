using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public sealed class RangeRecordList : RecordList<RangeRecord> {
  public RangeRecordList(WritableFontData data) :base(data) {
  }

  public RangeRecordList(ReadableFontData data) :base(data) {
  }

  public static int sizeOfListOfCount(int count) {
    return RecordList<Record>.DATA_OFFSET + count * RangeRecord.RECORD_SIZE;
  }

  public override RangeRecord getRecordAt(ReadableFontData data, int offset) {
    return new RangeRecord(data, offset);
  }

  public override int recordSize() {
    return RangeRecord.RECORD_SIZE;
  }
}

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public sealed class SubstLookupRecordList : RecordList<SubstLookupRecord> {
  private SubstLookupRecordList(WritableFontData data) :base(data) {
  }

  public SubstLookupRecordList(ReadableFontData data, int @base) :base(data, 0, @base) {
  }

  public SubstLookupRecordList(ReadableFontData data, int countOffset, int valuesOffset) :base(data, 0, countOffset, valuesOffset) {
  }

  public override SubstLookupRecord getRecordAt(ReadableFontData data, int offset) {
    return new SubstLookupRecord(data, offset);
  }

  public override int recordSize() {
    return SubstLookupRecord.RECORD_SIZE;
  }
}

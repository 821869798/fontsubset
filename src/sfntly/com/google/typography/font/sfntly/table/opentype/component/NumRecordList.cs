using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;






public class NumRecordList : RecordList<NumRecord> {
  public NumRecordList(WritableFontData data) :base(data) {
  }

  public NumRecordList(ReadableFontData data) :base(data) {
  }

  public NumRecordList(ReadableFontData data, int countDecrement) :base(data, countDecrement) {
  }

  public NumRecordList(ReadableFontData data, int countDecrement, int countOffset) :base(data, countDecrement, countOffset) {
  }

  public NumRecordList(
      ReadableFontData data, int countDecrement, int countOffset, int valuesOffset) :base(data, countDecrement, countOffset, valuesOffset) {
  }

  public NumRecordList(NumRecordList other) :base(other) {
  }

  public static int sizeOfListOfCount(int count) {
    return RecordList<Record>.DATA_OFFSET + count * NumRecord.RECORD_SIZE;
  }

  public boolean contains(int value) {
    var iterator = this.GetEnumerator();
    while (iterator.MoveNext()) {
      NumRecord record = iterator.Current;
      if (record.value == value) {
        return true;
      }
    }
    return false;
  }

  public override NumRecord getRecordAt(ReadableFontData data, int offset) {
    return new NumRecord(data, offset);
  }

  public override int recordSize() {
    return NumRecord.RECORD_SIZE;
  }
}

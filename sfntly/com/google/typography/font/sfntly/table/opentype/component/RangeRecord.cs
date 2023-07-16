using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public sealed class RangeRecord : Record {
  public static readonly int RECORD_SIZE =6;
  private static readonly int START_OFFSET = 0;
  private static readonly int END_OFFSET = 2;
  private static readonly int PROPERTY_OFFSET = 4;
    public readonly int start;
    public readonly int end;
    public readonly int property;

    public RangeRecord(ReadableFontData data, int @base) {
    this.start = data.readUShort(@base + START_OFFSET);
    this.end = data.readUShort(@base + END_OFFSET);
    this.property = data.readUShort(@base + PROPERTY_OFFSET);
  }

  public int writeTo(WritableFontData newData, int @base) {
    newData.writeUShort(@base + START_OFFSET, start);
    newData.writeUShort(@base + END_OFFSET, end);
    return RECORD_SIZE;
  }
}

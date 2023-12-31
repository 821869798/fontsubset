using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




sealed class TagOffsetRecord : Record {
  public static readonly int RECORD_SIZE =6;
  private static readonly int TAG_POS = 0;
  private static readonly int OFFSET_POS = 4;
  public readonly int tag;
  public readonly int offset;

  public TagOffsetRecord(ReadableFontData data, int @base) {
    this.tag = data.readULongAsInt(@base + TAG_POS);
    this.offset = data.readUShort(@base + OFFSET_POS);
  }

    public TagOffsetRecord(int tag, int offset) {
    this.tag = tag;
    this.offset = offset;
  }

  public int writeTo(WritableFontData newData, int @base) {
    newData.writeULong(@base + TAG_POS, tag);
    newData.writeUShort(@base + OFFSET_POS, offset);
    return RECORD_SIZE;
  }
}

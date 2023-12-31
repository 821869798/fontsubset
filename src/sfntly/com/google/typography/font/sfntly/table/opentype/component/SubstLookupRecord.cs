using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public sealed class SubstLookupRecord : Record {
  public static readonly int RECORD_SIZE =4;
  private static readonly int SEQUENCE_INDEX_OFFSET = 0;
  private static readonly int LOOKUP_LIST_INDEX_OFFSET = 2;
    public readonly int sequenceIndex;
    public readonly int lookupListIndex;

  public SubstLookupRecord(ReadableFontData data, int @base) {
    this.sequenceIndex = data.readUShort(@base + SEQUENCE_INDEX_OFFSET);
    this.lookupListIndex = data.readUShort(@base + LOOKUP_LIST_INDEX_OFFSET);
  }

  public int writeTo(WritableFontData newData, int @base) {
    newData.writeUShort(@base + SEQUENCE_INDEX_OFFSET, sequenceIndex);
    newData.writeUShort(@base + LOOKUP_LIST_INDEX_OFFSET, lookupListIndex);
    return RECORD_SIZE;
  }
}

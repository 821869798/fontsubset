using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;






sealed class TagOffsetRecordList : RecordList<TagOffsetRecord>
{
    public TagOffsetRecordList(WritableFontData data) : base(data)
    {
    }

    public TagOffsetRecordList(ReadableFontData data) : base(data)
    {
    }

    public static int sizeOfListOfCount(int count)
    {
        return RecordList<Record>.DATA_OFFSET + count * TagOffsetRecord.RECORD_SIZE;
    }

    public TagOffsetRecord getRecordForTag(int tag)
    {
        IEnumerator<TagOffsetRecord> iterator = GetEnumerator();
        while (iterator.MoveNext())
        {
            TagOffsetRecord record = iterator.Current;
            if (record.tag == tag)
            {
                return record;
            }
        }
        return null;
    }

    public override TagOffsetRecord getRecordAt(ReadableFontData data, int offset)
    {
        return new TagOffsetRecord(data, offset);
    }

    public override int recordSize()
    {
        return TagOffsetRecord.RECORD_SIZE;
    }
}
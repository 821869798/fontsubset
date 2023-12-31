using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;




public sealed class NumRecord : Record
{
    public static readonly int RECORD_SIZE = 2;
    private static readonly int TAG_POS = 0;
    public readonly int value;

    public NumRecord(ReadableFontData data, int @base)
    {
        this.value = data.readUShort(@base + TAG_POS);
    }

    public NumRecord(int num)
    {
        this.value = num;
    }

    public int writeTo(WritableFontData newData, int @base)
    {
        newData.writeUShort(@base + TAG_POS, value);
        return RECORD_SIZE;
    }
}
using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;



public class RangeRecordTable : RecordsTable<RangeRecord>
{
    public RangeRecordTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override RecordList<RangeRecord> createRecordList(ReadableFontData data)
    {
        return new RangeRecordList(data);
    }

    public override int fieldCount()
    {
        return 0;
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }

    public interface IBuilder : RecordsTable<RangeRecord>.IBuilder<RangeRecordTable>
    {

    }

    private class Builder : RecordsTable<RangeRecord>.Builder<RangeRecordTable>, IBuilder
    {
        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override RangeRecordTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new RangeRecordTable(data, @base, dataIsCanonical);
        }

        public override RecordList<RangeRecord> readRecordList(ReadableFontData data, int @base)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new RangeRecordList(data);
        }

        public override int fieldCount()
        {
            return 0;
        }

        public override void initFields()
        {
        }
    }
}
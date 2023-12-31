using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;



public class NumRecordTable : RecordsTable<NumRecord>
{

    public NumRecordTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public NumRecordTable(NumRecordList records) : base(records)
    {
    }

    public override RecordList<NumRecord> createRecordList(ReadableFontData data)
    {
        return new NumRecordList(data);
    }

    public override int fieldCount()
    {
        return 0;
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }


    public static IBuilder createBuilder(NumRecordTable table)
    {
        return new Builder(table);
    }

    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<NumRecordTable>
    {

    }

    private class Builder : RecordsTable<NumRecord>.Builder<NumRecordTable>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public Builder(NumRecordTable table) : base(table)
        {
        }

        public override NumRecordTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new NumRecordTable(data, @base, dataIsCanonical);
        }

        public override RecordList<NumRecord> readRecordList(ReadableFontData data, int @base)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new NumRecordList(data);
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
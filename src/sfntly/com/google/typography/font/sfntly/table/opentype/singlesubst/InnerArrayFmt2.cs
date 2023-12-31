using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.singlesubst;








public class InnerArrayFmt2 : RecordsTable<NumRecord>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;
    public readonly CoverageTable coverage;

    public InnerArrayFmt2(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public override RecordList<NumRecord> createRecordList(ReadableFontData data)
    {
        return new NumRecordList(data);
    }



    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }


    public static IBuilder createBuilder(InnerArrayFmt2 table)
    {
        return new Builder(table);
    }

    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<InnerArrayFmt2>
    {

    }

    private class Builder : RecordsTable<NumRecord>.Builder<InnerArrayFmt2>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(InnerArrayFmt2 table) : base(table)
        {
        }

        public override InnerArrayFmt2 readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new InnerArrayFmt2(data, @base, dataIsCanonical);
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override RecordList<NumRecord> readRecordList(ReadableFontData data, int @base)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new NumRecordList(data);
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }
}
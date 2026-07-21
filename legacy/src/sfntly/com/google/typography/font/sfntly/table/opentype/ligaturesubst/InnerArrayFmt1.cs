using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.ligaturesubst;






public class InnerArrayFmt1 : OffsetRecordTable<LigatureSet>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;
    public readonly CoverageTable coverage;

    public InnerArrayFmt1(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public override LigatureSet readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new LigatureSet(data, 0, dataIsCanonical);
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }


    public static IBuilder createBuilder(InnerArrayFmt1 table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<LigatureSet>.IBuilder<InnerArrayFmt1>
    {
    }
    private class Builder : OffsetRecordTable<LigatureSet>.Builder<InnerArrayFmt1>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(InnerArrayFmt1 table) : base(table)
        {
        }

        public override InnerArrayFmt1 readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new InnerArrayFmt1(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<LigatureSet> createSubTableBuilder()
        {
            return LigatureSet.createBuilder();
        }

        public override VisibleSubTable.IBuilder<LigatureSet> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return LigatureSet.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<LigatureSet> createSubTableBuilder(LigatureSet subTable)
        {
            return LigatureSet.createBuilder(subTable);
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }
}
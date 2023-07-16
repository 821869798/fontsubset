using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;






public class ChainSubRuleSetArray : OffsetRecordTable<ChainSubRuleSet>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;

    public readonly CoverageTable coverage;

    public ChainSubRuleSetArray(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public override ChainSubRuleSet readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new ChainSubRuleSet(data, 0, dataIsCanonical);
    }
    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(ChainSubRuleSetArray table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<ChainSubRuleSet>.IBuilder<ChainSubRuleSetArray>
    {

    }
    private class Builder : OffsetRecordTable<ChainSubRuleSet>.Builder<ChainSubRuleSetArray>, IBuilder
    {

        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(ChainSubRuleSetArray table) : base(table)
        {
        }

        public override ChainSubRuleSetArray readTable(
            ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new ChainSubRuleSetArray(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubRuleSet> createSubTableBuilder()
        {
            return ChainSubRuleSet.createBuilder();
        }

        public override VisibleSubTable.IBuilder<ChainSubRuleSet> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return ChainSubRuleSet.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubRuleSet> createSubTableBuilder(
            ChainSubRuleSet subTable)
        {
            return ChainSubRuleSet.createBuilder(subTable);
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
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;






public class SubRuleSetArray : OffsetRecordTable<SubRuleSet>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;

    public readonly CoverageTable coverage;

    public SubRuleSetArray(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public override SubRuleSet readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new SubRuleSet(data, 0, dataIsCanonical);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(SubRuleSetArray table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<SubRuleSet>.IBuilder<SubRuleSetArray>
    {

    }

    private class Builder : OffsetRecordTable<SubRuleSet>.Builder<SubRuleSetArray>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(SubRuleSetArray table) : base(table)
        {
        }

        public override SubRuleSetArray readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new SubRuleSetArray(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubRuleSet> createSubTableBuilder()
        {
            return SubRuleSet.createBuilder();
        }

        public override VisibleSubTable.IBuilder<SubRuleSet> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return SubRuleSet.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubRuleSet> createSubTableBuilder(SubRuleSet subTable)
        {
            return SubRuleSet.createBuilder(subTable);
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
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;







public class SubClassSetArray : OffsetRecordTable<SubClassSet>
{
    private static readonly int FIELD_COUNT = 2;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;
    private static readonly int CLASS_DEF_INDEX = 1;
    private static readonly int CLASS_DEF_DEFAULT = 0;

    public readonly CoverageTable coverage;
    public readonly ClassDefTable classDef;

    public SubClassSetArray(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
        int classDefOffset = getField(CLASS_DEF_INDEX);
        classDef = new ClassDefTable(data.slice(classDefOffset), 0, dataIsCanonical);
    }

    public override SubClassSet readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new SubClassSet(data, 0, dataIsCanonical);
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical, boolean isFmt2)
    {
        return new Builder(data, dataIsCanonical, isFmt2);
    }

    public static IBuilder createBuilder(SubClassSetArray table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<SubClassSet>.IBuilder<SubClassSetArray>
    {

    }

    private class Builder : OffsetRecordTable<SubClassSet>.Builder<SubClassSetArray>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical, boolean isFmt2) : base(data, dataIsCanonical)
        {
        }

        public Builder(SubClassSetArray table) : base(table)
        {
        }

        public override SubClassSetArray readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new SubClassSetArray(data, @base, dataIsCanonical);
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
            setField(CLASS_DEF_INDEX, CLASS_DEF_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override VisibleSubTable.IBuilder<SubClassSet> createSubTableBuilder()
        {
            return SubClassSet.createBuilder();
        }

        public override VisibleSubTable.IBuilder<SubClassSet> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return SubClassSet.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubClassSet> createSubTableBuilder(SubClassSet subTable)
        {
            return SubClassSet.createBuilder(subTable);
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }
}
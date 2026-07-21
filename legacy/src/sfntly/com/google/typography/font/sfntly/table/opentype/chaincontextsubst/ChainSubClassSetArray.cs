using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;







public class ChainSubClassSetArray : OffsetRecordTable<ChainSubClassSet>
{
    private static readonly int FIELD_COUNT = 4;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;
    private static readonly int BACKTRACK_CLASS_DEF_INDEX = 1;
    private static readonly int BACKTRACK_CLASS_DEF_DEFAULT = 0;
    private static readonly int INPUT_CLASS_DEF_INDEX = 2;
    private static readonly int INPUT_CLASS_DEF_DEFAULT = 0;
    private static readonly int LOOK_AHEAD_CLASS_DEF_INDEX = 3;
    private static readonly int LOOK_AHEAD_CLASS_DEF_DEFAULT = 0;

    public readonly CoverageTable coverage;
    public readonly ClassDefTable backtrackClassDef;
    public readonly ClassDefTable inputClassDef;
    public readonly ClassDefTable lookAheadClassDef;

    public ChainSubClassSetArray(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
        int classDefOffset = getField(BACKTRACK_CLASS_DEF_INDEX);
        backtrackClassDef = new ClassDefTable(data.slice(classDefOffset), 0, dataIsCanonical);
        classDefOffset = getField(INPUT_CLASS_DEF_INDEX);
        inputClassDef = new ClassDefTable(data.slice(classDefOffset), 0, dataIsCanonical);
        classDefOffset = getField(LOOK_AHEAD_CLASS_DEF_INDEX);
        lookAheadClassDef = new ClassDefTable(data.slice(classDefOffset), 0, dataIsCanonical);
    }

    public override ChainSubClassSet readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new ChainSubClassSet(data, 0, dataIsCanonical);
    }

    public interface IBuilder : OffsetRecordTable<ChainSubClassSet>.IBuilder<ChainSubClassSetArray>
    {

    }

    private class Builder : OffsetRecordTable<ChainSubClassSet>.Builder<ChainSubClassSetArray>, IBuilder
    {

        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(ChainSubClassSetArray table) : base(table)
        {
        }

        public override ChainSubClassSetArray readTable(
            ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new ChainSubClassSetArray(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubClassSet> createSubTableBuilder()
        {
            return ChainSubClassSet.createBuilder();
        }

        public override VisibleSubTable.IBuilder<ChainSubClassSet> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return ChainSubClassSet.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubClassSet> createSubTableBuilder(ChainSubClassSet subTable)
        {
            return ChainSubClassSet.createBuilder(subTable);
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
            setField(BACKTRACK_CLASS_DEF_INDEX, BACKTRACK_CLASS_DEF_DEFAULT);
            setField(INPUT_CLASS_DEF_INDEX, INPUT_CLASS_DEF_DEFAULT);
            setField(LOOK_AHEAD_CLASS_DEF_INDEX, LOOK_AHEAD_CLASS_DEF_DEFAULT);
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
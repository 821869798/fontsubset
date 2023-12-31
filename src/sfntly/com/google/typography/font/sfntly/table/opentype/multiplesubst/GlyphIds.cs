using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.multiplesubst;







public class GlyphIds : OffsetRecordTable<NumRecordTable>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;
    public readonly CoverageTable coverage;

    public GlyphIds(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    public override NumRecordTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new NumRecordTable(data, 0, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(GlyphIds table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<NumRecordTable>.IBuilder<GlyphIds>
    {

    }
    private class Builder : OffsetRecordTable<NumRecordTable>.Builder<GlyphIds>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(GlyphIds table) : base(table)
        {
        }

        public override GlyphIds readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new GlyphIds(data, @base, dataIsCanonical);
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override VisibleSubTable.IBuilder<NumRecordTable> createSubTableBuilder()
        {
            return NumRecordTable.createBuilder();
        }

        public override VisibleSubTable.IBuilder<NumRecordTable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return NumRecordTable.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<NumRecordTable> createSubTableBuilder(NumRecordTable subTable)
        {
            return NumRecordTable.createBuilder(subTable);
        }
    }
}
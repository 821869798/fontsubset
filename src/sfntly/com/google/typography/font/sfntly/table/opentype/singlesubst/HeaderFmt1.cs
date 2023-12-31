using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.singlesubst;





public class HeaderFmt1 : HeaderTable
{
    private static readonly int FIELD_COUNT = 2;

    private static readonly int COVERAGE_INDEX = 0;
    private static readonly int COVERAGE_DEFAULT = 0;

    private static readonly int DELTA_GLYPH_ID_INDEX = 1;
    private static readonly int DELTA_GLYPH_ID_DEFAULT = 0;

    public readonly CoverageTable coverage;

    public HeaderFmt1(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);
    }

    public int getDelta()
    {
        int delta = getField(DELTA_GLYPH_ID_INDEX);
        if (delta > 0x7FFF)
        {
            // Converting read unsigned int to signed short
            return (short)delta;
        }
        return delta;
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

    public static IBuilder createBuilder(HeaderFmt1 table)
    {
        return new Builder(table);
    }

    public interface IBuilder : IBuilder<HeaderFmt1>
    {

    }

    protected class Builder : Builder<HeaderFmt1>, IBuilder
    {
        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(HeaderFmt1 table) : base(table)
        {
        }

        public Builder() : base()
        {
        }

        public override void initFields()
        {
            setField(COVERAGE_INDEX, COVERAGE_DEFAULT);
            setField(DELTA_GLYPH_ID_INDEX, DELTA_GLYPH_ID_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override HeaderFmt1 subBuildTable(ReadableFontData data)
        {
            return new HeaderFmt1(data, 0, false);
        }
    }
}
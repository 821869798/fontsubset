using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;




public class SubClassRule : DoubleRecordTable
{
    public SubClassRule(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public GlyphClassList inputClasses()
    {
        return new GlyphClassList(inputGlyphs);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder(SubClassRule table)
    {
        return new Builder(table);
    }

    public interface IBuilder : DoubleRecordTable.IBuilder<SubClassRule>
    {

    }

    private class Builder : DoubleRecordTable.Builder<SubClassRule>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(SubClassRule table) : base(table)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override SubClassRule subBuildTable(ReadableFontData data)
        {
            return new SubClassRule(data, 0, true);
        }
    }
}
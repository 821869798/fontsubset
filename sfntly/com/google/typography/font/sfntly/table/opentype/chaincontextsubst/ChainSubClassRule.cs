using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;



public class ChainSubClassRule : ChainSubGenericRule
{
    public ChainSubClassRule(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ChainSubClassRule table)
    {
        return new Builder(table);
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }

    public interface IBuilder : ChainSubGenericRule.IBuilder<ChainSubClassRule>
    {

    }

    private class Builder : ChainSubGenericRule.Builder<ChainSubClassRule>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ChainSubClassRule table) : base(table)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override ChainSubClassRule subBuildTable(ReadableFontData data)
        {
            return new ChainSubClassRule(data, 0, true);
        }

    }
}
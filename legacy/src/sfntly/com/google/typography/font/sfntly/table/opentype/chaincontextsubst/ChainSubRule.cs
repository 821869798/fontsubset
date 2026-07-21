using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;



public class ChainSubRule : ChainSubGenericRule
{
    public ChainSubRule(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder(ChainSubRule table)
    {
        return new Builder(table);
    }
    public interface IBuilder : ChainSubGenericRule.IBuilder<ChainSubRule>
    {

    }
    private class Builder : ChainSubGenericRule.Builder<ChainSubRule>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ChainSubRule table) : base(table)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override ChainSubRule subBuildTable(ReadableFontData data)
        {
            return new ChainSubRule(data, 0, true);
        }
    }
}
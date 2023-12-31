using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;




public class ChainSubClassSet : ChainSubGenericRuleSet<ChainSubClassRule>
{
    public ChainSubClassSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override ChainSubClassRule readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new ChainSubClassRule(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ChainSubClassSet table)
    {
        return new Builder(table);
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public interface IBuilder : ChainSubGenericRuleSet<ChainSubClassRule>.IBuilder<ChainSubClassSet>
    {

    }

    private class Builder : ChainSubGenericRuleSet<ChainSubClassRule>.Builder<ChainSubClassSet>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ChainSubClassSet table) : base(table)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override ChainSubClassSet readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new ChainSubClassSet(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubClassRule> createSubTableBuilder()
        {
            return ChainSubClassRule.createBuilder();
        }

        public override VisibleSubTable.IBuilder<ChainSubClassRule> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return ChainSubClassRule.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubClassRule> createSubTableBuilder(
            ChainSubClassRule subTable)
        {
            return ChainSubClassRule.createBuilder(subTable);
        }

    }
}
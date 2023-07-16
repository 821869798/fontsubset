using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;




public class ChainSubRuleSet : ChainSubGenericRuleSet<ChainSubRule>
{
    public ChainSubRuleSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override ChainSubRule readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new ChainSubRule(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(ChainSubRuleSet table)
    {
        return new Builder(table);
    }
    public interface IBuilder : ChainSubGenericRuleSet<ChainSubRule>.IBuilder<ChainSubRuleSet>
    {

    }
    private class Builder : ChainSubGenericRuleSet<ChainSubRule>.Builder<ChainSubRuleSet>,IBuilder
    {

        public Builder() : base()
        {
        }

        public Builder(ChainSubRuleSet table) : base(table)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override ChainSubRuleSet readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new ChainSubRuleSet(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubRule> createSubTableBuilder()
        {
            return ChainSubRule.createBuilder();
        }

        public override VisibleSubTable.IBuilder<ChainSubRule> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return ChainSubRule.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ChainSubRule> createSubTableBuilder(ChainSubRule subTable)
        {
            return ChainSubRule.createBuilder(subTable);
        }
    }
}
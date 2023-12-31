using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;
using com.google.typography.font.sfntly.table.opentype.ligaturesubst;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;




public class SubRuleSet : SubGenericRuleSet<SubRule>
{
    public SubRuleSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override SubRule readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new SubRule(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(SubRuleSet table)
    {
        return new Builder(table);
    }

    public interface IBuilder : SubGenericRuleSet<SubRule>.IBuilder<SubRuleSet>
    {

    }

    private class Builder : SubGenericRuleSet<SubRule>.Builder<SubRuleSet>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(SubRuleSet table) : base(table)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override SubRuleSet readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new SubRuleSet(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubRule> createSubTableBuilder()
        {
            return SubRule.createBuilder();
        }

        public override VisibleSubTable.IBuilder<SubRule> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return SubRule.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubRule> createSubTableBuilder(SubRule subTable)
        {
            return SubRule.createBuilder(subTable);
        }
    }
}
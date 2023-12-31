using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;




public class SubClassSet : SubGenericRuleSet<SubClassRule>
{
    public SubClassSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(SubClassSet table)
    {
        return new Builder(table);
    }
    public override SubClassRule readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new SubClassRule(data, @base, dataIsCanonical);
    }

    public interface IBuilder : SubGenericRuleSet<SubClassRule>.IBuilder<SubClassSet>
    {

    }

    private class Builder : SubGenericRuleSet<SubClassRule>.Builder<SubClassSet>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(SubClassSet table) : base(table)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override VisibleSubTable.IBuilder<SubClassRule> createSubTableBuilder()
        {
            return SubClassRule.createBuilder();
        }

        public override VisibleSubTable.IBuilder<SubClassRule> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return SubClassRule.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubClassRule> createSubTableBuilder(SubClassRule subTable)
        {
            return SubClassRule.createBuilder(subTable);
        }

        public override SubClassSet readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new SubClassSet(data, @base, dataIsCanonical);
        }
    }
}
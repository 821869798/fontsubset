using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;




public abstract class ChainSubGenericRuleSet<TRule> : OffsetRecordTable<TRule> where TRule : ChainSubGenericRule
{
    public ChainSubGenericRuleSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override int fieldCount()
    {
        return 0;
    }



    new public interface IBuilder<out TRuleSet> : OffsetRecordTable<TRule>.IBuilder<TRuleSet> where TRuleSet : ChainSubGenericRuleSet<TRule>
    {

    }

    new protected abstract class Builder<TRuleSet> : OffsetRecordTable<TRule>.Builder<TRuleSet> , IBuilder<TRuleSet> where TRuleSet : ChainSubGenericRuleSet<TRule>
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder() : base()
        {
        }

        public Builder(TRuleSet table) : base(table)
        {
        }

        public override void initFields()
        {
        }

        public override int fieldCount()
        {
            return 0;
        }
    }
}
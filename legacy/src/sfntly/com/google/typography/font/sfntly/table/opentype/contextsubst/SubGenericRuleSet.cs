using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;




public abstract class SubGenericRuleSet<TDoubleRecordTable> : OffsetRecordTable<TDoubleRecordTable> where TDoubleRecordTable : DoubleRecordTable
{
    public SubGenericRuleSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override int fieldCount()
    {
        return 0;
    }

    new public interface IBuilder<out TSubGenericRuleSet> : OffsetRecordTable<TDoubleRecordTable>.IBuilder<TSubGenericRuleSet>
        where TSubGenericRuleSet : SubGenericRuleSet<TDoubleRecordTable>
    {

    }

    new protected abstract class Builder<TSubGenericRuleSet> : OffsetRecordTable<TDoubleRecordTable>.Builder<TSubGenericRuleSet>, IBuilder<TSubGenericRuleSet>
        where TSubGenericRuleSet : SubGenericRuleSet<TDoubleRecordTable>
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder() : base()
        {
        }

        public Builder(TSubGenericRuleSet table) : base(table)
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
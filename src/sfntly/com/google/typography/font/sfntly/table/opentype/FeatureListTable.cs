using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;





public class FeatureListTable : TagOffsetsTable<FeatureTable>
{

    public FeatureListTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    public override FeatureTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new FeatureTable(data, dataIsCanonical);
    }


    public interface IBuilder : TagOffsetsTable<FeatureTable>.IBuilder<FeatureListTable>
    {

    }

    private class Builder : TagOffsetsTable<FeatureTable>.Builder<FeatureListTable>, IBuilder
    {

        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, 0, false)
        {
        }

        public override VisibleSubTable.IBuilder<FeatureTable> createSubTableBuilder(
            ReadableFontData data, int tag, boolean dataIsCanonical)
        {
            return FeatureTable.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<FeatureTable> createSubTableBuilder()
        {
            return FeatureTable.createBuilder();
        }

        public override FeatureListTable readTable(
            ReadableFontData data, int baseUnused, boolean dataIsCanonical)
        {
            return new FeatureListTable(data, dataIsCanonical);
        }

        public override void initFields()
        {
        }

        public override int fieldCount()
        {
            return 0;
        }
    }

    public override int fieldCount()
    {
        return 0;
    }
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;



class GsubCommonTable : LayoutCommonTable<GsubLookupTable>
{

    public GsubCommonTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    public override LookupListTable createLookupList()
    {
        return base.createLookupList();
    }

    public override LookupListTable handleCreateLookupList(ReadableFontData data, boolean dataIsCanonical)
    {
        return new LookupListTable(data, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    new public interface IBuilder : LayoutCommonTable<GsubLookupTable>.IBuilder
    {
    }

    new private class Builder : LayoutCommonTable<GsubLookupTable>.Builder, IBuilder
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder() : base(null, false)
        {
        }

        public override LookupListTable handleCreateLookupList(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return new LookupListTable(data, dataIsCanonical);
        }

        public override LayoutCommonTable<GsubLookupTable> subBuildTable(ReadableFontData data)
        {
            return new GsubCommonTable(data, true);
        }

        public override LookupListTable.IBuilder createLookupListBuilder()
        {
            return LookupListTable.createBuilder();
        }

        public override int subDataSizeToSerialize()
        {
            // TODO(cibu): do real implementation
            return 0;
        }

        public override void subDataSet()
        {
            // TODO(cibu): do real implementation
        }
    }
}
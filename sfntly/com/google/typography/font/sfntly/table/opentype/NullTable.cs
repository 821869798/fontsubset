using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;





public sealed class NullTable : SubstSubtable
{
    private static readonly int RECORD_SIZE = 0;

    public NullTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    private NullTable(ReadableFontData data) : base(data, 0, false)
    {
    }

    private NullTable() : base(null, 0, false)
    {
    }

    private sealed class Builder : VisibleSubTable.Builder<NullTable>
    {
        private Builder()
        {
        }

        private Builder(ReadableFontData data, boolean dataIsCanonical)
        {
        }

        private Builder(NullTable table)
        {
        }

        public override int subDataSizeToSerialize()
        {
            return NullTable.RECORD_SIZE;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return NullTable.RECORD_SIZE;
        }

        public override NullTable subBuildTable(ReadableFontData data)
        {
            return new NullTable(data);
        }

        public override void subDataSet()
        {
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }
    }
}
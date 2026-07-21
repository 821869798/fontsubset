using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;





public class LookupListTable : OffsetRecordTable<LookupTable>
{
    private static readonly int FIELD_COUNT = 0;

    public LookupListTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    public override LookupTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new LookupTable(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public interface IBuilder : OffsetRecordTable<LookupTable>.IBuilder<LookupListTable>
    {

    }
    private class Builder : OffsetRecordTable<LookupTable>.Builder<LookupListTable>, IBuilder
    {

        public override LookupListTable readTable(
            ReadableFontData data, int baseUnused, boolean dataIsCanonical)
        {
            return new LookupListTable(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<LookupTable> createSubTableBuilder()
        {
            return LookupTable.createBuilder();
        }

        public override VisibleSubTable.IBuilder<LookupTable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return LookupTable.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<LookupTable> createSubTableBuilder(LookupTable subTable)
        {
            return LookupTable.createBuilder(subTable);
        }

        public override void initFields()
        {
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }
}
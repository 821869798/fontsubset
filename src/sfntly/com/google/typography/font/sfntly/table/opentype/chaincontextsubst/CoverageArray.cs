using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;







public class CoverageArray : OffsetRecordTable<CoverageTable>
{
    private static readonly int FIELD_COUNT = 0;

    private CoverageArray(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public CoverageArray(NumRecordList records) : base(records)
    {
    }

    public override CoverageTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new CoverageTable(data, 0, dataIsCanonical);
    }

    public static IBuilder createBuilder(NumRecordList records)
    {
        return new Builder(records);
    }

    public interface IBuilder : OffsetRecordTable<CoverageTable>.IBuilder<CoverageArray>
    {
        int limit();
        int subTableSizeToSerialize();
        int tableSizeToSerialize();
    }

    private class Builder : OffsetRecordTable<CoverageTable>.Builder<CoverageArray>, IBuilder
    {

        public Builder(NumRecordList records) : base(records)
        {
        }

        public override CoverageArray readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new CoverageArray(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<CoverageTable> createSubTableBuilder()
        {
            return CoverageTable.createBuilder();
        }

        public override VisibleSubTable.IBuilder<CoverageTable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return CoverageTable.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<CoverageTable> createSubTableBuilder(CoverageTable subTable)
        {
            return CoverageTable.createBuilder(subTable);
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
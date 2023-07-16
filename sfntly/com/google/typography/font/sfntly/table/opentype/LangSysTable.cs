using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;







public class LangSysTable : RecordsTable<NumRecord>
{
    private static readonly int FIELD_COUNT = 2;

    private static readonly int LOOKUP_ORDER_INDEX = 0;
    private static readonly int LOOKUP_ORDER_CONST = 0;

    private static readonly int REQ_FEATURE_INDEX_INDEX = 1;
    private static readonly int NO_REQ_FEATURE = 0xffff;

    public LangSysTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
        if (getField(LOOKUP_ORDER_INDEX) != LOOKUP_ORDER_CONST)
        {
            throw new IllegalArgumentException();
        }
    }

    public override RecordList<NumRecord> createRecordList(ReadableFontData data)
    {
        return new NumRecordList(data);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }
    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<LangSysTable>
    {

    }

    private class Builder : RecordsTable<NumRecord>.Builder<LangSysTable>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        // //////////////////////////////
        // private methods to update

        public override void initFields()
        {
            setField(LOOKUP_ORDER_INDEX, LOOKUP_ORDER_CONST);
            setField(REQ_FEATURE_INDEX_INDEX, NO_REQ_FEATURE);
        }

        public override LangSysTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new LangSysTable(data, dataIsCanonical);
        }

        public override RecordList<NumRecord> readRecordList(ReadableFontData data, int @base)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new NumRecordList(data);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }
    }
}

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;







public class FeatureTable : RecordsTable<NumRecord>
{
    private static readonly int FIELD_COUNT = 1;
    private static readonly int FEATURE_PARAMS_INDEX = 0;
    private static readonly int FEATURE_PARAMS_DEFAULT = 0;

    public FeatureTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
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

    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<FeatureTable> { 
    }

    private class Builder : RecordsTable<NumRecord>.Builder<FeatureTable>, IBuilder
    {

        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override FeatureTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new FeatureTable(data, dataIsCanonical);
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

        public override void initFields()
        {
            setField(FEATURE_PARAMS_INDEX, FEATURE_PARAMS_DEFAULT);
        }
    }
}


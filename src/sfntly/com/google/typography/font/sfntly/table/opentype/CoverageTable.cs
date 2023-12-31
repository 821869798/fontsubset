using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;







public class CoverageTable : SubstSubtable
{
    public readonly /*RecordsTable<?>*/ object array;

    // //////////////
    // Constructors

    public CoverageTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        switch (format)
        {
            case 1:
                array = new NumRecordTable(data, headerSize(), dataIsCanonical);
                break;
            case 2:
                array = new RangeRecordTable(data, headerSize(), dataIsCanonical);
                break;
            default:
                throw new IllegalArgumentException("coverage format " + format + " unexpected");
        }
    }

    // ////////////////////////////////////////
    // Utility methods specific to this class

    public NumRecordTable fmt1Table()
    {
        switch (format)
        {
            case 1:
                return (NumRecordTable)array;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public RangeRecordTable fmt2Table()
    {
        switch (format)
        {
            case 2:
                return (RangeRecordTable)array;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(CoverageTable table)
    {
        return new Builder(table);
    }

    public interface IBuilder : SubstSubtable.IBuilder<CoverageTable>
    {
    }
    protected class Builder : SubstSubtable.Builder<CoverageTable>, IBuilder
    {
        private readonly /*RecordsTable<?>.Builder<HeaderTable>*/ dynamic arrayBuilder;

        public Builder() : base()
        {
            arrayBuilder = NumRecordTable.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            switch (format)
            {
                case 1:
                    arrayBuilder = NumRecordTable.createBuilder(data, headerSize(), dataIsCanonical);
                    break;
                case 2:
                    arrayBuilder = RangeRecordTable.createBuilder(data, headerSize(), dataIsCanonical);
                    break;
                default:
                    throw new IllegalArgumentException("coverage format " + format + " unexpected");
            }
        }

        public Builder(CoverageTable table) : this(table.readFontData(), table.dataIsCanonical)
        {
        }

        public override int subDataSizeToSerialize()
        {
            return base.subDataSizeToSerialize() + arrayBuilder.subDataSizeToSerialize();
        }

        public override int subSerialize(WritableFontData newData)
        {
            int newOffset = base.subSerialize(newData);
            return arrayBuilder.subSerialize(newData.slice(newOffset));
        }

        public override CoverageTable subBuildTable(ReadableFontData data)
        {
            return new CoverageTable(data, 0, false);
        }

        public override boolean subReadyToSerialize()
        {
            return base.subReadyToSerialize();
        }

        public override void subDataSet()
        {
            base.subDataSet();
            arrayBuilder.subDataSet();
        }
    }
}
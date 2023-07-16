using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.classdef;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;







public class ClassDefTable : SubstSubtable
{
    public readonly /*RecordsTable<?>*/object array;
    private boolean dataIsCanonical;

    // //////////////
    // Constructors

    public ClassDefTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        this.dataIsCanonical = dataIsCanonical;

        switch (format)
        {
            case 1:
                array = new InnerArrayFmt1(data, headerSize(), dataIsCanonical);
                break;
            case 2:
                array = new RangeRecordTable(data, headerSize(), dataIsCanonical);
                break;
            default:
                throw new IllegalArgumentException("class def format " + format + " unexpected");
        }
    }

    // ////////////////////////////////////////
    // Utility methods specific to this class

    public InnerArrayFmt1 fmt1Table()
    {
        switch (format)
        {
            case 1:
                return (InnerArrayFmt1)array;
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

    public interface IBuilder : SubstSubtable.IBuilder<ClassDefTable>
    {

    }

    protected class Builder : SubstSubtable.Builder<ClassDefTable>, IBuilder
    {
        private readonly /*RecordsTable.Builder<?, ?>*/dynamic arrayBuilder;

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            switch (format)
            {
                case 1:
                    arrayBuilder = InnerArrayFmt1.createBuilder(data, headerSize(), dataIsCanonical);
                    break;
                case 2:
                    arrayBuilder = RangeRecordTable.createBuilder(data, headerSize(), dataIsCanonical);
                    break;
                default:
                    throw new IllegalArgumentException("class def format " + format + " unexpected");
            }
        }

        public Builder(ClassDefTable table) : this(table.readFontData(), table.dataIsCanonical)
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

        // ///////////////////
        // Overriden methods

        public override ClassDefTable subBuildTable(ReadableFontData data)
        {
            return new ClassDefTable(data, 0, false);
        }

        public override boolean subReadyToSerialize()
        {
            return base.subReadyToSerialize() && true;
        }

        public override void subDataSet()
        {
            base.subDataSet();
            arrayBuilder.subDataSet();
        }
    }
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.singlesubst;

namespace com.google.typography.font.sfntly.table.opentype;






public class SingleSubst : SubstSubtable
{
    private readonly HeaderFmt1 fmt1;
    private readonly InnerArrayFmt2 fmt2;

    // //////////////
    // Constructors

    public SingleSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        switch (format)
        {
            case 1:
                fmt1 = new HeaderFmt1(data, headerSize(), dataIsCanonical);
                fmt2 = null;
                break;
            case 2:
                fmt1 = null;
                fmt2 = new InnerArrayFmt2(data, headerSize(), dataIsCanonical);
                break;
            default:
                throw new IllegalStateException("Subt format value is " + format + " (should be 1 or 2).");
        }
    }

    // //////////////////////////////////
    // Methods specific to this class

    public CoverageTable coverage()
    {
        switch (format)
        {
            case 1:
                return fmt1.coverage;
            case 2:
                return fmt2.coverage;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public HeaderFmt1 fmt1Table()
    {
        if (format == 1)
        {
            return fmt1;
        }
        throw new IllegalArgumentException("unexpected format table requested: " + format);
    }

    public InnerArrayFmt2 fmt2Table()
    {
        if (format == 2)
        {
            return fmt2;
        }
        throw new IllegalArgumentException("unexpected format table requested: " + format);
    }
    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(SubstSubtable table)
    {
        return new Builder(table);
    }

    public interface IBuilder : SubstSubtable.IBuilder<SubstSubtable>
    {

    }
    protected class Builder : SubstSubtable.Builder<SubstSubtable>, IBuilder
    {

        private readonly HeaderFmt1.IBuilder fmt1Builder;
        private readonly InnerArrayFmt2.IBuilder fmt2Builder;

        public Builder() : base()
        {
            fmt1Builder = HeaderFmt1.createBuilder();
            fmt2Builder = InnerArrayFmt2.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            fmt1Builder = HeaderFmt1.createBuilder(data, dataIsCanonical);
            fmt2Builder = InnerArrayFmt2.createBuilder(data, dataIsCanonical);
        }

        public Builder(SubstSubtable subTable)
        {
            SingleSubst ligSubst = (SingleSubst)subTable;
            fmt1Builder = HeaderFmt1.createBuilder(ligSubst.fmt1);
            fmt2Builder = InnerArrayFmt2.createBuilder(ligSubst.fmt2);
        }

        public override int subDataSizeToSerialize()
        {
            return fmt1Builder.subDataSizeToSerialize() + fmt2Builder.subDataSizeToSerialize();
        }

        public override int subSerialize(WritableFontData newData)
        {
            int byteCount = fmt1Builder.subSerialize(newData);
            byteCount += fmt2Builder.subSerialize(newData.slice(byteCount));
            return byteCount;
        }

        // /////////////////////////////////
        // must implement abstract methods

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            fmt1Builder.subDataSet();
            fmt2Builder.subDataSet();
        }

        public override SubstSubtable subBuildTable(ReadableFontData data)
        {
            return new SingleSubst(data, 0, true);
        }
    }
}
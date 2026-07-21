
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;
using com.google.typography.font.sfntly.table.opentype.ligaturesubst;

namespace com.google.typography.font.sfntly.table.opentype;








public class LigatureSubst : SubstSubtable
{
    private readonly InnerArrayFmt1 array;

    // //////////////
    // Constructors

    public LigatureSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        if (format != 1)
        {
            throw new IllegalStateException("Subt format value is " + format + " (should be 1).");
        }
        array = new InnerArrayFmt1(data, headerSize(), dataIsCanonical);
    }

    // //////////////////////////////////
    // Methods redirected to the array

    public int subTableCount()
    {
        return array.recordList.count();
    }

    public LigatureSet subTableAt(int index)
    {
        return array.subTableAt(index);
    }

    public virtual IEnumerator<LigatureSet> GetEnumerator()
    {
        return array.GetEnumerator();
    }

    // //////////////////////////////////
    // Methods specific to this class

    public CoverageTable coverage()
    {
        return array.coverage;
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

    // //////////////////////////////////
    // Builder

    private class Builder : SubstSubtable.Builder<SubstSubtable>, IBuilder
    {

        private readonly InnerArrayFmt1.IBuilder arrayBuilder;

        // //////////////
        // Constructors

        public Builder() : base()
        {
            arrayBuilder = InnerArrayFmt1.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            arrayBuilder = InnerArrayFmt1.createBuilder(data, dataIsCanonical);
        }

        public Builder(SubstSubtable subTable)
        {
            LigatureSubst ligSubst = (LigatureSubst)subTable;
            arrayBuilder = InnerArrayFmt1.createBuilder(ligSubst.array);
        }

        // /////////////////////////////
        // private methods for builders



        // ///////////////////////////////
        // private methods to serialize

        public override int subDataSizeToSerialize()
        {
            return arrayBuilder.subDataSizeToSerialize();
        }

        public override int subSerialize(WritableFontData newData)
        {
            return arrayBuilder.subSerialize(newData);
        }

        // /////////////////////////////////
        // must implement abstract methods

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            arrayBuilder.subDataSet();
        }

        public override SubstSubtable subBuildTable(ReadableFontData data)
        {
            return new LigatureSubst(data, 0, true);
        }
    }
}
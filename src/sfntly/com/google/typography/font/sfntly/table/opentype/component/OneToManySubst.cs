using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.multiplesubst;

namespace com.google.typography.font.sfntly.table.opentype.component;









public class OneToManySubst : SubstSubtable
{
    private readonly GlyphIds array;

    // //////////////
    // Constructors

    public OneToManySubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        if (format != 1)
        {
            throw new IllegalStateException("Subt format value is " + format + " (should be 1).");
        }
        array = new GlyphIds(data, headerSize(), dataIsCanonical);
    }

    // //////////////////////////////////
    // Methods redirected to the array

    public NumRecordList recordList()
    {
        return array.recordList;
    }

    public NumRecordTable subTableAt(int index)
    {
        return array.subTableAt(index);
    }

    public virtual IEnumerator<NumRecordTable> GetEnumerator()
    {
        return array.GetEnumerator();
    }

    // //////////////////////////////////
    // Methods specific to this class

    public CoverageTable coverage()
    {
        return array.coverage;
    }

    public interface IBuilder : SubstSubtable.IBuilder<SubstSubtable>
    {

    }

    protected class Builder : SubstSubtable.Builder<SubstSubtable>, IBuilder
    {
        private readonly GlyphIds.IBuilder arrayBuilder;

        public Builder() : base()
        {
            arrayBuilder = GlyphIds.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            arrayBuilder = GlyphIds.createBuilder(data, dataIsCanonical);
        }

        public Builder(SubstSubtable subTable)
        {
            OneToManySubst multiSubst = (OneToManySubst)subTable;
            arrayBuilder = GlyphIds.createBuilder(multiSubst.array);
        }

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
            return new OneToManySubst(data, 0, true);
        }
    }
}
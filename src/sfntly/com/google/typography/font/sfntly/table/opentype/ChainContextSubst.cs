using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;









public class ChainContextSubst : SubstSubtable
{
    private readonly ChainSubRuleSetArray ruleSets;
    private readonly ChainSubClassSetArray classSets;
    public readonly InnerArraysFmt3 fmt3Array;

    // //////////////
    // Constructors

  public  ChainContextSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        switch (format)
        {
            case 1:
                ruleSets = new ChainSubRuleSetArray(_data, headerSize(), dataIsCanonical);
                classSets = null;
                fmt3Array = null;
                break;
            case 2:
                ruleSets = null;
                classSets = new ChainSubClassSetArray(_data, headerSize(), dataIsCanonical);
                fmt3Array = null;
                break;
            case 3:
                ruleSets = null;
                classSets = null;
                fmt3Array = new InnerArraysFmt3(_data, headerSize(), dataIsCanonical);
                break;
            default:
                throw new IllegalStateException("Subt format value is " + format + " (should be 1 or 2).");
        }
    }

    // //////////////////////////////////
    // Methods redirected to the array

    public ChainSubRuleSetArray fmt1Table()
    {
        switch (format)
        {
            case 1:
                return ruleSets;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public ChainSubClassSetArray fmt2Table()
    {
        switch (format)
        {
            case 2:
                return classSets;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public InnerArraysFmt3 fmt3Table()
    {
        switch (format)
        {
            case 3:
                return fmt3Array;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public NumRecordList recordList()
    {
        switch (format)
        {
            case 1:
                return ruleSets.recordList;
            case 2:
                return classSets.recordList;
            default:
                return null;
        }
    }

    public /*ChainSubGenericRuleSet<?>*/object subTableAt(int index)
    {
        switch (format)
        {
            case 1:
                return ruleSets.subTableAt(index);
            case 2:
                return classSets.subTableAt(index);
            default:
                return null;
        }
    }



    // //////////////////////////////////
    // Methods specific to this class

    public CoverageTable coverage()
    {
        switch (format)
        {
            case 1:
                return ruleSets.coverage;
            case 2:
                return classSets.coverage;
            default:
                return null;
        }
    }

    public ClassDefTable backtrackClassDef()
    {
        return (format == 2) ? classSets.backtrackClassDef : null;
    }

    public ClassDefTable inputClassDef()
    {
        return (format == 2) ? classSets.inputClassDef : null;
    }

    public ClassDefTable lookAheadClassDef()
    {
        return (format == 2) ? classSets.lookAheadClassDef : null;
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
        private readonly ChainSubRuleSetArray.IBuilder arrayBuilder;

        public Builder() : base()
        {
            arrayBuilder = ChainSubRuleSetArray.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            arrayBuilder = ChainSubRuleSetArray.createBuilder(data, dataIsCanonical);
        }

        public Builder(SubstSubtable subTable)
        {
            ChainContextSubst ligSubst = (ChainContextSubst)subTable;
            arrayBuilder = ChainSubRuleSetArray.createBuilder(ligSubst.ruleSets);
        }

        // ///////////////////////////////
        // Public methods to serialize

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
            return new ChainContextSubst(data, 0, true);
        }
    }
}
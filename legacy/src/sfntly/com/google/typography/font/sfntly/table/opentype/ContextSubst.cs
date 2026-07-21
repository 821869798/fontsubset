using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;
using com.google.typography.font.sfntly.table.opentype.contextsubst;

namespace com.google.typography.font.sfntly.table.opentype;









public class ContextSubst : SubstSubtable
{
    private readonly SubRuleSetArray ruleSets;
    private SubClassSetArray classSets;

    // //////////////
    // Constructors

    public ContextSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        switch (format)
        {
            case 1:
                ruleSets = new SubRuleSetArray(data, headerSize(), dataIsCanonical);
                classSets = null;
                break;
            case 2:
                ruleSets = null;
                classSets = new SubClassSetArray(data, headerSize(), dataIsCanonical);
                break;
            default:
                throw new IllegalStateException("Subt format value is " + format + " (should be 1 or 2).");
        }
    }

    // //////////////////////////////////
    // Methods redirected to the array

    public SubRuleSetArray fmt1Table()
    {
        switch (format)
        {
            case 1:
                return ruleSets;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public SubClassSetArray fmt2Table()
    {
        switch (format)
        {
            case 2:
                return classSets;
            default:
                throw new IllegalArgumentException("unexpected format table requested: " + format);
        }
    }

    public NumRecordList recordList()
    {
        return (format == 1) ? ruleSets.recordList : classSets.recordList;
    }

    public SubGenericRuleSet<DoubleRecordTable> subTableAt(int index)
    {
        return (format == 1) ? ruleSets.subTableAt(index).As<SubGenericRuleSet<DoubleRecordTable>>() : classSets.subTableAt(index).As<SubGenericRuleSet<DoubleRecordTable>>();
    }

    // //////////////////////////////////
    // Methods specific to this class

    public CoverageTable coverage()
    {
        return (format == 1) ? ruleSets.coverage : classSets.coverage;
    }

    public ClassDefTable classDef()
    {
        return (format == 2) ? classSets.classDef : null;
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder(SubstSubtable subTable)
    {
        return new Builder(subTable);
    }

    public interface IBuilder : SubstSubtable.IBuilder<SubstSubtable>
    {

    }

    private class Builder : SubstSubtable.Builder<SubstSubtable>, IBuilder
    {
        private readonly SubRuleSetArray.IBuilder arrayBuilder;

        public Builder() : base()
        {
            arrayBuilder = SubRuleSetArray.createBuilder();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
            arrayBuilder = SubRuleSetArray.createBuilder(data, dataIsCanonical);
        }

        public Builder(SubstSubtable subTable)
        {
            ContextSubst ligSubst = (ContextSubst)subTable;
            arrayBuilder = SubRuleSetArray.createBuilder(ligSubst.ruleSets);
        }

        /**
         * Even though public, not to be used by the end users. Made public only
         * make it available to packages under
         * {@code com.google.typography.font.sfntly.table.opentype}.
         */
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
            return new ContextSubst(data, 0, true);
        }
    }
}
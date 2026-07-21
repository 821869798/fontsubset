using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;








public class ChainSubGenericRule : VisibleSubTable
{
    public readonly NumRecordList backtrackGlyphs;
    public readonly NumRecordList inputClasses;
    public readonly NumRecordList lookAheadGlyphs;
    public readonly SubstLookupRecordList lookupRecords;

    // //////////////
    // Constructors

    public ChainSubGenericRule(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
    {
        backtrackGlyphs = new NumRecordList(data);
        inputClasses = new NumRecordList(data, 1, backtrackGlyphs.limit());
        lookAheadGlyphs = new NumRecordList(data, 0, inputClasses.limit());
        lookupRecords = new SubstLookupRecordList(
            data, lookAheadGlyphs.limit(), lookAheadGlyphs.limit() + 2);
    }

    new public interface IBuilder<TRule> : VisibleSubTable.IBuilder<TRule> where TRule : ChainSubGenericRule
    {

    }
    new protected  abstract class Builder<TRule> : VisibleSubTable.Builder<TRule>, IBuilder<TRule> where TRule : ChainSubGenericRule
    {
        private NumRecordList backtrackGlyphsBuilder;
        private NumRecordList inputGlyphsBuilder;
        private NumRecordList lookAheadGlyphsBuilder;
        private SubstLookupRecordList lookupRecordsBuilder;

        public Builder() : base()
        {
        }

        public Builder(ChainSubGenericRule table) : this(table.readFontData(), 0, false)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
        {
            if (!dataIsCanonical)
            {
                prepareToEdit();
            }
        }

        public override int subDataSizeToSerialize()
        {
            if (lookupRecordsBuilder != null)
            {
                serializedLength = lookupRecordsBuilder.limit();
            }
            else
            {
                computeSizeFromData(internalReadData());
            }
            return serializedLength;
        }

        public override int subSerialize(WritableFontData newData)
        {
            if (serializedLength == 0)
            {
                return 0;
            }

            if (backtrackGlyphsBuilder == null || inputGlyphsBuilder == null
                || lookAheadGlyphsBuilder == null || lookupRecordsBuilder == null)
            {
                return serializeFromData(newData);
            }

            return backtrackGlyphsBuilder.writeTo(newData) + inputGlyphsBuilder.writeTo(newData)
                + lookAheadGlyphsBuilder.writeTo(newData) + lookupRecordsBuilder.writeTo(newData);
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            backtrackGlyphsBuilder = null;
            inputGlyphsBuilder = null;
            lookupRecordsBuilder = null;
            lookAheadGlyphsBuilder = null;
        }

        // ////////////////////////////////////
        // private methods

        private void prepareToEdit()
        {
            initFromData(internalReadData());
            setModelChanged();
        }

        private void initFromData(ReadableFontData data)
        {
            if (backtrackGlyphsBuilder == null || inputGlyphsBuilder == null
                || lookAheadGlyphsBuilder == null || lookupRecordsBuilder == null)
            {
                backtrackGlyphsBuilder = new NumRecordList(data);
                inputGlyphsBuilder = new NumRecordList(data, 0, backtrackGlyphsBuilder.limit());
                lookAheadGlyphsBuilder = new NumRecordList(data, 0, inputGlyphsBuilder.limit());
                lookupRecordsBuilder = new SubstLookupRecordList(data, lookAheadGlyphsBuilder.limit());
            }
        }

        private void computeSizeFromData(ReadableFontData data)
        {
            // This assumes canonical data.
            int len = 0;
            if (data != null)
            {
                len = data.length();
            }
            serializedLength = len;
        }

        private int serializeFromData(WritableFontData newData)
        {
            // The source data must be canonical.
            ReadableFontData data = internalReadData();
            data.copyTo(newData);
            return data.length();
        }
    }
}
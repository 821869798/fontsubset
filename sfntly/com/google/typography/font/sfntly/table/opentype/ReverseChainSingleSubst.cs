using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;









public class ReverseChainSingleSubst : SubstSubtable
{
    private static readonly int FIELD_COUNT = 1;
    private static readonly int COVERAGE_INDEX = SubstSubtable.FIELD_SIZE;
    public readonly CoverageTable coverage;
    public readonly CoverageArray backtrackGlyphs;
    public readonly CoverageArray lookAheadGlyphs;
    public readonly NumRecordTable substitutes;

    // //////////////
    // Constructors

    public ReverseChainSingleSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        if (format != 1)
        {
            throw new IllegalStateException("Subt format value is " + format + " (should be 1).");
        }
        int coverageOffset = getField(COVERAGE_INDEX);
        coverage = new CoverageTable(data.slice(coverageOffset), 0, dataIsCanonical);

        NumRecordList records = new NumRecordList(data, 0, headerSize());
        backtrackGlyphs = new CoverageArray(records);

        records = new NumRecordList(data, 0, records.limit());
        lookAheadGlyphs = new CoverageArray(records);

        records = new NumRecordList(data, 0, records.limit());
        substitutes = new NumRecordTable(records);
    }

    public override int fieldCount()
    {
        return base.fieldCount() + FIELD_COUNT;
    }

    protected class Builder : VisibleSubTable.Builder<ReverseChainSingleSubst>
    {
        private CoverageTable.IBuilder coverageBuilder;
        private CoverageArray.IBuilder backtrackGlyphsBuilder;
        private CoverageArray.IBuilder lookAheadGlyphsBuilder;

        public Builder() : base()
        {
        }

        public Builder(InnerArraysFmt3 table) : this(table.readFontData(), 0, false)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
        {
            if (!dataIsCanonical)
            {
                prepareToEdit();
            }
        }

        public Builder(Builder other) : base()
        {
            coverageBuilder = other.coverageBuilder;
            backtrackGlyphsBuilder = other.backtrackGlyphsBuilder;
            lookAheadGlyphsBuilder = other.lookAheadGlyphsBuilder;
        }

        public override int subDataSizeToSerialize()
        {
            if (lookAheadGlyphsBuilder != null)
            {
                serializedLength = lookAheadGlyphsBuilder.limit();
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

            if (coverageBuilder == null
                || backtrackGlyphsBuilder == null || lookAheadGlyphsBuilder == null)
            {
                return serializeFromData(newData);
            }

            int tableOnlySize = 0;
            tableOnlySize += coverageBuilder.headerSize();
            tableOnlySize += backtrackGlyphsBuilder.tableSizeToSerialize();
            tableOnlySize += lookAheadGlyphsBuilder.tableSizeToSerialize();
            int subTableWriteOffset = tableOnlySize;

            coverageBuilder.subSerialize(newData);

            backtrackGlyphsBuilder.subSerialize(newData, subTableWriteOffset);
            subTableWriteOffset += backtrackGlyphsBuilder.subTableSizeToSerialize();
            int tableWriteOffset = backtrackGlyphsBuilder.tableSizeToSerialize();

            lookAheadGlyphsBuilder.subSerialize(newData.slice(tableWriteOffset), subTableWriteOffset);
            subTableWriteOffset += lookAheadGlyphsBuilder.subTableSizeToSerialize();

            return subTableWriteOffset;
        }

        public override ReverseChainSingleSubst subBuildTable(ReadableFontData data)
        {
            return new ReverseChainSingleSubst(data, 0, true);
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            backtrackGlyphsBuilder = null;
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
            if (backtrackGlyphsBuilder == null
                || lookAheadGlyphsBuilder == null)
            {
                NumRecordList records = new NumRecordList(data);
                backtrackGlyphsBuilder = CoverageArray.createBuilder(records);

                records = new NumRecordList(data, 0, records.limit());
                lookAheadGlyphsBuilder = CoverageArray.createBuilder(records);
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
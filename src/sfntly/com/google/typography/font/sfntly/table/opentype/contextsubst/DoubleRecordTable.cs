// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;








public class DoubleRecordTable : VisibleSubTable
{
    public readonly NumRecordList inputGlyphs;
    public readonly SubstLookupRecordList lookupRecords;

    // ///////////////
    // constructors

    public DoubleRecordTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
    {
        inputGlyphs = new NumRecordList(data, 1, @base, @base + 4);
        lookupRecords = new SubstLookupRecordList(data, @base + 2, inputGlyphs.limit());
    }

    public DoubleRecordTable(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
    {
    }

    new public interface IBuilder<T> : VisibleSubTable.IBuilder<T> where T : DoubleRecordTable
    {

    }
    new protected abstract class Builder<T> : VisibleSubTable.Builder<T> , IBuilder<T> where T : DoubleRecordTable
    {
        public NumRecordList inputGlyphIdsBuilder;
        public SubstLookupRecordList substLookupRecordsBuilder;
        public override int serializedLength { get; set; }

        public Builder() : base()
        {
        }

        public Builder(DoubleRecordTable table) : this(table.readFontData(), 0, false)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
        {
            if (!dataIsCanonical)
            {
                prepareToEdit();
            }
        }

        public Builder(Builder<T> other) : base()
        {
            inputGlyphIdsBuilder = other.inputGlyphIdsBuilder;
            substLookupRecordsBuilder = other.substLookupRecordsBuilder;
        }

        public override int subDataSizeToSerialize()
        {
            if (substLookupRecordsBuilder != null)
            {
                serializedLength = substLookupRecordsBuilder.limit();
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

            if (inputGlyphIdsBuilder == null || substLookupRecordsBuilder == null)
            {
                return serializeFromData(newData);
            }

            return inputGlyphIdsBuilder.writeTo(newData) + substLookupRecordsBuilder.writeTo(newData);
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            inputGlyphIdsBuilder = null;
            substLookupRecordsBuilder = null;
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
            if (inputGlyphIdsBuilder == null || substLookupRecordsBuilder == null)
            {
                inputGlyphIdsBuilder = new NumRecordList(data, 1, 0, 4);
                substLookupRecordsBuilder = new SubstLookupRecordList(
                    data, 2, inputGlyphIdsBuilder.limit());
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
// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;






public abstract class RecordsTable<TRecord> : HeaderTable where TRecord : Record
{
    public readonly RecordList<TRecord> recordList;

    // ///////////////
    // constructors

    public RecordsTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        recordList = createRecordList(data.slice(@base + headerSize()));
    }

    public RecordsTable(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
    {
    }

    public RecordsTable(RecordList<TRecord> records) : base(records.readData, records.@base, false)
    {
        recordList = records;
    }

    public virtual IEnumerator<TRecord> GetEnumerator()
    {
        return recordList.GetEnumerator();
    }

    // ////////////////////////////////////
    // implementations pushed to subclasses

    public abstract RecordList<TRecord> createRecordList(ReadableFontData data);


    new public interface IBuilder<out TRecordsTable> : HeaderTable.IBuilder<TRecordsTable> where TRecordsTable : RecordsTable<TRecord>
    {

    }


    new protected abstract class Builder<TRecordsTable> : HeaderTable.Builder<TRecordsTable>, IBuilder<TRecordsTable> where TRecordsTable : RecordsTable<TRecord>
    {

        public RecordList<TRecord> _records;
        new private int serializedLength;
        private readonly int @base;

        public Builder() : base()
        {
            @base = 0;
        }

        public Builder(RecordsTable<TRecord> table) : this(table.readFontData(), table.@base, table.dataIsCanonical)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
        {
            this.@base = @base;
            if (!dataIsCanonical)
            {
                prepareToEdit();
            }
        }

        public Builder(RecordsTable<TRecord>.Builder<TRecordsTable> other) : base()
        {
            @base = other.@base;
            _records = other._records;
        }

        // ////////////////
        // private methods

        public RecordList<TRecord> records()
        {
            return _records;
        }

        public int count()
        {
            initFromData(internalReadData(), @base);
            return _records.count();
        }

        // ////////////////////////////////////
        // overriden methods

        public override int subDataSizeToSerialize()
        {
            if (records != null)
            {
                serializedLength = _records.limit();
            }
            else
            {
                computeSizeFromData(internalReadData().slice(@base + headerSize()));
            }
            return serializedLength;
        }

        public override int subSerialize(WritableFontData newData)
        {
            if (serializedLength == 0)
            {
                return 0;
            }

            if (records == null)
            {
                return serializeFromData(newData);
            }

            return _records.writeTo(newData);
        }

        public override TRecordsTable subBuildTable(ReadableFontData data)
        {
            return readTable(data, 0, true);
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override void subDataSet()
        {
            _records = null;
        }

        // ////////////////////////////////////
        // implementations pushed to subclasses

        public abstract TRecordsTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical);

        public abstract RecordList<TRecord> readRecordList(ReadableFontData data, int @base);

        // ////////////////////////////////////
        // private methods

        private void prepareToEdit()
        {
            initFromData(internalReadData(), @base + headerSize());
            setModelChanged();
        }

        private void initFromData(ReadableFontData data, int @base)
        {
            if (_records == null)
            {
                _records = readRecordList(data, @base);
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
            ReadableFontData data = internalReadData().slice(@base + headerSize());
            data.copyTo(newData);
            return data.length();
        }
    }
}
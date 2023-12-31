// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;










public abstract class OffsetRecordTable<TSubTable> : HeaderTable where TSubTable : SubTable
{
    public readonly NumRecordList recordList;

    // ///////////////
    // constructors

    public OffsetRecordTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        recordList = new NumRecordList(data.slice(@base + headerSize()));
    }

    public OffsetRecordTable(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
    {
    }

    public OffsetRecordTable(NumRecordList records) : base(records.readData, records.@base, false)
    {
        recordList = records;
    }

    // ////////////////
    // public methods

    public int subTableCount()
    {
        return recordList.count();
    }

    public TSubTable subTableAt(int index)
    {
        NumRecord record = recordList.get(index);
        return subTableForRecord(record);
    }

    public virtual IEnumerator<TSubTable> GetEnumerator()
    {
        return recordList.GetEnumerator().BoxEnumerator().Select(subTableForRecord).GetEnumerator();
        //return new IEnumerator<S>() {
        //  IEnumerator<NumRecord> recordIterator = recordList.GetEnumerator();

        //  public override boolean hasNext() {
        //    return recordIterator.MoveNext();
        //  }

        //  public override S next() {
        //    if (!hasNext()) {
        //      throw new NoSuchElementException();
        //    }
        //    NumRecord record = recordIterator.Current;
        //    return subTableForRecord(record);
        //  }

        //  public override void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //};
    }

    // ////////////////////////////////////
    // implementations pushed to subclasses

    public abstract TSubTable readSubTable(ReadableFontData data, boolean dataIsCanonical);

    // ////////////////////////////////////
    // private methods

    private TSubTable subTableForRecord(NumRecord record)
    {
        if (record.value == 0)
        {
            // No reference to itself is allowed.
            return null;
        }
        ReadableFontData newBase = _data.slice(record.value);
        return readSubTable(newBase, dataIsCanonical);
    }

    new public interface IBuilder<out T> : HeaderTable.IBuilder<T> where T : OffsetRecordTable<TSubTable>
    {
        VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder();

        VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical);

        VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder(TSubTable subTable);

        int subSerialize(WritableFontData newData, int subTableWriteOffset);
    }

    new protected abstract class Builder<T> : HeaderTable.Builder<T>, IBuilder<T> where T : OffsetRecordTable<TSubTable>
    {

        private IList<VisibleSubTable.IBuilder<TSubTable>> builders;
        private boolean dataIsCanonical;
        private int serializedLength;
        private int serializedCount;
        private readonly int @base;
        private int serializedSubtablePartLength;
        private int serializedTablePartLength;

        public Builder() : base()
        {
            @base = 0;
        }

        public Builder(T table) : this(table.readFontData(), table.@base, table.dataIsCanonical)
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
        {
            this.@base = @base;
            this.dataIsCanonical = dataIsCanonical;
            if (!dataIsCanonical)
            {
                prepareToEdit();
            }
        }

        public Builder(NumRecordList records) : base()
        {
            @base = records.@base;
            if (builders == null)
            {
                initFromData(records);
                setModelChanged();
            }
        }

        // ////////////////
        // public methods

        public int subTableCount()
        {
            if (builders == null)
            {
                return new NumRecordList(internalReadData().slice(@base + headerSize())).count();
            }
            return builders.Count;
        }

        public SubTable.IBuilder<TSubTable> builderForTag(int tag)
        {
            prepareToEdit();
            return builders.get(tag);
        }

        public VisibleSubTable.IBuilder<TSubTable> addBuilder()
        {
            prepareToEdit();
            var builder = createSubTableBuilder();
            builders.Add(builder);
            return builder;
        }

        public VisibleSubTable.IBuilder<TSubTable> addBuilder(TSubTable subTable)
        {
            prepareToEdit();
            var builder = createSubTableBuilder(subTable);
            builders.Add(builder);
            return builder;
        }

        public void removeBuilderForTag(int tag)
        {
            prepareToEdit();
            builders.RemoveAt(tag);
        }

        public int limit()
        {
            return @base + serializedLength;
        }

        // ////////////////////////////////////
        // overriden methods

        public override int subDataSizeToSerialize()
        {
            if (builders != null)
            {
                computeSizeFromBuilders();
            }
            else
            {
                computeSizeFromData(internalReadData().slice(@base + headerSize()));
            }
            return serializedLength;
        }

        public int tableSizeToSerialize()
        {
            computeSizeFromBuilders();
            return serializedTablePartLength;
        }

        public int subTableSizeToSerialize()
        {
            computeSizeFromBuilders();
            return serializedSubtablePartLength;
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public int subSerialize(WritableFontData newData, int subTableWriteOffset)
        {
            if (serializedLength == 0)
            {
                return 0;
            }

            if (builders != null)
            {
                return serializeFromBuilders(newData, subTableWriteOffset);
            }
            return serializeFromData(newData);
        }

        public override int subSerialize(WritableFontData newData)
        {
            return subSerialize(newData, 0);
        }

        public override void subDataSet()
        {
            builders = null;
        }

        public override T subBuildTable(ReadableFontData data)
        {
            return readTable(data, 0, true);
        }

        // ////////////////////////////////////
        // implementations pushed to subclasses

        public abstract T readTable(ReadableFontData data, int @base, boolean dataIsCanonical);

        public abstract VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder();

        public abstract VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical);

        public abstract VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder(TSubTable subTable);

        // ////////////////////////////////////
        // private methods

        private void prepareToEdit()
        {
            if (builders == null)
            {
                initFromData(internalReadData(), @base);
                setModelChanged();
            }
        }

        private void initFromData(ReadableFontData data, int @base)
        {
            NumRecordList recordList = new NumRecordList(data, 0, @base + headerSize());
            initFromData(recordList);
        }

        private void initFromData(NumRecordList recordList)
        {
            ReadableFontData data = recordList.readData;
            builders = new List<VisibleSubTable.IBuilder<TSubTable>>();
            if (data == null)
            {
                return;
            }

            if (recordList.count() == 0)
            {
                return;
            }

            int subTableLimit = recordList.limit();
            var recordIterator = recordList.GetEnumerator();

            recordIterator.MoveNext();
            do
            {
                NumRecord record = recordIterator.Current;
                int offset = record.value;
                var builder = createSubTableBuilder(data, offset);
                builders.Add(builder);
            } while (recordIterator.MoveNext());
        }

        private void computeSizeFromBuilders()
        {
            // This does not merge LangSysTables that reference the same
            // features.

            // If there is no data in the default LangSysTable or any
            // of the other LangSysTables, the size is zero, and this table
            // will not be written.

            int len = 0;
            int count = 0;
            foreach (VisibleSubTable.Builder<TSubTable> builder in builders)
            {
                int sublen = builder.subDataSizeToSerialize();
                if (sublen > 0)
                {
                    ++count;
                    len += sublen;
                }
            }
            serializedSubtablePartLength = len;
            if (len > 0)
            {
                serializedTablePartLength = NumRecordList.sizeOfListOfCount(count);
            }
            serializedLength = serializedTablePartLength + serializedSubtablePartLength;
            serializedCount = count;
        }

        private void computeSizeFromData(ReadableFontData data)
        {
            // This assumes canonical data.
            int len = 0;
            int count = 0;
            if (data != null)
            {
                len = data.length();
                count = new NumRecordList(data).count();
            }
            serializedLength = len;
            serializedCount = count;
        }

        private int serializeFromBuilders(WritableFontData newData, int subTableWriteOffset)
        {
            // The canonical form of the data consists of the header,
            // the index, then the
            // scriptTables from the index in index order. All
            // scriptTables are distinct; there's no sharing of tables.

            // Find size for table
            int tableSize = NumRecordList.sizeOfListOfCount(serializedCount);

            // Fill header in table and serialize its builder.
            int subTableFillPos = tableSize;
            if (subTableWriteOffset > 0)
            {
                subTableFillPos = subTableWriteOffset;
            }

            NumRecordList recordList = new NumRecordList(newData);
            foreach (var builder in builders)
            {
                if (builder.serializedLength > 0)
                {
                    NumRecord record = new NumRecord(subTableFillPos);
                    recordList.add(record);
                    subTableFillPos += builder.subSerialize(newData.slice(subTableFillPos));
                }
            }
            recordList.writeTo(newData);
            return subTableFillPos;
        }

        private int serializeFromData(WritableFontData newData)
        {
            // The source data must be canonical.
            ReadableFontData data = internalReadData().slice(@base);
            data.copyTo(newData);
            return data.length();
        }

        private VisibleSubTable.IBuilder<TSubTable> createSubTableBuilder(ReadableFontData data, int offset)
        {
            ReadableFontData newData = data.slice(offset);
            return createSubTableBuilder(newData, dataIsCanonical);
        }
    }
}
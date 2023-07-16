// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;










public abstract class TagOffsetsTable<S> : HeaderTable where S : SubTable
{
  private readonly TagOffsetRecordList recordList;

  // ///////////////
  // constructors

  public TagOffsetsTable(ReadableFontData data, int @base, boolean dataIsCanonical) :base(data, @base, dataIsCanonical) {
    recordList = new TagOffsetRecordList(data.slice(headerSize() + @base));
  }

  public TagOffsetsTable(ReadableFontData data, boolean dataIsCanonical) :this(data, 0, dataIsCanonical) {
  }

  // ////////////////
  // private methods

  public int count() {
    return recordList.count();
  }

  public int tagAt(int index) {
    return recordList.get(index).tag;
  }

  public S subTableAt(int index) {
    TagOffsetRecord record = recordList.get(index);
    return subTableForRecord(record);
  }

  public virtual IEnumerator<S> GetEnumerator() {
        return recordList.GetEnumerator().BoxEnumerator().Select(subTableForRecord).GetEnumerator();
    //return new IEnumerator<S>() {
    //  private IEnumerator<TagOffsetRecord> recordIterator = recordList.GetEnumerator();

        //  public override boolean hasNext() {
        //    return recordIterator.MoveNext();
        //  }

        //  public override S next() {
        //    if (!hasNext()) {
        //      throw new NoSuchElementException();
        //    }
        //    TagOffsetRecord record = recordIterator.Current;
        //    return subTableForRecord(record);
        //  }

        //  public override void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //};
  }

  // ////////////////////////////////////
  // implementations pushed to subclasses

  public abstract S readSubTable(ReadableFontData data, boolean dataIsCanonical);

  // ////////////////////////////////////
  // private methods

  private S subTableForRecord(TagOffsetRecord record) {
    ReadableFontData newBase = _data.slice(record.offset);
    return readSubTable(newBase, dataIsCanonical);
  }

    new public interface IBuilder<T> : HeaderTable.IBuilder<T> where T : HeaderTable
    {

    }  

    new protected abstract class Builder<T> : HeaderTable.Builder<T>, IBuilder<T> where T: HeaderTable
    {

    private Dictionary<Integer, VisibleSubTable.IBuilder<S>> builders;
    private int serializedLength;
    private int serializedCount;
    private readonly int @base;

    public Builder() :base() {
            @base = 0;
    }

    public Builder(TagOffsetsTable<S>.Builder<T> other) :base() {
      builders = other.builders;
      dataIsCanonical = other.dataIsCanonical;
            @base = other.@base;
    }

    public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) :base(data) {
      this.@base = @base;
      this.dataIsCanonical = dataIsCanonical;
      if (!dataIsCanonical) {
        prepareToEdit();
      }
    }

    public override int subDataSizeToSerialize() {
      if (builders != null) {
        computeSizeFromBuilders();
      } else {
        computeSizeFromData(internalReadData().slice(headerSize() + this.@base));
      }
      serializedLength += base.subDataSizeToSerialize();
      return serializedLength;
    }

    public override int subSerialize(WritableFontData newData) {
      if (serializedLength == 0) {
        return 0;
      }

      int writtenBytes = base.subSerialize(newData);
      if (builders != null) {
        return serializeFromBuilders(newData.slice(writtenBytes));
      }
      return serializeFromData(newData.slice(writtenBytes));
    }

    public override boolean subReadyToSerialize() {
      return true;
    }

    public override void subDataSet() {
      builders = null;
    }

    public override T subBuildTable(ReadableFontData data) {
      return readTable(data, 0, true);
    }

    // ////////////////////////////////////
    // implementations pushed to subclasses

    public abstract T readTable(ReadableFontData data, int @base, boolean dataIsCanonical);

    public abstract VisibleSubTable.IBuilder<S> createSubTableBuilder();

    public abstract VisibleSubTable.IBuilder<S> createSubTableBuilder(
        ReadableFontData data, int tag, boolean dataIsCanonical);

    // ////////////////////////////////////
    // private methods

    private void prepareToEdit() {
      if (builders == null) {
        initFromData(internalReadData(), headerSize() + @base);
        setModelChanged();
      }
    }

    private void initFromData(ReadableFontData data, int @base) {
      builders = new Dictionary<Integer, VisibleSubTable.IBuilder<S>>();
      if (data == null) {
        return;
      }

      data = data.slice(@base);
      // Start of the first subtable in the data, if we're canonical.
      TagOffsetRecordList recordList = new TagOffsetRecordList(data);
      if (recordList.count() == 0) {
        return;
      }

      int subTableLimit = recordList.limit();
      var recordIterator = recordList.GetEnumerator();
      if (dataIsCanonical) {
        do {
          // Each table starts where the previous one ended.
          int offset = subTableLimit;
          TagOffsetRecord record = recordIterator.Current;
          int tag = record.tag;
          // Each table ends at the next start, or at the end of the data.
          subTableLimit = record.offset;
          // TODO(cibu): length computation does not seems to be correct.
          int length = subTableLimit - offset;
          var builder = createSubTableBuilder(data, offset, length, tag);
          builders.put(tag, builder);
        } while (recordIterator.MoveNext());
      } else {
        do {
          TagOffsetRecord record = recordIterator.Current;
          int offset = record.offset;
          int tag = record.tag;
          var builder = createSubTableBuilder(data, offset, -1, tag);
          builders.put(tag, builder);
        } while (recordIterator.MoveNext());
      }
    }

    private void computeSizeFromBuilders() {
      // This does not merge LangSysTables that reference the same
      // features.

      // If there is no data in the default LangSysTable or any
      // of the other LangSysTables, the size is zero, and this table
      // will not be written.

      int len = 0;
      int count = 0;
      foreach(VisibleSubTable.Builder<S> builder in builders.values()) {
        int sublen = builder.subDataSizeToSerialize();
        if (sublen > 0) {
          ++count;
          len += sublen;
        }
      }
      if (len > 0) {
        len += TagOffsetRecordList.sizeOfListOfCount(count);
      }
      serializedLength = len;
      serializedCount = count;
    }

    private void computeSizeFromData(ReadableFontData data) {
      // This assumes canonical data.
      int len = 0;
      int count = 0;
      if (data != null) {
        len = data.length();
        count = new TagOffsetRecordList(data).count();
      }
      serializedLength = len;
      serializedCount = count;
    }

    private int serializeFromBuilders(WritableFontData newData) {
      // The canonical form of the data consists of the header,
      // the index, then the
      // scriptTables from the index in index order. All
      // scriptTables are distinct; there's no sharing of tables.

      // Find size for table
      int tableSize = TagOffsetRecordList.sizeOfListOfCount(serializedCount);

      // Fill header in table and serialize its builder.
      int subTableFillPos = tableSize;

      TagOffsetRecordList recordList = new TagOffsetRecordList(newData);
      foreach(var entry in builders.entrySet()) {
        int tag = entry.getKey();
        var builder = entry.getValue();
        if (builder.serializedLength > 0) {
          TagOffsetRecord record = new TagOffsetRecord(tag, subTableFillPos);
          recordList.add(record);
          subTableFillPos += builder.subSerialize(newData.slice(subTableFillPos));
        }
      }
      recordList.writeTo(newData);
      return subTableFillPos;
    }

    private int serializeFromData(WritableFontData newData) {
      // The source data must be canonical.
      ReadableFontData data = internalReadData().slice(@base);
      data.copyTo(newData);
      return data.length();
    }

    private VisibleSubTable.IBuilder<S> createSubTableBuilder(
        ReadableFontData data, int offset, int length, int tag) {
      boolean dataIsCanonical = (length >= 0);
      ReadableFontData newData = dataIsCanonical ? data.slice(offset, length) : data.slice(offset);
      return createSubTableBuilder(newData, tag, dataIsCanonical);
    }
  }
}

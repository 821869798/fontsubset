using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;









public abstract class RecordList<T> where T : Record
{
    private static readonly int COUNT_OFFSET = 0;
    public static readonly int DATA_OFFSET = 2;
    public readonly int @base;
    private readonly int recordBase;

    public readonly ReadableFontData readData;
    private readonly WritableFontData writeData;
    private int _count;
    private IList<T> recordsToWrite;

    /*
     *private RecordList(WritableFontData data) { this.readData = null;
     * this.writeData = data; this.count = 0; this.base = 0; this.recordBase =
     * RECORD_BASE_DEFAULT; if (writeData != null) {
     * writeData.writeUShort(COUNT_OFFSET, 0); } }
     */
    public RecordList(ReadableFontData data, int countDecrement, int countOffset,
        int valuesOffset)
    {
        this.readData = data;
        this.writeData = null;
        this.@base = countOffset;
        this.recordBase = valuesOffset; // base + RECORD_BASE_DEFAULT +
                                        // recordBaseOffset;
        if (readData != null)
        {
            this._count = data.readUShort(countOffset + COUNT_OFFSET) - countDecrement;
        }
    }

    public RecordList(RecordList<T> other)
    {
        this.readData = other.readData;
        this.writeData = other.writeData;
        this.@base = other.@base;
        this.recordBase = other.recordBase;
        this._count = other._count;
        this.recordsToWrite = other.recordsToWrite;
    }

    public RecordList(ReadableFontData data) : this(data, 0)
    {
    }

    public RecordList(ReadableFontData data, int countDecrement) : this(data, countDecrement, 0, DATA_OFFSET)
    {
    }

    public RecordList(ReadableFontData data, int countDecrement, int countOffset) : this(data, countDecrement, countOffset, countOffset + DATA_OFFSET)
    {
    }

    public int count()
    {
        if (recordsToWrite != null)
        {
            return recordsToWrite.Count;
        }
        return _count;
    }

    public int limit()
    {
        return sizeOfList(count());
    }

    private int sizeOfList(int count)
    {
        return baseAt(recordBase, count);
    }

    private int baseAt(int @base, int index)
    {
        return @base + index * recordSize();
    }

    public T get(int index)
    {
        if (recordsToWrite != null)
        {
            return recordsToWrite.get(index);
        }
        return getRecordAt(readData, sizeOfList(index));
    }

    public boolean contains(T record)
    {
        if (recordsToWrite != null)
        {
            return recordsToWrite.Contains(record);
        }

        var iterator = this.GetEnumerator();
        while (iterator.MoveNext())
        {
            if (record.Equals(iterator.Current))
            {
                return true;
            }
        }
        return false;
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        if (recordsToWrite != null)
        {
            return recordsToWrite.GetEnumerator();
        }

        return Enumerable.Range(0, count()).Select(index => getRecordAt(readData, index)).GetEnumerator();

        //return new IEnumerator<T>() {
        //  private int current = 0;

        //  public override boolean hasNext() {
        //    return current < count;
        //  }

        //  public override T next() {
        //    if (!hasNext()) {
        //      throw new NoSuchElementException();
        //    }
        //    return getRecordAt(readData, sizeOfList(current++));
        //  }

        //  public override void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //};
    }

    public RecordList<T> add(T record)
    {
        copyFromRead();
        recordsToWrite.Add(record);
        return this;
    }

    public int writeTo(WritableFontData writeData)
    {
        copyFromRead();

        writeData.writeUShort(@base + COUNT_OFFSET, this._count);
        int nextWritePos = recordBase;
        foreach (T record in recordsToWrite)
        {
            nextWritePos += record.writeTo(writeData, nextWritePos);
        }
        return nextWritePos - recordBase + DATA_OFFSET; // bytes wrote
    }

    private void copyFromRead()
    {
        if (recordsToWrite == null)
        {
            recordsToWrite = new List<T>(this._count);
            var iterator = this.GetEnumerator();
            while (iterator.MoveNext())
            {
                recordsToWrite.Add(iterator.Current);
            }
        }
    }

    public abstract T getRecordAt(ReadableFontData data, int pos);

    public abstract int recordSize();
}
using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;









public abstract class HeaderTable : VisibleSubTable
{
    public static readonly int FIELD_SIZE = 2;
    public boolean dataIsCanonical = false;
    public int @base = 0;

    public HeaderTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data)
    {
        this.@base = @base;
        this.dataIsCanonical = dataIsCanonical;
    }

    public int getField(int index)
    {
        return _data.readUShort(@base + index * FIELD_SIZE);
    }

    public int headerSize()
    {
        return FIELD_SIZE * fieldCount();
    }

    public abstract int fieldCount();

    new public interface IBuilder<out THeaderTable> : VisibleSubTable.IBuilder<THeaderTable> where THeaderTable : HeaderTable
    {
        int headerSize();

    }
    new protected abstract class Builder<THeaderTable> : VisibleSubTable.Builder<THeaderTable> where THeaderTable : HeaderTable
    {
        private IDictionary<Integer, Integer> map = new Dictionary<Integer, Integer>();
        public boolean dataIsCanonical = false;

        public Builder() : base()
        {
            initFields();
        }

        public Builder(ReadableFontData data) : base(data)
        {
            initFields();
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data)
        {
            this.dataIsCanonical = dataIsCanonical;
            initFields();
        }

        public Builder(THeaderTable table) : base()
        {
            initFields();
            for (int i = 0; i < table.fieldCount(); i++)
            {
                map.put(i, table.getField(i));
            }
        }

        public void setField(int index, int value)
        {
            map.put(index, value);
        }

        public int getField(int index)
        {
            return map.get(index);
        }

        public abstract void initFields();

        public abstract int fieldCount();

        public int headerSize()
        {
            return FIELD_SIZE * fieldCount();
        }

        /**
         * Even though public, not to be used by the end users. Made public only
         * make it available to packages under
         * {@code com.google.typography.font.sfntly.table.opentype}.
         */
        public override int subDataSizeToSerialize()
        {
            return headerSize();
        }

        public override int subSerialize(WritableFontData newData)
        {
            foreach (var entry in map.entrySet())
            {
                newData.writeUShort(entry.getKey() * FIELD_SIZE, entry.getValue());
            }
            return headerSize();
        }

        public override void subDataSet()
        {
            map = new Dictionary<Integer, Integer>();
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }
    }
}
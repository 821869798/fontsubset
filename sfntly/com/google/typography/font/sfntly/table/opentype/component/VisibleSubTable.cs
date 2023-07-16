using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;





public abstract class VisibleSubTable : SubTable
{
    protected VisibleSubTable(ReadableFontData data) : base(data)
    {
    }

    new public interface IBuilder<out TSubTable> : SubTable.IBuilder<TSubTable> where TSubTable : SubTable
    {
        int serializedLength { get; }

        void subDataSet();
    }

    new protected abstract class Builder<TSubTable> : SubTable.Builder<TSubTable>, IBuilder<TSubTable> where TSubTable : SubTable
    {
        public virtual int serializedLength { get; set; }

        public Builder() : base(null)
        {
        }

        public Builder(ReadableFontData data) : base(data)
        {
        }

        public override abstract int subSerialize(WritableFontData newData);

        /**
         * Even though public, not to be used by the end users. Made public only
         * make it available to packages under
         * {@code com.google.typography.font.sfntly.table.opentype}.
         */
        public override abstract int subDataSizeToSerialize();

        public override abstract void subDataSet();

        public override abstract TSubTable subBuildTable(ReadableFontData data);
    }
}
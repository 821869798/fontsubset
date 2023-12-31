using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;




public abstract class SubstSubtable : HeaderTable
{
    private static readonly int FIELD_COUNT = 1;
    private static readonly int FORMAT_INDEX = 0;
    private static readonly int FORMAT_DEFAULT = 0;
    public readonly int format;

    public SubstSubtable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        format = getField(FORMAT_INDEX);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    new public interface IBuilder<out TSubstSubtable> : HeaderTable.IBuilder<TSubstSubtable> where TSubstSubtable : SubstSubtable
    {

    }

    new protected abstract class Builder<TSubstSubtable> : HeaderTable.Builder<TSubstSubtable>, IBuilder<TSubstSubtable> where TSubstSubtable : SubstSubtable
    {
        public int format;

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data)
        {
            format = getField(FORMAT_INDEX);
        }

        public Builder() : base()
        {
        }

        public override void initFields()
        {
            setField(FORMAT_INDEX, FORMAT_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }
    }
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.ligaturesubst;

namespace com.google.typography.font.sfntly.table.opentype.contextsubst;



public class SubRule : DoubleRecordTable
{
    public SubRule(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(SubRule table)
    {
        return new Builder(table);
    }

    public interface IBuilder : DoubleRecordTable.IBuilder<SubRule>
    {

    }

    private class Builder : DoubleRecordTable.Builder<SubRule>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(SubRule table) : base(table)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override SubRule subBuildTable(ReadableFontData data)
        {
            return new SubRule(data, 0, true);
        }
    }
}
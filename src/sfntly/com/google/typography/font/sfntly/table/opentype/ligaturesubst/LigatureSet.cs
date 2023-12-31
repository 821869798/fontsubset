using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.ligaturesubst;





public class LigatureSet : OffsetRecordTable<Ligature>
{
    public LigatureSet(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(LigatureSet table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<Ligature>.IBuilder<LigatureSet>
    {

    }
    private class Builder : OffsetRecordTable<Ligature>.Builder<LigatureSet>,IBuilder
    {
        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder() : base()
        {
        }

        public Builder(LigatureSet table) : base(table)
        {
        }

        public override LigatureSet readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new LigatureSet(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<Ligature> createSubTableBuilder()
        {
            return Ligature.createBuilder();
        }

        public override VisibleSubTable.IBuilder<Ligature> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return Ligature.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<Ligature> createSubTableBuilder(Ligature subTable)
        {
            return Ligature.createBuilder(subTable);
        }

        public override void initFields()
        {
        }

        public override int fieldCount()
        {
            return 0;
        }
    }

    public override Ligature readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Ligature(data, @base, dataIsCanonical);
    }

    public override int fieldCount()
    {
        return 0;
    }
}
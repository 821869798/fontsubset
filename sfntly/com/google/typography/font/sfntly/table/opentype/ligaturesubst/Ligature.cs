using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.ligaturesubst;







public class Ligature : RecordsTable<NumRecord>
{
    private static readonly int FIELD_COUNT = 1;

    public static readonly int LIG_GLYPH_INDEX = 0;
    private static readonly int LIG_GLYPH_DEFAULT = 0;

    public Ligature(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }


    public static IBuilder createBuilder(Ligature table)
    {
        return new Builder(table);
    }

    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<Ligature>
    {

    }

    private class Builder : RecordsTable<NumRecord>.Builder<Ligature>, IBuilder
    {
        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder() : base()
        {
        }

        public Builder(Ligature table) : base(table)
        {
        }

        public override Ligature readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new Ligature(data, @base, dataIsCanonical);
        }

        public override void initFields()
        {
            setField(LIG_GLYPH_INDEX, LIG_GLYPH_DEFAULT);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override RecordList<NumRecord> readRecordList(ReadableFontData data, int @base)
        {
            return new NumRecordList(data);
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    public override RecordList<NumRecord> createRecordList(ReadableFontData data)
    {
        return new NumRecordList(data, 1);
    }
}
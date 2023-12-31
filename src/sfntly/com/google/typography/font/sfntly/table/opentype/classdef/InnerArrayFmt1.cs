using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.classdef;







public class InnerArrayFmt1 : RecordsTable<NumRecord>
{
    private static readonly int FIELD_COUNT = 1;

    public static readonly int START_GLYPH_INDEX = 0;
    private static readonly int START_GLYPH_CONST = 0;

    public InnerArrayFmt1(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public override RecordList<NumRecord> createRecordList(ReadableFontData data)
    {
        return new NumRecordList(data);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }
    public interface IBuilder : RecordsTable<NumRecord>.IBuilder<InnerArrayFmt1>
    {

    }

    private class Builder : RecordsTable<NumRecord>.Builder<InnerArrayFmt1>, IBuilder
    {
        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public override void initFields()
        {
            setField(START_GLYPH_INDEX, START_GLYPH_CONST);
        }

        public override InnerArrayFmt1 readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new InnerArrayFmt1(data, @base, dataIsCanonical);
        }

        public override RecordList<NumRecord> readRecordList(ReadableFontData data, int @base)
        {
            if (@base != 0)
            {
                throw new UnsupportedOperationException();
            }
            return new NumRecordList(data);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }
    }
}
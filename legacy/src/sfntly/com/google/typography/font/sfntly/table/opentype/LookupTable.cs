using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;



public static class LookupFlagBitExt
{
    public static int getValue(this LookupTable.LookupFlagBit @this, int value)
    {
        return ((int)@this) & value;
    }
}


public class LookupTable : OffsetRecordTable<SubstSubtable>
{
    private static readonly int FIELD_COUNT = 2;

    static readonly int LOOKUP_TYPE_INDEX = 0;
    private static readonly int LOOKUP_TYPE_DEFAULT = 0;

    private static readonly int LOOKUP_FLAG_INDEX = 1;

    public enum LookupFlagBit
    {
        RIGHT_TO_LEFT = (0x0001),
        IGNORE_BASE_GLYPHS = (0x0002),
        IGNORE_LIGATURES = (0x0004),
        IGNORE_MARKS = (0x0008),
        USE_MARK_FILTERING_SET = (0x0010),
        RESERVED = (0x00E0),
        MARK_ATTACHMENT_TYPE = (0xFF00),
        /*
        private int bit;

        private LookupFlagBit(int bit) {
          this.bit = bit;
        }

        private int getValue(int value) {
          return bit & value;
        }*/
    }

    public LookupTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        int lookupFlag = getField(LOOKUP_FLAG_INDEX);
        if (LookupFlagBit.USE_MARK_FILTERING_SET.getValue(lookupFlag) != 0)
        {
            throw new IllegalArgumentException(
                "Lookup Flag has Use Mark Filtering Set which is unimplemented.");
        }
        if (LookupFlagBit.RESERVED.getValue(lookupFlag) != 0)
        {
            throw new IllegalArgumentException("Reserved bits of Lookup Flag are not 0");
        }
    }

    public GsubLookupType lookupType()
    {
        return GsubLookupType.forTypeNum(getField(LOOKUP_TYPE_INDEX));
    }

    public GsubLookupType lookupFlag()
    {
        return GsubLookupType.forTypeNum(getField(LOOKUP_FLAG_INDEX));
    }

    public override SubstSubtable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        int lookupType = getField(LOOKUP_TYPE_INDEX);
        GsubLookupType gsubLookupType = GsubLookupType.forTypeNum(lookupType);
        switch (gsubLookupType.name())
        {
            case nameof(GsubLookupType.GSUB_LIGATURE):
                return new LigatureSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_SINGLE):
                return new SingleSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_MULTIPLE):
                return new MultipleSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_ALTERNATE):
                return new AlternateSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_CONTEXTUAL):
                return new ContextSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_CHAINING_CONTEXTUAL):
                return new ChainContextSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_EXTENSION):
                return new ExtensionSubst(data, @base, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_REVERSE_CHAINING_CONTEXTUAL_SINGLE):
                return new ReverseChainSingleSubst(data, @base, dataIsCanonical);
            default:
                Console.Error.WriteLine("Unimplemented LookupType: " + gsubLookupType);
                return new NullTable(data, @base, dataIsCanonical);
                // throw new IllegalArgumentException("LookupType is " + lookupType);
        }
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }

    Builder builder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }


    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }


    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }


    public static IBuilder createBuilder(LookupTable table)
    {
        return new Builder(table);
    }

    public interface IBuilder : OffsetRecordTable<SubstSubtable>.IBuilder<LookupTable>
    {

    }

    protected class Builder : OffsetRecordTable<SubstSubtable>.Builder<LookupTable>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : this(data, 0, dataIsCanonical)
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
        }

        public Builder(LookupTable table) : base(table)
        {
        }

        public override LookupTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new LookupTable(data, @base, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubstSubtable> createSubTableBuilder()
        {
            return LigatureSubst.createBuilder();
        }

        public override VisibleSubTable.IBuilder<SubstSubtable> createSubTableBuilder(
            ReadableFontData data, boolean dataIsCanonical)
        {
            return LigatureSubst.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<SubstSubtable> createSubTableBuilder(SubstSubtable subTable)
        {
            return LigatureSubst.createBuilder(subTable);
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override void initFields()
        {
            setField(LOOKUP_TYPE_INDEX, LOOKUP_TYPE_DEFAULT);
            setField(LOOKUP_FLAG_INDEX, LOOKUP_FLAG_INDEX);
        }
    }
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;




public class ExtensionSubst : SubstSubtable
{
    private static readonly int LOOKUP_TYPE_OFFSET = 0;
    private static readonly int LOOKUP_OFFSET_OFFSET = 2;

    readonly GsubLookupType _lookupType;
    readonly int lookupOffset;

    public ExtensionSubst(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
        if (format != 1)
        {
            throw new IllegalArgumentException("illegal extension format " + format);
        }
        this._lookupType = GsubLookupType.forTypeNum(
        data.readUShort(@base + headerSize() + LOOKUP_TYPE_OFFSET));
        lookupOffset = data.readULongAsInt(@base + headerSize() + LOOKUP_OFFSET_OFFSET);
    }

    public GsubLookupType lookupType()
    {
        return _lookupType;
    }

    public SubstSubtable subTable()
    {
        ReadableFontData data = this._data.slice(lookupOffset);
        switch (_lookupType.name())
        {
            case nameof(GsubLookupType.GSUB_LIGATURE):
                return new LigatureSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_SINGLE):
                return new SingleSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_MULTIPLE):
                return new MultipleSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_ALTERNATE):
                return new AlternateSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_CONTEXTUAL):
                return new ContextSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_CHAINING_CONTEXTUAL):
                return new ChainContextSubst(data, 0, dataIsCanonical);
            case nameof(GsubLookupType.GSUB_REVERSE_CHAINING_CONTEXTUAL_SINGLE):
                return new ReverseChainSingleSubst(data, 0, dataIsCanonical);
            default:
                throw new IllegalArgumentException("LookupType is " + _lookupType);
        }
    }
    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, boolean dataIsCanonical)
    {
        return new Builder(data, dataIsCanonical);
    }
    public interface IBuilder : SubstSubtable.IBuilder<SubstSubtable>
    {

    }
    protected class Builder : SubstSubtable.Builder<SubstSubtable>, IBuilder
    {
        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public override SubstSubtable subBuildTable(ReadableFontData data)
        {
            return null;
        }
    }
}
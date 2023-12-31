using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;
using com.google.typography.font.sfntly.table.opentype.contextsubst;

namespace com.google.typography.font.sfntly.table.opentype;








public class ScriptTable : TagOffsetsTable<LangSysTable>
{
    private static readonly int FIELD_COUNT = 1;

    private static readonly int DEFAULT_LANG_SYS_INDEX = 0;
    private static readonly int NO_DEFAULT_LANG_SYS = 0;

    public ScriptTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    public LangSysTable defaultLangSysTable()
    {
        int defaultLangSysOffset = getField(DEFAULT_LANG_SYS_INDEX);
        if (defaultLangSysOffset == NO_DEFAULT_LANG_SYS)
        {
            return null;
        }

        ReadableFontData newData = _data.slice(defaultLangSysOffset);
        LangSysTable langSysTable = new LangSysTable(newData, dataIsCanonical);
        return langSysTable;
    }

    private LanguageTag langSysAt(int index)
    {
        return LanguageTag.fromTag(this.tagAt(index));
    }

    public IDictionary<LanguageTag, LangSysTable> map()
    {
        IDictionary<LanguageTag, LangSysTable> map = new Dictionary<LanguageTag, LangSysTable>();
        LangSysTable defaultLangSys = defaultLangSysTable();
        if (defaultLangSys != null)
        {
            map.put(LanguageTag.DFLT, defaultLangSys);
        }
        for (int i = 0; i < count(); i++)
        {
            LanguageTag lang;
            try
            {
                lang = langSysAt(i);
            }
            catch (IllegalArgumentException e)
            {
                Console.Error.WriteLine("Invalid LangSys tag found: " + e.Message);
                continue;
            }
            map.put(lang, subTableAt(i));
        }
        return map;
    }

    public override LangSysTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new LangSysTable(data, dataIsCanonical);
    }

    public override int fieldCount()
    {
        return FIELD_COUNT;
    }


    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(ReadableFontData data, int @base, boolean dataIsCanonical)
    {
        return new Builder(data, @base, dataIsCanonical);
    }
    public interface IBuilder : TagOffsetsTable<LangSysTable>.IBuilder<ScriptTable>
    {

    }
    private class Builder : TagOffsetsTable<LangSysTable>.Builder<ScriptTable>, IBuilder
    {
        private VisibleSubTable.IBuilder<LangSysTable> defLangSysBuilder;

        public Builder() : base()
        {
        }

        public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
        {
            int defLangSys = getField(DEFAULT_LANG_SYS_INDEX);
            if (defLangSys != NO_DEFAULT_LANG_SYS)
            {
                defLangSysBuilder = LangSysTable.createBuilder(data.slice(defLangSys), dataIsCanonical);
            }
        }

        public override VisibleSubTable.IBuilder<LangSysTable> createSubTableBuilder(
            ReadableFontData data, int tag, boolean dataIsCanonical)
        {
            return LangSysTable.createBuilder(data, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<LangSysTable> createSubTableBuilder()
        {
            return LangSysTable.createBuilder();
        }

        public override ScriptTable readTable(ReadableFontData data, int @base, boolean dataIsCanonical)
        {
            return new ScriptTable(data, @base, dataIsCanonical);
        }

        public override int subDataSizeToSerialize()
        {
            int size = base.subDataSizeToSerialize();
            if (defLangSysBuilder != null)
            {
                size += defLangSysBuilder.subDataSizeToSerialize();
            }
            return size;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int byteCount = base.subSerialize(newData);
            if (defLangSysBuilder != null)
            {
                byteCount += defLangSysBuilder.subSerialize(newData.slice(byteCount));
            }
            return byteCount;
        }

        public override void subDataSet()
        {
            base.subDataSet();
            defLangSysBuilder = null;
        }

        public override int fieldCount()
        {
            return FIELD_COUNT;
        }

        public override void initFields()
        {
            setField(DEFAULT_LANG_SYS_INDEX, NO_DEFAULT_LANG_SYS);
        }
    }
}
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;







public class ScriptListTable : TagOffsetsTable<ScriptTable>
{

   public ScriptListTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    public override ScriptTable readSubTable(ReadableFontData data, boolean dataIsCanonical)
    {
        return new ScriptTable(data, 0, dataIsCanonical);
    }

    public ScriptTag scriptAt(int index)
    {
        return ScriptTag.fromTag(this.tagAt(index));
    }

    public IDictionary<ScriptTag, ScriptTable> map()
    {
        IDictionary<ScriptTag, ScriptTable> map = new Dictionary<ScriptTag, ScriptTable>();
        for (int i = 0; i < count(); i++)
        {
            ScriptTag script;
            try
            {
                script = scriptAt(i);
            }
            catch (IllegalArgumentException e)
            {
                Console.Error.WriteLine("Invalid Script tag found: " + e.Message);
                continue;
            }
            map.put(script, subTableAt(i));
        }
        return map;
    }

    public interface IBuilder : TagOffsetsTable<ScriptTable>.IBuilder<ScriptListTable>
    {
    }


    private class Builder : TagOffsetsTable<ScriptTable>.Builder<ScriptListTable>
    {

        public override VisibleSubTable.IBuilder<ScriptTable> createSubTableBuilder(
            ReadableFontData data, int tag, boolean dataIsCanonical)
        {
            return ScriptTable.createBuilder(data, 0, dataIsCanonical);
        }

        public override VisibleSubTable.IBuilder<ScriptTable> createSubTableBuilder()
        {
            return ScriptTable.createBuilder();
        }

        public override ScriptListTable readTable(
            ReadableFontData data, int baseUnused, boolean dataIsCanonical)
        {
            return new ScriptListTable(data, dataIsCanonical);
        }

        public override void initFields()
        {
        }

        public override int fieldCount()
        {
            return 0;
        }
    }

    public override int fieldCount()
    {
        return 0;
    }
}
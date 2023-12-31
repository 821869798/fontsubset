namespace com.google.typography.font.sfntly.table.core;

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.core;
using static com.google.typography.font.sfntly.table.core.CMapTable;

/**
 * A cmap format 0 sub table.
 *
 */
public sealed class CMapFormat0 : CMap
{
    public CMapFormat0(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format0, cmapId)
    {

    }

    public override int glyphId(int character)
    {
        if (character < 0 || character > 255)
        {
            return CMapTable.NOTDEF;
        }
        return this._data.readUByte(character + (int)Offset.format0GlyphIdArray);
    }

    public override int language()
    {
        return this._data.readUShort((int)Offset.format0Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return Enumerable.Range(0, 0xff + 1).GetEnumerator();
        // return new CharacterIterator();
    }
    /*
    private class CharacterIterator : IEnumerator<Integer>
    {
        int character = 0;
        public static readonly int MAX_CHARACTER = 0xff;

        public CharacterIterator()
        {
        }

        public override boolean hasNext()
        {
            if (character <= MAX_CHARACTER)
            {
                return true;
            }
            return false;
        }

        public override Integer next()
        {
            if (!hasNext())
            {
                throw new NoSuchElementException("No more characters to iterate.");
            }
            return this.character++;
        }

        public override void remove()
        {
            throw new UnsupportedOperationException("Unable to remove a character from cmap.");
        }
    }*/

    public static IBuilder createBuilder(WritableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public static IBuilder createBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public interface IBuilder : CMap.IBuilder<CMapFormat0>
    {

    }
    private sealed class Builder : CMap.Builder<CMapFormat0>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format0Length)), CMapFormat.Format0,
              cmapId)
        {

        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format0Length)), CMapFormat.Format0,
              cmapId)
        {

        }

        public override CMapFormat0 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat0(data, this.cmapId());
        }
    }
}
using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;


/**
 * A cmap format 10 sub table.
 *
 */
public sealed class CMapFormat10 : CMap
{
    private readonly int startCharCode;
    private readonly int numChars;

    public CMapFormat10(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format10, cmapId)
    {

        this.startCharCode = this._data.readULongAsInt((int)Offset.format10StartCharCode);
        this.numChars = this._data.readUShort((int)Offset.format10NumChars);
    }

    public override int glyphId(int character)
    {
        if (character < startCharCode || character >= (startCharCode + numChars))
        {
            return CMapTable.NOTDEF;
        }
        return this.readFontData().readUShort(character - startCharCode);
    }

    public override int language()
    {
        return this._data.readULongAsInt((int)Offset.format10Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return Enumerable.Range(startCharCode, numChars).GetEnumerator();
        // return new CharacterIterator();
    }
    /*
    private class CharacterIterator : IEnumerator<Integer>
    {
        private int character = startCharCode;

        private CharacterIterator()
        {
            // Prevent construction.
        }

        public override boolean hasNext()
        {
            if (character < startCharCode + numChars)
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

    public interface IBuilder : CMap.IBuilder<CMapFormat10>
    {

    }
    private class Builder : CMap.Builder<CMapFormat10>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format10Length)),
              CMapFormat.Format10, cmapId)
        {

        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format10Length)),
              CMapFormat.Format10, cmapId)
        {

        }

        public override CMapFormat10 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat10(data, this.cmapId());
        }
    }
}
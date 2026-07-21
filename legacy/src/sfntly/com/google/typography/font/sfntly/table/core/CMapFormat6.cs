using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;










/**
 * A cmap format 6 sub table.
 */
public sealed class CMapFormat6 : CMap
{

    private readonly int firstCode;
    private readonly int entryCount;

    public CMapFormat6(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format6, cmapId)
    {
        this.firstCode = this._data.readUShort((int)Offset.format6FirstCode);
        this.entryCount = this._data.readUShort((int)Offset.format6EntryCount);
    }

    public override int glyphId(int character)
    {
        if (character < this.firstCode || character >= this.firstCode + this.entryCount)
        {
            return CMapTable.NOTDEF;
        }
        return this._data.readUShort((int)Offset.format6GlyphIdArray + (character - this.firstCode)
            * (int)FontData.DataSize.USHORT);
    }

    public override int language()
    {
        return this._data.readUShort((int)Offset.format6Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return Enumerable.Range(firstCode, entryCount).GetEnumerator();
        // return new CharacterIterator();
    }
    /*
  private class CharacterIterator : IEnumerator<Integer> {
    private int character = firstCode;

    private CharacterIterator() {
      // Prevent construction.
    }

    public override boolean hasNext() {
      if (character < (firstCode + entryCount)) {
        return true;
      }
      return false;
    }

    public override Integer next() {
      if (!hasNext()) {
        throw new NoSuchElementException("No more characters to iterate.");
      }
      return this.character++;
    }

    public override void remove() {
      throw new UnsupportedOperationException("Unable to remove a character from cmap.");
    }
  }
    */

    public static IBuilder createBuilder(WritableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public static IBuilder createBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public interface IBuilder : CMap.IBuilder<CMapFormat6>
    {

    }
    private class Builder : CMap.Builder<CMapFormat6>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format6Length)), CMapFormat.Format6,
              cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format6Length)), CMapFormat.Format6,
              cmapId)
        {
        }

        public override CMapFormat6 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat6(data, this.cmapId());
        }
    }
}
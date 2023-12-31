using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;

/**
 * A cmap format 13 sub table.
 */
public sealed class CMapFormat13 : CMap
{
    private readonly int numberOfGroups;

    public CMapFormat13(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format12, cmapId)
    {

        this.numberOfGroups = this._data.readULongAsInt((int)Offset.format12nGroups);
    }

    private int groupStartChar(int groupIndex)
    {
        return this._data.readULongAsInt(
            (int)Offset.format13Groups + groupIndex * (int)Offset.format13Groups_structLength
                + (int)Offset.format13_startCharCode);
    }

    private int groupEndChar(int groupIndex)
    {
        return this._data.readULongAsInt(
            (int)Offset.format13Groups + groupIndex * (int)Offset.format13Groups_structLength
                + (int)Offset.format13_endCharCode);
    }

    private int groupGlyph(int groupIndex)
    {
        return this._data.readULongAsInt(
            (int)Offset.format13Groups + groupIndex * (int)Offset.format13Groups_structLength
                + (int)Offset.format13_glyphId);
    }

    public override int glyphId(int character)
    {
        int group = this._data.searchULong(
            (int)Offset.format13Groups + (int)Offset.format13_startCharCode,
            (int)Offset.format13Groups_structLength,
            (int)Offset.format13Groups + (int)Offset.format13_endCharCode,
            (int)Offset.format13Groups_structLength,
            this.numberOfGroups,
            character);
        if (group == -1)
        {
            return CMapTable.NOTDEF;
        }
        return groupGlyph(group);
    }

    public override int language()
    {
        return this._data.readULongAsInt((int)Offset.format12Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return Enumerable
            .Range(0, numberOfGroups)
            .SelectMany(groupIndex =>
            {
                var startChar = groupStartChar(groupIndex);
                var endChar = groupEndChar(groupIndex);

                return Enumerable.Range(startChar, endChar - startChar + 1);
            })
            .GetEnumerator();
        // return new CharacterIterator();
    }
    /*
    private sealed class CharacterIterator : IEnumerator<Integer>
    {
        private int groupIndex = 0;
        private int groupEndChar;

        private boolean nextSet = false;
        private int nextChar;

        private CharacterIterator()
        {
            nextChar = groupStartChar(groupIndex);
            groupEndChar = groupEndChar(groupIndex);
            nextSet = true;
        }

        public override boolean hasNext()
        {
            if (nextSet)
            {
                return true;
            }
            if (groupIndex >= numberOfGroups)
            {
                return false;
            }
            if (nextChar < groupEndChar)
            {
                nextChar++;
                nextSet = true;
                return true;
            }
            groupIndex++;
            if (groupIndex < numberOfGroups)
            {
                nextSet = true;
                nextChar = groupStartChar(groupIndex);
                groupEndChar = groupEndChar(groupIndex);
                return true;
            }
            return false;
        }

        public override Integer next()
        {
            if (!this.nextSet)
            {
                if (!hasNext())
                {
                    throw new NoSuchElementException("No more characters to iterate.");
                }
            }
            this.nextSet = false;
            return nextChar;
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

    public interface IBuilder : CMap.IBuilder<CMapFormat13>
    {

    }
    private class Builder : CMap.Builder<CMapFormat13>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format13Length)),
              CMapFormat.Format13, cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format13Length)),
              CMapFormat.Format13, cmapId)
        {
        }

        public override CMapFormat13 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat13(data, this.cmapId());
        }
    }
}
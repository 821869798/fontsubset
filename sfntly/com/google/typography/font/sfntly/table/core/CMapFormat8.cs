using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;









/**
 * A cmap format 8 sub table.
 *
 */
public sealed class CMapFormat8 : CMap
{
    private readonly int numberOfGroups;

    public CMapFormat8(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format8, cmapId)
    {
        this.numberOfGroups = this._data.readULongAsInt((int)Offset.format8nGroups);
    }

    private int firstChar(int groupIndex)
    {
        return this.readFontData().readULongAsInt(
            (int)Offset.format8Groups + groupIndex * (int)Offset.format8Group_structLength
                + (int)Offset.format8Group_startCharCode);
    }

    private int endChar(int groupIndex)
    {
        return this.readFontData().readULongAsInt(
            (int)Offset.format8Groups + groupIndex * (int)Offset.format8Group_structLength
                + (int)Offset.format8Group_endCharCode);
    }

    public override int glyphId(int character)
    {
        return this.readFontData().searchULong((int)Offset.format8Groups
            + (int)Offset.format8Group_startCharCode,
            (int)Offset.format8Group_structLength,
            (int)Offset.format8Groups + (int)Offset.format8Group_endCharCode,
            (int)Offset.format8Group_structLength,
            numberOfGroups,
            character);
    }

    public override int language()
    {
        return this._data.readULongAsInt((int)Offset.format8Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        // return new CharacterIterator();

        return Internal().GetEnumerator();

        IEnumerable<Integer> Internal()
        {
            int groupIndex = default;
            int firstCharInGroup = default;
            int endCharInGroup = default;

            int nextChar = default;
            boolean nextCharSet = default;

            groupIndex = 0;
            firstCharInGroup = -1;

            while (hasNext())
            {
                yield return next();
            }


            boolean hasNext()
            {
                if (nextCharSet == true)
                {
                    return true;
                }
                while (groupIndex < numberOfGroups)
                {
                    if (firstCharInGroup < 0)
                    {
                        firstCharInGroup = firstChar(groupIndex);
                        endCharInGroup = endChar(groupIndex);
                        nextChar = firstCharInGroup;
                        nextCharSet = true;
                        return true;
                    }
                    if (nextChar < endCharInGroup)
                    {
                        nextChar++;
                        nextCharSet = true;
                        return true;
                    }
                    groupIndex++;
                    firstCharInGroup = -1;
                }
                return false;
            }

            Integer next()
            {
                if (!nextCharSet)
                {
                    if (!hasNext())
                    {
                        throw new NoSuchElementException("No more characters to iterate.");
                    }
                }
                nextCharSet = false;
                return nextChar;
            }
        }
    }
    /*
      private class CharacterIterator : IEnumerator<Integer> {
        private int groupIndex;
        private int firstCharInGroup;
        private int endCharInGroup;

        private int nextChar;
        private boolean nextCharSet;

        private CharacterIterator() {
          groupIndex = 0;
          firstCharInGroup = -1;
        }

        public override boolean hasNext() {
          if (nextCharSet == true) {
            return true;
          }
          while (groupIndex < numberOfGroups) {
            if (firstCharInGroup < 0) {
              firstCharInGroup = firstChar(groupIndex);
              endCharInGroup = endChar(groupIndex);
              nextChar = firstCharInGroup;
              nextCharSet = true;
              return true;
            }
            if (nextChar < endCharInGroup) {
              nextChar++;
              nextCharSet = true;
              return true;
            }
            groupIndex++;
            firstCharInGroup = -1;
          }
          return false;
        }

        public override Integer next() {
          if (!nextCharSet) {
            if (!hasNext()) {
              throw new NoSuchElementException("No more characters to iterate.");
            }
          }
          nextCharSet = false;
          return nextChar;
        }

        public override void remove() {
          throw new UnsupportedOperationException("Unable to remove a character from cmap.");
        }
      }*/

    public static IBuilder createBuilder(WritableFontData data, int offset, CMapId cmapId) { 
        return new Builder(data, offset, cmapId);
    }

    public static IBuilder createBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public interface IBuilder : CMap.IBuilder<CMapFormat8>
    {

    }

    private class Builder : CMap.Builder<CMapFormat8>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format8Length)), CMapFormat.Format8,
              cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format8Length)), CMapFormat.Format8,
              cmapId)
        {
        }

        public override CMapFormat8 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat8(data, this.cmapId());
        }
    }
}
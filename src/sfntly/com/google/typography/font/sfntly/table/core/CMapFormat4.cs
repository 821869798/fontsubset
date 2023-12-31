using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.math;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;













/**
 * A cmap format 4 sub table.
 */
public sealed class CMapFormat4 : CMap
{
    private readonly int _segCount;
    private readonly int _glyphIdArrayOffset;

    public CMapFormat4(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format4, cmapId)
    {

        this._segCount = this._data.readUShort((int)Offset.format4SegCountX2) / 2;
        this._glyphIdArrayOffset = glyphIdArrayOffset(this._segCount);
    }

    public override int glyphId(int character)
    {
        int segment = this._data.searchUShort(CMapFormat4.startCodeOffset(this._segCount),
            (int)FontData.DataSize.USHORT,
            (int)Offset.format4EndCount,
            (int)FontData.DataSize.USHORT,
            this._segCount,
            character);
        if (segment == -1)
        {
            return CMapTable.NOTDEF;
        }
        int startCode = this.startCode(segment);

        return retrieveGlyphId(segment, startCode, character);
    }

    /**
     * Lower level glyph code retrieval that requires processing the Format 4 segments to use.
     *
     * @param segment the cmap segment
     * @param startCode the start code for the segment
     * @param character the character to be looked up
     * @return the glyph id for the character; CMapTable.NOTDEF if not found
     */
    public int retrieveGlyphId(int segment, int startCode, int character)
    {
        if (character < startCode)
        {
            return CMapTable.NOTDEF;
        }
        int idRangeOffset = this.idRangeOffset(segment);
        if (idRangeOffset == 0)
        {
            return (character + this.idDelta(segment)) % 65536;
        }
        int gid = this._data.readUShort(
            idRangeOffset + this.idRangeOffsetLocation(segment) + 2 * (character - startCode));
        if (gid != 0)
        {
            gid = (gid + this.idDelta(segment)) % 65536;
        }
        return gid;
    }

    /**
     * Gets the count of the number of segments in this cmap.
     *
     * @return the number of segments
     */
    public int getSegCount()
    {
        return _segCount;
    }

    /**
     * Gets the start code for a segment.
     *
     * @param segment the segment in the look up table
     * @return the start code for the segment
     */
    public int startCode(int segment)
    {
        isValidIndex(segment);
        return startCode(this._data, this._segCount, segment);
    }

    private static int length(ReadableFontData data)
    {
        int length = data.readUShort((int)Offset.format4Length);
        return length;
    }

    private static int segCount(ReadableFontData data)
    {
        int segCount = data.readUShort((int)Offset.format4SegCountX2) / 2;
        return segCount;
    }

    private static int startCode(ReadableFontData data, int segCount, int index)
    {
        int startCode =
            data.readUShort(startCodeOffset(segCount) + index * (int)FontData.DataSize.USHORT);
        return startCode;
    }

    private static int startCodeOffset(int segCount)
    {
        int startCodeOffset =
            (int)Offset.format4EndCount + (int)FontData.DataSize.USHORT + segCount
                * (int)FontData.DataSize.USHORT;
        return startCodeOffset;
    }

    private static int endCode(ReadableFontData data, int segCount, int index)
    {
        int endCode =
            data.readUShort((int)Offset.format4EndCount + index * (int)FontData.DataSize.USHORT);
        return endCode;
    }

    private static int idDelta(ReadableFontData data, int segCount, int index)
    {
        int idDelta =
            data.readShort(idDeltaOffset(segCount) + index * (int)FontData.DataSize.SHORT);
        return idDelta;
    }

    private static int idDeltaOffset(int segCount)
    {
        int idDeltaOffset =
            (int)Offset.format4EndCount + ((2 * segCount) + 1) * (int)FontData.DataSize.USHORT;
        return idDeltaOffset;
    }

    private static int idRangeOffset(ReadableFontData data, int segCount, int index)
    {
        int idRangeOffset =
            data.readUShort(idRangeOffsetOffset(segCount) + index * (int)FontData.DataSize.USHORT);
        return idRangeOffset;
    }

    private static int idRangeOffsetOffset(int segCount)
    {
        int idRangeOffsetOffset =
            (int)Offset.format4EndCount + ((2 * segCount) + 1) * (int)FontData.DataSize.USHORT
                + segCount * (int)FontData.DataSize.SHORT;
        return idRangeOffsetOffset;
    }

    private static int glyphIdArrayOffset(int segCount)
    {
        int glyphIdArrayOffset =
            (int)Offset.format4EndCount + ((3 * segCount) + 1) * (int)FontData.DataSize.USHORT
                + segCount * (int)FontData.DataSize.SHORT;
        return glyphIdArrayOffset;
    }

    /**
     * Gets the end code for a segment.
     *
     * @param segment the segment in the look up table
     * @return the end code for the segment
     */
    public int endCode(int segment)
    {
        isValidIndex(segment);
        return endCode(this._data, this._segCount, segment);
    }

    private void isValidIndex(int segment)
    {
        if (segment < 0 || segment >= this._segCount)
        {
            throw new IllegalArgumentException();
        }
    }

    /**
     * Gets the id delta for a segment.
     *
     * @param segment the segment in the look up table
     * @return the id delta for the segment
     */
    public int idDelta(int segment)
    {
        isValidIndex(segment);
        return idDelta(this._data, this._segCount, segment);
    }

    /**
     * Gets the id range offset for a segment.
     *
     * @param segment the segment in the look up table
     * @return the id range offset for the segment
     */
    public int idRangeOffset(int segment)
    {
        isValidIndex(segment);
        return this._data.readUShort(this.idRangeOffsetLocation(segment));
    }

    /**
     * Get the location of the id range offset for a segment.
     * @param segment the segment in the look up table
     * @return the location of the id range offset for the segment
     */
    public int idRangeOffsetLocation(int segment)
    {
        isValidIndex(segment);
        return idRangeOffsetOffset(this._segCount) + segment * (int)FontData.DataSize.USHORT;
    }

    private int glyphIdArray(int index)
    {
        return this._data.readUShort(
            this._glyphIdArrayOffset + index * (int)FontData.DataSize.USHORT);
    }

    public override int language()
    {
        return this._data.readUShort((int)Offset.format4Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        // return new CharacterIterator();

        return Internal().GetEnumerator();

        IEnumerable<Integer> Internal()
        {
            int segmentIndex = default;
            int firstCharInSegment = default;
            int lastCharInSegment = default;

            int nextChar = default;
            boolean nextCharSet = default;

            segmentIndex = 0;
            firstCharInSegment = -1;

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
                while (segmentIndex < _segCount)
                {
                    if (firstCharInSegment < 0)
                    {
                        firstCharInSegment = startCode(segmentIndex);
                        lastCharInSegment = endCode(segmentIndex);
                        nextChar = firstCharInSegment;
                        nextCharSet = true;
                        return true;
                    }
                    if (nextChar < lastCharInSegment)
                    {
                        nextChar++;
                        nextCharSet = true;
                        return true;
                    }
                    segmentIndex++;
                    firstCharInSegment = -1;
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
    private int segmentIndex;
    private int firstCharInSegment;
    private int lastCharInSegment;

    private int nextChar;
    private boolean nextCharSet;

    private CharacterIterator() {
      segmentIndex = 0;
      firstCharInSegment = -1;
    }

    public override boolean hasNext() {
      if (nextCharSet == true) {
        return true;
      }
      while (segmentIndex < _segCount) {
        if (firstCharInSegment < 0) {
          firstCharInSegment = startCode(segmentIndex);
          lastCharInSegment = endCode(segmentIndex);
          nextChar = firstCharInSegment;
          nextCharSet = true;
          return true;
        }
        if (nextChar < lastCharInSegment) {
          nextChar++;
          nextCharSet = true;
          return true;
        }
        segmentIndex++;
        firstCharInSegment = -1;
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

    public interface IBuilder : CMap.IBuilder<CMapFormat4>
    {

        IList<IBuilder.Segment> getSegments();

        void setSegments(IList<IBuilder.Segment> segments);

        IList<Integer> getGlyphIdArray();

        void setGlyphIdArray(IList<Integer> glyphIdArray);

        public class Segment
        {
            public static IList<Segment> deepCopy(IList<Segment> original)
            {
                IList<Segment> list = new List<Segment>(original.Count);
                foreach (var segment in original)
                {
                    list.Add(new Segment(segment));
                }
                return list;
            }

            private int startCount;
            private int endCount;
            private int idDelta;
            private int idRangeOffset;

            public Segment()
            {
            }

            public Segment(Segment other) : this(other.startCount, other.endCount, other.idDelta, other.idRangeOffset)
            {
            }

            public Segment(int startCount, int endCount, int idDelta, int idRangeOffset)
            {
                this.startCount = startCount;
                this.endCount = endCount;
                this.idDelta = idDelta;
                this.idRangeOffset = idRangeOffset;
            }

            /**
             * @return the startCount
             */
            public int getStartCount()
            {
                return startCount;
            }

            /**
             * @param startCount the startCount to set
             */
            public void setStartCount(int startCount)
            {
                this.startCount = startCount;
            }

            /**
             * @return the endCount
             */
            public int getEndCount()
            {
                return endCount;
            }

            /**
             * @param endCount the endCount to set
             */
            public void setEndCount(int endCount)
            {
                this.endCount = endCount;
            }

            /**
             * @return the idDelta
             */
            public int getIdDelta()
            {
                return idDelta;
            }

            /**
             * @param idDelta the idDelta to set
             */
            public void setIdDelta(int idDelta)
            {
                this.idDelta = idDelta;
            }

            /**
             * @return the idRangeOffset
             */
            public int getIdRangeOffset()
            {
                return idRangeOffset;
            }

            /**
             * @param idRangeOffset the idRangeOffset to set
             */
            public void setIdRangeOffset(int idRangeOffset)
            {
                this.idRangeOffset = idRangeOffset;
            }

            public override String ToString()
            {
                return String.Format("[0x%04x - 0x%04x, delta = 0x%04x, rangeOffset = 0x%04x]",
                    this.startCount, this.endCount, this.idDelta, this.idRangeOffset);
            }
        }
    }
    private class Builder : CMap.Builder<CMapFormat4>, IBuilder
    {

        private IList<IBuilder.Segment> segments;
        private IList<Integer> glyphIdArray;

        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format4Length)), CMapFormat.Format4,
              cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format4Length)), CMapFormat.Format4,
              cmapId)
        {
        }

        private void initialize(ReadableFontData data)
        {
            this.segments = new List<IBuilder.Segment>();
            this.glyphIdArray = new List<Integer>();

            if (data == null || data.length() == 0)
            {
                return;
            }

            // build segments
            int segCount = CMapFormat4.segCount(data);
            for (int index = 0; index < segCount; index++)
            {
                IBuilder.Segment segment = new IBuilder.Segment();
                segment.setStartCount(CMapFormat4.startCode(data, segCount, index));
                segment.setEndCount(CMapFormat4.endCode(data, segCount, index));
                segment.setIdDelta(CMapFormat4.idDelta(data, segCount, index));
                segment.setIdRangeOffset(CMapFormat4.idRangeOffset(data, segCount, index));

                this.segments.Add(segment);
            }

            // build glyph id array
            int glyphIdArrayLength =
                CMapFormat4.length(data) - CMapFormat4.glyphIdArrayOffset(segCount);
            for (int index = 0; index < glyphIdArrayLength; index += (int)FontData.DataSize.USHORT)
            {
                this.glyphIdArray.Add(data.readUShort(index + CMapFormat4.glyphIdArrayOffset(segCount)));
            }
        }

        public IList<IBuilder.Segment> getSegments()
        {
            if (this.segments == null)
            {
                this.initialize(this.internalReadData());
                this.setModelChanged();
            }
            return this.segments;
        }

        public void setSegments(IList<IBuilder.Segment> segments)
        {
            this.segments = IBuilder.Segment.deepCopy(segments);
            this.setModelChanged();
        }

        public IList<Integer> getGlyphIdArray()
        {
            if (this.glyphIdArray == null)
            {
                this.initialize(this.internalReadData());
                this.setModelChanged();
            }
            return this.glyphIdArray;
        }

        public void setGlyphIdArray(IList<Integer> glyphIdArray)
        {
            this.glyphIdArray = new System.Collections.Generic.List<Integer>(glyphIdArray);
            this.setModelChanged();
        }

        public override CMapFormat4 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat4(data, this.cmapId());
        }

        public override void subDataSet()
        {
            this.segments = null;
            this.glyphIdArray = null;
            base.setModelChanged(false);
        }

        public override int subDataSizeToSerialize()
        {
            if (!this.modelChanged())
            {
                return base.subDataSizeToSerialize();
            }

            int size = (int)Offset.format4FixedSize + this.segments.Count
                * (3 * (int)FontData.DataSize.USHORT + (int)FontData.DataSize.SHORT)
                + this.glyphIdArray.Count * (int)FontData.DataSize.USHORT;
            return size;
        }

        public override boolean subReadyToSerialize()
        {
            if (!this.modelChanged())
            {
                return base.subReadyToSerialize();
            }

            if (this.segments != null)
            {
                return true;
            }
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            if (!this.modelChanged())
            {
                return base.subSerialize(newData);
            }

            int index = 0;
            index += newData.writeUShort(index, (int)CMapFormat.Format4);
            index += (int)FontData.DataSize.USHORT; // length - write this at the
                                                    // end
            index += newData.writeUShort(index, this.language());
            int segCount = this.segments.Count;
            index += newData.writeUShort(index, segCount * 2);
            int log2SegCount = FontMath.log2(this.segments.Count);
            int searchRange = 1 << (log2SegCount + 1);
            index += newData.writeUShort(index, searchRange);
            int entrySelector = log2SegCount;
            index += newData.writeUShort(index, entrySelector);
            int rangeShift = 2 * segCount - searchRange;
            index += newData.writeUShort(index, rangeShift);

            for (int i = 0; i < segCount; i++)
            {
                index += newData.writeUShort(index, this.segments.get(i).getEndCount());
            }
            index += (int)FontData.DataSize.USHORT; // reserved UShort
            for (int i = 0; i < segCount; i++)
            {
                index += newData.writeUShort(index, this.segments.get(i).getStartCount());
            }
            for (int i = 0; i < segCount; i++)
            {
                index += newData.writeShort(index, this.segments.get(i).getIdDelta());
            }
            for (int i = 0; i < segCount; i++)
            {
                index += newData.writeUShort(index, this.segments.get(i).getIdRangeOffset());
            }

            for (int i = 0; i < this.glyphIdArray.Count; i++)
            {
                index += newData.writeUShort(index, this.glyphIdArray.get(i));
            }

            newData.writeUShort((int)Offset.format4Length, index);

            return index;
        }
    }
}
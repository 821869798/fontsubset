using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;


/**
 * A cmap format 2 sub table.
 *
 * The format 2 cmap is used for multi-byte encodings such as SJIS,
 * EUC-JP/KR/CN, Big5, etc.
 */
public sealed class CMapFormat2 : CMap
{

    public CMapFormat2(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format2, cmapId)
    {

    }

    private int subHeaderOffset(int subHeaderIndex)
    {
        int subHeaderOffset = this._data.readUShort(
            (int)Offset.format2SubHeaderKeys + subHeaderIndex * (int)FontData.DataSize.USHORT);
        return subHeaderOffset;
    }

    private int firstCode(int subHeaderIndex)
    {
        int subHeaderOffset = this.subHeaderOffset(subHeaderIndex);
        int firstCode =
            this._data.readUShort(subHeaderOffset + (int)Offset.format2SubHeaderKeys
                + (int)Offset.format2SubHeader_firstCode);
        return firstCode;
    }

    private int entryCount(int subHeaderIndex)
    {
        int subHeaderOffset = this.subHeaderOffset(subHeaderIndex);
        int entryCount =
            this._data.readUShort(subHeaderOffset + (int)Offset.format2SubHeaderKeys
                + (int)Offset.format2SubHeader_entryCount);
        return entryCount;
    }

    private int idRangeOffset(int subHeaderIndex)
    {
        int subHeaderOffset = this.subHeaderOffset(subHeaderIndex);
        int idRangeOffset = this._data.readUShort(subHeaderOffset + (int)Offset.format2SubHeaderKeys
            + (int)Offset.format2SubHeader_idRangeOffset);
        return idRangeOffset;
    }

    private int idDelta(int subHeaderIndex)
    {
        int subHeaderOffset = this.subHeaderOffset(subHeaderIndex);
        int idDelta =
            this._data.readShort(subHeaderOffset + (int)Offset.format2SubHeaderKeys
                + (int)Offset.format2SubHeader_idDelta);
        return idDelta;
    }

    /**
     * Returns how many bytes would be consumed by a lookup of this character
     * with this cmap. This comes about because the cmap format 2 table is
     * designed around multi-byte encodings such as SJIS, EUC-JP, Big5, etc.
     *
     * @param character
     * @return the number of bytes consumed from this "character" - either 1 or
     *         2
     */
    public int bytesConsumed(int character)
    {
        int highByte = (character >> 8) & 0xff;
        int offset = subHeaderOffset(highByte);

        if (offset == 0)
        {
            return 1;
        }
        return 2;
    }

    public override int glyphId(int character)
    {
        if (character > 0xffff)
        {
            return CMapTable.NOTDEF;
        }

        int highByte = (character >> 8) & 0xff;
        int lowByte = character & 0xff;
        int offset = subHeaderOffset(highByte);

        // only consume one byte
        if (offset == 0)
        {
            lowByte = highByte;
            highByte = 0;
        }

        int firstCode = this.firstCode(highByte);
        int entryCount = this.entryCount(highByte);

        if (lowByte < firstCode || lowByte >= firstCode + entryCount)
        {
            return CMapTable.NOTDEF;
        }

        int idRangeOffset = this.idRangeOffset(highByte);

        // position of idRangeOffset + value of idRangeOffset + index for low byte
        // = firstcode
        int pLocation = (offset + (int)Offset.format2SubHeader_idRangeOffset) + idRangeOffset
            + (lowByte - firstCode) * (int)FontData.DataSize.USHORT;
        int p = this._data.readUShort(pLocation);
        if (p == 0)
        {
            return CMapTable.NOTDEF;
        }

        if (offset == 0)
        {
            return p;
        }
        int idDelta = this.idDelta(highByte);
        return (p + idDelta) % 65536;
    }

    public override int language()
    {
        return this._data.readUShort((int)Offset.format2Language);
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return Enumerable.Range(0, 0xffff + 1).GetEnumerator();
        // return new CharacterIterator(0, 0xffff);
    }

    public static IBuilder createBuilder(WritableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public static IBuilder createBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public interface IBuilder : CMap.IBuilder<CMapFormat2>
    {

    }

    private sealed class Builder : CMap.Builder<CMapFormat2>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format2Length)), CMapFormat.Format2,
              cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readUShort(offset + (int)Offset.format2Length)), CMapFormat.Format2,
              cmapId)
        {

        }

        public override CMapFormat2 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat2(data, this.cmapId());
        }
    }
}
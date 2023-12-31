using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.CMapTable;

namespace com.google.typography.font.sfntly.table.core;








/**
 * A cmap format 14 sub table.
 */
// TODO(stuartg): completely unsupported yet
public sealed class CMapFormat14 : CMap
{

    public CMapFormat14(ReadableFontData data, CMapId cmapId) : base(data, (int)CMapFormat.Format14, cmapId)
    {
    }

    public override int glyphId(int character)
    {
        return CMapTable.NOTDEF;
    }

    public override int language()
    {
        return 0;
    }

    public override IEnumerator<Integer> GetEnumerator()
    {
        return null;
    }

    public static IBuilder createBuilder(WritableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public static IBuilder createBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        return new Builder(data, offset, cmapId);
    }

    public interface IBuilder : CMap.IBuilder<CMapFormat14>
    {

    }

    private class Builder : CMap.Builder<CMapFormat14>, IBuilder
    {
        public Builder(WritableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format14Length)),
              CMapFormat.Format14, cmapId)
        {
        }

        public Builder(ReadableFontData data, int offset, CMapId cmapId) : base(data == null ? null : data.slice(
              offset, data.readULongAsInt(offset + (int)Offset.format14Length)),
              CMapFormat.Format14, cmapId)
        {
        }

        public override CMapFormat14 subBuildTable(ReadableFontData data)
        {
            return new CMapFormat14(data, this.cmapId());
        }
    }
}
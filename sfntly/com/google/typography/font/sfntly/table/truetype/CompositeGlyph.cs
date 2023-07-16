using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.truetype.SimpleGlyph;

namespace com.google.typography.font.sfntly.table.truetype;








public sealed class CompositeGlyph : Glyph
{
    public static readonly int FLAG_ARG_1_AND_2_ARE_WORDS = 0x01;
    public static readonly int FLAG_ARGS_ARE_XY_VALUES = 0x01 << 1;
    public static readonly int FLAG_ROUND_XY_TO_GRID = 0x01 << 2;
    public static readonly int FLAG_WE_HAVE_A_SCALE = 0x01 << 3;
    public static readonly int FLAG_RESERVED = 0x01 << 4;
    public static readonly int FLAG_MORE_COMPONENTS = 0x01 << 5;
    public static readonly int FLAG_WE_HAVE_AN_X_AND_Y_SCALE = 0x01 << 6;
    public static readonly int FLAG_WE_HAVE_A_TWO_BY_TWO = 0x01 << 7;
    public static readonly int FLAG_WE_HAVE_INSTRUCTIONS = 0x01 << 8;
    public static readonly int FLAG_USE_MY_METRICS = 0x01 << 9;
    public static readonly int FLAG_OVERLAP_COMPOUND = 0x01 << 10;
    public static readonly int FLAG_SCALED_COMPONENT_OFFSET = 0x01 << 11;
    public static readonly int FLAG_UNSCALED_COMPONENT_OFFSET = 0x01 << 12;

    private readonly IList<Integer> contourIndex = new List<Integer>();
    private int _instructionsOffset;
    private int _instructionSize;

    public CompositeGlyph(ReadableFontData data, int offset, int length) : base(data, offset, length, GlyphType.Composite)
    {
        initialize();
    }

    public CompositeGlyph(ReadableFontData data) : base(data, GlyphType.Composite)
    {
        initialize();
    }

    public override void initialize()
    {
        if (this.initialized)
        {
            return;
        }
        lock (this.initializationLock)
        {
            if (this.initialized)
            {
                return;
            }

            int index = 5 * (int)FontData.DataSize.USHORT; // header
            int flags = FLAG_MORE_COMPONENTS;
            while ((flags & FLAG_MORE_COMPONENTS) == FLAG_MORE_COMPONENTS)
            {
                contourIndex.Add(index);
                flags = this._data.readUShort(index);
                index += 2 * (int)FontData.DataSize.USHORT; // flags and
                                                              // glyphIndex
                if ((flags & FLAG_ARG_1_AND_2_ARE_WORDS) == FLAG_ARG_1_AND_2_ARE_WORDS)
                {
                    index += 2 * (int)FontData.DataSize.SHORT;
                }
                else
                {
                    index += 2 * (int)FontData.DataSize.BYTE;
                }
                if ((flags & FLAG_WE_HAVE_A_SCALE) == FLAG_WE_HAVE_A_SCALE)
                {
                    index += (int)FontData.DataSize.F2DOT14;
                }
                else if ((flags & FLAG_WE_HAVE_AN_X_AND_Y_SCALE) == FLAG_WE_HAVE_AN_X_AND_Y_SCALE)
                {
                    index += 2 * (int)FontData.DataSize.F2DOT14;
                }
                else if ((flags & FLAG_WE_HAVE_A_TWO_BY_TWO) == FLAG_WE_HAVE_A_TWO_BY_TWO)
                {
                    index += 4 * (int)FontData.DataSize.F2DOT14;
                }
            }
            int nonPaddedDataLength = index;
            if ((flags & FLAG_WE_HAVE_INSTRUCTIONS) == FLAG_WE_HAVE_INSTRUCTIONS)
            {
                this._instructionSize = this._data.readUShort(index);
                index += (int)FontData.DataSize.USHORT;
                this._instructionsOffset = index;
                nonPaddedDataLength = index + (this._instructionSize * (int)FontData.DataSize.BYTE);
            }
            this.setPadding(this.dataLength() - nonPaddedDataLength);
        }
    }

    public int flags(int contour)
    {
        return this._data.readUShort(this.contourIndex.get(contour));
    }

    public int numGlyphs()
    {
        return this.contourIndex.Count;
    }

    public int glyphIndex(int contour)
    {
        return this._data.readUShort((int)FontData.DataSize.USHORT + this.contourIndex.get(contour));
    }

    public int argument1(int contour)
    {
        int index = 2 * (int)FontData.DataSize.USHORT + this.contourIndex.get(contour);
        int flags = this.flags(contour);
        if ((flags & FLAG_ARG_1_AND_2_ARE_WORDS) == FLAG_ARG_1_AND_2_ARE_WORDS)
        {
            return this._data.readUShort(index);
        }
        return this._data.readByte(index);
    }

    public int argument2(int contour)
    {
        int index = 2 * (int)FontData.DataSize.USHORT + this.contourIndex.get(contour);
        int flags = this.flags(contour);
        if ((flags & FLAG_ARG_1_AND_2_ARE_WORDS) == FLAG_ARG_1_AND_2_ARE_WORDS)
        {
            return this._data.readUShort(index + (int)FontData.DataSize.USHORT);
        }
        return this._data.readByte(index + (int)FontData.DataSize.BYTE);
    }

    public int transformationSize(int contour)
    {
        int flags = this.flags(contour);
        if ((flags & FLAG_WE_HAVE_A_SCALE) == FLAG_WE_HAVE_A_SCALE)
        {
            return (int)FontData.DataSize.F2DOT14;
        }
        else if ((flags & FLAG_WE_HAVE_AN_X_AND_Y_SCALE) == FLAG_WE_HAVE_AN_X_AND_Y_SCALE)
        {
            return 2 * (int)FontData.DataSize.F2DOT14;
        }
        else if ((flags & FLAG_WE_HAVE_A_TWO_BY_TWO) == FLAG_WE_HAVE_A_TWO_BY_TWO)
        {
            return 4 * (int)FontData.DataSize.F2DOT14;
        }
        return 0;
    }

    public byte[] transformation(int contour)
    {
        int flags = this.flags(contour);
        int index = this.contourIndex.get(contour) + 2 * (int)FontData.DataSize.USHORT;
        if ((flags & FLAG_ARG_1_AND_2_ARE_WORDS) == FLAG_ARG_1_AND_2_ARE_WORDS)
        {
            index += 2 * (int)FontData.DataSize.SHORT;
        }
        else
        {
            index += 2 * (int)FontData.DataSize.BYTE;
        }

        int tsize = transformationSize(contour);
        byte[] transformation = new byte[tsize];
        this._data.readBytes(index, transformation, 0, tsize);
        return transformation;
    }

    public override int instructionSize()
    {
        return this._instructionSize;
    }

    public override ReadableFontData instructions()
    {
        return this._data.slice(this._instructionsOffset, this.instructionSize());
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder(base.ToString());
        sb.Append("\ncontourOffset.length = ");
        sb.Append(this.contourIndex.Count);
        sb.Append("\ninstructionSize = ");
        sb.Append(this._instructionSize);
        sb.Append("\n\tcontour index = [");
        for (int contour = 0; contour < this.contourIndex.Count; contour++)
        {
            if (contour != 0)
            {
                sb.Append(", ");
            }
            sb.Append(this.contourIndex.get(contour));
        }
        sb.Append("]\n");
        for (int contour = 0; contour < this.contourIndex.Count; contour++)
        {
            sb.Append("\t" + contour + " = [gid = " + this.glyphIndex(contour) + ", arg1 = "
                + this.argument1(contour) + ", arg2 = " + this.argument2(contour) + "]\n");
        }
        return sb.ToString();
    }


    public static ICompositeGlyphBuilder createBuilder(WritableFontData data, int offset, int length)
    {
        return new CompositeGlyphBuilder(data, offset, length);
    }


    public static ICompositeGlyphBuilder createBuilder(ReadableFontData data, int offset, int length)
    {
        return new CompositeGlyphBuilder(data, offset, length);
    }

    public interface ICompositeGlyphBuilder : Glyph.IBuilder<CompositeGlyph>
    {

    }

    private sealed class CompositeGlyphBuilder : Glyph.Builder<CompositeGlyph>, ICompositeGlyphBuilder
    {
        public CompositeGlyphBuilder(WritableFontData data, int offset, int length) : base(data.slice(offset, length))
        {
        }

        public CompositeGlyphBuilder(ReadableFontData data, int offset, int length) : base(data.slice(offset, length))
        {
        }

        public override CompositeGlyph subBuildTable(ReadableFontData data)
        {
            return new CompositeGlyph(data);
        }
    }
}
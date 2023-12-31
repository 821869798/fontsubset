using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.truetype.GlyphTable;

namespace com.google.typography.font.sfntly.table.truetype;






public abstract class Glyph : SubTable
{

    public enum GlyphType
    {
        Simple,
        Composite,
    }

    public volatile boolean initialized = false;
    // TOO(stuartg): should we replace this with a shared lock? more contention
    // but less space
    public readonly Object initializationLock = new Object();

    private readonly Glyph.GlyphType _glyphType;
    private readonly int _numberOfContours;

    public Glyph(ReadableFontData data, Glyph.GlyphType glyphType) : base(data)
    {
        this._glyphType = glyphType;

        if (this._data.length() == 0)
        {
            this._numberOfContours = 0;
        }
        else
        {
            // -1 if composite
            this._numberOfContours = this._data.readShort((int)Offset.numberOfContours);
        }
    }

    public Glyph(ReadableFontData data, int offset, int length, Glyph.GlyphType glyphType) : base(data, offset, length)
    {
        this._glyphType = glyphType;

        if (this._data.length() == 0)
        {
            this._numberOfContours = 0;
        }
        else
        {
            // -1 if composite
            this._numberOfContours = this._data.readShort((int)Offset.numberOfContours);
        }
    }

    private static Glyph.GlyphType glyphType(ReadableFontData data, int offset, int length)
    {
        if (offset > data.length())
        {
            throw new IndexOutOfBoundsException();
        }
        if (length == 0)
        {
            return GlyphType.Simple;
        }
        int numberOfContours = data.readShort(offset);
        if (numberOfContours >= 0)
        {
            return GlyphType.Simple;
        }
        return GlyphType.Composite;
    }

    //  @SuppressWarnings("unchecked")
    //  static <T : Glyph> T getGlyph(
    //      GlyphTable table, ReadableFontData data, int offset, int length) {
    //    Glyph.GlyphType type = Glyph.glyphType(data, offset, length);
    //    if (type == GlyphType.Simple) {
    //      return (T) new SimpleGlyph(data, offset, length);
    //    }
    //    return (T) new CompositeGlyph(data, offset, length);
    //  }

    public static Glyph getGlyph(
        GlyphTable table, ReadableFontData data, int offset, int length)
    {
        Glyph.GlyphType type = Glyph.glyphType(data, offset, length);
        if (type == GlyphType.Simple)
        {
            return new SimpleGlyph(data, offset, length);
        }
        return new CompositeGlyph(data, offset, length);
    }

    public abstract void initialize();


    public override int padding()
    {
        this.initialize();
        return base.padding();
    }

    public Glyph.GlyphType glyphType()
    {
        return this._glyphType;
    }

    /**
     * Gets the number of contours in the glyph. If this returns a number greater
     * than or equal to zero it is the actual number of contours and this is a
     * simple glyph. If there are zero contours in the glyph then none of the
     * other data operations will return usable values. If it -1 then the glyph is
     * a composite glyph.
     * 
     * @return number of contours
     */
    public int numberOfContours()
    {
        return this._numberOfContours;
    }

    public int xMin()
    {
        return this._data.readShort((int)Offset.xMin);
    }

    public int xMax()
    {
        return this._data.readShort((int)Offset.xMax);
    }

    public int yMin()
    {
        return this._data.readShort((int)Offset.yMin);
    }

    public int yMax()
    {
        return this._data.readShort((int)Offset.yMax);
    }

    public abstract int instructionSize();

    public abstract ReadableFontData instructions();

    public override String ToString()
    {
        return this.toString(0);
    }

    public String toString(int length)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.glyphType());
        sb.Append(", contours=");
        sb.Append(this.numberOfContours());
        sb.Append(", [xmin=");
        sb.Append(this.xMin());
        sb.Append(", ymin=");
        sb.Append(this.yMin());
        sb.Append(", xmax=");
        sb.Append(this.xMax());
        sb.Append(", ymax=");
        sb.Append(this.yMax());
        sb.Append("]");
        sb.Append("\n");
        return sb.ToString();
    }

    // TODO(stuartg): interface? need methods from Composite?
    public abstract class Contour
    {
        public Contour()
        {
        }
    }

    public static Glyph.IBuilder<Glyph> getBuilder(GlyphTable.IBuilder tableBuilder, ReadableFontData data)
    {
        return getBuilder(tableBuilder, data, 0, data.length());
    }

    public static Glyph.IBuilder<Glyph> getBuilder(GlyphTable.IBuilder tableBuilder, ReadableFontData data, int offset, int length)
    {
        Glyph.GlyphType type = Glyph.glyphType(data, offset, length);
        if (type == GlyphType.Simple)
        {
            return SimpleGlyph.createBuilder(data, offset, length);
        }
        return CompositeGlyph.createBuilder(data, offset, length);
    }

    new public interface IBuilder<out TGlyph> : SubTable.IBuilder<TGlyph> where TGlyph : Glyph
    {
    }

    new protected abstract class Builder<TGlyph> : SubTable.Builder<TGlyph>, IBuilder<TGlyph> where TGlyph : Glyph
    {
        public int format;

        public Builder(WritableFontData data) : base(data)
        {
        }

        public Builder(ReadableFontData data) : base(data)
        {
        }

        /**
         * @param data
         * @param offset
         * @param length
         */
        public Builder(WritableFontData data, int offset, int length) : this(data.slice(offset, length))
        {
        }

        public override void subDataSet()
        {
            // NOP
        }

        public override int subDataSizeToSerialize()
        {
            return this.internalReadData().length();
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return this.internalReadData().copyTo(newData);
        }
    }
}
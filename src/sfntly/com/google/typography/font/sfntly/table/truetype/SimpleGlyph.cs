using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.truetype.GlyphTable;

namespace com.google.typography.font.sfntly.table.truetype;






public sealed class SimpleGlyph : Glyph
{
    private static readonly int FLAG_ONCURVE = 0x01;
    private static readonly int FLAG_XSHORT = 0x01 << 1;
    private static readonly int FLAG_YSHORT = 0x01 << 2;
    private static readonly int FLAG_REPEAT = 0x01 << 3;
    private static readonly int FLAG_XREPEATSIGN = 0x01 << 4;
    private static readonly int FLAG_YREPEATSIGN = 0x01 << 5;

    private int _instructionSize;
    private int _numberOfPoints;

    // start offsets of the arrays
    private int _instructionsOffset;
    private int _flagsOffset;
    private int _xCoordinatesOffset;
    private int _yCoordinatesOffset;

    private int _flagByteCount;
    private int _xByteCount;
    private int _yByteCount;

    private int[] _xCoordinates;
    private int[] _yCoordinates;
    private boolean[] _onCurve;
    private int[] _contourIndex;

    public class SimpleContour : Glyph.Contour
    {
        public SimpleContour() : base()
        {
        }
    }

    public SimpleGlyph(ReadableFontData data, int offset, int length) : base(data, offset, length, GlyphType.Simple)
    {
    }

    private SimpleGlyph(ReadableFontData data) : base(data, GlyphType.Simple)
    {
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

            if (this.readFontData().length() == 0)
            {
                this._instructionSize = 0;
                this._numberOfPoints = 0;
                this._instructionsOffset = 0;
                this._flagsOffset = 0;
                this._xCoordinatesOffset = 0;
                this._yCoordinatesOffset = 0;
                return;
            }
            this._instructionSize =
                this._data.readUShort((int)Offset.simpleEndPtsOfCountours + this.numberOfContours()
                    * (int)FontData.DataSize.USHORT);
            this._instructionsOffset =
                (int)Offset.simpleEndPtsOfCountours + (this.numberOfContours() + 1)
                    * (int)FontData.DataSize.USHORT;
            this._flagsOffset =
                this._instructionsOffset + this._instructionSize * (int)FontData.DataSize.BYTE;
            this._numberOfPoints = this.contourEndPoint(this.numberOfContours() - 1) + 1;
            this._xCoordinates = new int[this._numberOfPoints];
            this._yCoordinates = new int[this._numberOfPoints];
            this._onCurve = new boolean[this._numberOfPoints];
            parseData(false);
            this._xCoordinatesOffset =
                this._flagsOffset + this._flagByteCount * (int)FontData.DataSize.BYTE;
            this._yCoordinatesOffset =
                this._xCoordinatesOffset + this._xByteCount * (int)FontData.DataSize.BYTE;
            this._contourIndex = new int[this.numberOfContours() + 1];
            _contourIndex[0] = 0;
            for (int contour = 0; contour < this._contourIndex.Length - 1; contour++)
            {
                _contourIndex[contour + 1] = this.contourEndPoint(contour) + 1;
            }
            parseData(true);
            int nonPaddedDataLength =
                5 * (int)FontData.DataSize.SHORT
                    + (this.numberOfContours() * (int)FontData.DataSize.USHORT)
                    + (int)FontData.DataSize.USHORT
                    + (this._instructionSize * (int)FontData.DataSize.BYTE)
                    + (_flagByteCount * (int)FontData.DataSize.BYTE)
                    + (_xByteCount * (int)FontData.DataSize.BYTE)
                    + (_yByteCount * (int)FontData.DataSize.BYTE);
            this.setPadding(this.dataLength() - nonPaddedDataLength);
            this.initialized = true;
        }
    }

    // TODO(stuartg): think about replacing double parsing with ArrayList
    private void parseData(boolean fillArrays)
    {
        int flag = 0;
        int flagRepeat = 0;
        int flagIndex = 0;
        int xByteIndex = 0;
        int yByteIndex = 0;

        for (int pointIndex = 0; pointIndex < this._numberOfPoints; pointIndex++)
        {
            // get the flag for the current point
            if (flagRepeat == 0)
            {
                flag = this.flagAsInt(flagIndex++);
                if ((flag & FLAG_REPEAT) == FLAG_REPEAT)
                {
                    flagRepeat = flagAsInt(flagIndex++);
                }
            }
            else
            {
                flagRepeat--;
            }

            // on the curve?
            if (fillArrays)
            {
                this._onCurve[pointIndex] = ((flag & FLAG_ONCURVE) == FLAG_ONCURVE) ? true : false;
            }
            // get the x coordinate
            if ((flag & FLAG_XSHORT) == FLAG_XSHORT)
            {
                // single byte x coord value
                if (fillArrays)
                {
                    this._xCoordinates[pointIndex] =
                        this._data.readUByte(this._xCoordinatesOffset + xByteIndex);
                    this._xCoordinates[pointIndex] *=
                        ((flag & FLAG_XREPEATSIGN) == FLAG_XREPEATSIGN) ? 1 : -1;
                }
                xByteIndex++;
            }
            else
            {
                // double byte coord value
                if (!((flag & FLAG_XREPEATSIGN) == FLAG_XREPEATSIGN))
                {
                    if (fillArrays)
                    {
                        this._xCoordinates[pointIndex] =
                            this._data.readShort(this._xCoordinatesOffset + xByteIndex);
                    }
                    xByteIndex += 2;
                }
            }
            if (fillArrays && pointIndex > 0)
            {
                this._xCoordinates[pointIndex] += this._xCoordinates[pointIndex - 1];
            }

            // get the y coordinate
            if ((flag & FLAG_YSHORT) == FLAG_YSHORT)
            {
                if (fillArrays)
                {
                    this._yCoordinates[pointIndex] =
                        this._data.readUByte(this._yCoordinatesOffset + yByteIndex);
                    this._yCoordinates[pointIndex] *=
                        ((flag & FLAG_YREPEATSIGN) == FLAG_YREPEATSIGN) ? 1 : -1;
                }
                yByteIndex++;
            }
            else
            {
                if (!((flag & FLAG_YREPEATSIGN) == FLAG_YREPEATSIGN))
                {
                    if (fillArrays)
                    {
                        this._yCoordinates[pointIndex] =
                            this._data.readShort(this._yCoordinatesOffset + yByteIndex);
                    }
                    yByteIndex += 2;
                }
            }
            if (fillArrays && pointIndex > 0)
            {
                this._yCoordinates[pointIndex] += this._yCoordinates[pointIndex - 1];
            }
        }
        this._flagByteCount = flagIndex;
        this._xByteCount = xByteIndex;
        this._yByteCount = yByteIndex;
    }

    private int flagAsInt(int index)
    {
        return this._data.readUByte(this._flagsOffset + index * (int)FontData.DataSize.BYTE);
    }

    public int contourEndPoint(int contour)
    {
        return this._data.readUShort(
            contour * (int)FontData.DataSize.USHORT + (int)Offset.simpleEndPtsOfCountours);
    }

    public override int instructionSize()
    {
        this.initialize();
        return this._instructionSize;
    }

    public override ReadableFontData instructions()
    {
        this.initialize();
        return this._data.slice(this._instructionsOffset, this.instructionSize());
    }

    public int numberOfPoints(int contour)
    {
        this.initialize();
        if (contour >= this.numberOfContours())
        {
            return 0;
        }
        return this._contourIndex[contour + 1] - this._contourIndex[contour];
    }

    public int xCoordinate(int contour, int point)
    {
        this.initialize();
        return this._xCoordinates[this._contourIndex[contour] + point];
    }

    public int yCoordinate(int contour, int point)
    {
        this.initialize();
        return this._yCoordinates[this._contourIndex[contour] + point];
    }

    public boolean onCurve(int contour, int point)
    {
        this.initialize();
        return this._onCurve[this._contourIndex[contour] + point];
    }

    public override String ToString()
    {
        this.initialize();
        StringBuilder sb = new StringBuilder(base.ToString());
        sb.Append("\tinstruction bytes = " + this.instructionSize() + "\n");
        for (int contour = 0; contour < this.numberOfContours(); contour++)
        {
            for (int point = 0; point < this.numberOfPoints(contour); point++)
            {
                sb.Append("\t" + contour + ":" + point + " = [" + this.xCoordinate(contour, point) + ", "
                    + this.yCoordinate(contour, point) + ", " + this.onCurve(contour, point) + "]\n");
            }
        }
        return sb.ToString();
    }

    public static ISimpleGlyphBuilder createBuilder(WritableFontData data, int offset, int length)
    {
        return new SimpleGlyphBuilder(data, offset, length);
    }


    public static ISimpleGlyphBuilder createBuilder(ReadableFontData data, int offset, int length)
    {
        return new SimpleGlyphBuilder(data, offset, length);
    }

    public interface ISimpleGlyphBuilder : Glyph.IBuilder<SimpleGlyph>
    {

    }

    private sealed class SimpleGlyphBuilder : Glyph.Builder<SimpleGlyph>, ISimpleGlyphBuilder
    {
        public SimpleGlyphBuilder(WritableFontData data, int offset, int length) : base(data.slice(offset, length))
        {
        }

        public SimpleGlyphBuilder(ReadableFontData data, int offset, int length) : base(data.slice(offset, length))
        {
        }

        public override SimpleGlyph subBuildTable(ReadableFontData data)
        {
            return new SimpleGlyph(data, 0, data.length());
        }
    }
}
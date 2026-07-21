/*
 * Copyright 2010 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace com.google.typography.font.sfntly.data;

/**
 * An output stream for writing font data.
 *
 * The data types used are as listed:
 * <table>
 * <table>
 * <tr>
 * <td>BYTE</td>
 * <td>8-bit unsigned integer.</td>
 * </tr>
 * <tr>
 * <td>CHAR</td>
 * <td>8-bit signed integer.</td>
 * </tr>
 * <tr>
 * <td>USHORT</td>
 * <td>16-bit unsigned integer.</td>
 * </tr>
 * <tr>
 * <td>SHORT</td>
 * <td>16-bit signed integer.</td>
 * </tr>
 * <tr>
 * <td>UINT24</td>
 * <td>24-bit unsigned integer.</td>
 * </tr>
 * <tr>
 * <td>ULONG</td>
 * <td>32-bit unsigned integer.</td>
 * </tr>
 * <tr>
 * <td>LONG</td>
 * <td>32-bit signed integer.</td>
 * </tr>
 * <tr>
 * <td>Fixed</td>
 * <td>32-bit signed fixed-point number (16.16)</td>
 * </tr>
 * <tr>
 * <td>FUNIT</td>
 * <td>Smallest measurable distance in the em space.</td>
 * </tr>
 * <tr>
 * <td>FWORD</td>
 * <td>16-bit signed integer (SHORT) that describes a quantity in FUnits.</td>
 * </tr>
 * <tr>
 * <td>UFWORD</td>
 * <td>16-bit unsigned integer (USHORT) that describes a quantity in FUnits.
 * </td>
 * </tr>
 * <tr>
 * <td>F2DOT14</td>
 * <td>16-bit signed fixed number with the low 14 bits of fraction (2.14).</td>
 * </tr>
 * <tr>
 * <td>LONGDATETIME</td>
 * <td>Date represented in number of seconds since 12:00 midnight, January 1,
 * 1904. The value is represented as a signed 64-bit integer.</td>
 * </tr>
 * </table>
 *
 * @author Stuart Gill
 * @see FontInputStream
 */
public class FontOutputStream : OutputStream
{
    private readonly OutputStream @out;
    private long _position;

    /**
     * Constructor.
     *
     * @param os output stream to wrap
     */
    public FontOutputStream(OutputStream os)
    {
        @out = os;
    }

    /**
     * Get the current position in the stream in bytes.
     *
     * @return the current position in bytes
     */
    public long position()
    {
        return this._position;
    }

    public override void write(int b)
    {
        @out.write(b);
        this._position++;
    }

    public override void write(byte[] b)
    {
        this.write(b, 0, b.Length);
    }

    public override void write(byte[] b, int off, int len)
    {
        if (off < 0 || len < 0 || off + len < 0 || off + len > b.Length)
        {
            throw new IndexOutOfBoundsException();
        }
        @out.write(b, off, len);
        _position += len;
    }

    /**
     * Write a Char value.
     *
     * @param c Char value
     * @
     */
    public void writeChar(byte c)
    {
        this.write(c);
    }

    /**
     * Write a UShort value.
     *
     * @param us UShort value
     * @
     */
    public void writeUShort(int us)
    {
        this.write((byte)((us >> 8) & 0xff));
        this.write((byte)(us & 0xff));
    }

    /**
     * Write a Short value.
     *
     * @param s Short value
     * @
     */
    public void writeShort(int s)
    {
        this.writeUShort(s);
    }

    /**
     * Write a UInt24 value.
     *
     * @param ui UInt24 value
     * @
     */
    public void writeUInt24(int ui)
    {
        this.write((byte)((ui >> 16) & 0xff));
        this.write((byte)((ui >> 8) & 0xff));
        this.write((byte)(ui & 0xff));
    }

    /**
     * Write a ULong value.
     *
     * @param ul ULong value
     * @
     */
    public void writeULong(long ul)
    {
        this.write((byte)((ul >> 24) & 0xff));
        this.write((byte)((ul >> 16) & 0xff));
        this.write((byte)((ul >> 8) & 0xff));
        this.write((byte)(ul & 0xff));
    }

    /**
     * Write a Long value.
     *
     * @param l Long value
     * @
     */
    public void writeLong(long l)
    {
        this.writeULong(l);
    }

    /**
     * Write a Fixed value.
     *
     * @param f Fixed value
     * @
     */
    public void writeFixed(int f)
    {
        this.writeULong(f);
    }

    /**
     * Write DateTime value.
     *
     * @param date date/time value
     * @
     */
    public void writeDateTime(long date)
    {
        this.writeULong((date >> 32) & 0xffffffff);
        this.writeULong(date & 0xffffffff);
    }
}
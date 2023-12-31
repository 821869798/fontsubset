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
 * An input stream for reading font data.
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
 * @see FontOutputStream
 */
public class FontInputStream : FilterInputStream {
  private long _position;
  private long _length;  // bound on length of data to read
  private boolean _bounded;

  /**
   * Constructor.
   *
   * @param is input stream to wrap
   */
  public FontInputStream(InputStream @is):base(@is) {
    
  }

  /**
   * Constructor for a bounded font input stream.
   *
   * @param is input stream to wrap
   * @param length the maximum length of bytes to read
   */
  public FontInputStream(InputStream @is, int length): this(@is) {
    
    this._length = length;
    this._bounded = true;
  }

  public override int read() {
    if (this._bounded && this._position >= this._length) {
      return -1;
    }
    int b = base.read();
    if (b >= 0) {
      this._position++;
    }
    return b;
  }

  public int read(byte[] b, int off, int len) {
    if (this._bounded && this._position >= this._length) {
      return -1;
    }
    int bytesToRead = _bounded ? (int) Math.Min(len, this._length - this._position) : len;
    int bytesRead = base.read(b, off, bytesToRead);
    this._position += bytesRead;
    return bytesRead;
  }

  public override int read(byte[] b)  {
    return this.read(b, 0, b.Length);
  }

  /**
   * Get the current position in the stream in bytes.
   *
   * @return the current position in bytes
   */
  public long position() {
    return this._position;
  }

  /**
   * Read a Char value.
   *
   * @return Char value
   * @
   */
  public int readChar()  {
    return this.read();
  }

  /**
   * Read a UShort value.
   *
   * @return UShort value
   * @
   */
  public int readUShort()  {
    return 0xffff & (this.read() << 8 | this.read());
  }

  /**
   * Read a Short value.
   *
   * @return Short value
   * @
   */
  public int readShort()  {
    return ((this.read() << 8 | this.read()) << 16) >> 16;
  }

  /**
   * Read a UInt24 value.
   *
   * @return UInt24 value
   * @
   */
  public int readUInt24()  {
    return 0xffffff & (this.read() << 16 | this.read() << 8 | this.read());
  }

  /**
   * Read a ULong value.
   *
   * @return ULong value
   * @
   */
  public long readULong()  {
    return 0xffffffffL & this.readLong();
  }

  /**
   * Read a ULong value as an int. If the value is not representable as an
   * integer an <code>ArithmeticException</code> is thrown.
   *
   * @return Ulong value
   * @
   * @throws ArithmeticException
   */
  public int readULongAsInt()  {
    long @ulong = this.readULong();
    if ((@ulong & 0x80000000) == 0x80000000) {
      throw new ArithmeticException("Long value too large to fit into an integer.");
    }
    return (int)(((int)@ulong) & ~0x80000000);
  }

  /**
   * Read a Long value.
   *
   * @return Long value
   * @
   */
  public int readLong()  {
    return this.read() << 24 | this.read() << 16 | this.read() << 8 | this.read();
  }

  /**
   * Read a Fixed value.
   *
   * @return Fixed value
   * @
   */
  public int readFixed()  {
    return this.readLong();
  }

  /**
   * Read a DateTime value as a long.
   *
   * @return DateTime value.
   * @
   */
  public long readDateTimeAsLong()  {
    return this.readULong() << 32 | this.readULong() ;
  }

  public override long skip(long n)  {
    long skipped = base.skip(n);
    this._position += skipped;
    return skipped;
  }
}

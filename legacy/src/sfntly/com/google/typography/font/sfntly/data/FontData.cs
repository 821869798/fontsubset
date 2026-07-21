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
 * An abstract base for font data in the TrueType / OpenType spec.
 * 
 * @author Stuart Gill
 */
public abstract class FontData
{

    public readonly static int GROWABLE_SIZE = Integer.MaxValue;

    /**
     * Note: Enum names intended to match the names used in the OpenType and sfnt specs.
     */
    public enum DataSize
    {
        BYTE = (1),
        CHAR = (1),
        USHORT = (2),
        SHORT = (2),
        UINT24 = (3),
        ULONG = (4),
        LONG = (4),
        Fixed = (4),
        FUNIT = (4),
        FWORD = (2),
        UFWORD = (2),
        F2DOT14 = (2),
        LONGDATETIME = (8),
        Tag = (4),
        GlyphID = (2),
        Offset = (2)/*;

    public readonly int _size;
    
    private DataSize(int size) {
      this._size = size;
    }

    public int size() {
      return this._size;
    }*/
    }

    /**
     * The public data.
     */
    public readonly ByteArray array;
    /**
     * Offset to apply as a lower bound on the public byte array.
     */
    private int _boundOffset;
    /**
     * The length of the bound on the public byte array.
     */
    private int _boundLength = FontData.GROWABLE_SIZE;

    /**
     * Constructor.
     *
     * @param ba the byte array to use for the backing data
     */
    public FontData(ByteArray ba)
    {
        this.array = ba;
    }

    /**
     * Constructor.
     *
     * @param data the data to wrap
     * @param (int)Offset.the to start the wrap from
     * @param length the length of the data wrapped
     */
    public FontData(FontData data, int offset, int length) : this(data.array)
    {
        this.bound(data._boundOffset + offset, length);
    }

    /**
     * Constructor.
     *
     * @param data the data to wrap
     * @param (int)Offset.the to start the wrap from
     */
    public FontData(FontData data, int offset) : this(data.array)
    {
        this.bound(data._boundOffset + offset,
            data._boundLength == FontData.GROWABLE_SIZE ? FontData.GROWABLE_SIZE : data._boundLength
                - offset);
    }

    /**
     * Sets limits on the size of the FontData. The FontData is then only 
     * visible within the bounds set.
     *
     * @param offset the start of the new bounds
     * @param length the number of bytes in the bounded array
     * @return true if the bounding range was successful; false otherwise
     */
    public boolean bound(int offset, int length)
    {
        if ((offset + length > this.size()) || offset < 0 || length < 0)
        {
            return false;
        }
        this._boundOffset += offset;
        this._boundLength = length;
        return true;
    }

    /**
     * Sets limits on the size of the FontData. This is an offset bound only so if
     * the FontData is writable and growable then there is no limit to that growth
     * from the bounding operation.
     *
     * @param offset the start of the new bounds which must be within the current
     *        size of the FontData
     * @return true if the bounding range was successful; false otherwise
     */
    public boolean bound(int offset)
    {
        if (offset > this.size() || offset < 0)
        {
            return false;
        }
        this._boundOffset += offset;
        return true;
    }

    /**
     * Makes a slice of this FontData. The returned slice will share the data with
     * the original <code>FontData</code>.
     *
     * @param offset the start of the slice
     * @param length the number of bytes in the slice
     * @return a slice of the original FontData
     */
    public FontData slice(int offset, int length)
    {
        return InternalSlice(offset, length);
    }

    /**
     * Makes a bottom bound only slice of this array. The returned slice will
     * share the data with the original <code>FontData</code>.
     *
     * @param offset the start of the slice
     * @return a slice of the original FontData
     */
    public FontData slice(int offset)
    {
        return InternalSlice(offset);
    }

    protected abstract FontData InternalSlice(int offset);

    protected abstract FontData InternalSlice(int offset, int length);

    /**
     * Gets the length of the data.
     *
     * @return the length of the data
     */
    public int length()
    {
        return Math.Min(this.array.length() - this._boundOffset, this._boundLength);
    }

    /**
     * Gets the maximum size of the FontData. This is the maximum number of bytes
     * that the font data can hold and all of it may not be filled with data or
     * even fully allocated yet.
     *
     * @return the maximum size of this font data
     */
    public int size()
    {
        return Math.Min(this.array.size() - this._boundOffset, this._boundLength);
    }

    /**
     * Returns the offset in the underlying data taking into account any bounds on
     * the data.
     */
    public int dataOffset()
    {
        return this._boundOffset;
    }

    /**
     * Gets the offset in the underlying data taking into account any bounds on
     * the data.
     *
     * @param offset
     *          the offset to get the bound compensated offset for
     * @return the bound compensated offset
     */
    public int boundOffset(int offset)
    {
        return offset + this._boundOffset;
    }

    /**
     * Gets the length in the underlying data taking into account any bounds on the data.
     * 
     * @param (int)Offset.the that the length is being used at
     * @param length the length to get the bound compensated length for
     * @return the bound compensated length
     */
    public int boundLength(int offset, int length)
    {
        return Math.Min(length, this._boundLength - offset);
    }

    public boolean boundsCheck(int offset, int length)
    {
        if (offset < 0 || offset >= this._boundLength)
        {
            return false;
        }
        if (length < 0 || length + offset > this._boundLength)
        {
            return false;
        }
        return true;
    }
}
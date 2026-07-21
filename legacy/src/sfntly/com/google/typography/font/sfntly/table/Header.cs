/*
 * Copyright 2011 Google Inc. All Rights Reserved.
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

namespace com.google.typography.font.sfntly.table;

/**
 * The header entry for a table in the OffsetTable for the font.
 *
 * For equality purposes the only property of the header that is considered is
 * the tag - the name of the table that is referred to by this header. There can
 * only be one table with each tag in the font and it doesn't matter what the
 * other properties of that header are for that purpose.
 *
 * @author Stuart Gill
 *
 */
public sealed class Header
{
    private readonly int _tag;
    private readonly int _offset;
    private readonly boolean _offsetValid;
    private readonly int _length;
    private readonly boolean _lengthValid;
    private readonly long _checksum;
    private readonly boolean _checksumValid;

    public static IComparer<Header> COMPARATOR_BY_OFFSET = new FuncComparer<Header>((h1, h2) =>
    {
        return h1._offset - h2._offset;
    });
    /*{
    public override int compare(Header h1, Header h2)
    {
        return h1.offset - h2.offset;
    }
};*/

    public static IComparer<Header> COMPARATOR_BY_TAG = new FuncComparer<Header>((h1, h2) =>
    {
        return h1._tag - h2._tag;
    });/*
{
    public override int compare(Header h1, Header h2)
{
    return h1.tag - h2.tag;
}
  };*/

    /**
     * Constructor.
     *
     * Make a full header as read from an existing font.
     *
     * @param tag
     * @param offset
     * @param length
     * @param checksum
     */
    public Header(int tag, long checksum, int offset, int length)
    {
        this._tag = tag;
        this._checksum = checksum;
        this._checksumValid = true;
        this._offset = offset;
        this._offsetValid = true;
        this._length = length;
        this._lengthValid = true;
    }

    /**
     * Constructor.
     *
     * Make a partial header with only the basic info for a new table.
     *
     * @param tag
     * @param length
     */
    public Header(int tag, int length)
    {
        this._tag = tag;
        this._checksum = 0;
        this._checksumValid = false;
        this._offset = 0;
        this._offsetValid = false;
        this._length = length;
        this._lengthValid = true;
    }

    /**
     * Constructor.
     *
     * Make a partial header with only the basic info for an empty new table.
     *
     * @param tag
     */
    public Header(int tag)
    {
        this._tag = tag;
        this._checksum = 0;
        this._checksumValid = false;
        this._offset = 0;
        this._offsetValid = false;
        this._length = 0;
        this._lengthValid = true;
    }

    /**
     * Get the table tag.
     *
     * @return the tag
     */
    public int tag()
    {
        return _tag;
    }

    /**
     * Get the table offset. The offset is from the start of the font file. This
     * offset value is what was read from the font file during construction of the
     * font. It may not be meaningful if the font was maninpulated through the
     * builders.
     *
     * @return the offset
     */
    public int offset()
    {
        return _offset;
    }

    /**
     * Is the offset in the header valid. The offset will not be valid if the
     * table was constructed during building and has no physical location in a
     * font file.
     *
     * @return true if the offset is valid; false otherwise
     */
    public boolean offsetValid()
    {
        return _offsetValid;
    }

    /**
     * Get the length of the table as recorded in the table record header. During
     * building the header length will reflect the length that was initially read
     * from the font file. This may not be consistent with the current state of
     * the data.
     *
     * @return the length
     */
    public int length()
    {
        return _length;
    }

    /**
     * Is the length in the header valid. The length will not be valid if the
     * table was constructed during building and has no physical location in a
     * font file until the table is built from the builder.
     *
     * @return true if the offset is valid; false otherwise
     */
    public boolean lengthValid()
    {
        return _lengthValid;
    }

    /**
     * Get the checksum for the table as recorded in the table record header.
     *
     * @return the checksum
     */
    public long checksum()
    {
        return _checksum;
    }

    /**
     * Is the checksum valid. The checksum will not be valid if the table was
     * constructed during building and has no physical location in a font file.
     * Note that this does <b>not</b> check the validity of the checksum against
     * the calculated checksum for the table data.
     *
     * @return true if the checksum is valid; false otherwise
     */
    public boolean checksumValid()
    {
        return _checksumValid;
    }

    /**
     * Checks equality of this Header against another object. The only property of
     * the Header object that is considered is the tag.
     */
    public override boolean Equals(Object obj)
    {
        if (!(obj is Header))
        {
            return false;
        }
        return ((Header)obj)._tag == this._tag;
    }

    /**
     * Computes the hashcode for this Header . The only property of the Header
     * object that is considered is the tag.
     */
    public override int GetHashCode()
    {
        return this._tag;
    }

    public override String ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[");
        builder.Append(Tag.stringValue(this._tag));
        builder.Append(", ");
        builder.Append(NumberHelper.toHexString(this._checksum));
        builder.Append(", ");
        builder.Append(NumberHelper.toHexString(this._offset));
        builder.Append(", ");
        builder.Append(NumberHelper.toHexString(this._length));
        builder.Append("]");
        return builder.ToString();
    }
}
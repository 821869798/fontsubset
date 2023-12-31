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

using com.google.typography.font.sfntly.data;
using System.Reflection.PortableExecutable;

namespace com.google.typography.font.sfntly.table;

public interface IByteArrayTableBuilder<T> : ITableBasedTableBuilder<T> where T : Table
{

}

/**
 * An abstract builder base for byte array based tables.
 *
 * @author Stuart Gill
 *
 */
internal abstract class ByteArrayTableBuilder<T> : TableBasedTableBuilder<T> where T : Table
{

    /**
     * Constructor.
     *
     * @param header
     * @param data
     */
    public ByteArrayTableBuilder(Header header, WritableFontData data) : base(header, data)
    {

    }

    /**
     * Constructor.
     *
     * @param header
     * @param data
     */
    public ByteArrayTableBuilder(Header header, ReadableFontData data) : base(header, data)
    {

    }

    /**
     * Get the byte value at the specified index. The index is relative to the
     * start of the table.
     *
     * @param index index relative to the start of the table
     * @return byte value at the given index
     * @
     */
    public int byteValue(int index)
    {
        ReadableFontData data = this.internalReadData();
        if (data == null)
        {
            throw new IOException("No font data for the table.");
        }
        return data.readByte(index);
    }

    /**
     * Get the byte value at the specified index. The index is relative to the
     * start of the table.
     *
     * @param index index relative to the start of the table
     * @param b byte value to tset
     * @
     */
    public void setByteValue(int index, byte b)
    {
        WritableFontData data = this.internalWriteData();
        if (data == null)
        {
            throw new IOException("No font data for the table.");
        }
        data.writeByte(index, b);
    }

    /**
     * Get the number of bytes set for this table. It may include padding bytes at
     * the end.
     *
     * @return number of bytes for the table
     * @
     */
    public int byteCount()
    {
        ReadableFontData data = this.internalReadData();
        if (data == null)
        {
            throw new IOException("No font data for the table.");
        }
        return data.length();
    }
}
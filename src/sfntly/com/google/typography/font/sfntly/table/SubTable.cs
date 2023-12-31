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

using com.google.typography.font.sfntly.data;

/**
 * An abstract base class for subtables. Subtables are smaller tables nested
 * within other tables and don't have an entry in the main font index. Examples
 * of these are the CMap subtables within CMap table (cmap) or a glyph within
 * the glyph table (glyf).
 *
 * @author Stuart Gill
 *
 */
public abstract class SubTable : FontDataTable
{
    /**
     * The data for the whole table in which this subtable is contained.
     */
    private readonly ReadableFontData masterData;

    private int _padding = 0;

    /**
     * Constructor.
     *
     * @param data the data representing the subtable
     * @param masterData the data representing the full table containing this
     *        subtable
     */
    public SubTable(ReadableFontData data, ReadableFontData masterData) : base(data)
    {

        this.masterData = masterData;
    }

    /**
     * Constructor.
     *
     * @param data the data representing the subtable
     */
    public SubTable(ReadableFontData data) : this(data, null)
    {

    }

    /**
     * Constructor.
     *
     * @param data the data object that contains the subtable
     * @param (int)Offset.the within the data where the subtable starts
     * @param length the length of the subtable data within the data object
     */
    public SubTable(ReadableFontData data, int offset, int length) : this(data.slice(offset, length))
    {

    }

    public ReadableFontData masterReadData()
    {
        return this.masterData;
    }

    new public interface IBuilder<out TSubTable> : FontDataTable.IBuilder<TSubTable> where TSubTable : SubTable
    {

    }

    /**
     * An abstract base class for subtable builders.
     *
     * @param <T> the type of the subtable
     */
    new protected abstract class Builder<TSubTable> : FontDataTable.Builder<TSubTable>, IBuilder<TSubTable> where TSubTable : SubTable
    {
        private ReadableFontData masterData;

        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         * @param masterData the data for the full table
         */
        public Builder(WritableFontData data, ReadableFontData masterData) : base(data)
        {

            this.masterData = masterData;
        }

        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         * @param masterData the data for the full table
         */
        public Builder(ReadableFontData data, ReadableFontData masterData) : base(data)
        {

            this.masterData = masterData;
        }

        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         */
        public Builder(WritableFontData data) : base(data)
        {

        }

        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         */
        public Builder(ReadableFontData data) : base(data)
        {

        }

        /**
         * Constructor.
         *
         * Creates a new empty sub-table.
         *
         * @param dataSize the initial size for the data; if it is positive then the
         *        size is fixed; if it is negative then it is variable sized
         */
        public Builder(int dataSize) : base(dataSize)
        {

        }

        public ReadableFontData masterReadData()
        {
            return this.masterData;
        }
    }

    /**
     * Get the number of bytes of padding used in the table. The padding bytes are
     * used to align the table length to a 4 byte boundary.
     *
     * @return the number of padding bytes
     */
    public virtual int padding()
    {
        return this._padding;
    }

    /**
     * Sets the amount of padding that is part of the data being used by this
     * subtable.
     *
     * @param padding
     */
    // TODO(stuartg): move to constructor
    public void setPadding(int padding)
    {
        this._padding = padding;
    }
}
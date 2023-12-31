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

using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.core.FontHeaderTable;

namespace com.google.typography.font.sfntly.table.truetype;












/**
 * A Loca table - 'loca'.
 *
 * @author Stuart Gill
 */
public sealed class LocaTable : Table
{

    private IndexToLocFormat _version;
    private int _numGlyphs;

    private LocaTable(Header header, ReadableFontData data, IndexToLocFormat version, int numGlyphs) : base(header, data)
    {
        this._version = version;
        this._numGlyphs = numGlyphs;
    }

    /**
     * Get the table version.
     *
     * @return the table version
     */
    public IndexToLocFormat formatVersion()
    {
        return this._version;
    }

    public int numGlyphs()
    {
        return this._numGlyphs;
    }

    /**
     * Return the offset for the given glyph id. Valid glyph ids are from 0 to the
     * one less than the number of glyphs. The zero entry is the special entry for
     * the notdef glyph. The final entry beyond the last glyph id is used to
     * calculate the size of the last glyph.
     *
     * @param glyphId the glyph id to get the offset for; must be less than or
     *        equal to one more than the number of glyph ids
     * @return the offset in the glyph table to the specified glyph id
     */
    public int glyphOffset(int glyphId)
    {
        if (glyphId < 0 || glyphId >= this._numGlyphs)
        {
            throw new IndexOutOfBoundsException("Glyph ID is out of bounds.");
        }
        return this.loca(glyphId);
    }

    /**
     * Get the length of the data in the glyph table for the specified glyph id.
     * @param glyphId the glyph id to get the offset for; must be greater than or
     *        equal to 0 and less than the number of glyphs in the font
     * @return the length of the data in the glyph table for the specified glyph id
     */
    public int glyphLength(int glyphId)
    {
        if (glyphId < 0 || glyphId >= this._numGlyphs)
        {
            throw new IndexOutOfBoundsException("Glyph ID is out of bounds.");
        }
        return this.loca(glyphId + 1) - this.loca(glyphId);
    }

    /**
     * Get the number of locations or locas. This will be one more than the number
     * of glyphs for this table since the last loca position is used to indicate
     * the size of the final glyph.
     *
     * @return the number of locas
     */
    public int numLocas()
    {
        return this._numGlyphs + 1;
    }

    /**
     * Get the value from the loca table for the index specified. These are the
     * raw values from the table that are used to compute the offset and size of a
     * glyph in the glyph table. Valid index values run from 0 to the number of
     * glyphs in the font.
     *
     * @param index the loca table index
     * @return the loca table value
     */
    public int loca(int index)
    {
        if (index > this._numGlyphs)
        {
            throw new IndexOutOfBoundsException();
        }
        if (this._version == IndexToLocFormat.shortOffset)
        {
            return 2 * this._data.readUShort(index * (int)FontData.DataSize.USHORT);
        }
        return this._data.readULongAsInt(index * (int)FontData.DataSize.ULONG);
    }

    /**
     * Get an iterator over the loca values for the table. The iterator returned
     * does not support the delete operation.
     *
     * @return loca iterator
     * @see #loca
     */
    public IEnumerator<Integer> GetEnumerator()
    {
        //return new LocaIterator();
        return Enumerable.Range(0, _numGlyphs).Select(loca).GetEnumerator();
    }

    ///**
    // * Iterator over the raw loca values.
    // *
    // */
    //private sealed class LocaIterator : IEnumerator<Integer> {
    //  int index;

    //  private LocaIterator() {
    //  }

    //  public override boolean hasNext() {
    //    if (this.index <= _numGlyphs) {
    //      return true;
    //    }
    //    return false;
    //  }

    //  public override Integer next() {
    //    return loca(index++);
    //  }

    //  public override void remove() {
    //    throw new UnsupportedOperationException();
    //  }
    //}

    /**
     * Builder for a loca table.
     *
     */

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : Table.IBuilder<LocaTable>
    {
        int numGlyphs();
        void setNumGlyphs(int numGlyphs);
        void setFormatVersion(IndexToLocFormat formatVersion);
        void setLocaList(IList<int> locaList);
    }

    private class Builder : Table.Builder<LocaTable>, IBuilder
    {

        // values that need to be set to properly passe an existing loca table
        private IndexToLocFormat _formatVersion = IndexToLocFormat.longOffset;
        private int _numGlyphs = -1;

        // parsed loca table
        private IList<Integer> _loca;


        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        /**
         * Initialize the public state from the data. Done lazily since in many
         * cases the builder will be just creating a table object with no parsing
         * required.
         *
         * @param data the data to initialize from
         */
        private void initialize(ReadableFontData data)
        {
            this.clearLoca(false);
            if (this._loca == null)
            {
                this._loca = new List<Integer>();
            }
            if (data != null)
            {
                if (this._numGlyphs < 0)
                {
                    throw new IllegalStateException("numglyphs not set on LocaTable Builder.");
                }

                LocaTable table = new LocaTable(this.header(), data, this._formatVersion, this._numGlyphs);
                IEnumerator<Integer> locaIter = table.GetEnumerator();
                while (locaIter.MoveNext())
                {
                    this._loca.Add(locaIter.Current);
                }
            }
        }

        /**
         * Checks that the glyph id is within the correct range.
         *
         * @param glyphId
         * @return the glyphId
         * @throws IndexOutOfBoundsException if the glyph id is not within the
         *         correct range
         */
        private int checkGlyphRange(int glyphId)
        {
            if (glyphId < 0 || glyphId > this.lastGlyphIndex())
            {
                throw new IndexOutOfBoundsException("Glyph ID is outside of the allowed range.");
            }
            return glyphId;
        }

        private int lastGlyphIndex()
        {
            return this.loca != null ? _loca.Count - 2 : this._numGlyphs - 1;
        }

        /**
         * Internal method to get the loca list if already generated and if not to
         * initialize the state of the builder.
         *
         * @return the loca list
         */
        private IList<Integer> getLocaList()
        {
            if (this.loca == null)
            {
                this.initialize(this.internalReadData());
                this.setModelChanged();
            }
            return this._loca;
        }

        private void clearLoca(boolean nullify)
        {
            if (this.loca != null)
            {
                this._loca.Clear();
            }
            if (nullify)
            {
                this._loca = null;
            }
            this.setModelChanged(false);
        }

        /**
         * Get the format version that will be used when the loca table is
         * generated.
         *
         * @return the loca table format version
         */
        public IndexToLocFormat formatVersion()
        {
            return this._formatVersion;
        }

        /**
         * Set the format version to be used when generating the loca table.
         *
         * @param formatVersion
         */
        public void setFormatVersion(IndexToLocFormat formatVersion)
        {
            this._formatVersion = formatVersion;
        }

        /**
         * Gets the List of locas for loca table builder. These may be manipulated
         * in any way by the caller and the changes will be reflected in the final
         * loca table produced as long as no subsequent call is made to the
         * {@link #setLocaList(List)} method.
         *
         *  If there is no current data for the loca table builder or the loca list
         * have not been previously set then this will return an empty List.
         *
         * @return the list of glyph builders
         * @see #setLocaList(List)
         */
        public IList<Integer> locaList()
        {
            return this.getLocaList();
        }

        /**
         * Set the list of locas to be used for building this table. If any existing
         * list was already retrieved with the {@link #locaList()} method then the
         * connection of that previous list to this builder will be broken.
         *
         * @param list
         * @see #locaList()
         */
        public void setLocaList(IList<Integer> list)
        {
            this._loca = list;
            this.setModelChanged();
        }

        /**
         * Return the offset for the given glyph id. Valid glyph ids are from 0 to
         * one more than the number of glyphs. The zero entry is the special entry
         * for the notdef glyph. The final entry beyond the last glyph id is used to
         * calculate the size of the last glyph.
         *
         * @param glyphId the glyph id to get the offset for; must be less than or
         *        equal to one more than the number of glyph ids
         * @return the offset in the glyph table to the specified glyph id
         */
        public int glyphOffset(int glyphId)
        {
            this.checkGlyphRange(glyphId);
            return this.getLocaList().get(glyphId);
        }

        /**
         * Get the length of the data in the glyph table for the specified glyph id.
         * This is a convenience method that uses the specified glyph id
         *
         * @param glyphId the glyph id to get the offset for; must be less than or
         *        equal to the number of glyphs
         * @return the length of the data in the glyph table for the specified glyph
         *         id
         */
        public int glyphLength(int glyphId)
        {
            this.checkGlyphRange(glyphId);
            return this.getLocaList().get(glyphId + 1) - this.getLocaList().get(glyphId);
        }

        /**
         * Set the number of glyphs.
         *
         *  This method sets the number of glyphs that the builder will attempt to
         * parse location data for from the raw binary data. This method only needs
         * to be called (and <b>must</b> be) when the raw data for this builder has
         * been changed. It does not by itself reset the data or clear any set loca
         * list.
         *
         * @param numGlyphs the number of glyphs represented by the data
         */
        public void setNumGlyphs(int numGlyphs)
        {
            this._numGlyphs = numGlyphs;
        }

        /**
         * Get the number of glyphs that this builder has support for.
         *
         * @return the number of glyphs.
         */
        public int numGlyphs()
        {
            return this.lastGlyphIndex() + 1;
        }

        /**
         * Revert the loca table builder to the state contained in the last raw data
         * set on the builder. That raw data may be that read from a font file when
         * the font builder was created, that set by a user of the loca table
         * builder, or null data if this builder was created as a new empty builder.
         */
        public void revert()
        {
            this._loca = null;
            this.setModelChanged(false);
        }

        /**
         * Get the number of locations or locas. This will be one more than the
         * number of glyphs for this table since the last loca position is used to
         * indicate the size of the final glyph.
         *
         * @return the number of locas
         */
        public int numLocas()
        {
            return this.getLocaList().Count;
        }

        /**
         * Get the value from the loca table for the index specified. These are the
         * raw values from the table that are used to compute the offset and size of
         * a glyph in the glyph table. Valid index values run from 0 to the number
         * of glyphs in the font.
         *
         * @param index the loca table index
         * @return the loca table value
         */
        public int loca(int index)
        {
            return this.getLocaList().get(index);
        }

        public override LocaTable subBuildTable(ReadableFontData data)
        {
            return new LocaTable(this.header(), data, this._formatVersion, this._numGlyphs);
        }

        public override void subDataSet()
        {
            this.initialize(this.internalReadData());
        }

        public override int subDataSizeToSerialize()
        {
            if (this._loca == null)
            {
                return 0;
            }
            if (this._formatVersion == IndexToLocFormat.longOffset)
            {
                return this._loca.Count * (int)FontData.DataSize.ULONG;
            }
            return this._loca.Count * (int)FontData.DataSize.USHORT;
        }

        public override boolean subReadyToSerialize()
        {
            return this.loca != null;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = 0;
            foreach (int l in this._loca)
            {
                if (this._formatVersion == IndexToLocFormat.longOffset)
                {
                    size += newData.writeULong(size, l);
                }
                else
                {
                    size += newData.writeUShort(size, l / 2);
                }
            }
            this._numGlyphs = this._loca.Count - 1;
            return size;
        }
    }
}
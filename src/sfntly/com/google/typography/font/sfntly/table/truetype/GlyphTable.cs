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

namespace com.google.typography.font.sfntly.table.truetype;









/**
 * A Glyph table.
 *
 * @author Stuart Gill
 */
public sealed class GlyphTable : SubTableContainerTable
{

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    public enum Offset
    {
        // header
        numberOfContours = (0),
        xMin = (2),
        yMin = (4),
        xMax = (6),
        yMax = (8),

        // Simple Glyph Description
        simpleEndPtsOfCountours = (10),
        // offset from the end of the contours array
        simpleInstructionLength = (0),
        simpleInstructions = (2),
        // flags
        // xCoordinates
        // yCoordinates

        // Composite Glyph Description
        compositeFlags = (0),
        compositeGyphIndexWithoutFlag = (0),
        compositeGlyphIndexWithFlag = (2)/*;

     readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    private GlyphTable(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public Glyph glyph(int offset, int length)
    {
        return Glyph.getGlyph(this, this._data, offset, length);
    }

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

    public interface IBuilder : SubTableContainerTable.IBuilder<GlyphTable>
    {
        IList<int> generateLocaList();
        Glyph.IBuilder<Glyph> glyphBuilder(ReadableFontData data);
        IList<Glyph.IBuilder<Glyph>> glyphBuilders();
    }

    private class Builder : SubTableContainerTable.Builder<GlyphTable>, IBuilder
    {

        private IList<Glyph.IBuilder<Glyph>> _glyphBuilders;
        private IList<Integer> _loca;

        /**
         * Constructor.
         *
         * @param header the table header
         * @param data the data for the table
         */
        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        /**
         * Constructor.
         *
         * @param header the table header
         * @param data the data for the table
         */
        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        // glyph table level building

        public void setLoca(IList<Integer> loca)
        {
            this._loca = new System.Collections.Generic.List<Integer>(loca);
            this.setModelChanged(false);
            this._glyphBuilders = null;
        }

        /**
         * Generate a loca table list from the current state of the glyph table
         * builder.
         *
         * @return a list of loca information for the glyphs
         */
        public IList<Integer> generateLocaList()
        {
            IList<Integer> locas = new List<Integer>(this.getGlyphBuilders().Count);
            locas.Add(0);
            if (this.getGlyphBuilders().Count == 0)
            {
                locas.Add(0);
            }
            else
            {
                int total = 0;
                foreach (var b in this.getGlyphBuilders())
                {
                    int size = b.subDataSizeToSerialize();
                    locas.Add(total + size);
                    total += size;
                }
            }
            return locas;
        }

        private void initialize(ReadableFontData data, IList<Integer> loca)
        {
            this._glyphBuilders = new List<Glyph.IBuilder<Glyph>>();

            if (data != null)
            {
                int locaValue;
                int lastLocaValue = loca.get(0);
                for (int i = 1; i < loca.Count; i++)
                {
                    locaValue = loca.get(i);
                    this._glyphBuilders.Add(Glyph.getBuilder(this, data, lastLocaValue /* offset */,
                        locaValue - lastLocaValue /* length */));
                    lastLocaValue = locaValue;
                }
            }
        }

        private IList<Glyph.IBuilder<Glyph>> getGlyphBuilders()
        {
            if (this._glyphBuilders == null)
            {
                if (this.internalReadData() != null && this._loca == null)
                {
                    throw new IllegalStateException("Loca values not set - unable to parse glyph data.");
                }
                this.initialize(this.internalReadData(), this._loca);
                this.setModelChanged();
            }
            return this._glyphBuilders;
        }

        public void revert()
        {
            this._glyphBuilders = null;
            this.setModelChanged(false);
        }

        /**
         * Gets the List of glyph builders for the glyph table builder. These may be
         * manipulated in any way by the caller and the changes will be reflected in
         * the final glyph table produced.
         *
         *  If there is no current data for the glyph builder or the glyph builders
         * have not been previously set then this will return an empty glyph builder
         * List. If there is current data (i.e. data read from an existing font) and
         * the <code>loca</code> list has not been set or is null, empty, or
         * invalid, then an empty glyph builder List will be returned.
         *
         * @return the list of glyph builders
         */
        public IList<Glyph.IBuilder<Glyph>> glyphBuilders()
        {
            return this.getGlyphBuilders();
        }

        /**
         * Replace the public glyph builders with the one provided. The provided
         * list and all contained objects belong to this builder.
         *
         *  This call is only required if the entire set of glyphs in the glyph
         * table builder are being replaced. If the glyph builder list provided from
         * the {@link GlyphTable.Builder#glyphBuilders()} is being used and modified
         * then those changes will already be reflected in the glyph table builder.
         *
         * @param glyphBuilders the new glyph builders
         */
        public void setGlyphBuilders(IList<Glyph.IBuilder<Glyph>> glyphBuilders)
        {
            this._glyphBuilders = glyphBuilders;
            this.setModelChanged();
        }

        // glyph builder factories

        public Glyph.IBuilder<Glyph> glyphBuilder(ReadableFontData data)
        {
            Glyph.IBuilder<Glyph> glyphBuilder = Glyph.getBuilder(this, data);
            return glyphBuilder;
        }


        // public API for building

        public override GlyphTable subBuildTable(ReadableFontData data)
        {
            return new GlyphTable(this.header(), data);
        }

        public override void subDataSet()
        {
            this._glyphBuilders = null;
            base.setModelChanged(false);
        }

        public override int subDataSizeToSerialize()
        {
            if (this._glyphBuilders == null || this._glyphBuilders.Count == 0)
            {
                return 0;
            }

            boolean variable = false;
            int size = 0;

            // calculate size of each table
            foreach (var b in this._glyphBuilders)
            {
                int glyphSize = b.subDataSizeToSerialize();
                size += Math.Abs(glyphSize);
                variable |= glyphSize <= 0;
            }
            return variable ? -size : size;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.glyphBuilders == null)
            {
                return false;
            }
            // TODO(stuartg): check glyphs for ready to build?
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = 0;
            foreach (var b in this._glyphBuilders)
            {
                size += b.subSerialize(newData.slice(size));
            }
            return size;
        }
    }
}
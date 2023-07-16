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

namespace com.google.typography.font.sfntly.table.bitmap;













/**
 * @author Stuart Gill
 *
 */
public sealed class EbdtTable : SubTableContainerTable
{
    public enum Offset
    {
        // header
        version = (0), headerLength = (FontData.DataSize.Fixed)/*;


    public final int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    public EbdtTable(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public int version()
    {
        return this._data.readFixed((int)Offset.version);
    }

    public BitmapGlyph glyph(int offset, int length, int format)
    {
        ReadableFontData glyphData = this._data.slice(offset, length);
        return BitmapGlyph.createGlyph(glyphData, format);
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

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, ReadableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : SubTableContainerTable.IBuilder<EbdtTable>
    {

    }
    private class Builder : SubTableContainerTable.Builder<EbdtTable>, IBuilder
    {
        private readonly int version = 0x00020000; // TODO(stuartg) need a constant/enum
        private IList<IDictionary<Integer, BitmapGlyphInfo>> _glyphLoca;
        private IList<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>> _glyphBuilders;


        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public void setLoca(IList<IDictionary<Integer, BitmapGlyphInfo>> locaList)
        {
            this.revert();
            this._glyphLoca = locaList;
        }

        public IList<IDictionary<Integer, BitmapGlyphInfo>> generateLocaList()
        {
            if (this._glyphBuilders == null)
            {
                if (this._glyphLoca == null)
                {
                    return new List<IDictionary<Integer, BitmapGlyphInfo>>(0);
                }
                return this._glyphLoca;
            }

            IList<IDictionary<Integer, BitmapGlyphInfo>> newLocaList =
                new List<IDictionary<Integer, BitmapGlyphInfo>>(this._glyphBuilders.Count);

            int startOffset = (int)Offset.headerLength;
            foreach (var builderMap in  this._glyphBuilders)
            {
                IDictionary<Integer, BitmapGlyphInfo> newLocaMap = new Dictionary<Integer, BitmapGlyphInfo>();
                int glyphOffset = 0;
                foreach (var glyphEntry in builderMap.entrySet())
                {
                    var builder = glyphEntry.getValue();
                    int size = builder.subDataSizeToSerialize();
                    BitmapGlyphInfo info = new BitmapGlyphInfo(
                        glyphEntry.getKey(), startOffset + glyphOffset, size, builder.format());
                    newLocaMap.put(glyphEntry.getKey(), info);
                    glyphOffset += size;
                }
                startOffset += glyphOffset;
                newLocaList.Add(Collections.unmodifiableMap(newLocaMap));
            }
            return Collections.unmodifiableList(newLocaList);
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
        public IList<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>> glyphBuilders()
        {
            return this.getGlyphBuilders();
        }

        /**
         * Replace the public glyph builders with the one provided. The provided
         * list and all contained objects belong to this builder.
         *
         *  This call is only required if the entire set of glyphs in the glyph
         * table builder are being replaced. If the glyph builder list provided from
         * the {@link EbdtTable.Builder#glyphBuilders()} is being used and modified
         * then those changes will already be reflected in the glyph table builder.
         *
         * @param glyphBuilders the new glyph builders
         */
        public void setGlyphBuilders(
            IList<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>> glyphBuilders)
        {
            this._glyphBuilders = glyphBuilders;
            this.setModelChanged();
        }

        private IList<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>> getGlyphBuilders()
        {
            if (this._glyphBuilders == null)
            {
                if (this._glyphLoca == null)
                {
                    throw new IllegalStateException("Loca values not set - unable to parse glyph data.");
                }
                this._glyphBuilders = Builder.initialize(this.internalReadData(), this._glyphLoca);
                this.setModelChanged();
            }
            return this._glyphBuilders;
        }

        public void revert()
        {
            this._glyphLoca = null;
            this._glyphBuilders = null;
            this.setModelChanged(false);
        }

        private static IList<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>> initialize(
            ReadableFontData data, IList<IDictionary<Integer, BitmapGlyphInfo>> locaList)
        {

            var glyphBuilderList =
                new List<IDictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>>(locaList.Count);
            if (data != null)
            {
                foreach (var locaMap in locaList)
                {
                    var glyphBuilderMap =
              new Dictionary<Integer, BitmapGlyph.IBuilder<BitmapGlyph>>();
                    foreach (var entry in locaMap.entrySet())
                    {
                        BitmapGlyphInfo info = entry.getValue();
                        var glyphBuilder =
                            BitmapGlyph.createGlyphBuilder(
                                data.slice(info.offset(), info.length()), info.format());
                        glyphBuilderMap.put(entry.getKey(), glyphBuilder);
                    }
                    glyphBuilderList.Add(glyphBuilderMap);
                }
            }
            return glyphBuilderList;
        }

        public override EbdtTable subBuildTable(ReadableFontData data)
        {
            return new EbdtTable(this.header(), data);
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this._glyphBuilders == null || this._glyphBuilders.Count == 0)
            {
                return 0;
            }

            boolean @fixed = true;
            int size = (int)Offset.headerLength;
            foreach (IDictionary<Integer, BitmapGlyph.Builder<BitmapGlyph>> builderMap in
                this._glyphBuilders)
            {
                IDictionary<Integer, BitmapGlyphInfo> newLocaMap = new Dictionary<Integer, BitmapGlyphInfo>();
                foreach (var glyphEntry in
            builderMap.entrySet())
                {
                    BitmapGlyph.Builder<BitmapGlyph> builder = glyphEntry.getValue();
                    int glyphSize = builder.subDataSizeToSerialize();
                    size += Math.Abs(glyphSize);
                    @fixed = (glyphSize <= 0) ? false : @fixed;
                }
            }
            return (@fixed ? 1 : -1) * size;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.glyphBuilders == null)
            {
                return false;
            }
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = 0;
            size += newData.writeFixed((int)Offset.version, this.version);

            foreach (IDictionary<Integer, BitmapGlyph.Builder<BitmapGlyph>> builderMap in
                this._glyphBuilders)
            {
                IDictionary<Integer, BitmapGlyphInfo> newLocaMap = new Dictionary<Integer, BitmapGlyphInfo>();
                foreach (var glyphEntry in
            builderMap.entrySet())
                {
                    BitmapGlyph.Builder<BitmapGlyph> builder = glyphEntry.getValue();
                    size += builder.subSerialize(newData.slice(size));
                }
            }
            return size;
        }
    }
}
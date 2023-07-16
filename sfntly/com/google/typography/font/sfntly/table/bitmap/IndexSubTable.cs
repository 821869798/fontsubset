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
using com.google.typography.font.sfntly.table.core;
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;








public abstract class IndexSubTable : SubTable
{
    private static readonly boolean DEBUG = false;

    public static class Format
    {
        public static readonly int FORMAT_1 = 1;
        public static readonly int FORMAT_2 = 2;
        public static readonly int FORMAT_3 = 3;
        public static readonly int FORMAT_4 = 4;
        public static readonly int FORMAT_5 = 5;
    }

    private readonly int _firstGlyphIndex;
    private readonly int _lastGlyphIndex;
    private readonly int _indexFormat;
    private readonly int _imageFormat;
    private readonly int _imageDataOffset;

    public static IndexSubTable createIndexSubTable(
        ReadableFontData data, int offsetToIndexSubTableArray, int arrayIndex)
    {

        var builder = createBuilder(data, offsetToIndexSubTableArray, arrayIndex);
        if (builder == null)
        {
            return null;
        }
        return builder.build();
    }

    public IndexSubTable(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data)
    {
        this._firstGlyphIndex = firstGlyphIndex;
        this._lastGlyphIndex = lastGlyphIndex;
        this._indexFormat = this._data.readUShort((int)Offset.indexSubHeader_indexFormat);
        this._imageFormat = this._data.readUShort((int)Offset.indexSubHeader_imageFormat);
        this._imageDataOffset = this._data.readULongAsInt((int)Offset.indexSubHeader_imageDataOffset);
    }

    public int indexFormat()
    {
        return this._indexFormat;
    }

    public int firstGlyphIndex()
    {
        return this._firstGlyphIndex;
    }

    public int lastGlyphIndex()
    {
        return this._lastGlyphIndex;
    }

    public int imageFormat()
    {
        return this._imageFormat;
    }

    public int imageDataOffset()
    {
        return this._imageDataOffset;
    }

    public BitmapGlyphInfo glyphInfo(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
        if (loca == -1)
        {
            return null;
        }
        if (this.glyphStartOffset(glyphId) == -1)
        {
            return null;
        }

        return new BitmapGlyphInfo(glyphId, this.imageDataOffset(), this.glyphStartOffset(glyphId),
            this.glyphLength(glyphId), this.imageFormat());
    }

    public int glyphOffset(int glyphId)
    {
        int glyphStartOffset = this.glyphStartOffset(glyphId);
        if (glyphStartOffset == -1)
        {
            return -1;
        }
        return this.imageDataOffset() + glyphStartOffset;
    }

    /**
     * Gets the offset of the glyph relative to the block for this index subtable.
     *
     * @param glyphId the glyph id
     * @return the glyph offset
     */
    public abstract int glyphStartOffset(int glyphId);

    public abstract int glyphLength(int glyphId);

    public abstract int numGlyphs();

    public static int checkGlyphRange(int glyphId, int firstGlyphId, int lastGlyphId)
    {
        if (glyphId < firstGlyphId || glyphId > lastGlyphId)
        {
            throw new IndexOutOfBoundsException("Glyph ID is outside of the allowed range.");
        }
        return glyphId - firstGlyphId;
    }

    public int checkGlyphRange(int glyphId)
    {
        return IndexSubTable.checkGlyphRange(glyphId, this.firstGlyphIndex(), this.lastGlyphIndex());
    }

    public override String ToString()
    {
        String s = "IndexSubTable: " + "[0x" + NumberHelper.toHexString(this.firstGlyphIndex()) + " : Ox"
            + NumberHelper.toHexString(this.lastGlyphIndex()) + "]" + ", format = " + this._indexFormat
            + ", image format = " + this.imageFormat() + ", imageOff = "
            + NumberHelper.toHexString(this.imageDataOffset()) + "\n";
        if (DEBUG)
        {
            for (int g = this.firstGlyphIndex(); g < this.lastGlyphIndex(); g++)
            {
                s += "\tgid = " + g + ", offset = " + this.glyphStartOffset(g) + "\n";
            }
        }
        return s;
    }

    public static IBuilder<IndexSubTable> createBuilder(int indexFormat)
    {
        switch (indexFormat)
        {
            case 1:
                return IndexSubTableFormat1.createBuilder();
            case 2:
                return IndexSubTableFormat2.createBuilder();
            case 3:
                return IndexSubTableFormat3.createBuilder();
            case 4:
                return IndexSubTableFormat4.createBuilder();
            case 5:
                return IndexSubTableFormat5.createBuilder();
            default:
                // unknown format and unable to process
                throw new IllegalArgumentException(String.Format("Invalid Index SubTable Format %i%n",
                    indexFormat));
        }
    }

    public static IBuilder<IndexSubTable> createBuilder(
        ReadableFontData data, int offsetToIndexSubTableArray, int arrayIndex)
    {

        int indexSubTableEntryOffset =
            offsetToIndexSubTableArray + arrayIndex * (int)Offset.indexSubTableEntryLength;

        int firstGlyphIndex = data.readUShort(
            indexSubTableEntryOffset + (int)Offset.indexSubTableEntry_firstGlyphIndex);
        int lastGlyphIndex = data.readUShort(
            indexSubTableEntryOffset + (int)Offset.indexSubTableEntry_lastGlyphIndex);
        int additionOffsetToIndexSubtable = data.readULongAsInt(indexSubTableEntryOffset
            + (int)Offset.indexSubTableEntry_additionalOffsetToIndexSubtable);

        int indexSubTableOffset = offsetToIndexSubTableArray + additionOffsetToIndexSubtable;

        int indexFormat = data.readUShort(indexSubTableOffset);
        switch (indexFormat)
        {
            case 1:
                return IndexSubTableFormat1.createBuilder(
                    data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
            case 2:
                return IndexSubTableFormat2.createBuilder(
                    data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
            case 3:
                return IndexSubTableFormat3.createBuilder(
                    data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
            case 4:
                return IndexSubTableFormat4.createBuilder(
                    data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
            case 5:
                return IndexSubTableFormat5.createBuilder(
                    data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
            default:
                // unknown format and unable to process
                throw new IllegalArgumentException(
                    String.Format("Invalid Index SubTable Foramt %i%n", indexFormat));
        }
    }

    new public interface IBuilder<out TIndexSubTable> : SubTable.IBuilder<TIndexSubTable> where TIndexSubTable : IndexSubTable
    {
        int firstGlyphIndex();
        BitmapGlyphInfo glyphInfo(int glyphId);
        int glyphLength(int glyphId);
        int glyphOffset(int glyphId);
        int imageFormat();
        int lastGlyphIndex();

        IEnumerator<BitmapGlyphInfo> GetEnumerator();
        int indexFormat();
    }

    new protected abstract class Builder<TIndexSubTable> : SubTable.Builder<TIndexSubTable>, IBuilder<TIndexSubTable> where TIndexSubTable : IndexSubTable
    {
        private int _firstGlyphIndex;
        private int _lastGlyphIndex;
        private int _indexFormat;
        private int _imageFormat;
        private int _imageDataOffset;


        public Builder(int dataSize, int indexFormat) : base(dataSize)
        {
            this._indexFormat = indexFormat;
        }

        public Builder(int indexFormat, int imageFormat, int imageDataOffset, int dataSize) : this(dataSize, indexFormat)
        {
            this._imageFormat = imageFormat;
            this._imageDataOffset = imageDataOffset;
        }

        public Builder(WritableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data)
        {
            this._firstGlyphIndex = firstGlyphIndex;
            this._lastGlyphIndex = lastGlyphIndex;
            this.initialize(data);
        }

        public Builder(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data)
        {
            this._firstGlyphIndex = firstGlyphIndex;
            this._lastGlyphIndex = lastGlyphIndex;
            this.initialize(data);
        }

        /**
         * @param data
         */
        private void initialize(ReadableFontData data)
        {
            this._indexFormat = data.readUShort((int)Offset.indexSubHeader_indexFormat);
            this._imageFormat = data.readUShort((int)Offset.indexSubHeader_imageFormat);
            this._imageDataOffset = data.readULongAsInt((int)Offset.indexSubHeader_imageDataOffset);
        }


        /**
         * Unable to fully revert unless some changes happen to hold the original
         * data. Until then keep as protected.
         */
        public virtual void revert()
        {
            this.setModelChanged(false);
            this.initialize(this.internalReadData());
        }

        public int indexFormat()
        {
            return this._indexFormat;
        }

        public int firstGlyphIndex()
        {
            return this._firstGlyphIndex;
        }

        public void setFirstGlyphIndex(int firstGlyphIndex)
        {
            this._firstGlyphIndex = firstGlyphIndex;
        }

        public int lastGlyphIndex()
        {
            return this._lastGlyphIndex;
        }

        public void setLastGlyphIndex(int lastGlyphIndex)
        {
            this._lastGlyphIndex = lastGlyphIndex;
        }

        public int imageFormat()
        {
            return this._imageFormat;
        }

        public void setImageFormat(int imageFormat)
        {
            this._imageFormat = imageFormat;
        }

        public int imageDataOffset()
        {
            return this._imageDataOffset;
        }

        public void setImageData(int offset)
        {
            this._imageDataOffset = offset;
        }

        public abstract int numGlyphs();

        /**
         * Gets the glyph info for the specified glyph id.
         *
         * @param glyphId the glyph id to look up
         * @return the glyph info
         */
        // TODO(stuartg): could be optimized by pushing down into subclasses
        public BitmapGlyphInfo glyphInfo(int glyphId)
        {
            return new BitmapGlyphInfo(glyphId, this.imageDataOffset(), this.glyphStartOffset(glyphId),
                this.glyphLength(glyphId), this.imageFormat());
        }

        /**
         * Gets the full offset of the glyph within the EBDT table.
         *
         * @param glyphId the glyph id
         * @return the glyph offset
         */
        public int glyphOffset(int glyphId)
        {
            return this.imageDataOffset() + this.glyphStartOffset(glyphId);
        }

        /**
         * Gets the offset of the glyph relative to the block for this index
         * subtable.
         *
         * @param glyphId the glyph id
         * @return the glyph offset
         */
        public abstract int glyphStartOffset(int glyphId);

        /**
         * Gets the length of the glyph within the EBDT table.
         *
         * @param glyphId the glyph id
         * @return the glyph offset
         */
        public abstract int glyphLength(int glyphId);

        /**
         * Checks that the glyph id is within the correct range. If it returns the
         * offset of the glyph id from the start of the range.
         *
         * @param glyphId
         * @return the offset of the glyphId from the start of the glyph range
         * @throws IndexOutOfBoundsException if the glyph id is not within the
         *         correct range
         */
        public int checkGlyphRange(int glyphId)
        {
            return IndexSubTable.checkGlyphRange(glyphId, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public int serializeIndexSubHeader(WritableFontData data)
        {
            int size = data.writeUShort((int)Offset.indexSubHeader_indexFormat, this._indexFormat);
            size += data.writeUShort((int)Offset.indexSubHeader_imageFormat, this._imageFormat);
            size += data.writeULong((int)Offset.indexSubHeader_imageDataOffset, this._imageDataOffset);
            return size;
        }

        public abstract IEnumerator<BitmapGlyphInfo> GetEnumerator();

        /*
         * The following methods will never be called but they need to be here to
         * allow the BitmapSizeTable to see these methods through an abstract
         * reference.
         */
        public override TIndexSubTable subBuildTable(ReadableFontData data)
        {
            return null;
        }

        public override void subDataSet()
        {
            // NOP
        }


        public override int subDataSizeToSerialize()
        {
            return 0;
        }

        public override boolean subReadyToSerialize()
        {
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return 0;
        }

        public override String ToString()
        {
            String s =
                "IndexSubTable: " + "[0x" + NumberHelper.toHexString(this.firstGlyphIndex()) + " : Ox"
                    + NumberHelper.toHexString(this.lastGlyphIndex()) + "]" + ", format = " + this._indexFormat
                    + ", image format = " + this.imageFormat() + ", imageOff = 0x"
                    + NumberHelper.toHexString(this.imageDataOffset()) + "\n";
            if (DEBUG)
            {
                for (int g = this.firstGlyphIndex(); g < this.lastGlyphIndex(); g++)
                {
                    s += "\tgid = " + g + ", offset = " + this.glyphStartOffset(g) + "\n";
                }
            }
            return s;
        }
    }
}
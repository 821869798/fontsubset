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

namespace com.google.typography.font.sfntly.table.bitmap;



/**
 * An immutable class holding bitmap glyph information.
 *
 * @author Stuart Gill
 */
public sealed class BitmapGlyphInfo
{
    private readonly int _glyphId;
    private readonly boolean _relative;
    private readonly int _blockOffset;
    private readonly int _startOffset;
    private readonly int _length;
    private readonly int _format;

    /**
     * Constructor for a relative located glyph. The glyph's position in the EBDT
     * table is a combination of it's block offset and it's own start offset.
     *
     * @param glyphId the glyph id
     * @param block(int)Offset.the of the block to which the glyph belongs
     * @param start(int)Offset.the of the glyph within the block
     * @param length the byte length
     * @param format the glyph image format
     */
    public BitmapGlyphInfo(int glyphId, int blockOffset, int startOffset, int length, int format)
    {
        this._glyphId = glyphId;
        this._relative = true;
        this._blockOffset = blockOffset;
        this._startOffset = startOffset;
        this._length = length;
        this._format = format;
    }

    /**
     * Constructor for an absolute located glyph. The glyph's position in the EBDT
     * table is only given by it's own start offset.
     *
     * @param glyphId the glyph id
     * @param start(int)Offset.the of the glyph within the block
     * @param length the byte length
     * @param format the glyph image format
     */
    public BitmapGlyphInfo(int glyphId, int startOffset, int length, int format)
    {
        this._glyphId = glyphId;
        this._relative = false;
        this._blockOffset = 0;
        this._startOffset = startOffset;
        this._length = length;
        this._format = format;
    }

    public int glyphId()
    {
        return this._glyphId;
    }

    public boolean relative()
    {
        return this._relative;
    }

    public int blockOffset()
    {
        return this._blockOffset;
    }

    public int offset()
    {
        return this.blockOffset() + this.startOffset();
    }

    public int startOffset()
    {
        return this._startOffset;
    }

    public int length()
    {
        return this._length;
    }

    public int format()
    {
        return this._format;
    }

    public override int GetHashCode()
    {
        int prime = 31;
        int result = 1;
        result = prime * result + _blockOffset;
        result = prime * result + _format;
        result = prime * result + _glyphId;
        result = prime * result + _length;
        result = prime * result + _startOffset;
        return result;
    }

    public override boolean Equals(Object obj)
    {
        if (this == obj)
        {
            return true;
        }
        if (obj == null)
        {
            return false;
        }
        if (!(obj is BitmapGlyphInfo))
        {
            return false;
        }
        BitmapGlyphInfo other = (BitmapGlyphInfo)obj;
        if (this.format != other.format)
        {
            return false;
        }
        if (this.glyphId != other.glyphId)
        {
            return false;
        }
        if (this.length != other.length)
        {
            return false;
        }
        if (this.offset() != other.offset())
        {
            return false;
        }
        return true;
    }

    public static readonly IComparer<BitmapGlyphInfo> StartOffsetComparator =
        new StartOffsetComparatorClass();

    private sealed class StartOffsetComparatorClass : IComparer<BitmapGlyphInfo>
    {
        public int Compare(BitmapGlyphInfo o1, BitmapGlyphInfo o2)
        {
            return (o1._startOffset - o2._startOffset);
        }
    }
}
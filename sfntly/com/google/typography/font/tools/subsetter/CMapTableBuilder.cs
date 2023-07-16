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

using com.google.typography.font.sfntly;
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.core;
using static com.google.typography.font.sfntly.table.core.CMap;

namespace com.google.typography.font.tools.subsetter;














/**
 * This is a medium-level builder for CMap tables, given the mapping from Unicode codepoint
 * to glyph id.
 *
 * @author Raph Levien
 */
public class CMapTableBuilder
{

    private static readonly int MAX_FORMAT4_ENDCODE = 0xffff;

    private readonly Font.Builder fontBuilder;
    private readonly IDictionary<Integer, Integer> mapping;

    public CMapTableBuilder(Font.Builder fontBuilder, IDictionary<Integer, Integer> mapping)
    {
        this.fontBuilder = fontBuilder;
        this.mapping = mapping;
    }

    private class CMap4Segment
    {
        private readonly int startCode;
        private int endCode;
        IList<Integer> glyphIds;

        public CMap4Segment(int startCode, int endCode)
        {
            this.startCode = startCode;
            this.endCode = endCode;
            this.glyphIds = new List<Integer>();
        }

        public boolean isContiguous()
        {
            int firstId = glyphIds.get(0);
            for (int index = 1; index < glyphIds.Count; index++)
            {
                if (glyphIds.get(index) != firstId + index)
                {
                    return false;
                }
            }
            return true;
        }

        public int idDelta()
        {
            return isContiguous() ? getGlyphIds().get(0) - getStartCode() : 0;
        }

        public int getStartCode()
        {
            return startCode;
        }

        public void setEndCode(int endCode)
        {
            this.endCode = endCode;
        }

        public int getEndCode()
        {
            return endCode;
        }

        public IList<Integer> getGlyphIds()
        {
            return glyphIds;
        }
    }

    // TODO(raph): This currently uses a simplistic algorithm to compute segments.
    // The segments computed are the longest contiguous segments that actually map
    // glyph ids. A smarter approach would leave "holes", or short runs of glyphs
    // mapped to notdef, to reduce the number of segments.
    private IList<CMap4Segment> getFormat4Segments()
    {
        IList<CMap4Segment> result = new List<CMap4Segment>();
        var sortedMap = new SortedDictionary<Integer, Integer>(mapping);
        if (!sortedMap.containsKey(MAX_FORMAT4_ENDCODE))
        {
            sortedMap.put(MAX_FORMAT4_ENDCODE, 0);
        }

        CMap4Segment curSegment = null;
        foreach (var entry in sortedMap.entrySet())
        {
            int unicode = entry.getKey();
            if (unicode > MAX_FORMAT4_ENDCODE)
            {
                break;
            }
            int glyphId = entry.getValue();
            if (curSegment == null || unicode != curSegment.getEndCode() + 1)
            {
                curSegment = new CMap4Segment(unicode, unicode);
                result.Add(curSegment);
            }
            else
            {
                curSegment.setEndCode(unicode);
            }
            curSegment.getGlyphIds().Add(glyphId);
        }
        return result;
    }

    private void buildCMapFormat4(CMapFormat4.IBuilder builder, IList<CMap4Segment> segments)
    {
        IList<CMapFormat4.IBuilder.Segment> segmentList =
            new List<CMapFormat4.IBuilder.Segment>();
        IList<Integer> glyphIdArray = new List<Integer>();

        // The glyphIndexArray immediately follows the idRangeOffset array, so idOffset counts the
        // offset (in shorts) from the beginning of the idRangeOffset array to the next block of
        // glyphIndexArray data.
        int idOffset = segments.Count;
        for (int i = 0; i < segments.Count; i++)
        {
            CMap4Segment segment = segments.get(i);
            int idRangeOffset;
            if (segment.isContiguous())
            {
                idRangeOffset = 0;
            }
            else
            {
                idRangeOffset = (idOffset - i) * (int)FontData.DataSize.USHORT;
                glyphIdArray.AddRange(segment.getGlyphIds());
                idOffset += segment.getGlyphIds().Count;
            }
            segmentList.Add(new CMapFormat4.IBuilder.Segment(segment.getStartCode(), segment.getEndCode(),
                segment.idDelta(), idRangeOffset));
        }
        builder.setGlyphIdArray(glyphIdArray);
        builder.setSegments(segmentList);
    }

    public void build()
    {
        CMapTable.IBuilder cmapTableBuilder = (CMapTable.IBuilder)fontBuilder.newTableBuilder(Tag.cmap);
        CMapFormat4.IBuilder cmapBuilder = (CMapFormat4.IBuilder)cmapTableBuilder.newCMapBuilder(CMapTable.CMapId.WINDOWS_BMP, CMapFormat.Format4);
        buildCMapFormat4(cmapBuilder, getFormat4Segments());
    }
}

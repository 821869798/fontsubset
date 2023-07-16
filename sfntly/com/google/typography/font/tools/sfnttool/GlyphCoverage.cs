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
using com.google.typography.font.sfntly.table.core;
using com.google.typography.font.sfntly.table.truetype;
using static com.google.typography.font.sfntly.table.core.CMap;
using static com.google.typography.font.sfntly.table.truetype.Glyph;

namespace com.google.typography.font.tools.sfnttool;


















/**
 * A class for computing which glyphs are needed to render a given string. Currently
 * this class is quite simplistic, only using the cmap, not taking into account any
 * ligature or complex layout.
 * 
 * @author Raph Levien
 */
public class GlyphCoverage {

  public static IList<Integer> getGlyphCoverage(Font font, String @string) {
    CMapTable cmapTable = font.getTable<CMapTable>(Tag.cmap);
    CMap cmap = getBestCMap(cmapTable);
    ISet<Integer> coverage = new HashSet<Integer>();
    coverage.Add(0);  // Always include notdef
    // TODO: doesn't support non-BMP scripts, should use StringCharacterIterator instead
    for (int i = 0; i < @string.Length; i++) {
      int c = (@string[i]) & 0xffff;
      int glyphId = cmap.glyphId(c);
      touchGlyph(font, coverage, glyphId);
    }
        var sortedCoverage = new List<Integer>(coverage);
        sortedCoverage.Sort();
    return sortedCoverage;
  }
  
  private static void touchGlyph(Font font, ISet<Integer> coverage, int glyphId) {
    if (!coverage.Contains(glyphId)) {
      coverage.Add(glyphId);
      Glyph glyph = getGlyph(font, glyphId);
      if (glyph != null && glyph.glyphType() == GlyphType.Composite) {
        CompositeGlyph composite = (CompositeGlyph) glyph;
        for (int i = 0; i < composite.numGlyphs(); i++) {
          touchGlyph(font, coverage, composite.glyphIndex(i));
        }
      }
    }
  }
  
  private static CMap getBestCMap(CMapTable cmapTable) {
    foreach(CMap cmap in cmapTable) {
      if (cmap.format() == (int)CMapFormat.Format12) {
        return cmap;
      }
    }
    foreach(CMap cmap in cmapTable) {
      if (cmap.format() == (int)CMapFormat.Format4) {
        return cmap;
      }
    }
    return null;
  }

  private static Glyph getGlyph(Font font, int glyphId) {
    LocaTable locaTable = font.getTable<LocaTable> (Tag.loca);
    GlyphTable glyfTable = font.getTable<GlyphTable>(Tag.glyf);
    int offset = locaTable.glyphOffset(glyphId);
    int length = locaTable.glyphLength(glyphId);
    return glyfTable.glyph(offset, length);
  }
}

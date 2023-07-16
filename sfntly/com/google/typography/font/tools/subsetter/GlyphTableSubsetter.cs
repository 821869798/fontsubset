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
using com.google.typography.font.sfntly.table.truetype;
using System.Diagnostics;

namespace com.google.typography.font.tools.subsetter;













/**
 * @author Stuart Gill
 */
public class GlyphTableSubsetter : TableSubsetterImpl {

  private static readonly boolean DEBUG = false;

  /**
   * Constructor.
   */
  public GlyphTableSubsetter():base(Tag.glyf, Tag.loca, Tag.maxp) {
    // Note: doesn't actually create the maxp table, that should be done in the
    // setUpTables method of the invoking subsetter.
    
  }

  public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder)
       {
    if (DEBUG) {
      Debug.WriteLine("GlyphTableSubsetter.subset()");
    }
    IList<Integer> permutationTable = subsetter.glyphMappingTable();
    if (permutationTable == null) {
      return false;
    }

    GlyphTable glyphTable = font.getTable<GlyphTable>(Tag.glyf);
    LocaTable locaTable = font.getTable<LocaTable>(Tag.loca);
    if (glyphTable == null || locaTable == null) {
      throw new RuntimeException("Font to subset is not valid.");
    }

    var glyphTableBuilder = (GlyphTable.IBuilder)fontBuilder.newTableBuilder(Tag.glyf);
        var locaTableBuilder = (LocaTable.IBuilder)fontBuilder.newTableBuilder(Tag.loca);
    if (glyphTableBuilder == null || locaTableBuilder == null) {
      throw new RuntimeException("Builder for subset is not valid.");
    }
    IDictionary<Integer, Integer> inverseMap = subsetter.getInverseMapping();

    IList<Glyph.IBuilder<Glyph>> glyphBuilders = glyphTableBuilder.glyphBuilders();
    foreach(int oldGlyphId in permutationTable) {
      // TODO(stuartg): add subsetting individual glyph data - remove hints etc.

      int oldOffset = locaTable.glyphOffset(oldGlyphId);
      int oldLength = locaTable.glyphLength(oldGlyphId);
      Glyph glyph = glyphTable.glyph(oldOffset, oldLength);
      ReadableFontData data = glyph.readFontData();
      ReadableFontData renumberedData = GlyphRenumberer.renumberGlyph(data, inverseMap);
      var glyphBuilder = glyphTableBuilder.glyphBuilder(renumberedData);
      if (DEBUG) {
        Debug.WriteLine("\toldGlyphId = " + oldGlyphId);
        Debug.WriteLine("\toldOffset = " + oldOffset);
        Debug.WriteLine("\toldLength = " + oldLength);
        Debug.WriteLine("\told glyph = " + glyph);
        Debug.WriteLine("\tnew glyph builder = " + glyphBuilder);
      }
      glyphBuilders.Add(glyphBuilder);

    }
    IList<Integer> locaList = glyphTableBuilder.generateLocaList();
    if (DEBUG) {
      Debug.WriteLine("\tlocaList = " + locaList);
    }
    locaTableBuilder.setLocaList(locaList);
        var maxpBuilder = (MaximumProfileTable.IBuilder)fontBuilder.getTableBuilder(Tag.maxp);
    maxpBuilder.setNumGlyphs(locaTableBuilder.numGlyphs());
    return true;
  }
}

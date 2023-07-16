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
using com.google.typography.font.sfntly.table.truetype;

namespace com.google.typography.font.tools.subsetter;











/**
 * @author Raph Levien
 */
public class GlyphTableStripper : TableSubsetterImpl
{

    public GlyphTableStripper() : base(Tag.glyf, Tag.loca)
    {
    }

    public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder)
    {

        GlyphTable glyphTable = font.getTable<GlyphTable>(Tag.glyf);
        LocaTable locaTable = font.getTable<LocaTable>(Tag.loca);
        if (glyphTable == null || locaTable == null)
        {
            throw new RuntimeException("Font to subset is not valid.");
        }
        ReadableFontData originalGlyfData = glyphTable.readFontData();

        var glyphTableBuilder = (GlyphTable.IBuilder)fontBuilder.newTableBuilder(Tag.glyf);
        var locaTableBuilder = (LocaTable.IBuilder)fontBuilder.newTableBuilder(Tag.loca);
        var glyphBuilders = glyphTableBuilder.glyphBuilders();

        GlyphStripper glyphStripper = new GlyphStripper(glyphTableBuilder);

        for (int i = 0; i < locaTable.numGlyphs(); i++)
        {
            int oldOffset = locaTable.glyphOffset(i);
            int oldLength = locaTable.glyphLength(i);
            Glyph glyph = glyphTable.glyph(oldOffset, oldLength);
            glyphBuilders.Add(glyphStripper.stripGlyph(glyph));
        }

        var locaList = glyphTableBuilder.generateLocaList();
        locaTableBuilder.setLocaList(locaList);
        return true;
    }
}
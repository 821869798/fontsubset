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
using static com.google.typography.font.sfntly.table.core.CMap;

namespace com.google.typography.font.tools.subsetter;













/**
 * @author Raph Levien
 */
public class RenumberingCMapTableSubsetter : TableSubsetterImpl {

  public RenumberingCMapTableSubsetter() :base(Tag.cmap) {
  }
 
  private static CMapFormat4 getCMapFormat4(Font font) {
    CMapTable cmapTable = font.getTable<CMapTable>(Tag.cmap);
    foreach(CMap cmap in cmapTable) {
      if (cmap.format() == (int)CMapFormat.Format4) {
        return (CMapFormat4) cmap;
      }
    }
    return null;
  }
  
  public static IDictionary<Integer, Integer> computeMapping(Subsetter subsetter, Font font) {
    CMapFormat4 cmap4 = getCMapFormat4(font);
    if (cmap4 == null) {
      throw new RuntimeException("CMap format 4 table in source font not found");
    }
    IDictionary<Integer, Integer> inverseMapping = subsetter.getInverseMapping();
    IDictionary<Integer, Integer> mapping = new Dictionary<Integer, Integer>();
    foreach(Integer unicode in cmap4) {
      int glyph = cmap4.glyphId(unicode);
      if (inverseMapping.containsKey(glyph)) {
        mapping.put(unicode, inverseMapping.get(glyph));
      }
    }
    return mapping;
  }
  
  public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder)  {
    CMapTableBuilder cmapBuilder =
        new CMapTableBuilder(fontBuilder, computeMapping(subsetter, font));
    cmapBuilder.build();
    return true;
  }

}

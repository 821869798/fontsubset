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

namespace com.google.typography.font.tools.subsetter;









/**
 * @author Stuart Gill
 *
 */
public class CMapTableSubsetter : TableSubsetterImpl
{

    /**
     * Constructor.
     */
    public CMapTableSubsetter() : base(Tag.cmap)
    {
    }

    public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder)
    {
        CMapTable cmapTable = font.getTable<CMapTable>(Tag.cmap);
        if (cmapTable == null)
        {
            throw new RuntimeException("Font to subset is not valid.");
        }

        var cmapTableBuilder =
             (CMapTable.IBuilder)fontBuilder
                 .newTableBuilder(Tag.cmap);

        foreach (CMapTable.CMapId cmapId in subsetter.cmapId())
        {
            CMap cmap = cmapTable.cmap(cmapId);
            if (cmap != null)
            {
                cmapTableBuilder.newCMapBuilder(cmapId, cmap.readFontData());
            }
        }
        return true;
    }
}
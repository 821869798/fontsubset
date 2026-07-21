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
 * Note: at the moment, this class just replicates the existing functionality in sfntly. It does
 * _not_ create a working font.
 * 
 * @author Raph Levien
 */
public class DumbSubsetter : Subsetter
{

    public DumbSubsetter(Font font, FontFactory fontFactory) : base(font, fontFactory)
    {
        ISet<TableSubsetter> temp = new HashSet<TableSubsetter>();
        temp.Add(new GlyphTableSubsetter());
        temp.Add(new CMapTableSubsetter());
        tableSubsetters = temp;
    }

    public override void setUpTables(Font.Builder fontBuilder)
    {
        fontBuilder.newTableBuilder(Tag.maxp, font.getTable<MaximumProfileTable>(Tag.maxp).readFontData());
    }
}
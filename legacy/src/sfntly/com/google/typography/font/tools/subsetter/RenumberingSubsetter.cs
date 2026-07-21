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
using com.google.typography.font.sfntly.table;

namespace com.google.typography.font.tools.subsetter;








/**
 * @author Raph Levien
 */
public class RenumberingSubsetter : Subsetter
{
    public RenumberingSubsetter(Font font, FontFactory fontFactory) : base(font, fontFactory)
    {
        ISet<TableSubsetter> temp = new HashSet<TableSubsetter>();
        temp.Add(new GlyphTableSubsetter());
        temp.Add(new RenumberingCMapTableSubsetter());
        temp.Add(new PostScriptTableSubsetter());
        temp.Add(new HorizontalMetricsTableSubsetter());
        tableSubsetters = temp;
    }

    public override void setUpTables(Font.Builder fontBuilder)
    {
        fontBuilder.newTableBuilder(Tag.hhea, font.getTable<Table>(Tag.hhea).readFontData());
        fontBuilder.newTableBuilder(Tag.maxp, font.getTable<Table>(Tag.maxp).readFontData());
    }
}
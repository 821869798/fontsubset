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
 * @author Raph Levien
 */
public class HorizontalMetricsTableSubsetter : TableSubsetterImpl
{

    public HorizontalMetricsTableSubsetter() : base(Tag.hmtx, Tag.hhea)
    {
        // Note: doesn't actually create the hhea table, that should be done in the
        // setUpTables method of the invoking subsetter.
        ;
    }

    public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder)
    {
        IList<Integer> permutationTable = subsetter.glyphMappingTable();
        if (permutationTable == null)
        {
            return false;
        }
        HorizontalMetricsTable origMetrics = font.getTable<HorizontalMetricsTable>(Tag.hmtx);
        IList<HorizontalMetricsTableBuilder.LongHorMetric> metrics =
            new List<HorizontalMetricsTableBuilder.LongHorMetric>();
        for (int i = 0; i < permutationTable.Count; i++)
        {
            int origGlyphId = permutationTable.get(i);
            int advanceWidth = origMetrics.advanceWidth(origGlyphId);
            int lsb = origMetrics.leftSideBearing(origGlyphId);
            metrics.Add(new HorizontalMetricsTableBuilder.LongHorMetric(advanceWidth, lsb));
        }
        new HorizontalMetricsTableBuilder(fontBuilder, metrics).build();
        return true;
    }
}
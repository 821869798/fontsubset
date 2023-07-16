/*
 * Copyright 2010 Google Inc. All Rights Reserved.
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

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.core;







/**
 * A Horizontal Metrics table - 'hmtx'.
 *
 * @author Stuart Gill
 */
public sealed class HorizontalMetricsTable : Table
{

    private int numHMetrics;
    private int numGlyphs;

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    private enum Offset
    {
        // hMetrics
        hMetricsStart = (0), hMetricsSize = (4),

        // Offsets within an hMetric
        hMetricsAdvanceWidth = (0),
        hMetricsLeftSideBearing = (2),

        LeftSideBearingSize = (2)/*;

    private readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    private HorizontalMetricsTable(
        Header header, ReadableFontData data, int numHMetrics, int numGlyphs) : base(header, data)
    {
        this.numHMetrics = numHMetrics;
        this.numGlyphs = numGlyphs;
    }

    public int numberOfHMetrics()
    {
        return this.numHMetrics;
    }

    public int numberOfLSBs()
    {
        return this.numGlyphs - this.numHMetrics;
    }

    public int hMetricAdvanceWidth(int entry)
    {
        if (entry > this.numHMetrics)
        {
            throw new IndexOutOfBoundsException();
        }
        int offset =
          (int)Offset.hMetricsStart +
          (entry * (int)Offset.hMetricsSize) + (int)Offset.hMetricsAdvanceWidth;
        return this._data.readUShort(offset);
    }

    public int hMetricLSB(int entry)
    {
        if (entry > this.numHMetrics)
        {
            throw new IndexOutOfBoundsException();
        }
        int offset =
          (int)Offset.hMetricsStart +
          (entry * (int)Offset.hMetricsSize) + (int)Offset.hMetricsLeftSideBearing;
        return this._data.readShort(offset);
    }

    public int lsbTableEntry(int entry)
    {
        if (entry > this.numberOfLSBs())
        {
            throw new IndexOutOfBoundsException();
        }
        int offset =
          (int)Offset.hMetricsStart +
          (this.numHMetrics * (int)Offset.hMetricsSize) + (entry * (int)Offset.LeftSideBearingSize);
        return this._data.readShort(offset);

    }

    public int advanceWidth(int glyphId)
    {
        if (glyphId < this.numHMetrics)
        {
            return this.hMetricAdvanceWidth(glyphId);
        }
        return this.hMetricAdvanceWidth(this.numHMetrics - 1);
    }

    public int leftSideBearing(int glyphId)
    {
        if (glyphId < this.numHMetrics)
        {
            return this.hMetricLSB(glyphId);
        }
        return this.lsbTableEntry(glyphId - this.numHMetrics);
    }

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : ITableBasedTableBuilder<HorizontalMetricsTable>
    {
        void setNumberOfHMetrics(int numHMetrics);
        void setNumGlyphs(int numGlyphs);
    }

    /**
     * Builder for a Horizontal Metrics Table - 'hmtx'.
     *
     */
    private class Builder : TableBasedTableBuilder<HorizontalMetricsTable>, IBuilder
    {
        private int numHMetrics = -1;
        private int numGlyphs = -1;


        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public override HorizontalMetricsTable subBuildTable(ReadableFontData data)
        {
            return new HorizontalMetricsTable(this.header(), data, this.numHMetrics, this.numGlyphs);
        }

        public void setNumberOfHMetrics(int numHMetrics)
        {
            if (numHMetrics < 0)
            {
                throw new IllegalArgumentException("Number of metrics can't be negative.");
            }
            this.numHMetrics = numHMetrics;
            this.table().numHMetrics = numHMetrics;
        }

        public void setNumGlyphs(int numGlyphs)
        {
            if (numGlyphs < 0)
            {
                throw new IllegalArgumentException("Number of glyphs can't be negative.");
            }
            this.numGlyphs = numGlyphs;
            this.table().numGlyphs = numGlyphs;
        }
    }
}
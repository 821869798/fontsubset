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

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.core;







/**
 * A Horizontal Device Metrics table - 'hdmx'.
 * 
 * @author raph@google.com (Raph Levien)
 */
public class HorizontalDeviceMetricsTable : Table
{

    private int numGlyphs;

    private enum Offset
    {
        version = (0),
        numRecords = (2),
        sizeDeviceRecord = (4),
        records = (8),

        // Offsets within a device record
        deviceRecordPixelSize = (0),
        deviceRecordMaxWidth = (1),
        deviceRecordWidths = (2)/*;
    
    private readonly int offset;
    
    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    private HorizontalDeviceMetricsTable(Header header, ReadableFontData data, int numGlyphs) : base(header, data)
    {
        this.numGlyphs = numGlyphs;
    }

    public int version()
    {
        return _data.readUShort((int)Offset.version);
    }

    public int numRecords()
    {
        return _data.readShort((int)Offset.numRecords);
    }

    public int recordSize()
    {
        return _data.readLong((int)Offset.sizeDeviceRecord);
    }

    public int pixelSize(int recordIx)
    {
        if (recordIx < 0 || recordIx >= numRecords())
        {
            throw new IndexOutOfBoundsException();
        }
        return _data.readUByte((int)Offset.records + recordIx * recordSize() +
            (int)Offset.deviceRecordPixelSize);
    }

    public int maxWidth(int recordIx)
    {
        if (recordIx < 0 || recordIx >= numRecords())
        {
            throw new IndexOutOfBoundsException();
        }
        return _data.readUByte((int)Offset.records + recordIx * recordSize() +
            (int)Offset.deviceRecordMaxWidth);
    }

    public int width(int recordIx, int glyphNum)
    {
        if (recordIx < 0 || recordIx >= numRecords() || glyphNum < 0 || glyphNum >= numGlyphs)
        {
            throw new IndexOutOfBoundsException();
        }
        return _data.readUByte((int)Offset.records + recordIx * recordSize() +
            (int)Offset.deviceRecordWidths + glyphNum);
    }

    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : ITableBasedTableBuilder<HorizontalDeviceMetricsTable>
    {
        void setNumGlyphs(int numGlyphs);
    }

    /**
     * Builder for a Horizontal Device Metrics Table - 'hdmx'.
     */
    private class Builder : TableBasedTableBuilder<HorizontalDeviceMetricsTable>, IBuilder
    {
        private int numGlyphs = -1;

        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public override HorizontalDeviceMetricsTable subBuildTable(ReadableFontData data)
        {
            return new HorizontalDeviceMetricsTable(this.header(), data, this.numGlyphs);
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
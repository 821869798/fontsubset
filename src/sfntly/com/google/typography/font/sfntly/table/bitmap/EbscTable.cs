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

namespace com.google.typography.font.sfntly.table.bitmap;








/**
 * @author Stuart Gill
 *
 */
public class EbscTable : Table
{

    public enum Offset
    {
        // header
        version = (0),
        numSizes = (FontData.DataSize.Fixed),
        headerLength = (numSizes + FontData.DataSize.ULONG),
        bitmapScaleTableStart = (headerLength),

        // bitmapScaleTable
        bitmapScaleTable_hori = (0),
        bitmapScaleTable_vert = ((int)EblcTable.Offset.sbitLineMetricsLength),
        bitmapScaleTable_ppemX = (bitmapScaleTable_vert + (int)EblcTable.Offset.sbitLineMetricsLength),
        bitmapScaleTable_ppemY = (bitmapScaleTable_ppemX + FontData.DataSize.BYTE),
        bitmapScaleTable_substitutePpemX = (bitmapScaleTable_ppemY + FontData.DataSize.BYTE),
        bitmapScaleTable_substitutePpemY = (bitmapScaleTable_substitutePpemX
            + FontData.DataSize.BYTE),
        bitmapScaleTableLength = (bitmapScaleTable_substitutePpemY + FontData.DataSize.BYTE)/*;

     readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
    }

    /**
     * @param header
     * @param data
     */
    private EbscTable(Header header, ReadableFontData data) : base(header, data)
    {
    }

    public int version()
    {
        return this._data.readFixed((int)Offset.version);
    }

    public int numSizes()
    {
        return this._data.readULongAsInt((int)Offset.numSizes);
    }

    public BitmapScaleTable bitmapScaleTable(int index)
    {
        if (index < 0 || index > this.numSizes() - 1)
        {
            throw new IndexOutOfBoundsException(
                "BitmapScaleTable index is outside the bounds of available tables.");
        }
        return new BitmapScaleTable(this._data,
            (int)Offset.bitmapScaleTableStart + index * (int)Offset.bitmapScaleTableLength);
    }

    public class BitmapScaleTable : SubTable
    {
        public BitmapScaleTable(ReadableFontData data, int offset) : base(data, offset, (int)Offset.bitmapScaleTableLength)
        {
        }

        public int ppemX()
        {
            return this._data.readByte((int)Offset.bitmapScaleTable_ppemX);
        }

        public int ppemY()
        {
            return this._data.readByte((int)Offset.bitmapScaleTable_ppemY);
        }

        public int substitutePpemX()
        {
            return this._data.readByte((int)Offset.bitmapScaleTable_substitutePpemX);
        }

        public int substitutePpemY()
        {
            return this._data.readByte((int)Offset.bitmapScaleTable_substitutePpemY);
        }
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
    public static IBuilder createBuilder(Header header, ReadableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : Table.IBuilder<EbscTable>
    {

    }
    // TODO(stuartg): currently the builder just builds from initial data
    // - need to make fully working but few if any examples to test with
    protected class Builder : Table.Builder<EbscTable>, IBuilder
    {

        /**
         * @param header
         * @param data
         */
        public Builder(Header header, WritableFontData data) : base(header, data)
        {
        }

        /**
         * @param header
         * @param data
         */
        public Builder(Header header, ReadableFontData data) : base(header, data)
        {
        }

        public override EbscTable subBuildTable(ReadableFontData data)
        {
            return new EbscTable(this.header(), data);
        }

        public override void subDataSet()
        {
            // NOP
        }

        public override int subDataSizeToSerialize()
        {
            return 0;
        }

        public override boolean subReadyToSerialize()
        {
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return 0;
        }

    }
}
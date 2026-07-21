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

namespace com.google.typography.font.sfntly.table;

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.bitmap;
using com.google.typography.font.sfntly.table.core;
using com.google.typography.font.sfntly.table.opentype;
using com.google.typography.font.sfntly.table.truetype;


/**
 * A concrete implementation of a root level table in the font. This is the base
 * class used for all specific table implementations and is used as the generic
 * table for all tables which have no specific implementations.
 *
 * @author Stuart Gill
 */
public class Table : FontDataTable
{

    private Header _header;

    public Table(Header header, ReadableFontData data) : base(data)
    {
        this._header = header;
    }

    /**
     * Get the calculated checksum for the data in the table.
     *
     * @return the checksum
     */
    public long calculatedChecksum()
    {
        return this._data.checksum();
    }

    /**
     * Get the header for the table.
     *
     * @return the table header
     */
    public Header header()
    {
        return this._header;
    }

    /**
     * Get the tag for the table from the record header.
     *
     * @return the tag for the table
     * @see #header
     */
    public int headerTag()
    {
        return this.header().tag();
    }

    /**
     * Get the offset for the table from the record header.
     *
     * @return the offset for the table
     * @see #header
     */
    public int headerOffset()
    {
        return this.header().offset();
    }

    /**
     * Get the length of the table from the record header.
     *
     * @return the length of the table
     * @see #header
     */
    public int headerLength()
    {
        return this.header().length();
    }

    /**
     * Get the checksum for the table from the record header.
     *
     * @return the checksum for the table
     * @see #header
     */
    public long headerChecksum()
    {
        return this.header().checksum();
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(Tag.stringValue(this._header.tag()));
        sb.Append(", cs=0x");
        sb.Append(NumberHelper.toHexString(this._header.checksum()));
        sb.Append(", offset=0x");
        sb.Append(NumberHelper.toHexString(this._header.offset()));
        sb.Append(", size=0x");
        sb.Append(NumberHelper.toHexString(this._header.length()));
        sb.Append("]");
        return sb.ToString();
    }

    new public interface IBuilder<out TTable> : FontDataTable.IBuilder<TTable> where TTable : Table
    {
        Header header();
    }

    new internal protected abstract class Builder<TTable> : FontDataTable.Builder<TTable>, IBuilder<TTable> where TTable : Table
    {
        private Header _header;

        public Builder(Header header, WritableFontData data) : base(data)
        {
            this._header = header;
        }

        public Builder(Header header, ReadableFontData data) : base(data)
        {
            this._header = header;
        }

        public Builder(Header header) : this(header, null)
        {

        }

        public override String ToString()
        {
            return "Table Builder for - " + this._header.ToString();
        }

        /***********************************************************************************
         * 
         * Public Interface for Table Building
         * 
         ***********************************************************************************/

        public Header header()
        {
            return this._header;
        }

        /***********************************************************************************
         * Internal Interface for Table Building
         ***********************************************************************************/

        public override void notifyPostTableBuild(TTable table)
        {
            if (this.modelChanged() || this.dataChanged())
            {
                Header header = new Header(this.header().tag(), table.dataLength());
                ((Table)table)._header = header;
            }
        }

        // static interface for building

        /**
         * Get a builder for the table type specified by the data in the header.
         *
         * @param header the header for the table
         * @param tableData the data to be used to build the table from
         * @return builder for the table specified
         */
        public static Table.IBuilder<Table> getBuilder(
            Header header, WritableFontData tableData)
        {

            int tag = header.tag();

            if (tag == Tag.cmap)
            {
                return CMapTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.head)
            {
                return FontHeaderTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.hhea)
            {
                return HorizontalHeaderTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.hmtx)
            {
                return HorizontalMetricsTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.maxp)
            {
                return MaximumProfileTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.name)
            {
                return NameTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.OS_2)
            {
                return OS2Table.createBuilder(header, tableData);
            }
            else if (tag == Tag.post)
            {
                return PostScriptTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.cvt)
            {
                return ControlValueTable.createBuilder(header, tableData);
                // } else if (tag == Tag.fpgm) {
                // break;
            }
            else if (tag == Tag.glyf)
            {
                return GlyphTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.loca)
            {
                return LocaTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.prep)
            {
                return ControlProgramTable.createBuilder(header, tableData);
                // } else if (tag == CFF) {
                // break;
                // } else if (tag == VORG) {
                // break;
            }
            else if (tag == Tag.EBDT)
            {
                return EbdtTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.EBLC)
            {
                return EblcTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.EBSC)
            {
                return EbscTable.createBuilder(header, tableData);
                // } else if (tag == BASE) {
                // break;
                // } else if (tag == GDEF) {
                // break;
                // } else if (tag == GPOS) {
                // break;
            }
            else if (tag == Tag.GSUB)
            {
                return GSubTable.createBuilder(header, tableData);
                // break;
                // } else if (tag == JSTF) {
                // break;
                // } else if (tag == DSIG) {
                // break;
                // } else if (tag == gasp) {
                // break;
            }
            else if (tag == Tag.hdmx)
            {
                return HorizontalDeviceMetricsTable.createBuilder(header, tableData);
                // break;
                // } else if (tag == kern) {
                // break;
                // } else if (tag == LTSH) {
                // break;
                // } else if (tag == PCLT) {
                // break;
                // } else if (tag == VDMX) {
                // break;
                // } else if (tag == vhea) {
                // break;
                // } else if (tag == vmtx) {
                // break;
            }
            else if (tag == Tag.bhed)
            {
                return FontHeaderTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.bdat)
            {
                return EbdtTable.createBuilder(header, tableData);
            }
            else if (tag == Tag.bloc)
            {
                return EblcTable.createBuilder(header, tableData);
            }
            return GenericTableBuilder.createBuilder(header, tableData);
        }
    }
}
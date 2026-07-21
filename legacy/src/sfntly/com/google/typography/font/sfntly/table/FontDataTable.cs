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

namespace com.google.typography.font.sfntly.table;

using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.FontDataTable;

/**
 * An abstract base for any table that contains a FontData. This is the root of
 * the table class hierarchy.
 *
 * @author Stuart Gill
 *
 */
public abstract class FontDataTable
{
    public ReadableFontData _data;
    /**
     * Constructor.
     *
     * @param data the data to back this table
     */
    public FontDataTable(ReadableFontData data)
    {
        this._data = data;
    }

    /**
     * Gets the readable font data for this table.
     *
     * @return the read only data
     */
    public ReadableFontData readFontData()
    {
        return this._data;
    }

    public override String ToString()
    {
        return this._data.ToString();
    }

    /**
     * Gets the length of the data for this table in bytes. This is the full
     * allocated length of the data underlying the table and may or may not
     * include any padding.
     *
     * @return the data length in bytes
     */
    public int dataLength()
    {
        return this._data.length();
    }

    public int serialize(OutputStream os)
    {
        return this._data.copyTo(os);
    }

    public int serialize(WritableFontData data)
    {
        return this._data.copyTo(data);
    }

    public interface IBuilder<out TFontDataTable> where TFontDataTable : FontDataTable
    {
        boolean changed();

        boolean readyToBuild();

        TFontDataTable build();

        int subSerialize(WritableFontData newData);
        int subDataSizeToSerialize();

        boolean subReadyToSerialize();
    }

    internal protected abstract class Builder<TFontDataTable> : IBuilder<TFontDataTable> where TFontDataTable : FontDataTable
    {
        private WritableFontData _wData;
        private ReadableFontData _rData;
        private boolean _modelChanged;
        private boolean _containedModelChanged; // may expand to list of submodel states
        private boolean _dataChanged;

        /**
         * Constructor.
         * 
         * Construct a FontDataTable.Builder with a WritableFontData backing store
         * of size given. A positive size will create a fixed size backing store and
         * a 0 or less size is an estimate for a growable backing store with the
         * estimate being the absolute of the size.
         * 
         * @param dataSize if positive then a fixed size; if 0 or less then an
         *        estimate for a growable size
         */
        public Builder(int dataSize)
        {
            this._wData = WritableFontData.createWritableFontData(dataSize);
        }

        public Builder(WritableFontData data)
        {
            this._wData = data;
        }

        public Builder(ReadableFontData data)
        {
            this._rData = data;
        }

        /**
         * Gets a snapshot copy of the public data of the builder.
         *
         *  This causes any public data structures to be serialized to a new data
         * object. This data object belongs to the caller and must be properly
         * disposed of. No changes are made to the builder and any changes to the
         * data directly do not affect the public state. To do that a subsequent
         * call must be made to {@link #setData(WritableFontData)}.
         *
         * @return a copy of the public data of the builder
         * @see FontDataTable.Builder#setData(WritableFontData)
         */
        public WritableFontData data()
        {
            WritableFontData newData;
            if (this._modelChanged)
            {
                if (!this.subReadyToSerialize())
                {
                    throw new RuntimeException("Table not ready to build.");
                }
                int size = subDataSizeToSerialize();
                newData = WritableFontData.createWritableFontData(size);
                this.subSerialize(newData);
            }
            else
            {
                ReadableFontData data = internalReadData();
                newData = WritableFontData.createWritableFontData(data != null ? data.length() : 0);
                if (data != null)
                {
                    data.copyTo(newData);
                }
            }
            return newData;
        }

        public void setData(WritableFontData data)
        {
            this.internalSetData(data, true);
        }

        /**
         * @param data
         */
        public void setData(ReadableFontData data)
        {
            this.internalSetData(data, true);
        }

        private void internalSetData(WritableFontData data, boolean dataChanged)
        {
            this._wData = data;
            this._rData = null;
            if (dataChanged)
            {
                this._dataChanged = true;
                this.subDataSet();
            }
        }

        private void internalSetData(ReadableFontData data, boolean dataChanged)
        {
            this._rData = data;
            this._wData = null;
            if (dataChanged)
            {
                this._dataChanged = true;
                this.subDataSet();
            }
        }

        public virtual TFontDataTable build()
        {
            TFontDataTable table = null;

            ReadableFontData data = this.internalReadData();
            if (this._modelChanged)
            {
                // let subclass serialize from model
                if (!this.subReadyToSerialize())
                {
                    return null;
                }
                int size = subDataSizeToSerialize();
                WritableFontData newData = WritableFontData.createWritableFontData(size);
                this.subSerialize(newData);
                data = newData;
            }

            if (data != null)
            {
                table = this.subBuildTable(data);
                this.notifyPostTableBuild(table);
            }
            this._rData = null;
            this._wData = null;

            return table;
        }

        public virtual boolean readyToBuild()
        {
            return true;
        }

        public ReadableFontData internalReadData()
        {
            if (this._rData != null)
            {
                return this._rData;
            }
            return this._wData;
        }

        public WritableFontData internalWriteData()
        {
            if (this._wData == null)
            {
                WritableFontData newData =
                    WritableFontData.createWritableFontData(this._rData == null ? 0 : this._rData.length());
                if (this._rData != null)
                {
                    this._rData.copyTo(newData);
                }
                this.internalSetData(newData, false);
            }
            return this._wData;
        }

        /**
         * Determines whether the state of this builder has changed - either the data or the public 
         * model representing the data.
         * 
         * @return true if the builder has changed
         */
        public boolean changed()
        {
            return this.dataChanged() || this.modelChanged();
        }

        public boolean dataChanged()
        {
            return this._dataChanged;
        }

        public boolean modelChanged()
        {
            return this.currentModelChanged() || this.containedModelChanged();
        }

        public boolean currentModelChanged()
        {
            return this._modelChanged;
        }

        public boolean containedModelChanged()
        {
            return this._containedModelChanged;
        }

        public boolean setModelChanged()
        {
            return this.setModelChanged(true);
        }

        public boolean setModelChanged(boolean changed)
        {
            boolean old = this._modelChanged;
            this._modelChanged = changed;
            return old;
        }

        // subclass API

        /**
         * Notification to subclasses that a table was built.
         */
        public virtual void notifyPostTableBuild(TFontDataTable table)
        {
            // NOP -
        }

        /**
         * Serialize the table to the data provided.
         *
         * @param newData the data object to serialize to
         * @return the number of bytes written
         */
        public abstract int subSerialize(WritableFontData newData);

        /**
         * @return true if the subclass is ready to serialize it's structure into
         *         data
         */
        public abstract boolean subReadyToSerialize();

        /**
         * Query if the subclass needs to serialize and how much data is required.
         *
         * @return positive bytes needed to serialize if a fixed size; and zero or
         *         negative bytes as an estimate if growable data is needed
         */
        public abstract int subDataSizeToSerialize();

        /**
         * Tell the subclass that the data has been changed and any structures must
         * be discarded.
         */
        public abstract void subDataSet();

        /**
         * Build a table with the data provided.
         *
         * @param data the data to use to build the table
         * @return a table
         */
        public abstract TFontDataTable subBuildTable(ReadableFontData data);
    }
}
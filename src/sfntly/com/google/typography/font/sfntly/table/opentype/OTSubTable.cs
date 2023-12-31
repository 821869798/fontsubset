// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;






/**
 * Consolidates dataIsCanonical handling and building logic used by the OpenType tables.
 *
 * @author dougfelt@google.com (Doug Felt)
 */
abstract class OTSubTable : VisibleSubTable
{
    readonly boolean dataIsCanonical;

    public OTSubTable(ReadableFontData data, boolean dataIsCanonical) : base(data)
    {
        this.dataIsCanonical = dataIsCanonical;
    }

    public abstract IBuilder<OTSubTable> builder();

    new public interface IBuilder<T> : VisibleSubTable.IBuilder<T> where T : OTSubTable
    {

    }

    new protected abstract class Builder<T> : VisibleSubTable.Builder<T>, IBuilder<T> where T : OTSubTable
    {
        private readonly boolean dataIsCanonical;
        public override int serializedLength { get; set; }

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data)
        {
            this.dataIsCanonical = data == null ? false : dataIsCanonical;
            if (this.dataIsCanonical)
            {
                serializedLength = data.length();
            }
            else if (data == null || data.length() == 0)
            {
                serializedLength = 0;
            }
            else
            {
                serializedLength = -1;
                setModelChanged();
            }
        }

        public Builder(T table) : this(table.readFontData(), table.dataIsCanonical)
        {
        }

        /**
         * Returns true if the data has not been edited, and thus there is no model
         * to serialize.
         */
        public abstract boolean unedited();

        /**
         * Create a model for editing from the data.
         * This causes <code>unedited()</code> to return false;
         * @param data
         */
        public abstract void readModel(ReadableFontData data, boolean dataIsCanonical);

        /**
         * Computes the serialized length of the data.  This computes its own serialized
         * length and calls subDataSizeToSerialize on any subTables to get their length.
         */
        public abstract int computeSerializedLength();

        /**
         * Writes the model, which is exactly as long as computeSerializedLength.
         */
        public abstract void writeModel(WritableFontData data);

        /**
         * The first time this is called, it calls initFromData to build the model and
         * calls setModelChanged to indicate that the model will need to be written out.
         * It also resets the serializedLength so it will be recomputed.
         */
        public void prepareToEdit()
        {
            if (unedited())
            {
                readModel(internalReadData(), dataIsCanonical);
                serializedLength = -1;
                setModelChanged();
            }
        }

        /**
         * This is called first by FontDataTable when the tables is to be built.  It calls
         * subDataSizeToSerialize and returns true if the result > 0.  This ensures that
         * no object is built if there is no data.
         */
        public override sealed boolean subReadyToSerialize()
        {
            return subDataSizeToSerialize() > 0;
        }

        /**
         * This is called twice, once by subReadyToSerialize, and then again by FontDataTable.
         * The first call will compute the serializedLength, and the second just returns the
         * cached value.  The actual work is done in computeSerializedLength, which might
         * call this recursively on its sub-tables.
         */
        public override sealed int subDataSizeToSerialize()
        {
            if (serializedLength == -1)
            {
                if (unedited())
                {
                    prepareToEdit();
                }
                serializedLength = computeSerializedLength();
            }
            return serializedLength;
        }

        /**
         * This is called after subDataSizeToSerialize, newData has a length equal to
         * that returned by that call. If the data has not been edited, it is because
         * the data is canonical and can be copied straight to newData.  Otherwise,
         * serializeEditState is called to do the actual serialization.  When this
         * finishes, resets serializedLength.
         */
        public override sealed int subSerialize(WritableFontData newData)
        {
            if (unedited())
            {
                internalReadData().copyTo(newData);
            }
            else
            {
                writeModel(newData);
            }
            int length = serializedLength;
            serializedLength = -1;
            return length;
        }
    }
}
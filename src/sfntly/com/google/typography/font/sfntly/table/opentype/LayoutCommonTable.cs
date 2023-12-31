// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype;





/**
 * @author dougfelt@google.com (Doug Felt)
 */
abstract class LayoutCommonTable<T> : SubTable where T : LookupTable
{
    private static int VERSION_OFFSET = 0;
    private static int SCRIPT_LIST_OFFSET = 4;
    private static int FEATURE_LIST_OFFSET = 6;
    private static int LOOKUP_LIST_OFFSET = 8;
    private static int HEADER_SIZE = 10;

    private static int VERSION_ID = 0x00010000;

    private readonly boolean dataIsCanonical;

    /**
     * @param data
     *          the GSUB or GPOS data
     */
    public LayoutCommonTable(ReadableFontData data, boolean dataIsCanonical) : base(data)
    {
        this.dataIsCanonical = dataIsCanonical;
    }

    private static int readScriptListOffset(ReadableFontData data)
    {
        return data.readUShort(SCRIPT_LIST_OFFSET);
    }

    private static ReadableFontData scriptListData(ReadableFontData commonData,
        boolean dataIsCanonical)
    {
        int start = readScriptListOffset(commonData);
        if (dataIsCanonical)
        {
            int limit = readFeatureListOffset(commonData);
            return commonData.slice(start, limit - start);
        }
        return commonData.slice(start);
    }

    public ScriptListTable createScriptList()
    {
        return new ScriptListTable(scriptListData(_data, dataIsCanonical), dataIsCanonical);
    }

    private static int readFeatureListOffset(ReadableFontData data)
    {
        return data.readUShort(FEATURE_LIST_OFFSET);
    }

    private static ReadableFontData featureListData(ReadableFontData commonData,
        boolean dataIsCanonical)
    {
        int start = readFeatureListOffset(commonData);
        if (dataIsCanonical)
        {
            int limit = readLookupListOffset(commonData);
            return commonData.slice(start, limit - start);
        }
        return commonData.slice(start);
    }

    public FeatureListTable createFeatureList()
    {
        return new FeatureListTable(featureListData(_data, dataIsCanonical), dataIsCanonical);
    }

    private static int readLookupListOffset(ReadableFontData data)
    {
        return data.readUShort(LOOKUP_LIST_OFFSET);
    }

    private static ReadableFontData lookupListData(ReadableFontData commonData,
        boolean dataIsCanonical)
    {
        int start = readLookupListOffset(commonData);
        if (dataIsCanonical)
        {
            int limit = commonData.length();
            return commonData.slice(start, limit - start);
        }
        return commonData.slice(start);
    }

    public virtual LookupListTable createLookupList()
    {
        return handleCreateLookupList(lookupListData(_data, dataIsCanonical), dataIsCanonical);
    }

    public abstract LookupListTable handleCreateLookupList(
        ReadableFontData data, boolean dataIsCanonical);

    public interface IBuilder : SubTable.IBuilder<LayoutCommonTable<T>>
    {

    }

    protected abstract class Builder : SubTable.Builder<LayoutCommonTable<T>>, IBuilder
    {
        private int serializedLength;
        private ScriptListTable.IBuilder serializedScriptListBuilder;
        private FeatureListTable.IBuilder serializedFeatureListBuilder;
        private LookupListTable.IBuilder serializedLookupListBuilder;

        /**
         * @param data
         *          the GSUB or GPOS data
         */
        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data)
        {
        }

        public Builder() : base(null)
        {
        }

        public abstract LookupListTable handleCreateLookupList(
            ReadableFontData data, boolean dataIsCanonical);

        public abstract LookupListTable.IBuilder createLookupListBuilder();

        public override int subSerialize(WritableFontData newData)
        {
            if (serializedLength == 0)
            {
                return 0;
            }
            newData.writeULong(VERSION_OFFSET, VERSION_ID);
            int pos = HEADER_SIZE;
            newData.writeUShort(SCRIPT_LIST_OFFSET, pos);
            pos += serializedScriptListBuilder.subSerialize(newData.slice(pos));
            newData.writeUShort(FEATURE_LIST_OFFSET, pos);
            pos += serializedFeatureListBuilder.subSerialize(newData.slice(pos));
            newData.writeUShort(LOOKUP_LIST_OFFSET, pos);
            pos += serializedLookupListBuilder.subSerialize(newData.slice(pos));
            return serializedLength;
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override abstract LayoutCommonTable<T> subBuildTable(ReadableFontData data);
    }
}
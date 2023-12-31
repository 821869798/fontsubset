
// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;









/**
 * @author dougfelt@google.com (Doug Felt)
 */
abstract class LookupList : SubTable
{
    private LookupList(ReadableFontData data, boolean dataIsCanonical) : base(data)
    {
    }

    private static readonly int LOOKUP_COUNT_OFFSET = 0;
    private static readonly int LOOKUP_OFFSET_BASE = 2;
    private static readonly int LOOKUP_OFFSET_SIZE = 2;

    private static int readLookupCount(ReadableFontData data)
    {
        if (data == null)
        {
            return 0;
        }
        return data.readUShort(LOOKUP_COUNT_OFFSET);
    }

    private static int readLookupOffsetAt(ReadableFontData data, int index)
    {
        if (data == null)
        {
            return -1;
        }
        return data.readUShort(LOOKUP_OFFSET_BASE + index * LOOKUP_OFFSET_SIZE);
    }

    private static ReadableFontData readLookupData(ReadableFontData data, boolean dataIsCanonical,
        int index)
    {
        ReadableFontData newData;
        int offset = readLookupOffsetAt(data, index);
        if (dataIsCanonical)
        {
            int nextOffset;
            if (index < readLookupCount(data) - 1)
            {
                nextOffset = readLookupOffsetAt(data, index + 1);
            }
            else
            {
                nextOffset = data.length();
            }
            newData = data.slice(offset, nextOffset - offset);
        }
        else
        {
            newData = data.slice(offset);
        }
        return newData;
    }

    public abstract LookupType lookupTypeAt(int index);

    public abstract LookupTable createLookup(ReadableFontData data);

    public interface IBuilder : SubTable.IBuilder<LookupList>
    {

    }

    protected abstract class Builder : SubTable.Builder<LookupList>, IBuilder
    {
        private IList<LookupTable.IBuilder> builders;
        private boolean dataIsCanonical;
        private int serializedCount;
        private int serializedLength;

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data)
        {
            this.dataIsCanonical = dataIsCanonical;
        }

        public Builder() : this(null, false)
        {
        }

        public abstract LookupTable.IBuilder createLookupBuilder(
            ReadableFontData lookupData);

        private void initFromData(ReadableFontData data)
        {
            int count = readLookupCount(data);
            builders = new List<LookupTable.IBuilder>(count);
            for (int i = 0; i < count; ++i)
            {
                ReadableFontData lookupData = readLookupData(data, dataIsCanonical, i);
                LookupTable.IBuilder lookup = createLookupBuilder(lookupData);
                if (lookup != null)
                {
                    builders.Add(lookup);
                }
            }
        }

        private void prepareToEdit()
        {
            if (builders == null)
            {
                initFromData(internalReadData());
            }
        }

        private int serializeFromBuilders(WritableFontData newData)
        {
            if (serializedCount == 0)
            {
                return 0;
            }
            newData.writeUShort(LOOKUP_COUNT_OFFSET, serializedCount);
            int rpos = LOOKUP_OFFSET_BASE;
            int spos = rpos + serializedCount * LOOKUP_OFFSET_SIZE;
            for (int i = 0; i < builders.Count; ++i)
            {
                LookupTable.IBuilder builder = builders.get(i);
                int s = builder.subDataSizeToSerialize();
                if (s > 0)
                {
                    newData.writeUShort(rpos, spos);
                    rpos += LOOKUP_OFFSET_SIZE;

                    WritableFontData targetData = newData.slice(spos);
                    builder.subSerialize(targetData);
                    spos += s;
                }
            }
            return serializedLength;
        }

        public override int subSerialize(WritableFontData newData)
        {
            if (builders == null)
            {
                // Only the case if data is canonical
                ReadableFontData data = internalReadData();
                data.copyTo(newData);
                return data.length();
            }
            return serializeFromBuilders(newData);
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        private int computeSerializedSizeFromBuilders()
        {
            int size = 0;
            int count = 0;
            for (int i = 0; i < builders.Count; ++i)
            {
                int s = builders.get(i).subDataSizeToSerialize();
                if (s > 0)
                {
                    ++count;
                    size += s;
                }
            }
            if (count > 0)
            {
                size += LOOKUP_OFFSET_BASE + count * LOOKUP_OFFSET_SIZE;
            }

            serializedCount = count;
            serializedLength = size;

            return serializedLength;
        }

        public override int subDataSizeToSerialize()
        {
            if (builders == null)
            {
                if (dataIsCanonical)
                {
                    return internalReadData().length();
                }
                prepareToEdit();
            }
            return computeSerializedSizeFromBuilders();
        }

        public override void subDataSet()
        {
            builders = null;
        }

        public override abstract LookupList subBuildTable(ReadableFontData data);
    }
}
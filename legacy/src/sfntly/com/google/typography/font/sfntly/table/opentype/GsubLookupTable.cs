// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype;



/**
 * @author dougfelt@google.com (Doug Felt)
 */
abstract class GsubLookupTable : LookupTable
{

    public GsubLookupTable(ReadableFontData data, int @base, boolean dataIsCanonical) : base(data, @base, dataIsCanonical)
    {
    }

    new public interface IBuilder<T> : LookupTable.IBuilder where T : GsubLookupTable
    {

    }
    new protected abstract class Builder<T> : LookupTable.Builder, IBuilder<T> where T : GsubLookupTable
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder()
        {
        }

        public Builder(T table) : base(table)
        {
        }
    }
}
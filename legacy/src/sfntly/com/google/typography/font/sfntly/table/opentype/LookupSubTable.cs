// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype;




abstract class LookupSubTable : OTSubTable
{

    public LookupSubTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    // @Override
    // private abstract Builder<LookupSubTable> builder();

    public abstract LookupType lookupType();

    new protected abstract class Builder<T> : OTSubTable.Builder<T> where T : LookupSubTable
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(T table) : base(table)
        {
        }

        public abstract LookupType lookupType();
    }
}
// Copyright 2012 Google Inc. All Rights Reserved.

using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;
using System.Runtime.CompilerServices;

namespace com.google.typography.font.sfntly.table.opentype;




/**
 * @author dougfelt@google.com (Doug Felt)
 */
abstract class GsubLookupSubTable : LookupSubTable
{

    public GsubLookupSubTable(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
    {
    }

    public abstract IBuilder<GsubLookupSubTable> gbuilder();

    public abstract GsubLookupType glookupType();


    public override OTSubTable.IBuilder<OTSubTable> builder()
    {
        return Unsafe.As<OTSubTable.Builder<OTSubTable>>(gbuilder());
    }

    public override LookupType lookupType()
    {
        throw new NotImplementedException();
    }

    new public interface IBuilder<T> : LookupSubTable.IBuilder<T> where T : GsubLookupSubTable
    {

    }
    new protected abstract class Builder<T> : LookupSubTable.Builder<T> where T : GsubLookupSubTable
    {

        public Builder(ReadableFontData data, boolean dataIsCanonical) : base(data, dataIsCanonical)
        {
        }

        public Builder(T table) : base(table)
        {
        }

        //public override abstract GsubLookupType lookupType();
    }
}
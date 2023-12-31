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
using System.Reflection.PortableExecutable;

namespace com.google.typography.font.sfntly.table;

public interface ITableBasedTableBuilder<out T>:Table.IBuilder<T> where T : Table
{

}

/**
 * An abstract base to be used building tables in which the builder can use the
 * table itself to build from.
 *
 * @author Stuart Gill
 *
 * @param <T> the type of table to be built
 */
internal abstract class TableBasedTableBuilder<T> : Table.Builder<T>, ITableBasedTableBuilder<T> where T : Table
{
    private T _table;

    /**
     * Constructor.
     *
     * @param header
     * @param data
     */
    public TableBasedTableBuilder(Header header, WritableFontData data) : base(header, data)
    {

    }

    /**
     * Constructor.
     *
     * @param header
     * @param data
     */
    public TableBasedTableBuilder(Header header, ReadableFontData data) : base(header, data)
    {

    }

    public TableBasedTableBuilder(Header header) : base(header)
    {

    }

    public T table()
    {
        if (this._table == null)
        {
            this._table = this.subBuildTable(this.internalReadData());
        }
        return this._table;
    }

    public override void subDataSet()
    {
        this._table = null;
    }

    public override int subDataSizeToSerialize()
    {
        return 0;
    }

    public override boolean subReadyToSerialize()
    {
        return true;
    }

    public override int subSerialize(WritableFontData newData)
    {
        return 0;
    }

    public override T build()
    {
        if (!this.subReadyToSerialize())
        {
            return null;
        }
        T table = this.table();
        this.notifyPostTableBuild(table);
        return table;
    }
}
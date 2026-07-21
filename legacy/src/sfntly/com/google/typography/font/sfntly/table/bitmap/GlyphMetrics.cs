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
 */
public abstract class GlyphMetrics : SubTable
{

    /**
     * Constructor.
     *
     * @param data
     */
    public GlyphMetrics(ReadableFontData data) : base(data)
    {
    }

    new public interface IBuilder<out TGlyphMetrics> : SubTable.IBuilder<TGlyphMetrics> where TGlyphMetrics : GlyphMetrics
    {

    }

    new protected abstract class Builder<TGlyphMetrics> : SubTable.Builder<TGlyphMetrics>, IBuilder<TGlyphMetrics> where TGlyphMetrics : GlyphMetrics
    {
        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         */
        public Builder(WritableFontData data) : base(data)
        {
        }

        /**
         * Constructor.
         *
         * @param data the data for the subtable being built
         */
        public Builder(ReadableFontData data) : base(data)
        {
        }
    }
}
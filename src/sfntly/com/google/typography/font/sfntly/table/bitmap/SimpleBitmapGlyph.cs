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
 *
 */
public sealed class SimpleBitmapGlyph : BitmapGlyph
{

    public SimpleBitmapGlyph(ReadableFontData data, int format) : base(data, format)
    {
    }
    public static IBuilder createBuilder(WritableFontData data, int format)
    {
        return new Builder(data, format);
    }

    public static IBuilder createBuilder(ReadableFontData data, int format)
    {
        return new Builder(data, format);
    }

    public interface IBuilder : BitmapGlyph.IBuilder<BitmapGlyph>
    {

    }


    private class Builder : BitmapGlyph.Builder<BitmapGlyph>, IBuilder
    {

        public Builder(WritableFontData data, int format) : base(data, format)
        {
        }

        public Builder(ReadableFontData data, int format) : base(data, format)
        {
        }

        public override BitmapGlyph subBuildTable(ReadableFontData data)
        {
            return new SimpleBitmapGlyph(data, this.format());
        }
    }
}
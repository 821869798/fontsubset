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

using com.google.typography.font.sfntly;

namespace com.google.typography.font.tools.subsetter;






/**
 * @author Stuart Gill
 */
public abstract class TableSubsetterImpl : TableSubsetter
{

    public readonly ISet<Integer> tags;

    public TableSubsetterImpl(params Integer[] tags)
    {
        ISet<Integer> temp = new HashSet<Integer>(tags.Length);
        foreach (Integer tag in tags)
        {
            temp.Add(tag);
        }
        this.tags = Collections.unmodifiableSet(temp);
    }

    public abstract bool subset(Subsetter subsetter, Font font, Font.Builder fontBuilder);

    public virtual boolean tagHandled(int tag)
    {
        return this.tags.Contains(tag);
    }

    public virtual ISet<Integer> tagsHandled()
    {
        return this.tags;
    }
}
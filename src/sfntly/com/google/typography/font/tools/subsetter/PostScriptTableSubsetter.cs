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
using com.google.typography.font.sfntly.table.core;

namespace com.google.typography.font.tools.subsetter;









/**
 * @author Raph Levien
 */
public class PostScriptTableSubsetter : TableSubsetterImpl {

  public PostScriptTableSubsetter() :base(Tag.post) {
  }
  
  public override boolean subset(Subsetter subsetter, Font font, Font.Builder fontBuilder) {
    IList<Integer> permutationTable = subsetter.glyphMappingTable();
    if (permutationTable == null) {
      return false;
    }
    PostScriptTableBuilder postBuilder = new PostScriptTableBuilder();
    PostScriptTable post = font.getTable<PostScriptTable>(Tag.post);
    postBuilder.initV1From(post);
    if (post.version() == 0x10000 || post.version() == 0x20000) {
      IList<String> names = new List<String>();
      for (int i = 0; i < permutationTable.Count; i++) {
        names.Add(post.glyphName(permutationTable.get(i)));
      }
      postBuilder.setNames(names);
    }
    fontBuilder.newTableBuilder(Tag.post, postBuilder.build());
    return true;
  }
}

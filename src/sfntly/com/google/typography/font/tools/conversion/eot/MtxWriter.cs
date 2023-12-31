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
using com.google.typography.font.sfntly.table.truetype;

namespace com.google.typography.font.tools.conversion.eot;













/**
 * @author Raph Levien
 */
public class MtxWriter {
  
  private static readonly ISet<Integer> REMOVE_TABLES = createRemoveTables();
  
  private static ISet<Integer> createRemoveTables() {
    ISet<Integer> result = new HashSet<Integer>();
    result.Add(Tag.VDMX);
    result.Add(Tag.glyf);
    result.Add(Tag.cvt);
    result.Add(Tag.loca);
    result.Add(Tag.hdmx);
    result.Add(Tag.head);
    return Collections.unmodifiableSet(result);
  }

  public byte[] compress(Font sfntlyFont) {
    MtxFontBuilder fontBuilder = new MtxFontBuilder();
    foreach(var entry in sfntlyFont.tableMap().entrySet()) {
      Integer tag = entry.getKey();
      if (!REMOVE_TABLES.Contains(tag)) {
        fontBuilder.addTable(tag, entry.getValue().readFontData());
      }
    }
    FontHeaderTable srcHead = sfntlyFont.getTable<FontHeaderTable>(Tag.head);
    fontBuilder.getHeadBuilder().initFrom(srcHead);

    GlyfEncoder glyfEncoder = new GlyfEncoder();
    glyfEncoder.encode(sfntlyFont);
    fontBuilder.addTableBytes(Tag.glyf, glyfEncoder.getGlyfBytes());
    fontBuilder.addTable(Tag.loca, null);

    ControlValueTable cvtTable = sfntlyFont.getTable<ControlValueTable>(Tag.cvt);
    if (cvtTable != null) {
      CvtEncoder cvtEncoder = new CvtEncoder();
      cvtEncoder.encode(cvtTable);
      fontBuilder.addTableBytes(Tag.cvt, cvtEncoder.toByteArray());
    }

    HorizontalDeviceMetricsTable hdmxTable = sfntlyFont.getTable<HorizontalDeviceMetricsTable>(Tag.hdmx);
    if (hdmxTable != null) {
      fontBuilder.addTable(Tag.hdmx, new HdmxEncoder().encode(sfntlyFont));
    }
    
    byte[] block1 = fontBuilder.build();
    byte[] block2 = glyfEncoder.getPushBytes();
    byte[] block3 = glyfEncoder.getCodeBytes();
    return packMtx(block1, block2, block3);
  }

  private static void writeBE24(byte[] data, int value, int off) {
    data[off] = (byte) ((value >> 16) & 0xff);
    data[off + 1] = (byte) ((value >> 8) & 0xff);
    data[off + 2] = (byte) (value & 0xff);
  }

  /**
   * Compress the blocks and pack them into the final container, as per section 2 of the spec.
   */
  private static byte[] packMtx(byte[] block1, byte[] block2, byte[] block3) {
    int copyDist = Math.Max(block1.Length, Math.Max(block2.Length, block3.Length)) +
        LzcompCompress.getPreloadSize();
    byte[] compressed1 = LzcompCompress.compress(block1);
    byte[] compressed2 = LzcompCompress.compress(block2);
    byte[] compressed3 = LzcompCompress.compress(block3);
    int resultSize = 10 + compressed1.Length + compressed2.Length + compressed3.Length;
    byte[] result = new byte[resultSize];
    result[0] = 3;
    writeBE24(result, copyDist, 1);
    int offset2 = 10 + compressed1.Length;
    int offset3 = offset2 + compressed2.Length;
    writeBE24(result, offset2, 4);
    writeBE24(result, offset3, 7);
    Arrays.arraycopy(compressed1, 0, result, 10, compressed1.Length);
    Arrays.arraycopy(compressed2, 0, result, offset2, compressed2.Length);
    Arrays.arraycopy(compressed3, 0, result, offset3, compressed3.Length);
    return result;
  }
}

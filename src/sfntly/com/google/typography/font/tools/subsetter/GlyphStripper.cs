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
using com.google.typography.font.sfntly.table.truetype;

namespace com.google.typography.font.tools.subsetter;








/**
 * Strip the hints from one glyph.
 * 
 * @author Raph Levien
 */
public class GlyphStripper {
  private readonly GlyphTable.IBuilder glyphTableBuilder;

  public GlyphStripper(GlyphTable.IBuilder glyphTableBuilder) {
    this.glyphTableBuilder = glyphTableBuilder;
  }

  public Glyph.IBuilder<Glyph> stripGlyph(Glyph glyph) {
    WritableFontData newGlyphData = null;
    if (glyph != null && glyph.readFontData().length() > 0) {
      switch (glyph.glyphType()) {
        case Glyph.GlyphType. Simple:
          newGlyphData = stripSimpleGlyph(glyph);
          break;
        case Glyph.GlyphType.Composite:
          newGlyphData = stripCompositeGlyph(glyph);
          break;
        default:
          break;
      }
    }
    if (newGlyphData == null) {
      newGlyphData = WritableFontData.createWritableFontData(0);
    }
    return glyphTableBuilder.glyphBuilder(newGlyphData);
  }

  private WritableFontData stripSimpleGlyph(Glyph glyph) {
    int size = computeSimpleStrippedGlyphSize(glyph);
    int paddedSize = (size + 1) & -2;
    // TODO(stuartg): look into this issue
    // Note: padding up the size of the data blocks is quite an unpleasant hack.
    // The sfntly builder
    // objects should be able to take glyph subtables of arbitrary size and
    // assemble them into a
    // correctly padded table, whether the loca format is large or small.
    // However, it just blithely
    // fails to add padding. We sidestep the issue by adding padding,
    WritableFontData newGlyf = WritableFontData.createWritableFontData(paddedSize);
    SimpleGlyph simpleGlyph = (SimpleGlyph) glyph;
    ReadableFontData originalGlyfData = glyph.readFontData();

    int dataWritten =
        writeHeaderAndContoursSize(newGlyf, 0, originalGlyfData, 0, simpleGlyph);
    dataWritten += writeZeroInstructionLength(newGlyf, dataWritten);
    dataWritten +=
        writeEndSimpleGlyph(newGlyf, dataWritten, originalGlyfData, dataWritten
            + (simpleGlyph.instructionSize() * (int)ReadableFontData.DataSize.BYTE), size
            - dataWritten);
    return newGlyf;
  }

  private int writeHeaderAndContoursSize(WritableFontData newGlyf, int newGlyfOffset,
      ReadableFontData originalGlyfData, int glyphOffset, SimpleGlyph simpleGlyph) {
    int headerAndNumberOfContoursSize =
        ((int)ReadableFontData.DataSize.SHORT * 5)
            + (simpleGlyph.numberOfContours() * (int)ReadableFontData.DataSize.USHORT);
    WritableFontData newGlyfSlice = newGlyf.slice(newGlyfOffset, headerAndNumberOfContoursSize);

    originalGlyfData.slice(glyphOffset, headerAndNumberOfContoursSize).copyTo(newGlyfSlice);
    return headerAndNumberOfContoursSize;
  }

  private int writeZeroInstructionLength(WritableFontData newGlyf, int offset) {
    newGlyf.writeUShort(offset, 0);
    return (int)ReadableFontData.DataSize.USHORT;
  }

  private int writeEndSimpleGlyph(WritableFontData newGlyf, int newGlyfOffset,
      ReadableFontData originalGlyfData, int glyphOffset, int length) {
    ReadableFontData originalGlyfSlice = originalGlyfData.slice(glyphOffset, length);
    WritableFontData newGlyfSlice = newGlyf.slice(newGlyfOffset, length);

    originalGlyfSlice.copyTo(newGlyfSlice);
    return length;
  }

  private WritableFontData stripCompositeGlyph(Glyph glyph) {
    int dataLength = computeCompositeStrippedGlyphSize(glyph);
    WritableFontData newGlyf = WritableFontData.createWritableFontData(dataLength);
    CompositeGlyph compositeGlyph = (CompositeGlyph) glyph;
    ReadableFontData originalGlyphSlice = glyph.readFontData().slice(0, dataLength);

    originalGlyphSlice.copyTo(newGlyf);
    if (compositeGlyph.instructionSize() > 0) {
      overrideCompositeGlyfFlags(newGlyf, dataLength);
    }
    return newGlyf;
  }

  private void overrideCompositeGlyfFlags(WritableFontData slice, int dataLength) {
    int index = 5 * (int)ReadableFontData.DataSize.USHORT;
    int flags = CompositeGlyph.FLAG_MORE_COMPONENTS;
    while ((flags & CompositeGlyph.FLAG_MORE_COMPONENTS) != 0) {
      flags = slice.readUShort(index);
      flags &= ~CompositeGlyph.FLAG_WE_HAVE_INSTRUCTIONS;
      slice.writeUShort(index, flags);
      index += 2 * (int)ReadableFontData.DataSize.USHORT;
      if ((flags & CompositeGlyph.FLAG_ARG_1_AND_2_ARE_WORDS) != 0) {
        index += 2 * (int)ReadableFontData.DataSize.SHORT;
      } else {
        index += 2 * (int)ReadableFontData.DataSize.BYTE;
      }
      if ((flags & CompositeGlyph.FLAG_WE_HAVE_A_SCALE) != 0) {
        index += (int)ReadableFontData.DataSize.F2DOT14;
      } else if ((flags & CompositeGlyph.FLAG_WE_HAVE_AN_X_AND_Y_SCALE) != 0) {
        index += 2 * (int)ReadableFontData.DataSize.F2DOT14;
      } else if ((flags & CompositeGlyph.FLAG_WE_HAVE_A_TWO_BY_TWO) != 0) {
        index += 4 * (int)ReadableFontData.DataSize.F2DOT14;
      }
    }
  }

  private int computeSimpleStrippedGlyphSize(Glyph glyph) {
    SimpleGlyph simpleGlyph = (SimpleGlyph) glyph;

    // Compute instruction size before querying padding, to work around a bug in
    // sfntly.
    int instructionSize = simpleGlyph.instructionSize();
    int nonPaddedSimpleGlyphLength = simpleGlyph.dataLength() - simpleGlyph.padding();

    if (instructionSize > 0) {
      return nonPaddedSimpleGlyphLength - computeInstructionsSize(simpleGlyph);
    }
    return nonPaddedSimpleGlyphLength;
  }

  private int computeInstructionsSize(SimpleGlyph simpleGlyph) {
    return simpleGlyph.instructionSize() * (int)ReadableFontData.DataSize.BYTE;
  }

  private int computeCompositeStrippedGlyphSize(Glyph glyph) {
    CompositeGlyph compositeGlyph = (CompositeGlyph) glyph;
    int instructionSize = compositeGlyph.instructionSize();
    int nonPaddedCompositeGlyphLength = compositeGlyph.dataLength() - compositeGlyph.padding();

    if (instructionSize > 0) {
      return nonPaddedCompositeGlyphLength
          - (instructionSize * (int)ReadableFontData.DataSize.BYTE)
          - (int)ReadableFontData.DataSize.USHORT;
    }
    return nonPaddedCompositeGlyphLength;
  }
}

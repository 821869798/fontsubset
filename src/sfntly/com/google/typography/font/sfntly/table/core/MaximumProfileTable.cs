/*
 * Copyright 2010 Google Inc. All Rights Reserved.
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

namespace com.google.typography.font.sfntly.table.core;







/**
 * A Maximum Profile table - 'maxp'.
 *
 * @author Stuart Gill
 */
public sealed class MaximumProfileTable : Table {

  /**
   * Offsets to specific elements in the underlying data. These offsets are relative to the
   * start of the table or the start of sub-blocks within the table.
   */
  private enum Offset {
    // version 0.5 and 1.0
    version = (0),
    numGlyphs=(4),

    // version 1.0
    maxPoints=(6),
    maxContours=(8),
    maxCompositePoints=(10),
    maxCompositeContours=(12),
    maxZones=(14),
    maxTwilightPoints=(16),
    maxStorage=(18),
    maxFunctionDefs=(20),
    maxInstructionDefs=(22),
    maxStackElements=(24),
    maxSizeOfInstructions=(26),
    maxComponentElements=(28),
    maxComponentDepth=(30)/*;

    private readonly int offset;
    private (int)Offset.int) {
      this.offset = offset;
    }*/
  }

  private MaximumProfileTable(Header header, ReadableFontData data) :base(header, data) {
  }

  public int tableVersion() {
    return this._data.readFixed((int)Offset.version);
  }

  public int numGlyphs() {
    return this._data.readUShort((int)Offset.numGlyphs);
  }

  public int maxPoints() {
    return this._data.readUShort((int)Offset.maxPoints);
  }

  public int maxContours() {
    return this._data.readUShort((int)Offset.maxContours);
  }

  public int maxCompositePoints() {
    return this._data.readUShort((int)Offset.maxCompositePoints);
  }
  
  public int maxCompositeContours() {
    return this._data.readUShort((int)Offset.maxCompositeContours);
  }

  public int maxZones() {
    return this._data.readUShort((int)Offset.maxZones);
  }

  public int maxTwilightPoints() {
    return this._data.readUShort((int)Offset.maxTwilightPoints);
  }

  public int maxStorage() {
    return this._data.readUShort((int)Offset.maxStorage);
  }

  public int maxFunctionDefs() {
    return this._data.readUShort((int)Offset.maxFunctionDefs);
  }

  public int maxStackElements() {
    return this._data.readUShort((int)Offset.maxStackElements);
  }

  public int maxSizeOfInstructions() {
    return this._data.readUShort((int)Offset.maxSizeOfInstructions);
  }

  public int maxComponentElements() {
    return this._data.readUShort((int)Offset.maxComponentElements);
  }

  public int maxComponentDepth() {
    return this._data.readUShort((int)Offset.maxComponentDepth);
    }

    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static IBuilder createBuilder(Header header, WritableFontData data)
    {
        return new Builder(header, data);
    }

    public interface IBuilder : ITableBasedTableBuilder<MaximumProfileTable>
    {

        int tableVersion();

        void setTableVersion(int version);
        int numGlyphs();
        void setNumGlyphs(int numGlyphs);
        int maxPoints();
        void maxPoints(int maxPoints);

        int maxContours()
        ;

        void setMaxContours(int maxContours)
        ;

        int maxCompositePoints()
        ;

        void setMaxCompositePoints(int maxCompositePoints)
        ;

        int maxCompositeContours()
        ;

        void setMaxCompositeContours(int maxCompositeContours)
        ;

        int maxZones()
        ;

        void setMaxZones(int maxZones)
        ;

        int maxTwilightPoints()
        ;

        void setMaxTwilightPoints(int maxTwilightPoints)
        ;

        int maxStorage()
        ;

        void setMaxStorage(int maxStorage)
        ;

        int maxFunctionDefs()
        ;

        void setMaxFunctionDefs(int maxFunctionDefs)
        ;

        int maxStackElements()
        ;

        void setMaxStackElements(int maxStackElements)
        ;

        int maxSizeOfInstructions()
        ;

        void setMaxSizeOfInstructions(int maxSizeOfInstructions)
        ;

        int maxComponentElements()
        ;

        void setMaxComponentElements(int maxComponentElements);

        int maxComponentDepth();

        void setMaxComponentDepth(int maxComponentDepth);
    }

    /**
     * Builder for a Maximum Profile table - 'maxp'.
     *
     */
    private sealed class Builder : TableBasedTableBuilder<MaximumProfileTable>, IBuilder
    {
    
    public Builder(Header header, WritableFontData data) :base(header, data) {
    }

    public Builder(Header header, ReadableFontData data) :base(header, data) {
    }

    public override MaximumProfileTable subBuildTable(ReadableFontData data) {
      return new MaximumProfileTable(this.header(), data);
    }

    public int tableVersion() {
      return this.internalReadData().readUShort((int)Offset.version);
    }

    public void setTableVersion(int version) {
      this.internalWriteData().writeUShort((int)Offset.version, version);
    }

    public int numGlyphs() {
      return this.internalReadData().readUShort((int)Offset.numGlyphs);
    }

    public void setNumGlyphs(int numGlyphs) {
      this.internalWriteData().writeUShort((int)Offset.numGlyphs, numGlyphs);
    }

    public int maxPoints() {
      return this.internalReadData().readUShort((int)Offset.maxPoints);
    }

    public void maxPoints(int maxPoints) {
      this.internalWriteData().writeUShort((int)Offset.maxPoints, maxPoints);
    }

    public int maxContours() {
      return this.internalReadData().readUShort((int)Offset.maxContours);
    }

    public void setMaxContours(int maxContours) {
      this.internalWriteData().writeUShort((int)Offset.maxContours, maxContours);
    }

    public int maxCompositePoints() {
      return this.internalReadData().readUShort((int)Offset.maxCompositePoints);
    }

    public void setMaxCompositePoints(int maxCompositePoints) {
      this.internalWriteData().writeUShort((int)Offset.maxCompositePoints, maxCompositePoints);
    }
    
    public int maxCompositeContours() {
      return this.internalReadData().readUShort((int)Offset.maxCompositeContours);
    }

    public void setMaxCompositeContours(int maxCompositeContours) {
      this.internalWriteData().writeUShort(
          (int)Offset.maxCompositeContours, maxCompositeContours);
    }

    public int maxZones() {
      return this.internalReadData().readUShort((int)Offset.maxZones);
    }

    public void setMaxZones(int maxZones) {
      this.internalWriteData().writeUShort((int)Offset.maxZones, maxZones);
    }

    public int maxTwilightPoints() {
      return this.internalReadData().readUShort((int)Offset.maxTwilightPoints);
    }

    public void setMaxTwilightPoints(int maxTwilightPoints) {
      this.internalWriteData().writeUShort((int)Offset.maxTwilightPoints, maxTwilightPoints);
    }

    public int maxStorage() {
      return this.internalReadData().readUShort((int)Offset.maxStorage);
    }

    public void setMaxStorage(int maxStorage) {
      this.internalWriteData().writeUShort((int)Offset.maxStorage, maxStorage);
    }

    public int maxFunctionDefs() {
      return this.internalReadData().readUShort((int)Offset.maxFunctionDefs);
    }

    public void setMaxFunctionDefs(int maxFunctionDefs) {
      this.internalWriteData().writeUShort((int)Offset.maxFunctionDefs, maxFunctionDefs);
    }

    public int maxStackElements() {
      return this.internalReadData().readUShort((int)Offset.maxStackElements);
    }

    public void setMaxStackElements(int maxStackElements) {
      this.internalWriteData().writeUShort((int)Offset.maxStackElements, maxStackElements);
    }

    public int maxSizeOfInstructions() {
      return this.internalReadData().readUShort((int)Offset.maxSizeOfInstructions);
    }

    public void setMaxSizeOfInstructions(int maxSizeOfInstructions) {
      this.internalWriteData().writeUShort(
          (int)Offset.maxSizeOfInstructions, maxSizeOfInstructions);
    }

    public int maxComponentElements() {
      return this.internalReadData().readUShort((int)Offset.maxComponentElements);
    }

    public void setMaxComponentElements(int maxComponentElements) {
      this.internalWriteData().writeUShort(
          (int)Offset.maxComponentElements, maxComponentElements);
    }

    public int maxComponentDepth() {
      return this.internalReadData().readUShort((int)Offset.maxComponentDepth);
    }

    public void setMaxComponentDepth(int maxComponentDepth) {
      this.internalWriteData().writeUShort((int)Offset.maxComponentDepth, maxComponentDepth);
    }
  }
}

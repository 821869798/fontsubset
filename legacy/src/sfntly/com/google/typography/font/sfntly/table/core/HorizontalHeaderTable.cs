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
 * A Horizontal Header table - 'hhea'.
 *
 * @author Stuart Gill
 */
public sealed class HorizontalHeaderTable : Table {

  /**
   * Offsets to specific elements in the underlying data. These offsets are relative to the
   * start of the table or the start of sub-blocks within the table.
   */
  private enum Offset {
    version=(0),
    Ascender=(4),
    Descender=(6),
    LineGap=(8),
    advanceWidthMax=(10),
    minLeftSideBearing=(12),
    minRightSideBearing=(14),
    xMaxExtent=(16),
    caretSlopeRise=(18),
    caretSlopeRun=(20),
    caretOffset=(22),
    metricDataFormat=(32),
    numberOfHMetrics=(34)/*;

    private readonly int offset;

    private (int)Offset.int) {
      this.offset = offset;
    }*/
  }

  private HorizontalHeaderTable(Header header, ReadableFontData data) :base(header, data) {
  }

  public int tableVersion() {
    return this._data.readFixed((int)Offset.version);
  }

  public int ascender() {
    return this._data.readShort((int)Offset.Ascender);
  }

  public int descender() {
    return this._data.readShort((int)Offset.Descender);
  }

  public int lineGap() {
    return this._data.readShort((int)Offset.LineGap);
  }

  public int advanceWidthMax() {
    return this._data.readUShort((int)Offset.advanceWidthMax);
  }

  public int minLeftSideBearing() {
    return this._data.readShort((int)Offset.minLeftSideBearing);
  }

  public int minRightSideBearing() {
    return this._data.readShort((int)Offset.minRightSideBearing);
  }

  public int xMaxExtent() {
    return this._data.readShort((int)Offset.xMaxExtent);
  }

  public int caretSlopeRise() {
    return this._data.readShort((int)Offset.caretSlopeRise);
  }

  public int caretSlopeRun() {
    return this._data.readShort((int)Offset.caretSlopeRun);
  }

  public int caretOffset() {
    return this._data.readShort((int)Offset.caretOffset);
  }

  // TODO(stuartg): an enum?
  public int metricDataFormat() {
    return this._data.readShort((int)Offset.metricDataFormat);
  }

  public int numberOfHMetrics() {
    return this._data.readUShort((int)Offset.numberOfHMetrics);
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
    public interface IBuilder : ITableBasedTableBuilder<HorizontalHeaderTable>
    {

        int tableVersion();

        void setTableVersion(int version);

        int ascender();

        void setAscender(int version);

        int descender();

        void setDescender(int version);

        int lineGap();

        void setLineGap(int version);

        int advanceWidthMax();

        void setAdvanceWidthMax(int version);

        int minLeftSideBearing();

        void setMinLeftSideBearing(int version);

        int minRightSideBearing();

        void setMinRightSideBearing(int version);

        int xMaxExtent();

        void setXMaxExtent(int version);

        int caretSlopeRise();

        void setCaretSlopeRise(int version);

        int caretSlopeRun();

        void setCaretSlopeRun(int version);

        int caretOffset();

        void setCaretOffset(int version);

        // TODO(stuartg): an enum?
        int metricDataFormat();

        void setMetricDataFormat(int version);

        int numberOfHMetrics();

         void setNumberOfHMetrics(int version)
        ;
    }

    /**
     * Builder for a Horizontal Header table - 'hhea'.
     *
     */
    private class Builder : TableBasedTableBuilder<HorizontalHeaderTable>, IBuilder
    {


    public Builder(Header header, WritableFontData data) :base(header, data) {
    }

    public Builder(Header header, ReadableFontData data) :base(header, data) {
    }

    public override HorizontalHeaderTable subBuildTable(ReadableFontData data) {
      return new HorizontalHeaderTable(this.header(), data);
    }

    public int tableVersion() {
      return this.internalReadData().readFixed((int)Offset.version);
    }

    public void setTableVersion(int version) {
      this.internalWriteData().writeFixed((int)Offset.version, version);
    }

    public int ascender() {
      return this.internalReadData().readShort((int)Offset.Ascender);
    }

    public void setAscender(int version) {
      this.internalWriteData().writeShort((int)Offset.Ascender, version);
    }

    public int descender() {
      return this.internalReadData().readShort((int)Offset.Descender);
    }

    public void setDescender(int version) {
      this.internalWriteData().writeShort((int)Offset.Descender, version);
    }

    public int lineGap() {
      return this.internalReadData().readShort((int)Offset.LineGap);
    }

    public void setLineGap(int version) {
      this.internalWriteData().writeShort((int)Offset.LineGap, version);
    }

    public int advanceWidthMax() {
      return this.internalReadData().readUShort((int)Offset.advanceWidthMax);
    }

    public void setAdvanceWidthMax(int version) {
      this.internalWriteData().writeUShort((int)Offset.advanceWidthMax, version);
    }

    public int minLeftSideBearing() {
      return this.internalReadData().readShort((int)Offset.minLeftSideBearing);
    }

    public void setMinLeftSideBearing(int version) {
      this.internalWriteData().writeShort((int)Offset.minLeftSideBearing, version);
    }

    public int minRightSideBearing() {
      return this.internalReadData().readShort((int)Offset.minRightSideBearing);
    }

    public void setMinRightSideBearing(int version) {
      this.internalWriteData().writeShort((int)Offset.minRightSideBearing, version);
    }

    public int xMaxExtent() {
      return this.internalReadData().readShort((int)Offset.xMaxExtent);
    }

    public void setXMaxExtent(int version) {
      this.internalWriteData().writeShort((int)Offset.xMaxExtent, version);
    }

    public int caretSlopeRise() {
      return this.internalReadData().readUShort((int)Offset.caretSlopeRise);
    }

    public void setCaretSlopeRise(int version) {
      this.internalWriteData().writeUShort((int)Offset.caretSlopeRise, version);
    }

    public int caretSlopeRun() {
      return this.internalReadData().readUShort((int)Offset.caretSlopeRun);
    }

    public void setCaretSlopeRun(int version) {
      this.internalWriteData().writeUShort((int)Offset.caretSlopeRun, version);
    }

    public int caretOffset() {
      return this.internalReadData().readUShort((int)(int)Offset.caretOffset);
    }

    public void setCaretOffset(int version) {
      this.internalWriteData().writeUShort((int)(int)Offset.caretOffset, version);
    }

    // TODO(stuartg): an enum?
    public int metricDataFormat() {
      return this.internalReadData().readUShort((int)Offset.metricDataFormat);
    }

    public void setMetricDataFormat(int version) {
      this.internalWriteData().writeUShort((int)Offset.metricDataFormat, version);
    }

    public int numberOfHMetrics() {
      return this.internalReadData().readUShort((int)Offset.numberOfHMetrics);
    }

    public void setNumberOfHMetrics(int version) {
      this.internalWriteData().writeUShort((int)Offset.numberOfHMetrics, version);
    }
  }
}

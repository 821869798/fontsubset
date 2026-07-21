using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.opentype.component;

namespace com.google.typography.font.sfntly.table.opentype.chaincontextsubst;








public class InnerArraysFmt3 : VisibleSubTable
{
  public readonly CoverageArray backtrackGlyphs;
  public readonly CoverageArray inputGlyphs;
  public readonly CoverageArray lookAheadGlyphs;
  public readonly SubstLookupRecordList lookupRecords;

  // //////////////
  // Constructors

  public InnerArraysFmt3(ReadableFontData data, int @base, boolean dataIsCanonical) :base(data) {
    NumRecordList records = new NumRecordList(data, 0, @base);
    backtrackGlyphs = new CoverageArray(records);

    records = new NumRecordList(data, 0, records.limit());
    inputGlyphs = new CoverageArray(records);

    records = new NumRecordList(data, 0, records.limit());
    lookAheadGlyphs = new CoverageArray(records);

    lookupRecords = new SubstLookupRecordList(data, records.limit());
  }

  private class Builder : VisibleSubTable.Builder<InnerArraysFmt3> {
    private CoverageArray.IBuilder backtrackGlyphsBuilder;
    private CoverageArray.IBuilder inputGlyphsBuilder;
    private CoverageArray.IBuilder lookAheadGlyphsBuilder;
    private SubstLookupRecordList lookupRecordsBuilder;

    public Builder() :base() {
    }

    public Builder(InnerArraysFmt3 table) :this(table.readFontData(), 0, false) {
    }

    public Builder(ReadableFontData data, int @base, boolean dataIsCanonical) :base(data) {
      if (!dataIsCanonical) {
        prepareToEdit();
      }
    }

    public Builder(Builder other) :base() {
      backtrackGlyphsBuilder = other.backtrackGlyphsBuilder;
      inputGlyphsBuilder = other.inputGlyphsBuilder;
      lookAheadGlyphsBuilder = other.lookAheadGlyphsBuilder;
      lookupRecordsBuilder = other.lookupRecordsBuilder;
    }

    public override int subDataSizeToSerialize() {
      if (lookupRecordsBuilder != null) {
        serializedLength = lookupRecordsBuilder.limit();
      } else {
        computeSizeFromData(internalReadData());
      }
      return serializedLength;
    }

    public override int subSerialize(WritableFontData newData) {
      if (serializedLength == 0) {
        return 0;
      }

      if (backtrackGlyphsBuilder == null || inputGlyphsBuilder == null
          || lookAheadGlyphsBuilder == null || lookupRecordsBuilder == null) {
        return serializeFromData(newData);
      }

      int tableOnlySize = 0;
      tableOnlySize += backtrackGlyphsBuilder.tableSizeToSerialize();
      tableOnlySize += inputGlyphsBuilder.tableSizeToSerialize();
      tableOnlySize += lookAheadGlyphsBuilder.tableSizeToSerialize();
      int subTableWriteOffset = tableOnlySize
          + lookupRecordsBuilder.writeTo(newData.slice(tableOnlySize));

      backtrackGlyphsBuilder.subSerialize(newData, subTableWriteOffset);
      subTableWriteOffset += backtrackGlyphsBuilder.subTableSizeToSerialize();
      int tableWriteOffset = backtrackGlyphsBuilder.tableSizeToSerialize();

      inputGlyphsBuilder.subSerialize(newData.slice(tableWriteOffset), subTableWriteOffset);
      subTableWriteOffset += inputGlyphsBuilder.subTableSizeToSerialize();
      tableWriteOffset += inputGlyphsBuilder.tableSizeToSerialize();

      lookAheadGlyphsBuilder.subSerialize(newData.slice(tableWriteOffset), subTableWriteOffset);
      subTableWriteOffset += lookAheadGlyphsBuilder.subTableSizeToSerialize();

      return subTableWriteOffset;
    }

    public override InnerArraysFmt3 subBuildTable(ReadableFontData data) {
      return new InnerArraysFmt3(data, 0, true);
    }

    public override boolean subReadyToSerialize() {
      return true;
    }

    public override void subDataSet() {
      backtrackGlyphsBuilder = null;
      inputGlyphsBuilder = null;
      lookupRecordsBuilder = null;
      lookAheadGlyphsBuilder = null;
    }

    // ////////////////////////////////////
    // private methods

    private void prepareToEdit() {
      initFromData(internalReadData());
      setModelChanged();
    }

    private void initFromData(ReadableFontData data) {
      if (backtrackGlyphsBuilder == null || inputGlyphsBuilder == null
          || lookAheadGlyphsBuilder == null || lookupRecordsBuilder == null) {
        NumRecordList records = new NumRecordList(data);
        backtrackGlyphsBuilder = CoverageArray.createBuilder(records);

        records = new NumRecordList(data, 0, records.limit());
        inputGlyphsBuilder = CoverageArray.createBuilder(records);

        records = new NumRecordList(data, 0, records.limit());
        lookAheadGlyphsBuilder = CoverageArray.createBuilder(records);

        lookupRecordsBuilder = new SubstLookupRecordList(data, records.limit());
      }
    }

    private void computeSizeFromData(ReadableFontData data) {
      // This assumes canonical data.
      int len = 0;
      if (data != null) {
        len = data.length();
      }
      serializedLength = len;
    }

    private int serializeFromData(WritableFontData newData) {
      // The source data must be canonical.
      ReadableFontData data = internalReadData();
      data.copyTo(newData);
      return data.length();
    }
  }
}

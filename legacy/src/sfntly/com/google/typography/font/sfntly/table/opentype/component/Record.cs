using com.google.typography.font.sfntly.data;

namespace com.google.typography.font.sfntly.table.opentype.component;



public interface Record {
  int writeTo(WritableFontData newData, int @base);
}

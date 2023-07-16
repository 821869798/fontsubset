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
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.core;

namespace com.google.typography.font.tools.conversion.eot;













/**
 * @author Jeremie Lenfant-Engelmann
 */
public class EOTWriter {

  private readonly boolean compressed;
  
  private readonly FontFactory factory = FontFactory.getInstance();

  private readonly static long RESERVED = 0;
  private readonly static short PADDING = 0;
  private readonly static long VERSION = 0x00020002;
  private readonly static short MAGIC_NUMBER = 0x504c;
  private readonly static long DEFAULT_FLAGS = 0;
  private readonly static long FLAGS_TT_COMPRESSED = 0x4;
  private readonly static byte DEFAULT_CHARSET = 1;
  private readonly static long CS_XORKEY = 0x50475342;

  public EOTWriter() {
    compressed = false;
  }
  
  public EOTWriter(boolean compressed) {
    this.compressed = compressed;
  }

  public WritableFontData convert(Font font)  {
    ByteArrayOutputStream baos = new ByteArrayOutputStream();
    factory.serializeFont(font, baos);
    byte[] fontData = baos.ToArray();
    NameTable name = font.getTable<NameTable>(Tag.name);
    byte[] familyName = convertUTF16StringToLittleEndian(name.nameAsBytes(3, 1, 0x409, 1));
    byte[] styleName = convertUTF16StringToLittleEndian(name.nameAsBytes(3, 1, 0x409, 2));
    byte[] versionName = convertUTF16StringToLittleEndian(name.nameAsBytes(3, 1, 0x409, 5));
    byte[] fullName = convertUTF16StringToLittleEndian(name.nameAsBytes(3, 1, 0x409, 4));
    long flags = DEFAULT_FLAGS;
    
    if (compressed) {
      flags |= FLAGS_TT_COMPRESSED;
      MtxWriter mtxWriter = new MtxWriter();
      fontData = mtxWriter.compress(font);
    }
    
    long eotSize = computeEotSize(
      familyName.Length, styleName.Length, versionName.Length, fullName.Length, fontData.Length);

    WritableFontData writableFontData = createWritableFontData((int) eotSize);

    OS2Table os2Table = font.getTable<OS2Table>(Tag.OS_2);
    int index = 0;

    index += writableFontData.writeULongLE(index, eotSize); // EOTSize
    index += writableFontData.writeULongLE(index, fontData.Length); // FontDataSize
    index += writableFontData.writeULongLE(index, VERSION); // Version
    index += writableFontData.writeULongLE(index, flags); // Flags
    index += writeFontPANOSE(index, os2Table, writableFontData); // FontPANOSE
    index += writableFontData.writeByte(index, DEFAULT_CHARSET); // Charset
    index += writableFontData.writeByte(index, (byte) (os2Table.fsSelectionAsInt() & 1)); // Italic
    index += writableFontData.writeULongLE(index, os2Table.usWeightClass()); // Weight
    index += writableFontData.writeUShortLE(index, (short) os2Table.fsTypeAsInt()); // fsType
    index += writableFontData.writeUShortLE(index, MAGIC_NUMBER); // MagicNumber
    index += writeUnicodeRanges(index, os2Table, writableFontData); // UnicodeRange{1, 2, 3, 4}
    index += writeCodePages(index, os2Table, writableFontData); // CodePageRange{1, 2}

    FontHeaderTable head = font.getTable<FontHeaderTable>(Tag.head);
    index += writableFontData.writeULongLE(index, head.checkSumAdjustment()); // CheckSumAdjustment

    index += writeReservedFields(index, writableFontData); // Reserved{1, 2, 3, 4}
    index += writePadding(index, writableFontData);

    // FamilyNameSize, FamilyName[FamilyNameSize]
    index += writeUTF16String(index, familyName, writableFontData); 
    index += writePadding(index, writableFontData);

    // StyleNameSize, StyleName[StyleNameSize]
    index += writeUTF16String(index, styleName, writableFontData);
    index += writePadding(index, writableFontData);

    // VersionNameSize, VersionName[VersionNameSize]
    index += writeUTF16String(index, versionName, writableFontData);
    index += writePadding(index, writableFontData);

    // FullNameSize, FullName[FullNameSize]
    index += writeUTF16String(index, fullName, writableFontData);
    index += writePadding(index, writableFontData);

    index += writePadding(index, writableFontData); // RootStringSize
    if (VERSION > 0x20001) {
      index += writableFontData.writeULongLE(index, CS_XORKEY);  // RootStringCheckSum
      index += writableFontData.writeULongLE(index, 0);  // EUDCCodePage
      index += writePadding(index, writableFontData);
      index += writePadding(index, writableFontData);  // SignatureSize
      index += writableFontData.writeULongLE(index, 0);  // EUDCFlags
      index += writableFontData.writeULongLE(index, 0);  // EUDCFontSize
    }
    writableFontData.writeBytes(index, fontData, 0, fontData.Length); // FontData[FontDataSize]
    return writableFontData;
  }

  private long computeEotSize(int familyNameSize, int styleNameSize, int versionNameSize,
      int fullNameSize, int fontDataSize) {
    return 16 * (int)ReadableFontData.DataSize.ULONG +
        12 * (int)ReadableFontData.DataSize.BYTE +
        12 * (int)ReadableFontData.DataSize.USHORT +
        familyNameSize * (int)ReadableFontData.DataSize.BYTE +
        styleNameSize * (int)ReadableFontData.DataSize.BYTE +
        versionNameSize * (int)ReadableFontData.DataSize.BYTE +
        fullNameSize * (int)ReadableFontData.DataSize.BYTE +
        fontDataSize * (int)ReadableFontData.DataSize.BYTE +
        (VERSION > 0x20001 ? 5 * (int)ReadableFontData.DataSize.ULONG : 0);
  }

  private int writeFontPANOSE(int index, OS2Table os2Table, WritableFontData writableFontData) {
    byte[] fontPANOSE = os2Table.panose();
    return writableFontData.writeBytes(index, fontPANOSE, 0, fontPANOSE.Length);
  }

  private int writeReservedFields(int start, WritableFontData writableFontData) {
    int index = start;
    for (int i = 0; i < 4; i++) {
      index += writableFontData.writeULongLE(index, RESERVED);
    }
    return index - start;
  }

  private int writeUnicodeRanges(int start, OS2Table os2Table, WritableFontData writableFontData) {
    int index = start;

    // TODO: change to loop when os2Table.ulUnicodeRange() is implemented
    index += writableFontData.writeULongLE(index, os2Table.ulUnicodeRange1());
    index += writableFontData.writeULongLE(index, os2Table.ulUnicodeRange2());
    index += writableFontData.writeULongLE(index, os2Table.ulUnicodeRange3());
    index += writableFontData.writeULongLE(index, os2Table.ulUnicodeRange4());
    return index - start;
  }

  private int writeCodePages(int start, OS2Table os2Table, WritableFontData writableFontData) {
    int index = start;
    if (os2Table.tableVersion() >= 1) {
    	index += writableFontData.writeULongLE(index, os2Table.ulCodePageRange1());
    	index += writableFontData.writeULongLE(index, os2Table.ulCodePageRange2());
    }
    else {
    	index += writableFontData.writeULongLE(index, 0x00000001);
    	index += writableFontData.writeULongLE(index, 0x00000000);
    }
    return index - start;
  }

  private int writePadding(int index, WritableFontData writableFontData) {
    return writableFontData.writeUShortLE(index, PADDING);
  }

  private int writeUTF16String(int start, byte[] str, WritableFontData writableFontData) {
    int index = start;
    index += writableFontData.writeUShortLE(index, (short) str.Length);
    index += writableFontData.writeBytes(index, str, 0, str.Length);
    return index - start;
  }

  private byte[] convertUTF16StringToLittleEndian(byte[] bytesString) {
    if (bytesString == null) {
      return new byte[0];
    }
    for (int i = 0; i < bytesString.Length; i+= 2) {
      byte tmp = bytesString[i];
      bytesString[i] = bytesString[i + 1];
      bytesString[i + 1] = tmp;
    }
    return bytesString;
  }

  private WritableFontData createWritableFontData(int length) {
    return WritableFontData.createWritableFontData(length);
  }
}

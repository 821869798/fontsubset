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

namespace com.google.typography.font.sfntly.data;

/**
 * A growable memory implementation of the ByteArray interface.
 * 
 * @author Stuart Gill
 */
sealed class GrowableMemoryByteArray : ByteArray {

  private static readonly int INITIAL_LENGTH = 256;
  private byte[] b;

  public GrowableMemoryByteArray():base(0, Integer.MaxValue, true /*growable*/) {
    
    b = new byte[INITIAL_LENGTH];
  }

  public override void internalPut(int index, byte b) {
    growTo(index + 1);
    this.b[index] = b;
  }

  public override int internalPut(int index, byte[] b, int offset, int length) {
    growTo(index + length);
    Arrays.arraycopy(b, offset, this.b, index, length);
    return length;
  }

  public override int internalGet(int index) {
    return this.b[index];
  }

  public override int internalGet(int index, byte[] b, int offset, int length) {
    Arrays.arraycopy(this.b, index, b, offset, length);
    return length;
  }

  public override void close() {
   
    this.b = null;
  }
  
  public override int copyTo(OutputStream os, int offset, int length)  {
    os.write(b, offset, length);
    return length;
  }

  private void growTo(int newSize) {
    if (newSize <= b.Length) {
      return;
    }
    newSize = Math.Max(newSize, b.Length * 2);
    byte[] newArray = new byte[newSize];
    Arrays.arraycopy(b, 0, newArray, 0, b.Length);
    b = newArray;
  }
}

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
 * An abstraction to a contiguous array of bytes.
 *
 * @param <T> the concrete sub-class of ByteArray
 *
 * @author Stuart Gill
 */
public abstract class ByteArray
{
  private static readonly int COPY_BUFFER_SIZE = 8192;

  private boolean _bound;

  private int _storageLength;
  private int _filledLength;
  private boolean _growable;

  /**
   * Constructor.
   * 
   * @param filledLength the length that is "filled" and readable counting from the offset
   * @param storageLength the maximum storage size of the underlying data
   * @param growable is the storage growable - storageLength is the maximum growable size
   */
  public ByteArray(int filledLength, int storageLength, boolean growable) {
    this._storageLength = storageLength;
    this.setFilledLength(filledLength);
    this._growable = growable;
  }

  /**
   * Constructor.
   * 
   * @param filledLength the length that is "filled" and readable counting from the offset
   * @param storageLength the maximum storage size of the underlying data
   */
  public ByteArray(int filledLength, int storageLength): this(filledLength, storageLength, false) {
    
  }

  /**
   * Gets the byte from the given index.
   * 
   * @param index the index into the byte array
   * @return the byte or -1 if reading beyond the bounds of the data
   */
  public int get(int index) {
    if (index < 0 || index >= this._filledLength) {
      return -1;
    }
    return this.internalGet(index) & 0xff;
  }

  /**
   * Gets the bytes from the given index and fill the buffer with them. 
   * As many bytes as will fit into the buffer are read unless that 
   * would go past the end of the array.
   * 
   * @param index the index into the byte array
   * @param b the buffer to put the bytes read into
   * @return the number of bytes read from the buffer
   */
  public int get(int index, byte[] b) {
    return this.get(index, b, 0, b.Length);
  }

  /**
   * Gets the bytes from the given index and fill the buffer with them starting at the offset given.
   * As many bytes as the specified length are read unless that would go past the end of the array.
   * 
   * @param index the index into the byte array
   * @param b the buffer to put the bytes read into
   * @param offset the location in the buffer to start putting the bytes
   * @param length the number of bytes to put into the buffer
   * @return the number of bytes read from the buffer
   */
  public int get(int index, byte[] b, int offset, int length) {
    if (index < 0 || index >= this._filledLength) {
      return -1;
    }
    int actualLength = Math.Min(length, this._filledLength - index);
    return this.internalGet(index, b, offset, actualLength);
  }

  /**
   * Gets the current filled and readable length of the array.
   *
   * @return the current length
   */
  public int length() {
    return this._filledLength;
  }

  /**
   * Gets the maximum size of the array. This is the maximum number of bytes that
   * the array can hold and all of it may not be filled with data or even fully
   * allocated yet.
   *
   * @return the size of this array
   */
  public int size() {
    return this._storageLength;
  }

  /**
   * Determines whether or not this array is growable or of fixed size.
   *
   * @return true if the array is growable; false otherwise
   */
  public boolean growable() {
    return this._growable;
  }

  public int setFilledLength(int filledLength) {
    this._filledLength = Math.Min(filledLength, this._storageLength);
    return this._filledLength;
  }

  /**
   * Puts the specified byte into the array at the given index unless that would 
   * be beyond the length of the array and it
   * isn't growable.
   * 
   * @param index the index into the byte array
   * @param b the byte to put into the array
   * @throws IndexOutOfBoundsException if attempt to write outside the bounds of the data
   */
  public void put(int index, byte b) {
    if (index < 0 || index >= this.size()) {
      throw new IndexOutOfBoundsException("Attempt to write outside the bounds of the data.");
    }
    this.internalPut(index, b);
    this._filledLength = Math.Max(this._filledLength, index + 1);
  }

  /**
   * Puts the specified bytes into the array at the given index.
   * The entire buffer is put into the array unless that would
   * extend beyond the length and the array isn't growable.
   * @param index the index into the byte array
   * @param b the bytes to put into the array
   * @return the number of bytes actually written
   * @throws IndexOutOfBoundsException if the index for writing is outside the bounds of the data
   */
  public int put(int index, byte[] b) {
    return this.put(index, b, 0, b.Length);
  }

  /**
   * Puts the specified bytes into the array at the given index. All of the bytes
   * specified are put into the array unless that would extend beyond the length
   * and the array isn't growable. The bytes to be put into the array are those
   * in the buffer from the given offset and for the given length.
   *
   * @param index the index into the ByteArray
   * @param b the bytes to put into the array
   * @param (int)Offset.the in the bytes to start copying from
   * @param length the number of bytes to copy into the array
   * @return the number of bytes actually written
   * @throws IndexOutOfBoundsException if the index for writing is outside the bounds of the data
   */
  public int put(int index, byte[] b, int offset, int length) {
    if (index < 0 || index >= this.size()) {
      throw new IndexOutOfBoundsException("Attempt to write outside the bounds of the data.");
    }
    int actualLength = Math.Min(length, this.size() - index);
    int bytesWritten = this.internalPut(index, b, offset, actualLength);
    this._filledLength = Math.Max(this._filledLength, index + bytesWritten);
    return bytesWritten;
  }

  /**
   * Fully copies this ByteArray to another ByteArray to the extent that the
   * destination array has storage for the data copied.
   *
   * @param array the destination
   * @return the number of bytes copied
   */
  public virtual int copyTo(ByteArray array) {
    return copyTo(array, 0, this.length());
  }

  /**
   * Copies a segment of this ByteArray to another ByteArray.
   *
   * @param array the destination
   * @param (int)Offset.the in this ByteArray to start copying from
   * @param length the maximum length in bytes to copy
   * @return the number of bytes copied
   */
  public virtual int  copyTo(ByteArray array, int offset, int length) {
    return this.copyTo(0, array, offset, length);
  }

  /**
   * Copies this ByteArray to another ByteArray.
   *
   * @param dst(int)Offset.the in the destination array to start copying to
   * @param array the destination
   * @param src(int)Offset.the in this ByteArray to start copying from
   * @param length the maximum length in bytes to copy
   * @return the number of bytes copied
   */
  public virtual int copyTo(int dstOffset, ByteArray array, int srcOffset, int length) {
    byte[] b = new byte[COPY_BUFFER_SIZE];
    int bytesRead = 0;
    int index = 0;
    int bufferLength = Math.Min(b.Length, length);
    while ((bytesRead = this.get(index + srcOffset, b, 0, bufferLength)) > 0) {
      int bytesWritten = array.put(index + dstOffset, b, 0, bytesRead);
      index += bytesRead;
      length -= bytesRead;
      bufferLength = Math.Min(b.Length, length);
    }
    return index;
  }

  /**
   * Copies this ByteArray to an OutputStream.
   * @param os the destination
   * @return the number of bytes copied
   * @
   */
  public virtual int copyTo(OutputStream os)  {
    return this.copyTo(os, 0, this.length());
  }

  /**
   * Copies this ByteArray to an OutputStream.
   *
   * @param os the destination
   * @param offset
   * @param length
   * @return the number of bytes copied
   * @
   */
  public virtual int copyTo(OutputStream os, int offset, int length)  {
    byte[] b = new byte[COPY_BUFFER_SIZE];
    int bytesRead = 0;
    int index = 0;
    int bufferLength = Math.Min(b.Length, length);
    while ((bytesRead = this.get(index + offset, b, 0, bufferLength)) > 0) {
      os.write(b, 0, bytesRead);
      index += bytesRead;
      bufferLength = Math.Min(b.Length, length - index);
    }
    return index;
  }

  /**
   * Copies from the InputStream into this ByteArray.
   *
   * @param is the source
   * @param length the number of bytes to copy
   * @
   */
  public virtual void copyFrom(InputStream @is, int length) {
    byte[] b = new byte[COPY_BUFFER_SIZE];
    int bytesRead = 0;
    int index = 0;
    int bufferLength = Math.Min(b.Length, length);
    while ((bytesRead = @is.read(b, 0, bufferLength)) > 0) {
      if (this.put(index, b, 0, bytesRead) != bytesRead) {
        throw new IOException("Error writing bytes.");
      }
      index += bytesRead;
      length -= bytesRead;
      bufferLength = Math.Min(b.Length, length);
    }
  }

  /**
   * Copies everything from the InputStream into this ByteArray.
   *
   * @param is the source
   * @
   */
  public virtual void copyFrom(InputStream @is)  {
    byte[] b = new byte[COPY_BUFFER_SIZE];
    int bytesRead = 0;
    int index = 0;
    int bufferLength = b.Length;
    while ((bytesRead = @is.read(b, 0, bufferLength)) > 0) {
      if (this.put(index, b, 0, bytesRead) != bytesRead) {
        throw new IOException("Error writing bytes.");
      }
      index += bytesRead;
    }
  }

  // ********************************************************************
  // Internal Subclass API
  // ********************************************************************

  /**
   * Stores the byte at the index given.
   *
   * @param index the location to store at
   * @param b the byte to store
   */
  public abstract void internalPut(int index, byte b);

  /**
   * Stores the array of bytes at the given index.
   *
   * @param index the location to store at
   * @param b the bytes to store
   * @param (int)Offset.the to start from in the byte array
   * @param length the length of the byte array to store from the offset
   * @return the number of bytes actually stored
   */
  public abstract int internalPut(int index, byte[] b, int offset, int length);

  /**
   * Gets the byte at the index given.
   *
   * @param index the location to get from
   * @return the byte stored at the index
   */
  public abstract int internalGet(int index);

  /**
   * Gets the bytes at the index given of the given length.
   *
   * @param index the location to start getting from
   * @param b the array to put the bytes into
   * @param (int)Offset.the in the array to put the bytes into
   * @param length the length of bytes to read
   * @return the number of bytes actually ready
   */
  public abstract int internalGet(int index, byte[] b, int offset, int length);

  /**
   * Close this instance of the ByteArray.
   */
  public abstract void close();

  /**
   * Returns a string representation of the ByteArray.
   *
   * @param length the number of bytes of the ByteArray to include in the String
   * @return a string representation of the ByteArray
   */
  public String toString(int offset, int length) {
    if (length == -1) {
      length = this.length();
    }
    length = Math.Min(length, this.length());
    StringBuilder sb = new StringBuilder();

    sb.Append("[l=" + this._filledLength + ", s=" + this.size() + "]");
    if (length > 0) {
      sb.Append("\n");
    }
    for (int i = 0; i < length; i++) {
      int r = this.get(i + offset);
      if (r < 0x10) {
        sb.Append("0");
      }
      sb.Append(NumberHelper.toHexString(r));
      sb.Append(" ");
      if (i > 0 && ((i + 1) % 16) == 0) {
        sb.Append("\n");
      }
    }
    return sb.ToString();
  }

  public override String ToString() {
    return this.toString(0, 0);
  }
}
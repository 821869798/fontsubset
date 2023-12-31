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

namespace com.google.typography.font.sfntly;

using com.google.typography.font.sfntly.data;


/**
 * The font factory. This is the root class for the creation and loading of fonts.
 *
 * @author Stuart Gill
 */
public sealed class FontFactory
{
    private static readonly int LOOKAHEAD_SIZE = 4;

    // font building settings
    private boolean fingerprint = false;

    // font serialization settings
    IList<Integer> tableOrdering;

    /**
     * Offsets to specific elements in the underlying data. These offsets are relative to the
     * start of the table or the start of sub-blocks within the table.
     */
    private enum Offset
    {
        // Offsets within the main directory
        TTCTag = (0),
        Version = (4),
        numFonts = (8),
        OffsetTable = (12),

        // TTC Version 2.0 extensions
        // offsets from end of OffsetTable
        ulDsigTag = (0),
        ulDsigLength = (4),
        ulDsigOffset = (8),
    }

    /**
     * Constructor.
     */
    private FontFactory()
    {
        // Prevent construction.
    }

    /**
     * Factory method for the construction of a font factory.
     *
     * @return a new font factory
     */
    public static FontFactory getInstance(/*buffer builder factory*/)
    {
        return new FontFactory();
    }

    // font building settings

    /**
     * Toggle whether fonts that are loaded are fingerprinted with a SHA-1 hash.
     * If a font is fingerprinted then a SHA-1 hash is generated at load time and stored in the
     * font. This is useful for uniquely identifying fonts. By default this is turned on.
     * @param fingerprint whether fingerprinting should be turned on or off
     * @see #fingerprintFont()
     * @see Font#digest()
     */
    public void fingerprintFont(boolean fingerprint)
    {
        this.fingerprint = fingerprint;
    }

    /**
     * Get the state of the fingerprinting option for fonts that are loaded.
     * @return true if fingerprinting is turned on; false otherwise
     * @see #fingerprintFont(boolean)
     * @see Font#digest()
     */
    public boolean fingerprintFont()
    {
        return this.fingerprint;
    }

    // input stream font loading

    /**
     * Load the font(s) from the input stream. The current settings on the factory
     * are used during the loading process. One or more fonts are returned if the
     * stream contains valid font data. Some font container formats may have more
     * than one font and in this case multiple font objects will be returned. If
     * the data in the stream cannot be parsed or is invalid an array of size zero
     * will be returned.
     *
     * @param is the input stream font data
     * @return one or more fonts
     * @
     */
    public Font[] loadFonts(InputStream @is)
    {
        PushbackInputStream pbis =
          new PushbackInputStream(new BufferedInputStream(@is), FontFactory.LOOKAHEAD_SIZE);
        if (isCollection(pbis))
        {
            return loadCollection(pbis);
        }
        return new Font[] { loadSingleOTF(pbis) };
    }

    /**
     * Load the font(s) from the input stream into font builders. The current
     * settings on the factory are used during the loading process. One or more
     * font builders are returned if the stream contains valid font data. Some
     * font container formats may have more than one font and in this case
     * multiple font builder objects will be returned. If the data in the stream
     * cannot be parsed or is invalid an array of size zero will be returned.
     *
     * @param is the input stream font data
     * @return one or more font builders
     * @
     */
    public Font.Builder[] loadFontsForBuilding(InputStream @is)
    {
        PushbackInputStream pbis =
          new PushbackInputStream(new BufferedInputStream(@is), FontFactory.LOOKAHEAD_SIZE);
        if (isCollection(pbis))
        {
            return loadCollectionForBuilding(pbis);
        }
        return new Font.Builder[] { loadSingleOTFForBuilding(pbis) };
    }

    private Font loadSingleOTF(InputStream @is)
    {
        return loadSingleOTFForBuilding(@is).build();
    }

    private Font[] loadCollection(InputStream @is)
    {
        Font.Builder[] builders = loadCollectionForBuilding(@is);
        Font[] fonts = new Font[builders.Length];
        for (int i = 0; i < fonts.Length; i++)
        {
            fonts[i] = builders[i].build();
        }
        return fonts;
    }

    private Font.Builder loadSingleOTFForBuilding(InputStream @is)
    {
        //MessageDigest digest = null;
        //if (this.fingerprintFont())
        //{
        //    try
        //    {
        //        digest = MessageDigest.getInstance("SHA-1");
        //    }
        //    catch (NoSuchAlgorithmException e)
        //    {
        //        throw new IOException("Unable to get requested message digest algorithm.", e);
        //    }
        //    DigestInputStream dis = new DigestInputStream(@is, digest);
        //    @is = dis;
        //}
        Font.Builder builder = Font.Builder.getOTFBuilder(this, @is);
        //if (this.fingerprintFont())
        //{
        //    builder.setDigest(digest.digest());
        //}
        return builder;
    }

    private Font.Builder[] loadCollectionForBuilding(InputStream @is)
    {
        WritableFontData wfd = WritableFontData.createWritableFontData(@is.available());
        wfd.copyFrom(@is);
        // TOOD(stuartg): add collection loading from a stream
        return loadCollectionForBuilding(wfd);
    }

    static private boolean isCollection(PushbackInputStream pbis)
    {
        byte[] tag = new byte[4];
        pbis.read(tag);
        pbis.unread(tag);
        return Tag.ttcf == Tag.intValue(tag);
    }

    // ByteArray font loading
    /**
     * Load the font(s) from the byte array. The current settings on the factory
     * are used during the loading process. One or more fonts are returned if the
     * stream contains valid font data. Some font container formats may have more
     * than one font and in this case multiple font objects will be returned. If
     * the data in the stream cannot be parsed or is invalid an array of size zero
     * will be returned.
     *
     * @param b the font data
     * @return one or more fonts
     * @
     */
    public Font[] loadFonts(byte[] b)
    {
        // TODO(stuartg): make a ReadableFontData when block loading moved to
        // FontFactory
        WritableFontData rfd = WritableFontData.createWritableFontData(b);
        if (isCollection(rfd))
        {
            return loadCollection(rfd);
        }
        return new Font[] { loadSingleOTF(rfd) };
    }

    /**
     * Load the font(s) from the byte array into font builders. The current
     * settings on the factory are used during the loading process. One or more
     * font builders are returned if the stream contains valid font data. Some
     * font container formats may have more than one font and in this case
     * multiple font builder objects will be returned. If the data in the stream
     * cannot be parsed or is invalid an array of size zero will be returned.
     *
     * @param b the byte array font data
     * @return one or more font builders
     * @
     */
    public Font.Builder[] loadFontsForBuilding(byte[] b)
    {
        WritableFontData wfd = WritableFontData.createWritableFontData(b);
        if (isCollection(wfd))
        {
            return loadCollectionForBuilding(wfd);
        }
        return new Font.Builder[] { loadSingleOTFForBuilding(wfd, 0) };
    }

    private Font loadSingleOTF(WritableFontData wfd)
    {
        return loadSingleOTFForBuilding(wfd, 0).build();
    }

    private Font[] loadCollection(WritableFontData wfd)
    {
        Font.Builder[] builders = loadCollectionForBuilding(wfd);
        Font[] fonts = new Font[builders.Length];
        for (int i = 0; i < fonts.Length; i++)
        {
            fonts[i] = builders[i].build();
        }
        return fonts;
    }

    private Font.Builder loadSingleOTFForBuilding(WritableFontData wfd, int offsetToOffsetTable)
    {
        //MessageDigest digest = null;
        if (this.fingerprintFont())
        {
            // TODO(stuartg): digest of ByteArray
        }
        Font.Builder builder = Font.Builder.getOTFBuilder(this, wfd, offsetToOffsetTable);
        return builder;
    }

    private Font.Builder[] loadCollectionForBuilding(WritableFontData wfd)
    {
        int ttcTag = wfd.readULongAsInt((int)Offset.TTCTag);
        long version = wfd.readFixed((int)Offset.Version);
        int numFonts = wfd.readULongAsInt((int)Offset.numFonts);

        Font.Builder[] builders = new Font.Builder[numFonts];
        int offsetTableOffset = (int)Offset.OffsetTable;
        for (int fontNumber = 0; fontNumber < numFonts; fontNumber++,
            offsetTableOffset += (int)FontData.DataSize.ULONG)
        {
            int offset = wfd.readULongAsInt(offsetTableOffset);
            builders[fontNumber] = this.loadSingleOTFForBuilding(wfd, offset);
        }
        return builders;
    }

    static private boolean isCollection(ReadableFontData rfd)
    {
        byte[] tag = new byte[4];
        rfd.readBytes(0, tag, 0, tag.Length);
        return Tag.ttcf == Tag.intValue(tag);
    }

    // font serialization

    /**
     * Serialize the font to the output stream.
     *
     * @param font the font to serialize
     * @param os the destination stream for the font
     * @
     */
    public void serializeFont(Font font, OutputStream os)
    {
        // TODO(stuartg) should have serialization options somewhere
        font.serialize(os, tableOrdering);
    }

    /**
     * Set the table ordering to be used in serializing a font. The table ordering
     * is an ordered list of table ids and tables will be serialized in the order
     * given. Any tables whose id is not listed in the ordering will be placed in
     * an unspecified order following those listed.
     *
     * @param tableOrdering the table ordering
     */
    public void setSerializationTableOrdering(IList<Integer> tableOrdering)
    {
        this.tableOrdering = new System.Collections.Generic.List<Integer>(tableOrdering);
    }

    // new fonts

    /**
     * Get an empty font builder for creating a new font from scratch.
     *
     * @return an empty font builder
     */
    public Font.Builder newFontBuilder()
    {
        return Font.Builder.getOTFBuilder(this);
    }
}
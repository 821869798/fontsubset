
namespace com.google.typography.font.sfntly.table.core;

using com.google.typography.font.sfntly;
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table;
using static com.google.typography.font.sfntly.table.core.CMapTable;

/**
 * The abstract base class for all cmaps.
 *
 * CMap equality is based on the equality of the (@link {@link CMapId} that
 * defines the CMap. In the cmap table for a font there can only be one cmap
 * with a given cmap id (pair of platform and encoding ids) no matter what the
 * type of the cmap is.
 *
 * The cmap implements {@code Iterable<Integer>} to allow iteration over
 * characters that are mapped by the cmap. This iteration mostly returns the
 * characters mapped by the cmap. It will return all characters mapped by the
 * cmap to anything but .notdef <b>but</b> it may return some that are not
 * mapped or are mapped to .notdef. Various cmap tables provide ranges and
 * such to describe characters for lookup but without going the full way to
 * mapping to the glyph id it isn't always possible to tell if a character
 * will end up with a valid glyph id. So, some of the characters returned from
 * the iterator may still end up pointing to the .notdef glyph. However, the
 * number of such characters should be small in most cases with well designed
 * cmaps.
 */
public abstract class CMap : SubTable /*, Iterable<Integer>*/
{
    public readonly int _format;
    public readonly CMapId _cmapId;

    /**
     * CMap subtable formats.
     *
     */
    public enum CMapFormat : int
    {
        Format0 = (0),
        Format2 = (2),
        Format4 = (4),
        Format6 = (6),
        Format8 = (8),
        Format10 = (10),
        Format12 = (12),
        Format13 = (13),
        Format14 = (14),
    }

    /**
     * Constructor.
     *
     * @param data data for the cmap
     * @param format the format of the cmap
     * @param cmapId the id information of the cmap
     */
    public CMap(ReadableFontData data, int format, CMapId cmapId) : base(data)
    {

        this._format = format;
        this._cmapId = cmapId;
    }

    /**
     * Gets the format of the cmap.
     *
     * @return the format
     */
    public int format()
    {
        return this._format;
    }

    /**
     * Gets the cmap id for this cmap.
     *
     * @return cmap id
     */
    public CMapId cmapId()
    {
        return this._cmapId;
    }

    /**
     * Gets the platform id for this cmap.
     *
     * @return the platform id
     * @see PlatformId
     */
    public int platformId()
    {
        return this.cmapId().platformId();
    }

    /**
     * Gets the encoding id for this cmap.
     *
     * @return the encoding id
     * @see MacintoshEncodingId
     * @see WindowsEncodingId
     * @see UnicodeEncodingId
     */
    public int encodingId()
    {
        return this.cmapId().encodingId();
    }

    // TODO(stuartg): simple implementation until all subclasses define their
    // own more efficient version
    /*public class CharacterIterator : IEnumerator<Integer> {
      private int character = 0;
      private readonly int maxCharacter;

      public CharacterIterator(int start, int end) {
        this.character = start;
        this.maxCharacter = end;
      }

      public boolean hasNext() {
        if (character < maxCharacter) {
          return true;
        }
        return false;
      }

      public Integer next() {
        if (!hasNext()) {
          throw new NoSuchElementException("No more characters to iterate.");
        }
        return this.character++;
      }

      public void remove() {
        throw new UnsupportedOperationException("Unable to remove a character from cmap.");
      }
    }*/


    public override int GetHashCode()
    {
        return this._cmapId.GetHashCode();
    }

    public override boolean Equals(Object obj)
    {
        if (this == obj)
        {
            return true;
        }
        if (!(obj is CMap))
        {
            return false;
        }
        return this._cmapId.Equals(((CMap)obj)._cmapId);
    }

    /**
     * Gets the language of the cmap.
     *
     *  Note on the language field in 'cmap' subtables: The language field must
     * be set to zero for all cmap subtables whose platform IDs are other than
     * Macintosh (platform ID 1). For cmap subtables whose platform IDs are
     * Macintosh, set this field to the Macintosh language ID of the cmap
     * subtable plus one, or to zero if the cmap subtable is not
     * language-specific. For example, a Mac OS Turkish cmap subtable must set
     * this field to 18, since the Macintosh language ID for Turkish is 17. A
     * Mac OS Roman cmap subtable must set this field to 0, since Mac OS Roman
     * is not a language-specific encoding.
     *
     * @return the language id
     */
    public abstract int language();

    /**
     * Gets the glyph id for the character code provided.
     *
     * The character code provided must be in the encoding used by the cmap table.
     *
     * @param character character value using the encoding of the cmap table
     * @return glyph id for the character code
     */
    public abstract int glyphId(int character);

    public override String ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("cmap: ");
        builder.Append(this.cmapId());
        builder.Append(", ");
        builder.Append((CMapFormat)this.format());
        builder.Append(", Data Size=0x");
        builder.Append(NumberHelper.toHexString(this._data.length()));
        return builder.ToString();
    }

    public abstract IEnumerator<Integer> GetEnumerator();


    public static CMap.IBuilder<CMap> getBuilder(ReadableFontData data, int offset, CMapId cmapId)
    {
        // read from the front of the cmap - 1st entry is always the format
        int rawFormat = data.readUShort(offset);
        CMapFormat format = (CMapFormat)rawFormat;

        switch (format)
        {
            case CMapFormat.Format0:
                return CMapFormat0.createBuilder(data, offset, cmapId);
            case CMapFormat.Format2:
                return CMapFormat2.createBuilder(data, offset, cmapId);
            case CMapFormat.Format4:
                return CMapFormat4.createBuilder(data, offset, cmapId);
            case CMapFormat.Format6:
                return CMapFormat6.createBuilder(data, offset, cmapId);
            case CMapFormat.Format8:
                return CMapFormat8.createBuilder(data, offset, cmapId);
            case CMapFormat.Format10:
                return CMapFormat10.createBuilder(data, offset, cmapId);
            case CMapFormat.Format12:
                return CMapFormat12.createBuilder(data, offset, cmapId);
            case CMapFormat.Format13:
                return CMapFormat13.createBuilder(data, offset, cmapId);
            case CMapFormat.Format14:
                return CMapFormat14.createBuilder(data, offset, cmapId);
            default:
                break;
        }
        return null;
    }

    // TODO: Instead of a root factory method, the individual subtable
    // builders should get created
    // from static factory methods in each subclass
    public static CMap.IBuilder<CMap> getBuilder(CMapFormat cmapFormat, CMapId cmapId)
    {
        switch (cmapFormat)
        {
            // TODO: builders for other formats, as they're implemented
            case CMapFormat.Format0:
                return CMapFormat0.createBuilder(null, 0, cmapId);
            case CMapFormat.Format4:
                return CMapFormat4.createBuilder(null, 0, cmapId);
            default:
                break;
        }
        return null;
    }

    new public interface IBuilder<out TCMap> : SubTable.IBuilder<TCMap> where TCMap : CMap
    {
        CMapId cmapId();
        int encodingId();
        int platformId();

        CMapFormat format();

        int language();

        void setLanguage(int language);
    }

    new protected abstract class Builder<TCMap> : SubTable.Builder<TCMap>, IBuilder<TCMap> where TCMap : CMap
    {

        private readonly CMapFormat _format;
        private readonly CMapId _cmapId;
        private int _language;

        /**
         * Constructor.
         *
         * @param data the data for the cmap
         * @param format cmap format
         * @param cmapId the id for this cmap
         */
        public Builder(ReadableFontData data, CMapFormat format, CMapId cmapId) : base(data)
        {

            this._format = format;
            this._cmapId = cmapId;
        }

        /**
         * @return the id for this cmap
         */
        public CMapId cmapId()
        {
            return this._cmapId;
        }

        /**
         * Gets the encoding id for the cmap. The encoding will from one of a
         * number of different sets depending on the platform id.
         *
         * @return the encoding id
         * @see MacintoshEncodingId
         * @see WindowsEncodingId
         * @see UnicodeEncodingId
         */
        public int encodingId()
        {
            return this.cmapId().encodingId();
        }

        /**
         * Gets the platform id for the cmap.
         *
         * @return the platform id
         * @see PlatformId
         */
        public int platformId()
        {
            return this.cmapId().platformId();
        }

        public CMapFormat format()
        {
            return this._format;
        }

        public int language()
        {
            return this._language;
        }

        public void setLanguage(int language)
        {
            this._language = language;
        }

        /**
         * @param data
         */
        public Builder(WritableFontData data, CMapFormat format, CMapId cmapId) : base(data)
        {

            this._format = format;
            this._cmapId = cmapId;
        }

        public override void subDataSet()
        {
            // NOP
        }

        public override int subDataSizeToSerialize()
        {
            return this.internalReadData().length();
        }

        public override boolean subReadyToSerialize()
        {
            return true;
        }

        public override int subSerialize(WritableFontData newData)
        {
            return this.internalReadData().copyTo(newData);
        }

        public override String ToString()
        {
            return String.Format("{0}, format = {1}", this.cmapId(), this.format());
        }
    }
}
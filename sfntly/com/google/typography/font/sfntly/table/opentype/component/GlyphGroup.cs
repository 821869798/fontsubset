using com.google.typography.font.sfntly.table.core;
using System.Drawing;

namespace com.google.typography.font.sfntly.table.opentype.component;







public class GlyphGroup : BitSet
{

    private boolean inverse = false;

    public GlyphGroup() : base()
    {
    }

    public GlyphGroup(int glyph)
    {
        base.set(glyph);
    }

    public GlyphGroup(IEnumerable<Integer> glyphs)
    {
        foreach (int glyph in glyphs)
        {
            base.set(glyph);
        }
    }

    public static GlyphGroup inverseGlyphGroup(IEnumerable<GlyphGroup> glyphGroups)
    {
        GlyphGroup result = new GlyphGroup();
        foreach (GlyphGroup glyphGroup in glyphGroups)
        {
            result.or(glyphGroup);
        }
        result.inverse = true;
        return result;
    }

    public void add(int glyph)
    {
        this.set(glyph);
    }

    public void addAll(IEnumerable<Integer> glyphs)
    {
        foreach (int glyph in glyphs)
        {
            base.set(glyph);
        }
    }

    public void addAll(GlyphGroup other)
    {
        this.or(other);
    }

    public void copyTo(ICollection<Integer> target)
    {
        //List<Integer> list = new LinkedList<Integer>();
        for (int i = this.nextSetBit(0); i >= 0; i = this.nextSetBit(i + 1))
        {
            target.Add(i);
        }
    }

    public GlyphGroup intersection(GlyphGroup other)
    {
        GlyphGroup intersection = new GlyphGroup();
        if (this.inverse && !other.inverse)
        {
            intersection.or(other);
            intersection.andNot(this);
        }
        else if (other.inverse && !this.inverse)
        {
            intersection.or(this);
            intersection.andNot(other);
        }
        else if (other.inverse && this.inverse)
        {
            intersection.inverse = true;
            intersection.or(this);
            intersection.or(other);
        }
        else
        {
            intersection.or(this);
            intersection.and(other);
        }
        return intersection;
    }

    boolean contains(int glyph)
    {
        return get(glyph) ^ inverse;
    }

    public override int size()
    {
        return cardinality();
    }

    public virtual IEnumerator<Integer> GetEnumerator()
    {
        return GetEnumerable().GetEnumerator();

        IEnumerable<Integer> GetEnumerable()
        {
            int i = -1;

            while ((i = nextSetBit(i + 1)) >= 0)
            {
                yield return i;
            }
        }
        //return new IEnumerator<Integer>() {
        //  int i = 0;
        //  public override boolean hasNext() {
        //    return nextSetBit(i) >= 0 ;
        //  }

        //  public override Integer next() {
        //    i = nextSetBit(i);
        //    return i++;
        //  }

        //  public override void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //};
    }

    public override String ToString()
    {
        return toString(null);
    }

    public String toString(PostScriptTable post)
    {
        StringBuilder sb = new StringBuilder();
        if (this.inverse)
        {
            sb.Append("not-");
        }
        int glyphCount = size();
        if (glyphCount > 1)
        {
            sb.Append("[ ");
        }
        foreach (int glyphId in this)
        {
            sb.Append(glyphId);

            if (post != null)
            {
                String glyphName = post.glyphName(glyphId);
                if (glyphName != null)
                {
                    sb.Append("-");
                    sb.Append(glyphName);
                }
            }
            sb.Append(" ");
        }
        if (glyphCount > 1)
        {
            sb.Append("] ");
        }
        return sb.ToString();
    }
}
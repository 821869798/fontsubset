using com.google.typography.font.sfntly.table.core;

namespace com.google.typography.font.sfntly.table.opentype.component;






public class RuleSegment : List<GlyphGroup>
{
    public RuleSegment() : base()
    {
    }

    public RuleSegment(GlyphGroup glyphGroup)
    {
        addInternal(glyphGroup);
    }

    public RuleSegment(int glyph)
    {
        GlyphGroup glyphGroup = new GlyphGroup(glyph);
        addInternal(glyphGroup);
    }

    public RuleSegment(GlyphList glyphs)
    {
        foreach (int glyph in glyphs)
        {
            GlyphGroup glyphGroup = new GlyphGroup(glyph);
            addInternal(glyphGroup);
        }
    }

    public boolean add(int glyph)
    {
        GlyphGroup glyphGroup = new GlyphGroup(glyph);
        return addInternal(glyphGroup);
    }

    public boolean addAll(IEnumerable<GlyphGroup> glyphGroups)
    {
        foreach (GlyphGroup glyphGroup in glyphGroups)
        {
            if (glyphGroup == null)
            {
                throw new IllegalArgumentException("Null GlyphGroup not allowed");
            }
        }
        base.AddRange(glyphGroups);

        return true;
    }

    private boolean addInternal(GlyphGroup glyphGroup)
    {
        if (glyphGroup == null)
        {
            throw new IllegalArgumentException("Null GlyphGroup not allowed");
        }
        base.Add(glyphGroup);

        return true;
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (GlyphGroup glyphGroup in this)
        {
            sb.Append(glyphGroup.ToString());
        }
        return sb.ToString();
    }

    public String toString(PostScriptTable post)
    {
        StringBuilder sb = new StringBuilder();
        foreach (GlyphGroup glyphGroup in this)
        {
            sb.Append(glyphGroup.toString(post));
            sb.Append(" ");
        }
        return sb.ToString();
    }

}
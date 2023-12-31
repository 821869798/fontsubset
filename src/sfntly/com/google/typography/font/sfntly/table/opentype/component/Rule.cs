using com.google.typography.font.sfntly.table.core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace com.google.typography.font.sfntly.table.opentype.component;






















public class Rule
{
    private readonly RuleSegment backtrack;
    private readonly RuleSegment input;
    private readonly RuleSegment lookAhead;
    public readonly RuleSegment subst;
    private readonly int _hashCode;

  public  Rule(RuleSegment backtrack, RuleSegment input, RuleSegment lookAhead, RuleSegment subst)
    {
        this.backtrack = backtrack;
        this.input = input;
        this.lookAhead = lookAhead;
        this.subst = subst;
        this._hashCode = getHashCode();
    }

    // Closure related
    public static GlyphGroup charGlyphClosure(Font font, String txt)
    {
        CMapTable cmapTable = font.getTable<CMapTable>(Tag.cmap);
        GlyphGroup glyphGroup = glyphGroupForText(txt, cmapTable);

        var featuredRules = Rule.featuredRules(font);
        IDictionary<Integer, ISet<Rule>> glyphRuleMap = createGlyphRuleMap(featuredRules);
        GlyphGroup ruleClosure = closure(glyphRuleMap, glyphGroup);
        return ruleClosure;
    }

    public static GlyphGroup closure(IDictionary<Integer, ISet<Rule>> glyphRuleMap, GlyphGroup glyphs)
    {
        int prevSize = 0;
        while (glyphs.size() > prevSize)
        {
            prevSize = glyphs.size();
            foreach (Rule rule in rulesForGlyph(glyphRuleMap, glyphs))
            {
                rule.addMatchingTargetGlyphs(glyphs);
            }
        }
        return glyphs;
    }

    private void addMatchingTargetGlyphs(GlyphGroup glyphs)
    {
        foreach (RuleSegment seg in new RuleSegment[] { input, backtrack, lookAhead })
        {
            if (seg == null)
            {
                continue;
            }
            foreach (GlyphGroup g in seg)
            {
                if (!g.intersects(glyphs))
                {
                    return;
                }
            }
        }

        foreach (GlyphGroup glyphGroup in subst)
        {
            glyphs.addAll(glyphGroup);
        }
    }

    public static IDictionary<Integer, ISet<Rule>> glyphRulesMap(Font font)
    {
        var featuredRules = Rule.featuredRules(font);
        if (featuredRules == null)
        {
            return null;
        }
        return createGlyphRuleMap(featuredRules);
    }

    private static IDictionary<Integer, ISet<Rule>> createGlyphRuleMap(ISet<Rule> lookupRules)
    {
        IDictionary<Integer, ISet<Rule>> map = new Dictionary<Integer, ISet<Rule>>();

        foreach (Rule rule in lookupRules)
        {
            foreach (int glyph in rule.input.get(0))
            {
                if (!map.containsKey(glyph))
                {
                    map.put(glyph, new HashSet<Rule>());
                }
                map.get(glyph).Add(rule);
            }
        }
        return map;
    }

    private static ISet<Rule> rulesForGlyph(IDictionary<Integer, ISet<Rule>> glyphRuleMap, GlyphGroup glyphs)
    {
        ISet<Rule> set = new HashSet<Rule>();
        foreach (int glyph in glyphs)
        {
            if (glyphRuleMap.containsKey(glyph))
            {
                set.AddRange(glyphRuleMap.get(glyph));
            }
        }
        return set;
    }

    private static HashSet<Rule> featuredRules(
        ISet<Integer> lookupIds, IDictionary<Integer, ISet<Rule>> ruleMap)
    {
        var rules = new HashSet<Rule>();
        foreach (int lookupId in lookupIds)
        {
            ISet<Rule> ruleForLookup = ruleMap.get(lookupId);
            if (ruleForLookup == null)
            {
                Console.Error.Write("Lookup ID %d is used in features but not defined.\n", lookupId);
                continue;
            }
            rules.AddRange(ruleForLookup);
        }
        return rules;
    }

    private static ISet<Integer> featuredLookups(Font font)
    {
        GSubTable gsub = font.getTable<GSubTable>(Tag.GSUB);
        if (gsub == null)
        {
            return null;
        }

        ScriptListTable scripts = gsub.scriptList();
        FeatureListTable featureList = gsub.featureList();
        LookupListTable lookupList = gsub.lookupList();
        IDictionary<Integer, ISet<Rule>> ruleMap = RuleExtractor.extract(lookupList);

        var features = new HashSet<Integer>();
        var lookupIds = new HashSet<Integer>();

        foreach (ScriptTable script in scripts.map().values())
        {
            foreach (LangSysTable langSys in script.map().values())
            {
                // We are assuming if required feature exists, it will be in the list
                // of features as well.
                foreach (NumRecord feature in langSys)
                {
                    if (!features.Contains(feature.value))
                    {
                        features.Add(feature.value);
                        foreach (NumRecord lookup in featureList.subTableAt(feature.value))
                        {
                            lookupIds.Add(lookup.value);
                        }
                    }
                }
            }
        }
        return lookupIds;
    }

    private static HashSet<Rule> featuredRules(Font font)
    {
        GSubTable gsub = font.getTable<GSubTable>(Tag.GSUB);
        if (gsub == null)
        {
            return null;
        }

        LookupListTable lookupList = gsub.lookupList();
        IDictionary<Integer, ISet<Rule>> ruleMap = RuleExtractor.extract(lookupList);
        ISet<Integer> lookupIds = featuredLookups(font);
        var featuredRules = Rule.featuredRules(lookupIds, ruleMap);
        return featuredRules;
    }

    // Utility method for glyphs for text

    public static GlyphGroup glyphGroupForText(String str, CMapTable cmapTable)
    {
        GlyphGroup glyphGroup = new GlyphGroup();
        ISet<Integer> codes = codepointsFromStr(str);
        foreach (int code in codes)
        {
            foreach (CMap cmap in cmapTable)
            {
                if (cmap.platformId() == 3 && cmap.encodingId() == 1 || // Unicode BMP
                    cmap.platformId() == 3 && cmap.encodingId() == 10 || // UCS2
                    cmap.platformId() == 0 && cmap.encodingId() == 5)
                { // Variation
                    int glyph = cmap.glyphId(code);
                    if (glyph != CMapTable.NOTDEF)
                    {
                        glyphGroup.add(glyph);
                    }
                    // Debug.WriteLine("code: " + code + " glyph: " + glyph + " platform: " + cmap.platformId() + " encodingId: " + cmap.encodingId() + " format: " + cmap.format());

                }
            }
        }
        return glyphGroup;
    }

    // Rule operation

    private void applyRuleOnRuleWithSubst(Rule targetRule, int at, LinkedList<Rule> accumulateTo)
    {
        RuleSegment matchSegment = targetRule.match(this, at);
        if (matchSegment == null)
        {
            return;
        }

        if (at < 0)
        {
            throw new IllegalStateException();
        }

        int backtrackSize = targetRule.backtrack != null ? targetRule.backtrack.Count : 0;
        RuleSegment newBacktrack = new RuleSegment();
        newBacktrack.addAll(matchSegment.subList(0, backtrackSize));

        if (at <= targetRule.subst.Count)
        {
            RuleSegment newInput = new RuleSegment();
            newInput.addAll(targetRule.input);
            newInput.addAll(matchSegment.subList(backtrackSize + targetRule.subst.Count, backtrackSize + at + input.Count));

            RuleSegment newLookAhead = new RuleSegment();
            newLookAhead.addAll(matchSegment.subList(backtrackSize + at + input.Count, matchSegment.Count));

            RuleSegment newSubst = new RuleSegment();
            newSubst.addAll(targetRule.subst.subList(0, at));
            newSubst.addAll(subst);
            if (at + input.Count < targetRule.subst.Count)
            {
                newSubst.addAll(targetRule.subst.subList(at + input.Count, targetRule.subst.Count));
            }

            Rule newTargetRule = new Rule(newBacktrack, newInput, newLookAhead, newSubst);
            accumulateTo.AddLast(newTargetRule);
            return;
        }

        if (at >= targetRule.subst.Count)
        {
            IList<GlyphGroup> skippedLookAheadPart = matchSegment.subList(backtrackSize + targetRule.subst.Count, at);
            ISet<RuleSegment> intermediateSegments = permuteToSegments(skippedLookAheadPart);

            RuleSegment newLookAhead = new RuleSegment();
            IList<GlyphGroup> remainingLookAhead = matchSegment.subList(backtrackSize + at + input.Count, matchSegment.Count);
            newLookAhead.addAll(remainingLookAhead);

            foreach (RuleSegment interRuleSegment in intermediateSegments)
            {

                RuleSegment newInput = new RuleSegment();
                newInput.addAll(targetRule.input);
                newInput.addAll(interRuleSegment);
                newInput.addAll(input);

                RuleSegment newSubst = new RuleSegment();
                newSubst.addAll(targetRule.subst);
                newInput.addAll(interRuleSegment);
                newSubst.addAll(subst);

                Rule newTargetRule = new Rule(newBacktrack, newInput, newLookAhead, newSubst);
                accumulateTo.AddLast(newTargetRule);
            }
        }
    }

    private static ISet<RuleSegment> permuteToSegments(IList<GlyphGroup> glyphGroups)
    {
        ISet<RuleSegment> result = new HashSet<RuleSegment>();
        result.Add(new RuleSegment());

        foreach (GlyphGroup glyphGroup in glyphGroups)
        {
            ISet<RuleSegment> newResult = new HashSet<RuleSegment>();
            foreach (Integer glyphId in glyphGroup)
            {
                foreach (RuleSegment segment in result)
                {
                    RuleSegment newSegment = new RuleSegment();
                    newSegment.addAll(segment);
                    newSegment.Add(new GlyphGroup(glyphId));
                    newResult.Add(newSegment);
                }
            }
            result = newResult;
        }
        return result;
    }

    private static Rule applyRuleOnRuleWithoutSubst(Rule ruleToApply, Rule targetRule, int at)
    {

        RuleSegment matchSegment = targetRule.match(ruleToApply, at);
        if (matchSegment == null)
        {
            return null;
        }

        int backtrackSize = targetRule.backtrack != null ? targetRule.backtrack.Count : 0;

        RuleSegment newBacktrack = new RuleSegment();
        newBacktrack.addAll(matchSegment.subList(0, backtrackSize + at));

        RuleSegment newLookAhead = new RuleSegment();
        newLookAhead.addAll(matchSegment.subList(backtrackSize + at + ruleToApply.input.Count, matchSegment.Count));

        return new Rule(newBacktrack, ruleToApply.input, newLookAhead, ruleToApply.subst);
    }

    private static void applyRulesOnRuleWithSubst(ISet<Rule> rulesToApply, Rule targetRule, int at,
        LinkedList<Rule> accumulateTo)
    {
        foreach (Rule ruleToApply in rulesToApply)
        {
            ruleToApply.applyRuleOnRuleWithSubst(targetRule, at, accumulateTo);
        }
    }

    private static void applyRulesOnRuleWithoutSubst(ISet<Rule> rulesToApply, Rule targetRule, int at,
        LinkedList<Rule> accumulateTo)
    {
        foreach (Rule ruleToApply in rulesToApply)
        {
            Rule newRule = applyRuleOnRuleWithoutSubst(ruleToApply, targetRule, at);
            if (newRule != null)
            {
                accumulateTo.AddLast(newRule);
            }
        }
    }

    public static LinkedList<Rule> applyRulesOnRules(ISet<Rule> rulesToApply, IList<Rule> targetRules,
        int at)
    {
        LinkedList<Rule> result = new LinkedList<Rule>();
        foreach (Rule targetRule in targetRules)
        {
            if (targetRule.subst != null)
            {
                applyRulesOnRuleWithSubst(rulesToApply, targetRule, at, result);
            }
            else
            {
                applyRulesOnRuleWithoutSubst(rulesToApply, targetRule, at, result);
            }
        }
        return result;
    }

    private RuleSegment match(Rule other, int at)
    {
        if (at < 0)
        {
            throw new IllegalStateException();
        }

        RuleSegment thisAllSegments = new RuleSegment();
        if (backtrack != null)
        {
            thisAllSegments.addAll(backtrack);
        }
        if (subst != null)
        {
            thisAllSegments.addAll(subst);
        }
        else
        {
            thisAllSegments.addAll(input);
        }
        if (lookAhead != null)
        {
            thisAllSegments.addAll(lookAhead);
        }

        RuleSegment otherAllSegments = new RuleSegment();
        if (other.backtrack != null)
        {
            otherAllSegments.addAll(other.backtrack);
        }
        otherAllSegments.addAll(other.input);
        if (other.lookAhead != null)
        {
            otherAllSegments.addAll(other.lookAhead);
        }

        int backtrackSize = backtrack != null ? backtrack.Count : 0;
        int otherBacktrackSize = other.backtrack != null ? other.backtrack.Count : 0;
        int initialPos = backtrackSize + at - otherBacktrackSize;

        if (initialPos < 0)
        {
            return null;
        }

        if (thisAllSegments.Count - initialPos < otherAllSegments.Count)
        {
            return null;
        }

        for (int i = 0; i < otherAllSegments.Count; i++)
        {
            GlyphGroup thisGlyphs = thisAllSegments.get(i + initialPos);
            GlyphGroup otherGlyphs = otherAllSegments.get(i);

            GlyphGroup intersection = thisGlyphs.intersection(otherGlyphs);
            if (intersection.isEmpty())
            {
                return null;
            }
            thisAllSegments.set(i + initialPos, intersection);
        }

        return thisAllSegments;
    }

    private static Rule prependToInput(int prefix, Rule other)
    {
        RuleSegment input = new RuleSegment(prefix);
        input.addAll(other.input);

        return new Rule(other.backtrack, input, other.lookAhead, other.subst);
    }

    public static IList<Rule> prependToInput(int prefix, IList<Rule> rules)
    {
        IList<Rule> result = new List<Rule>();
        foreach (Rule rule in rules)
        {
            result.Add(prependToInput(prefix, rule));
        }
        return result;
    }

    public static HashSet<Rule> deltaRules(IList<Integer> glyphIds, int delta)
    {
        HashSet<Rule> result = new HashSet<Rule>();
        foreach (int glyphId in glyphIds)
        {
            RuleSegment input = new RuleSegment(glyphId);
            RuleSegment subst = new RuleSegment(glyphId + delta);
            result.Add(new Rule(null, input, null, subst));
        }
        return result;
    }

    public static HashSet<Rule> oneToOneRules(RuleSegment backtrack, IList<Integer> inputs,
        RuleSegment lookAhead, IList<Integer> substs)
    {
        if (inputs.Count != substs.Count)
        {
            throw new IllegalArgumentException("input - subst should have same count");
        }

        HashSet<Rule> result = new HashSet<Rule>();
        for (int i = 0; i < inputs.Count; i++)
        {
            RuleSegment input = new RuleSegment(inputs.get(i));
            RuleSegment subst = new RuleSegment(substs.get(i));
            result.Add(new Rule(backtrack, input, lookAhead, subst));
        }
        return result;
    }

    public static HashSet<Rule> oneToOneRules(IList<Integer> inputs, IList<Integer> substs)
    {
        return oneToOneRules(null, inputs, null, substs);
    }

    // Dump routines
    private static HashSet<Integer> codepointsFromStr(String s)
    {
        var points = MemoryMarshal.Cast<byte, int>(Encoding.UTF32.GetBytes(s));

        HashSet<Integer> list = new HashSet<Integer>();

        foreach (var item in points)
        {
            list.Add(item);
        }

        return list;
        //HashSet<Integer> list = new HashSet<Integer>();
        //for (int cp, i = 0; i < s.Length; i += Character.charCount(cp))
        //{
        //    cp = s.codePointAt(i);
        //    list.add(cp);
        //}
        //return list;
    }

    private static void dumpRuleMap(IDictionary<Integer, ISet<Rule>> rulesList, PostScriptTable post)
    {
        foreach (int index in rulesList.keySet())
        {
            ISet<Rule> rules = rulesList.get(index);
            Console.WriteLine(
                "------------------------------ " + index + " --------------------------------");
            foreach (Rule rule in rules)
            {
                Console.WriteLine(rule.toString(post));
            }
        }
    }

    public static void dumpLookups(Font font)
    {
        GSubTable gsub = font.getTable<GSubTable>(Tag.GSUB);
        IDictionary<Integer, ISet<Rule>> ruleMap = RuleExtractor.extract(gsub.lookupList());
        PostScriptTable post = font.getTable<PostScriptTable>(Tag.post);
        dumpRuleMap(ruleMap, post);
        Console.WriteLine("\nFeatured Lookup IDs: " + Rule.featuredLookups(font));
    }

    private String toString(PostScriptTable post)
    {
        StringBuilder sb = new StringBuilder();
        if (backtrack != null && backtrack.Count > 0)
        {
            sb.Append(backtrack.toString(post));
            sb.Append("} ");
        }
        sb.Append(input.toString(post));
        if (lookAhead != null && lookAhead.Count > 0)
        {
            sb.Append("{ ");
            sb.Append(lookAhead.toString(post));
        }
        sb.Append("=> ");
        sb.Append(post.ToString());
        return sb.ToString();
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        if (backtrack != null && backtrack.Count > 0)
        {
            sb.Append(backtrack.ToString());
            sb.Append("} ");
        }
        sb.Append(input.ToString());
        if (lookAhead != null && lookAhead.Count > 0)
        {
            sb.Append("{ ");
            sb.Append(lookAhead.ToString());
        }
        sb.Append("=> ");
        sb.Append(subst.ToString());
        return sb.ToString();
    }

    public override boolean Equals(Object o)
    {
        if (o == this)
        {
            return true;
        }
        if (!(o is Rule))
        {
            return false;
        }
        Rule other = (Rule)o;
        if (_hashCode != other._hashCode)
        {
            return false;
        }
        RuleSegment[] these = new RuleSegment[] { input, subst, backtrack, lookAhead };
        RuleSegment[] others = new RuleSegment[] { other.input, other.subst, other.backtrack, other.lookAhead };
        for (int i = 0; i < these.Length; i++)
        {
            RuleSegment thisSeg = these[i];
            RuleSegment otherSeg = others[i];
            if (thisSeg != null)
            {
                if (!thisSeg.Equals(otherSeg))
                {
                    return false;
                }
            }
            else if (otherSeg != null)
            {
                return false;
            }
        }
        return true;
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    private int getHashCode()
    {
        int hashCode = 1;
        foreach (RuleSegment e in new RuleSegment[] { input, subst, backtrack, lookAhead })
        {
            hashCode = 31 * hashCode + (e == null ? 0 : e.GetHashCode());
        }
        return hashCode;
    }
}
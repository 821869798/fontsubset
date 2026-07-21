using com.google.typography.font.sfntly.table.opentype.chaincontextsubst;
using com.google.typography.font.sfntly.table.opentype.classdef;
using com.google.typography.font.sfntly.table.opentype.contextsubst;
using com.google.typography.font.sfntly.table.opentype.ligaturesubst;
using com.google.typography.font.sfntly.table.opentype.singlesubst;

namespace com.google.typography.font.sfntly.table.opentype.component;













































class RuleExtractor
{
    public static HashSet<Rule> extract(LigatureSubst table)
    {
        var allRules = new HashSet<Rule>();
        IList<Integer> prefixChars = extract(table.coverage());

        for (int i = 0; i < table.subTableCount(); i++)
        {
            IList<Rule> subRules = extract(table.subTableAt(i));
            subRules = Rule.prependToInput(prefixChars.get(i), subRules);
            allRules.AddRange(subRules);
        }
        return allRules;
    }

    public static GlyphList extract(CoverageTable table)
    {
        switch (table.format)
        {
            case 1:
                return extract(table.fmt1Table());
            case 2:
                RangeRecordTable array = table.fmt2Table();
                IDictionary<Integer, GlyphGroup> map = extract(array);
                var groups = map.values();
                GlyphList result = new GlyphList();
                foreach (GlyphGroup glyphIds in groups)
                {
                    glyphIds.copyTo(result);
                }
                return result;
            default:
                throw new IllegalArgumentException("unimplemented format " + table.format);
        }
    }

    public static GlyphList extract(RecordsTable<NumRecord> table)
    {
        GlyphList result = new GlyphList();
        foreach (NumRecord record in table.recordList)
        {
            result.Add(record.value);
        }
        return result;
    }

    public static IDictionary<Integer, GlyphGroup> extract(RangeRecordTable table)
    {
        // Order is important.
        IDictionary<Integer, GlyphGroup> result = new Dictionary<Integer, GlyphGroup>();
        foreach (RangeRecord record in table.recordList)
        {
            if (!result.containsKey(record.property))
            {
                result.put(record.property, new GlyphGroup());
            }
            GlyphGroup existingGlyphs = result.get(record.property);
            existingGlyphs.addAll(extract(record));
        }
        return result;
    }

    public static GlyphGroup extract(RangeRecord record)
    {
        int len = record.end - record.start + 1;
        GlyphGroup result = new GlyphGroup();
        for (int i = record.start; i <= record.end; i++)
        {
            result.add(i);
        }
        return result;
    }

    public static IList<Rule> extract(LigatureSet table)
    {
        IList<Rule> allRules = new List<Rule>();

        for (int i = 0; i < table.subTableCount(); i++)
        {
            Rule subRule = extract(table.subTableAt(i));
            allRules.Add(subRule);
        }
        return allRules;
    }

    public static Rule extract(Ligature table)
    {

        int glyphId = table.getField(Ligature.LIG_GLYPH_INDEX);
        RuleSegment subst = new RuleSegment(glyphId);
        RuleSegment input = new RuleSegment();
        foreach (NumRecord record in table.recordList)
        {
            input.add(record.value);
        }
        return new Rule(null, input, null, subst);
    }

    public static ISet<Rule> extract(SingleSubst table)
    {
        switch (table.format)
        {
            case 1:
                return extract(table.fmt1Table());
            case 2:
                return extract(table.fmt2Table());
            default:
                throw new IllegalArgumentException("unimplemented format " + table.format);
        }
    }

    public static ISet<Rule> extract(HeaderFmt1 fmt1Table)
    {
        IList<Integer> coverage = extract(fmt1Table.coverage);
        int delta = fmt1Table.getDelta();
        return Rule.deltaRules(coverage, delta);
    }

    public static ISet<Rule> extract(InnerArrayFmt2 fmt2Table)
    {
        IList<Integer> coverage = extract(fmt2Table.coverage);
        IList<Integer> substs = extract((RecordsTable<NumRecord>)fmt2Table);
        return Rule.oneToOneRules(coverage, substs);
    }

    public static ISet<Rule> extract(MultipleSubst table)
    {
        ISet<Rule> result = new HashSet<Rule>();

        GlyphList coverage = extract(table.coverage());
        int i = 0;
        foreach (NumRecordTable glyphIds in table)
        {
            RuleSegment input = new RuleSegment(coverage.get(i));

            GlyphList glyphList = extract(glyphIds);
            RuleSegment subst = new RuleSegment(glyphList);

            Rule rule = new Rule(null, input, null, subst);
            result.Add(rule);
            i++;
        }
        return result;
    }

    public static ISet<Rule> extract(AlternateSubst table)
    {
        ISet<Rule> result = new HashSet<Rule>();

        GlyphList coverage = extract(table.coverage());
        int i = 0;
        foreach (NumRecordTable glyphIds in table)
        {
            RuleSegment input = new RuleSegment(coverage.get(i));

            GlyphList glyphList = extract(glyphIds);
            GlyphGroup glyphGroup = new GlyphGroup(glyphList);
            RuleSegment subst = new RuleSegment(glyphGroup);

            Rule rule = new Rule(null, input, null, subst);
            result.Add(rule);
            i++;
        }
        return result;
    }

    public static ISet<Rule> extract(ContextSubst table, LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        switch (table.format)
        {
            case 1:
                return extract(table.fmt1Table(), lookupListTable, allLookupRules);
            case 2:
                return extract(table.fmt2Table(), lookupListTable, allLookupRules);
            default:
                throw new IllegalArgumentException("unimplemented format " + table.format);
        }
    }

    public static ISet<Rule> extract(SubRuleSetArray table, LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        GlyphList coverage = extract(table.coverage);

        ISet<Rule> result = new HashSet<Rule>();
        int i = 0;
        foreach (SubRuleSet subRuleSet in table)
        {
            ISet<Rule> subRules = extract(coverage.get(i), subRuleSet, lookupListTable, allLookupRules);
            result.AddRange(subRules);
            i++;
        }
        return result;
    }

    public static ISet<Rule> extract(
        Integer firstGlyph, SubRuleSet table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        ISet<Rule> result = new HashSet<Rule>();
        foreach (SubRule subRule in table)
        {
            ISet<Rule> subrules = extract(firstGlyph, subRule, lookupListTable, allLookupRules);
            if (subrules == null)
            {
                return null;
            }
            result.AddRange(subrules);
        }
        return result;
    }

    public static ISet<Rule> extract(
        Integer firstGlyph, SubRule table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        RuleSegment inputRow = new RuleSegment(firstGlyph);
        foreach (NumRecord record in table.inputGlyphs)
        {
            inputRow.add(record.value);
        }

        Rule ruleSansSubst = new Rule(null, inputRow, null, null);
        return applyChainingLookup(ruleSansSubst, table.lookupRecords, lookupListTable, allLookupRules);
    }

    public static ISet<Rule> extract(
        SubClassSetArray table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        GlyphList coverage = extract(table.coverage);
        IDictionary<Integer, GlyphGroup> classDef = extract(table.classDef);

        ISet<Rule> result = new HashSet<Rule>();
        int i = 0;
        foreach (SubClassSet subClassRuleSet in table)
        {
            if (subClassRuleSet != null)
            {
                ISet<Rule> subRules = extract(subClassRuleSet, i, classDef, lookupListTable, allLookupRules);
                result.AddRange(subRules);
            }
            i++;
        }
        return result;
    }

    public static ISet<Rule> extract(SubClassSet table, int firstInputClass,
        IDictionary<Integer, GlyphGroup> inputClassDef, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        ISet<Rule> result = new HashSet<Rule>();
        foreach (SubClassRule subRule in table)
        {
            ISet<Rule> subRules = extract(subRule, firstInputClass, inputClassDef, lookupListTable, allLookupRules);
            result.AddRange(subRules);
        }
        return result;
    }

    public static ISet<Rule> extract(SubClassRule table, int firstInputClass,
        IDictionary<Integer, GlyphGroup> inputClassDef, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        RuleSegment input = extract(firstInputClass, table.inputClasses(), inputClassDef);

        Rule ruleSansSubst = new Rule(null, input, null, null);
        return applyChainingLookup(ruleSansSubst, table.lookupRecords, lookupListTable, allLookupRules);
    }

    public static ISet<Rule> extract(
        ChainContextSubst table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        switch (table.format)
        {
            case 1:
                return extract(table.fmt1Table(), lookupListTable, allLookupRules);
            case 2:
                return extract(table.fmt2Table(), lookupListTable, allLookupRules);
            case 3:
                return extract(table.fmt3Table(), lookupListTable, allLookupRules);
            default:
                throw new IllegalArgumentException("unimplemented format " + table.format);
        }
    }

    public static ISet<Rule> extract(
        ChainSubRuleSetArray table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        GlyphList coverage = extract(table.coverage);

        ISet<Rule> result = new HashSet<Rule>();
        int i = 0;
        foreach (ChainSubRuleSet subRuleSet in table)
        {
            ISet<Rule> subRules = extract(coverage.get(i), subRuleSet, lookupListTable, allLookupRules);
            result.AddRange(subRules);
            i++;
        }
        return result;
    }

    public static ISet<Rule> extract(
        Integer firstGlyph, ChainSubRuleSet table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        ISet<Rule> result = new HashSet<Rule>();
        foreach (ChainSubRule subRule in table)
        {
            result.AddRange(extract(firstGlyph, subRule, lookupListTable, allLookupRules));
        }
        return result;
    }

    public static ISet<Rule> extract(
        Integer firstGlyph, ChainSubRule table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        RuleSegment inputRow = new RuleSegment(firstGlyph);
        foreach (NumRecord record in table.inputClasses)
        {
            inputRow.add(record.value);
        }

        RuleSegment backtrack = ruleSegmentFromGlyphs(table.backtrackGlyphs);
        RuleSegment lookAhead = ruleSegmentFromGlyphs(table.lookAheadGlyphs);

        Rule ruleSansSubst = new Rule(backtrack, inputRow, lookAhead, null);
        return applyChainingLookup(ruleSansSubst, table.lookupRecords, lookupListTable, allLookupRules);
    }

    public static RuleSegment ruleSegmentFromGlyphs(NumRecordList records)
    {
        RuleSegment segment = new RuleSegment();
        foreach (NumRecord record in records)
        {
            segment.Add(new GlyphGroup(record.value));
        }
        return segment;
    }

    public static ISet<Rule> extract(
        ChainSubClassSetArray table, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        IDictionary<Integer, GlyphGroup> backtrackClassDef = extract(table.backtrackClassDef);
        IDictionary<Integer, GlyphGroup> inputClassDef = extract(table.inputClassDef);
        IDictionary<Integer, GlyphGroup> lookAheadClassDef = extract(table.lookAheadClassDef);

        ISet<Rule> result = new HashSet<Rule>();
        int i = 0;
        foreach (ChainSubClassSet chainSubRuleSet in table)
        {
            if (chainSubRuleSet != null)
            {
                result.AddRange(extract(chainSubRuleSet,
                    backtrackClassDef,
                    i,
                    inputClassDef,
                    lookAheadClassDef,
                    lookupListTable,
                    allLookupRules));
            }
            i++;
        }
        return result;
    }

    public static IDictionary<Integer, GlyphGroup> extract(ClassDefTable table)
    {
        switch (table.format)
        {
            case 1:
                return extract(table.fmt1Table());
            case 2:
                return extract(table.fmt2Table());
            default:
                throw new IllegalArgumentException("unimplemented format " + table.format);
        }
    }

    public static IDictionary<Integer, GlyphGroup> extract(classdef.InnerArrayFmt1 table)
    {
        IDictionary<Integer, GlyphGroup> result = new Dictionary<Integer, GlyphGroup>();
        int glyphId = table.getField(classdef.InnerArrayFmt1.START_GLYPH_INDEX);
        foreach (NumRecord record in table)
        {
            int classId = record.value;
            if (!result.containsKey(classId))
            {
                result.put(classId, new GlyphGroup());
            }

            result.get(classId).add(glyphId);
            glyphId++;
        }
        return result;
    }

    public static IList<Rule> extract(ChainSubClassSet table,
        IDictionary<Integer, GlyphGroup> backtrackClassDef,
        int firstInputClass,
        IDictionary<Integer, GlyphGroup> inputClassDef,
        IDictionary<Integer, GlyphGroup> lookAheadClassDef,
        LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        IList<Rule> result = new List<Rule>();
        foreach (ChainSubClassRule chainSubRule in table)
        {
            result.AddRange(extract(chainSubRule,
                backtrackClassDef,
                firstInputClass,
                inputClassDef,
                lookAheadClassDef,
                lookupListTable,
                allLookupRules));
        }
        return result;
    }

    public static ISet<Rule> extract(ChainSubClassRule table,
        IDictionary<Integer, GlyphGroup> backtrackClassDef,
        int firstInputClass,
        IDictionary<Integer, GlyphGroup> inputClassDef,
        IDictionary<Integer, GlyphGroup> lookAheadClassDef,
        LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        RuleSegment backtrack = ruleSegmentFromClasses(table.backtrackGlyphs, backtrackClassDef);
        RuleSegment inputRow = extract(firstInputClass, table.inputClasses, inputClassDef);
        RuleSegment lookAhead = ruleSegmentFromClasses(table.lookAheadGlyphs, lookAheadClassDef);

        Rule ruleSansSubst = new Rule(backtrack, inputRow, lookAhead, null);
        return applyChainingLookup(ruleSansSubst, table.lookupRecords, lookupListTable, allLookupRules);
    }

    public static RuleSegment extract(
        int firstInputClass, NumRecordList inputClasses, IDictionary<Integer, GlyphGroup> classDef)
    {
        RuleSegment input = new RuleSegment(classDef.get(firstInputClass));
        foreach (NumRecord inputClass in inputClasses)
        {
            int classId = inputClass.value;
            GlyphGroup glyphs = classDef.get(classId);
            if (glyphs == null && classId == 0)
            {
                // Any glyph not mentioned in the classes
                glyphs = GlyphGroup.inverseGlyphGroup(classDef.values());
            }
            input.Add(glyphs);
        }
        return input;
    }

    public static RuleSegment ruleSegmentFromClasses(
        NumRecordList classes, IDictionary<Integer, GlyphGroup> classDef)
    {
        RuleSegment segment = new RuleSegment();
        foreach (NumRecord classRecord in classes)
        {
            int classId = classRecord.value;
            GlyphGroup glyphs = classDef.get(classId);
            if (glyphs == null && classId == 0)
            {
                // Any glyph not mentioned in the classes
                glyphs = GlyphGroup.inverseGlyphGroup(classDef.values());
            }
            segment.Add(glyphs);
        }
        return segment;
    }

    public static ISet<Rule> extract(InnerArraysFmt3 table, LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allLookupRules)
    {
        RuleSegment backtrackContext = extract(table.backtrackGlyphs);
        RuleSegment input = extract(table.inputGlyphs);
        RuleSegment lookAheadContext = extract(table.lookAheadGlyphs);

        Rule ruleSansSubst = new Rule(backtrackContext, input, lookAheadContext, null);
        ISet<Rule> result = applyChainingLookup(
            ruleSansSubst, table.lookupRecords, lookupListTable, allLookupRules);
        return result;
    }

    public static ISet<Rule> extract(ReverseChainSingleSubst table)
    {
        IList<Integer> coverage = extract(table.coverage);

        RuleSegment backtrackContext = new RuleSegment();
        backtrackContext.addAll(extract(table.backtrackGlyphs));

        RuleSegment lookAheadContext = new RuleSegment();
        lookAheadContext.addAll(extract(table.lookAheadGlyphs));

        IList<Integer> substs = extract(table.substitutes);

        return Rule.oneToOneRules(backtrackContext, coverage, lookAheadContext, substs);
    }

    public static ISet<Rule> applyChainingLookup(Rule ruleSansSubst,
        SubstLookupRecordList lookups, LookupListTable lookupListTable, IDictionary<Integer, ISet<Rule>> allLookupRules)
    {

        IList<Rule> targetRules = new List<Rule>();
        targetRules.Add(ruleSansSubst);
        foreach (SubstLookupRecord lookup in lookups)
        {
            int at = lookup.sequenceIndex;
            int lookupIndex = lookup.lookupListIndex;
            ISet<Rule> rulesToApply = extract(lookupListTable, allLookupRules, lookupIndex);
            if (rulesToApply == null)
            {
                throw new IllegalArgumentException(
                    "Out of bound lookup index for chaining lookup: " + lookupIndex);
            }
            LinkedList<Rule> newRules = Rule.applyRulesOnRules(rulesToApply, targetRules, at);

            IList<Rule> _result = new List<Rule>();
            _result.AddRange(newRules);
            _result.AddRange(targetRules);
            targetRules = _result;
        }

        ISet<Rule> result = new HashSet<Rule>();
        foreach (Rule rule in targetRules)
        {
            if (rule.subst == null)
            {
                continue;
            }
            result.Add(rule);
        }
        return result;
    }

    public static IDictionary<Integer, ISet<Rule>> extract(LookupListTable table)
    {
        IDictionary<Integer, ISet<Rule>> allRules = new Dictionary<Integer, ISet<Rule>>();
        for (int i = 0; i < table.subTableCount(); i++)
        {
            extract(table, allRules, i);
        }
        return allRules;
    }

    public static ISet<Rule> extract(LookupListTable lookupListTable,
        IDictionary<Integer, ISet<Rule>> allRules, int i)
    {
        if (allRules.containsKey(i))
        {
            return allRules.get(i);
        }

        HashSet<Rule> rules = new HashSet<Rule>();

        LookupTable lookupTable = lookupListTable.subTableAt(i);
        GsubLookupType lookupType = lookupTable.lookupType();
        foreach (SubstSubtable _substSubtable in lookupTable)
        {
            var substSubtable = _substSubtable;
            GsubLookupType subTableLookupType = lookupType;

            if (lookupType == GsubLookupType.GSUB_EXTENSION)
            {
                ExtensionSubst extensionSubst = (ExtensionSubst)substSubtable;
                substSubtable = extensionSubst.subTable();
                subTableLookupType = extensionSubst.lookupType();
            }

            ISet<Rule> subrules = null;

            switch (subTableLookupType.name())
            {
                case nameof(GsubLookupType.GSUB_LIGATURE):
                    subrules = extract((LigatureSubst)substSubtable);
                    break;
                case nameof(GsubLookupType.GSUB_SINGLE):
                    subrules = extract((SingleSubst)substSubtable);
                    break;
                case nameof(GsubLookupType.GSUB_ALTERNATE):
                    subrules = extract((AlternateSubst)substSubtable);
                    break;
                case nameof(GsubLookupType.GSUB_MULTIPLE):
                    subrules = extract((MultipleSubst)substSubtable);
                    break;
                case nameof(GsubLookupType.GSUB_REVERSE_CHAINING_CONTEXTUAL_SINGLE):
                    subrules = extract((ReverseChainSingleSubst)substSubtable);
                    break;
                case nameof(GsubLookupType.GSUB_CHAINING_CONTEXTUAL):
                    subrules = extract((ChainContextSubst)substSubtable, lookupListTable, allRules);
                    break;
                case nameof(GsubLookupType.GSUB_CONTEXTUAL):
                    subrules = extract((ContextSubst)substSubtable, lookupListTable, allRules);
                    break;
                default:
                    throw new IllegalStateException();
            }
            if (subrules == null)
            {
                throw new IllegalStateException();
            }
            rules.AddRange(subrules);
        }

        if (rules.Count == 0)
        {
            Console.Error.WriteLine("There are no rules in lookup " + i);
        }
        allRules.put(i, rules);
        return rules;
    }

    public static RuleSegment extract(CoverageArray table)
    {
        RuleSegment result = new RuleSegment();
        foreach (CoverageTable coverage in table)
        {
            GlyphGroup glyphGroup = new GlyphGroup();
            glyphGroup.addAll(extract(coverage));
            result.Add(glyphGroup);
        }
        return result;
    }
}
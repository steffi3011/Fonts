// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

namespace SixLabors.Fonts.Tables.AdvancedTypographic.Gsub
{
    /// <summary>
    /// A Chained Contexts Substitution subtable describes glyph substitutions in context
    /// with an ability to look back and/or look ahead in the sequence of glyphs.
    /// <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-6-chained-contexts-substitution-subtable"/>
    /// </summary>
    internal sealed class LookupType6SubTable
    {
        private LookupType6SubTable()
        {
        }

        public static LookupSubTable Load(BigEndianBinaryReader reader, long offset)
        {
            reader.Seek(offset, SeekOrigin.Begin);
            ushort substFormat = reader.ReadUInt16();

            return substFormat switch
            {
                1 => LookupType6Format1SubTable.Load(reader, offset),
                2 => LookupType6Format2SubTable.Load(reader, offset),
                3 => LookupType6Format3SubTable.Load(reader, offset),
                _ => throw new InvalidFontFileException($"Invalid value for 'substFormat' {substFormat}. Should be '1', '2', or '3'."),
            };
        }
    }

    internal sealed class LookupType6Format1SubTable : LookupSubTable
    {
        private readonly ChainedSequenceRuleSetTable[] seqRuleSetTables;
        private readonly CoverageTable coverageTable;

        private LookupType6Format1SubTable(ChainedSequenceRuleSetTable[] seqRuleSetTables, CoverageTable coverageTable)
        {
            this.seqRuleSetTables = seqRuleSetTables;
            this.coverageTable = coverageTable;
        }

        public static LookupType6Format1SubTable Load(BigEndianBinaryReader reader, long offset)
        {
            // ChainedSequenceContextFormat1
            // +----------+--------------------------------------------------+------------------------------------------+
            // | Type     | Name                                             | Description                              |
            // +==========+==================================================+==========================================+
            // | uint16   | format                                           | Format identifier: format = 1            |
            // +----------+--------------------------------------------------+------------------------------------------+
            // | Offset16 | coverageOffset                                   | Offset to Coverage table, from beginning |
            // |          |                                                  | of ChainSequenceContextFormat1 table     |
            // +----------+--------------------------------------------------+------------------------------------------+
            // | uint16   | chainedSeqRuleSetCount                           | Number of ChainedSequenceRuleSet tables  |
            // +----------+--------------------------------------------------+------------------------------------------+
            // | Offset16 | chainedSeqRuleSetOffsets[chainedSeqRuleSetCount] | Array of offsets to ChainedSeqRuleSet    |
            // |          |                                                  | tables, from beginning of                |
            // |          |                                                  | ChainedSequenceContextFormat1 table      |
            // |          |                                                  | (may be NULL)                            |
            // +----------+--------------------------------------------------+------------------------------------------+
            ushort coverageOffset = reader.ReadOffset16();
            ushort chainedSeqRuleSetCount = reader.ReadUInt16();
            ushort[] chainedSeqRuleSetOffsets = reader.ReadUInt16Array(chainedSeqRuleSetCount);

            var seqRuleSets = new ChainedSequenceRuleSetTable[chainedSeqRuleSetCount];

            for (int i = 0; i < seqRuleSets.Length; i++)
            {
                seqRuleSets[i] = ChainedSequenceRuleSetTable.Load(reader, offset + chainedSeqRuleSetOffsets[i]);
            }

            var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);
            return new LookupType6Format1SubTable(seqRuleSets, coverageTable);
        }

        public override bool TrySubstitution(GSubTable table, GlyphSubstitutionCollection collection, ushort index, int count)
        {
            int glyphId = collection[index][0];
            if (glyphId < 0)
            {
                return false;
            }

            int offset = this.coverageTable.CoverageIndexOf((ushort)glyphId);
            if (offset > -1)
            {
                // TODO: Implement
                // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#61-chained-contexts-substitution-format-1-simple-glyph-contexts
                return false;
            }

            return false;
        }
    }

    internal sealed class LookupType6Format2SubTable : LookupSubTable
    {
        private readonly CoverageTable coverageTable;
        private readonly ClassDefinitionTable inputClassDefinitionTable;
        private readonly ClassDefinitionTable backtrackClassDefinitionTable;
        private readonly ClassDefinitionTable lookaheadClassDefinitionTable;
        private readonly ChainedClassSequenceRuleSetTable[] sequenceRuleSetTables;

        private LookupType6Format2SubTable(
            ChainedClassSequenceRuleSetTable[] sequenceRuleSetTables,
            ClassDefinitionTable backtrackClassDefinitionTable,
            ClassDefinitionTable inputClassDefinitionTable,
            ClassDefinitionTable lookaheadClassDefinitionTable,
            CoverageTable coverageTable)
        {
            this.sequenceRuleSetTables = sequenceRuleSetTables;
            this.backtrackClassDefinitionTable = backtrackClassDefinitionTable;
            this.inputClassDefinitionTable = inputClassDefinitionTable;
            this.lookaheadClassDefinitionTable = lookaheadClassDefinitionTable;
            this.coverageTable = coverageTable;
        }

        public static LookupType6Format2SubTable Load(BigEndianBinaryReader reader, long offset)
        {
            // ChainedSequenceContextFormat2
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Type     | Name                                                       | Description                                                         |
            // +==========+============================================================+=====================================================================+
            // | uint16   | format                                                     | Format identifier: format = 2                                       |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Offset16 | coverageOffset                                             | Offset to Coverage table, from beginning                            |
            // |          |                                                            | of ChainedSequenceContextFormat2 table                              |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Offset16 | backtrackClassDefOffset                                    | Offset to ClassDef table containing                                 |
            // |          |                                                            | backtrack sequence context, from                                    |
            // |          |                                                            | beginning of ChainedSequenceContextFormat2 table                    |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Offset16 | inputClassDefOffset                                        | Offset to ClassDef table containing input                           |
            // |          |                                                            | sequence context, from beginning of                                 |
            // |          |                                                            | ChainedSequenceContextFormat2 table                                 |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Offset16 | lookaheadClassDefOffset                                    | Offset to ClassDef table containing                                 |
            // |          |                                                            | lookahead sequence context, from                                    |
            // |          |                                                            | beginning of ChainedSequenceContextFormat2 table                    |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | uint16   | chainedClassSeqRuleSetCount                                | Number of ChainedClassSequenceRuleSet tables                        |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            // | Offset16 | chainedClassSeqRuleSetOffsets[chainedClassSeqRuleSetCount] | Array of offsets to ChainedClassSequenceRuleSet tables,             |
            // |          |                                                            | from beginning of ChainedSequenceContextFormat2 table (may be NULL) |
            // +----------+------------------------------------------------------------+---------------------------------------------------------------------+
            ushort coverageOffset = reader.ReadOffset16();
            ushort backtrackClassDefOffset = reader.ReadOffset16();
            ushort inputClassDefOffset = reader.ReadOffset16();
            ushort lookaheadClassDefOffset = reader.ReadOffset16();
            ushort chainedClassSeqRuleSetCount = reader.ReadUInt16();
            ChainedClassSequenceRuleSetTable[] seqRuleSets = Array.Empty<ChainedClassSequenceRuleSetTable>();
            if (chainedClassSeqRuleSetCount != 0)
            {
                ushort[] chainedClassSeqRuleSetOffsets = new ushort[chainedClassSeqRuleSetCount];
                for (int i = 0; i < chainedClassSeqRuleSetCount; i++)
                {
                    chainedClassSeqRuleSetOffsets[i] = reader.ReadOffset16();
                }

                seqRuleSets = new ChainedClassSequenceRuleSetTable[chainedClassSeqRuleSetCount];
                for (int i = 0; i < seqRuleSets.Length; i++)
                {
                    if (chainedClassSeqRuleSetOffsets[i] > 0)
                    {
                        seqRuleSets[i] = ChainedClassSequenceRuleSetTable.Load(reader, offset + chainedClassSeqRuleSetOffsets[i]);
                    }
                }
            }

            var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);
            var backtrackClassDefTable = ClassDefinitionTable.Load(reader, offset + backtrackClassDefOffset);
            var inputClassDefTable = ClassDefinitionTable.Load(reader, offset + inputClassDefOffset);
            var lookaheadClassDefTable = ClassDefinitionTable.Load(reader, offset + lookaheadClassDefOffset);

            return new LookupType6Format2SubTable(seqRuleSets, backtrackClassDefTable, inputClassDefTable, lookaheadClassDefTable, coverageTable);
        }

        public override bool TrySubstitution(GSubTable table, GlyphSubstitutionCollection collection, ushort index, int count)
        {
            // Implements Chained Contexts Substitution for Format 2:
            // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#62-chained-contexts-substitution-format-2-class-based-glyph-contexts
            int glyphId = collection[index][0];
            if (glyphId < 0)
            {
                return false;
            }

            // Search for the current glyph in the Coverage table.
            int offset = this.coverageTable.CoverageIndexOf((ushort)glyphId);
            if (offset <= -1)
            {
                return false;
            }

            // Search in the class definition table to find the class value assigned to the currently glyph.
            int classId = this.inputClassDefinitionTable.ClassIndexOf((ushort)glyphId);
            ChainedClassSequenceRuleTable[]? rules = classId >= 0 && classId < this.sequenceRuleSetTables.Length ? this.sequenceRuleSetTables[classId].SubRules : null;
            if (rules is null)
            {
                return false;
            }

            // Apply ruleset for the given glyph class id.
            for (int lookupIndex = 0; lookupIndex < rules.Length; lookupIndex++)
            {
                ChainedClassSequenceRuleTable rule = rules[lookupIndex];
                if (rule.InputSequence.Length > 0 &&
                    !GSubUtils.MatchInputSequence(collection, index, rule.InputSequence))
                {
                    continue;
                }

                if (rule.BacktrackSequence.Length > 0
                    && !GSubUtils.MatchBacktrackClassIdSequence(collection, index, rule.BacktrackSequence.Length, rule.BacktrackSequence, this.backtrackClassDefinitionTable))
                {
                    continue;
                }

                if (rule.LookaheadSequence.Length > 0
                    && !GSubUtils.MatchLookAheadClassIdSequence(collection, index, 1 + rule.InputSequence.Length, rule.LookaheadSequence, this.lookaheadClassDefinitionTable))
                {
                    continue;
                }

                LookupTable lookup = table.LookupList.LookupTables[lookupIndex];
                if (lookup.TrySubstitution(table, collection, (ushort)lookupIndex, 1))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class LookupType6Format3SubTable : LookupSubTable
    {
        private readonly SequenceLookupRecord[] seqLookupRecords;
        private readonly CoverageTable[] backtrackCoverageTables;
        private readonly CoverageTable[] inputCoverageTables;
        private readonly CoverageTable[] lookaheadCoverageTables;

        private LookupType6Format3SubTable(
            SequenceLookupRecord[] seqLookupRecords,
            CoverageTable[] backtrackCoverageTables,
            CoverageTable[] inputCoverageTables,
            CoverageTable[] lookaheadCoverageTables)
        {
            this.seqLookupRecords = seqLookupRecords;
            this.backtrackCoverageTables = backtrackCoverageTables;
            this.inputCoverageTables = inputCoverageTables;
            this.lookaheadCoverageTables = lookaheadCoverageTables;
        }

        public static LookupType6Format3SubTable Load(BigEndianBinaryReader reader, long offset)
        {
            // ChainedSequenceContextFormat3 1
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | Type                 | Name                                          | Description                                                    |
            // +======================+===============================================+================================================================+
            // | uint16               | format                                        | Format identifier: format = 3                                  |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | uint16               | backtrackGlyphCount                           | Number of glyphs in the backtrack sequence                     |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | Offset16             | backtrackCoverageOffsets[backtrackGlyphCount] | Array of offsets to coverage tables for the backtrack sequence |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | uint16               | inputGlyphCount                               | Number of glyphs in the input sequence                         |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | Offset16             | inputCoverageOffsets[inputGlyphCount]         | Array of offsets to coverage tables for the input sequence     |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | uint16               | lookaheadGlyphCount                           | Number of glyphs in the lookahead sequence                     |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | Offset16             | lookaheadCoverageOffsets[lookaheadGlyphCount] | Array of offsets to coverage tables for the lookahead sequence |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | uint16               | seqLookupCount                                | Number of SequenceLookupRecords                                |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            // | SequenceLookupRecord | seqLookupRecords[seqLookupCount]              | Array of SequenceLookupRecords                                 |
            // +----------------------+-----------------------------------------------+----------------------------------------------------------------+
            ushort backtrackGlyphCount = reader.ReadUInt16();
            ushort[] backtrackCoverageOffsets = reader.ReadUInt16Array(backtrackGlyphCount);

            ushort inputGlyphCount = reader.ReadUInt16();
            ushort[] inputCoverageOffsets = reader.ReadUInt16Array(inputGlyphCount);

            ushort lookaheadGlyphCount = reader.ReadUInt16();
            ushort[] lookaheadCoverageOffsets = reader.ReadUInt16Array(lookaheadGlyphCount);

            ushort seqLookupCount = reader.ReadUInt16();
            SequenceLookupRecord[] seqLookupRecords = SequenceLookupRecord.LoadArray(reader, seqLookupCount);

            CoverageTable[] backtrackCoverageTables = CoverageTable.LoadArray(reader, offset, backtrackCoverageOffsets);
            CoverageTable[] inputCoverageTables = CoverageTable.LoadArray(reader, offset, inputCoverageOffsets);
            CoverageTable[] lookaheadCoverageTables = CoverageTable.LoadArray(reader, offset, lookaheadCoverageOffsets);

            return new LookupType6Format3SubTable(seqLookupRecords, backtrackCoverageTables, inputCoverageTables, lookaheadCoverageTables);
        }

        public override bool TrySubstitution(GSubTable table, GlyphSubstitutionCollection collection, ushort index, int count)
        {
            int glyphId = collection[index][0];
            if (glyphId < 0)
            {
                return false;
            }

            int inputLength = this.inputCoverageTables.Length;

            // Check that there are enough context glyphs.
            if (index < this.backtrackCoverageTables.Length
                || inputLength + this.lookaheadCoverageTables.Length > count)
            {
                return false;
            }

            // Check all coverages: if any of them does not match, abort substitution.
            for (int i = 0; i < this.inputCoverageTables.Length; ++i)
            {
                int id = collection[index + i][0];
                if (id < 0 || this.inputCoverageTables[i].CoverageIndexOf((ushort)id) < 0)
                {
                    return false;
                }
            }

            for (int i = 0; i < this.backtrackCoverageTables.Length; ++i)
            {
                int id = collection[index - 1 - i][0];
                if (id < 0 || this.backtrackCoverageTables[i].CoverageIndexOf((ushort)id) < 0)
                {
                    return false;
                }
            }

            for (int i = 0; i < this.lookaheadCoverageTables.Length; ++i)
            {
                int id = collection[index + inputLength + i][0];
                if (id < 0 || this.lookaheadCoverageTables[i].CoverageIndexOf((ushort)id) < 0)
                {
                    return false;
                }
            }

            // It's a match. Perform substitutions and return true if anything changed.
            bool hasChanged = false;
            foreach (SequenceLookupRecord lookupRecord in this.seqLookupRecords)
            {
                ushort sequenceIndex = lookupRecord.SequenceIndex;
                ushort lookupIndex = lookupRecord.LookupListIndex;

                LookupTable lookup = table.LookupList.LookupTables[lookupIndex];
                if (lookup.TrySubstitution(table, collection, (ushort)(index + sequenceIndex), count - sequenceIndex))
                {
                    hasChanged = true;
                }
            }

            return hasChanged;
        }
    }
}

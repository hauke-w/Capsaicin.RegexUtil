using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// A <see cref="NestedCaptureGroupingDefinition"/> defined for a <see cref="CaptureGroup"/>.
    /// </summary>
    public sealed class PartialNestedCaptureGroupingDefinition : NestedCaptureGroupingDefinition
    {
        internal PartialNestedCaptureGroupingDefinition(CaptureGroupingDefinitionBase parent, Group groupedBy, int index) : base(parent, groupedBy)
        {
            Index = index;
        }

        public sealed override int Index { get; }

        private int[] GetKeyIndexes(Group[] key)
        {
            var result = new int[key.Length];
            result[^2] = Index;

            var constraint = key[^2].Captures[Index];
            int max = constraint.Index + constraint.Length;

            var n = result.Length -2;
            bool found;
            for (int i = 0; i < n; i++)
            {
                found = false;
                var captures = key[i].Captures;
                for (int j = 0; j < captures.Count; j++)
                {
                    var capture = captures[j];
                    if (capture.Index <= constraint.Index && capture.Index + capture.Length >= max)
                    {
                        result[i] = j;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    result[i] = -1;
                }
            }

            var lastCaptures = key[^1].Captures;
            found = false;
            
            for (int j = 0; j < lastCaptures.Count; j++)
            {
                var capture = lastCaptures[j];
                if (capture.Index >= constraint.Index && capture.Index + capture.Length <= max)
                {
                    result[^1] = j;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                result[^1] = -1;
            }

            return result;
        }

        internal override IList<CaptureRowIndexes> GetCaptureIndexesWithinGroupedBy(Group[] columns)
        {
            var hierarchy = GetHierarchy();
            var keyHierarchy = hierarchy.Select(it => it.GroupedBy).ToArray();
            var keySearchIndexes = GetKeyIndexes(keyHierarchy);
            var columnSearchIndexes = new int[columns.Length];
            var result = new List<CaptureRowIndexes>();

            while (MoveToNextGroup())
            {
                var rowIndex = keySearchIndexes[^1];
                var row = GetCaptureIndexesRow(columns, columnSearchIndexes, rowIndex);
                var key = keySearchIndexes.Select((captureIndex, keyIndex) => keyHierarchy[keyIndex].Captures[captureIndex]).ToList();
                result.Add(row with { Key = key.ToArray() });
                keySearchIndexes[^1]++;
            }
            return result;

            bool MoveToNextGroup()
            {
                var currentCaptureIndexOfFirst = keySearchIndexes[0];
                var firstKeyGroup = keyHierarchy[0];
                if (currentCaptureIndexOfFirst >= 0 && currentCaptureIndexOfFirst < firstKeyGroup.Captures.Count)
                {
                    var captureOfFirst = firstKeyGroup.Captures[currentCaptureIndexOfFirst];
                    int min = captureOfFirst.Index;
                    int max = min + captureOfFirst.Length;

                    bool found = true;
                    for (int keyColumn = 1; keyColumn < keySearchIndexes.Length && found; keyColumn++)
                    {
                        var currentCaptureIndex = keySearchIndexes[keyColumn];
                        found = false;
                        if (currentCaptureIndex >= 0)
                        {
                            while (currentCaptureIndex < keyHierarchy[keyColumn].Captures.Count)
                            {
                                var currentCapture = keyHierarchy[keyColumn].Captures[currentCaptureIndex];
                                var captureEndIndex = currentCapture.Index + currentCapture.Length;
                                if (currentCapture.Index < min)
                                {
                                    keySearchIndexes[keyColumn] = ++currentCaptureIndex;
                                }
                                else if (captureEndIndex <= max)
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (found)
                    {
                        return true;
                    }

                    keySearchIndexes[0] = ++currentCaptureIndexOfFirst;
                }
                return false;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{

    public class NestedCaptureGroupingDefinition : CaptureGroupingDefinitionBase
    {
        internal NestedCaptureGroupingDefinition(CaptureGroupingDefinitionBase parent, Group groupedBy)
            : base(parent.Root, groupedBy)
        {
            Parent = parent;
        }

        public override CaptureGroupingDefinitionBase Parent { get; }

        public override int Index => 0;

        protected List<CaptureGroupingDefinitionBase> GetHierarchy()
        {
            var result = new List<CaptureGroupingDefinitionBase>()
            {
                this
            };

            CaptureGroupingDefinitionBase? parent = Parent;
            do
            {
                result.Add(Parent);
                parent = parent.Parent;
            } while (parent is not null);

            result.Reverse();
            return result;
        }


        internal override IList<CaptureRowIndexes> GetCaptureIndexesWithinGroupedBy(Group[] columns)
        {
            var hierarchy = GetHierarchy();
            var keyHierarchy = hierarchy.Select(it => it.GroupedBy).ToArray();
            var keySearchIndexes = new int[keyHierarchy.Length];
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
                while (currentCaptureIndexOfFirst < firstKeyGroup.Captures.Count)
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    // we do not implement IEnumerable<CaptureGroup> here because it would introduce ambiguities in sub class with more specific enumeration.
    public abstract class CaptureGroupingDefinitionBase : IEnumerable
    {
        internal CaptureGroupingDefinitionBase(CaptureGroupingRoot captureGroupingRoot, Group groupedBy)
        {
            Root = captureGroupingRoot;
            GroupedBy = groupedBy;
        }

        internal CaptureGroupingRoot Root { get; }
        public Group GroupedBy { get; }

        public abstract CaptureGroupingDefinitionBase? Parent { get; }

        private IList<CaptureRowIndexes>? _CaptureIndexes;
        internal IList<CaptureRowIndexes> GetCaptureIndexes() 
            => _CaptureIndexes ?? (_CaptureIndexes = GetCaptureIndexesWithinGroupedBy(Root.Groups.ToArray()));

        internal bool IsCaptureIndexesLoaded => _CaptureIndexes is not null;

        public abstract int Index { get; }

        //public IEnumerable<CaptureGroup> GetCaptureGroups()
        //{
        //    foreach (var item in this)
        //    {
        //        yield return item;
        //    }
        //}

        public IEnumerator<CaptureGroup> GetEnumerator()
        {
            var captureIndexes = GetCaptureIndexes();
            int nRows = captureIndexes.Count;
            for (int row = 0; row < nRows; row++)
            {
                var key = captureIndexes[row].Key;
                yield return new CaptureGroup(this, key, row);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public NestedCaptureGroupingDefinition ThenBy(GroupSpecifier group)
            => new NestedCaptureGroupingDefinition(this, group.GetGroup(Root.Match));

        public IEnumerable<ICaptureGrouping1> Into(GroupSpecifier toInclude)
            => Into(new[] { toInclude });

        public IEnumerable<ICaptureGrouping2> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2)
            => Into( new[] { toInclude1, toInclude2 });

        public IEnumerable<ICaptureGrouping5> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3)
            => Into(new[] { toInclude1, toInclude2, toInclude3 });

        public IEnumerable<ICaptureGrouping5> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3, GroupSpecifier toInclude4)
            => Into(new[] { toInclude1, toInclude2, toInclude3, toInclude4 });

        public IEnumerable<ICaptureGrouping5> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3, GroupSpecifier toInclude4, GroupSpecifier toInclude5)
            => Into(new[] { toInclude1, toInclude2, toInclude3, toInclude4, toInclude5 });

        public IEnumerable<CaptureGrouping> Into(params GroupSpecifier[] toInclude)
        {
            if (toInclude is null)
            {
                throw new ArgumentNullException(nameof(toInclude));
            }
            if (toInclude.Length == 0)
            {
                throw new ArgumentException("Groups to select is empty.", nameof(toInclude));
            }

            var groups = Root.GetGroups(toInclude);
            var columnIndexMap = Root.GetGroupIndexes(groups);

            foreach (var currentCaptureGroup in this)
            {
                var captureIndexes = new int[toInclude.Length];
                for (int i = 0; i < toInclude.Length; i++)
                {
                    var rootGroupIndex = columnIndexMap[i];
                    captureIndexes[i] = currentCaptureGroup.CaptureIndexes[rootGroupIndex];
                }
                yield return currentCaptureGroup.Into(groups, captureIndexes);
            }
        }

        public IEnumerable<IFlattenedCaptureGrouping2> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2)
        {
            return Flatten(new[] { toSelect1, toSelect2 });
        }

        public IEnumerable<IFlattenedCaptureGrouping3> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3)
        {
            return Flatten(new[] { toSelect1, toSelect2, toSelect3 });
        }

        public IEnumerable<IFlattenedCaptureGrouping4> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3, GroupSpecifier toSelect4)
        {
            return Flatten(new[] { toSelect1, toSelect2, toSelect3, toSelect4 });
        }

        public IEnumerable<IFlattenedCaptureGrouping5> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3, GroupSpecifier toSelect4, GroupSpecifier toSelect5)
        {
            return Flatten(new[] { toSelect1, toSelect2, toSelect3, toSelect4, toSelect5 });
        }

        public IEnumerable<FlattenedCaptureGrouping> Flatten(params GroupSpecifier[] toSelect)
        {
            foreach (var grouping in Into(toSelect))
            {
                foreach (var captureList in grouping.Captures)
                {
                    yield return new FlattenedCaptureGrouping(grouping.Grouping, grouping.Key, captureList);
                }
            }
        }

        internal record CaptureRowIndexes(int[] CaptureIndexes, params Capture[] Key);

        internal abstract IList<CaptureRowIndexes> GetCaptureIndexesWithinGroupedBy(Group[] columns);

        internal static int GetFirstCaptureIndexWithin(int min, int max, int startIndex, Group group)
        {
            var captures = group.Captures;
            var n = captures.Count;
            for (int i = startIndex; i < n; i++)
            {
                var capture = captures[i];
                if (capture.Index >= min && capture.Index + capture.Length <= max)
                {
                    return i;
                }
            }
            return -1;
        }

        internal CaptureRowIndexes GetCaptureIndexesRow(Group[] columns, int[] searchStartIndexes, int rowIndex)
        {
            // this method should not be called if indexes have already been evaluated.
            Debug.Assert(!IsCaptureIndexesLoaded);

            var parentCapture = GroupedBy.Captures[rowIndex];
            var rowIndexes = new int[columns.Length];
            int min = parentCapture.Index;
            int max = min + parentCapture.Length;

            for (int columnIndex = 0; columnIndex < rowIndexes.Length; columnIndex++)
            {
                var currentGroup = columns[columnIndex];
                if (currentGroup == GroupedBy)
                {
                    rowIndexes[columnIndex] = rowIndex;
                }
                else
                {
                    int searchStartIndex = searchStartIndexes[columnIndex];
                    int firstCaptureIndexWithinGroup;
                    if (searchStartIndex < currentGroup.Captures.Count)
                    {
                        firstCaptureIndexWithinGroup = GetFirstCaptureIndexWithin(min, max, searchStartIndexes[columnIndex], currentGroup);
                        searchStartIndexes[columnIndex] = firstCaptureIndexWithinGroup + 1;
                    }
                    else
                    {
                        firstCaptureIndexWithinGroup = -1;
                    }
                    rowIndexes[columnIndex] = firstCaptureIndexWithinGroup;
                }
            }
            return new CaptureRowIndexes(rowIndexes, parentCapture);
        }
    }
}

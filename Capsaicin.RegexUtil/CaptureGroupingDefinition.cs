using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGroupingDefinition
    {
        int Index { get; }

        CaptureGroupingDefinitionBase? Parent { get; }

        Group GroupedBy { get; }

        /// <summary>
        /// The capture groups defined by this grouping definition.
        /// </summary>
        IEnumerable<CaptureGroup> CaptureGroups { get; }

        /// <summary>
        /// The selected groups in the current grouping context.
        /// </summary>
        ImmutableArray<Group> SelectedGroups { get; }

        NestedCaptureGroupingDefinition ThenBy(GroupSpecifier group);

        IEnumerable<CaptureGrouping> Into(params GroupSpecifier[] toInclude);

        IEnumerable<CaptureGrouping> Into(params Group[] toInclude);

        IEnumerable<FlattenedCaptureGrouping> Flatten(params GroupSpecifier[] toSelect);

        IEnumerable<FlattenedCaptureGrouping> Flatten(params Group[] toSelect);

        /// <summary>
        /// Flattens the groupings and selects the captures from the <see cref="Group"/>s in the current grouping definition context (see <see cref="SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        IEnumerable<FlattenedCaptureGrouping> Flatten()
            => Flatten(SelectedGroups.ToArray());
    }

    public interface ICaptureGroupingDefinition1 : ICaptureGroupingDefinition, IEnumerable<IEnumerable<string>>
    {
        IEnumerable<ICaptureGrouping1> Into(GroupSpecifier toInclude);

        IEnumerable<string?> Flatten(GroupSpecifier toSelect);

        /// <summary>
        /// The grouped capture values from the first <see cref="Group"/>s in the current grouping definition context 
        /// (see <see cref="ICaptureGroupingDefinition.SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEnumerable<string>> Result
        {
            get
            {
                IEnumerable<ICaptureGrouping1> captureGroupings = Into(SelectedGroups[0]);
                return captureGroupings.Select(g => g.Values);
            }
        }

        IEnumerator<IEnumerable<string>> IEnumerable<IEnumerable<string>>.GetEnumerator()
            => Result.GetEnumerator();
    }

    public interface ICaptureGroupingDefinition2 : ICaptureGroupingDefinition1, IEnumerable<IEnumerable<(string Value1, string? Value2)>>
    {
        IEnumerable<ICaptureGrouping2> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2);

        /// <summary>
        /// The grouped capture tuples  that are the result of this grouping definition. Included are Captures for the first two <see cref="Group"/>s 
        /// in the current grouping definition context (see <see cref="ICaptureGroupingDefinition.SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IEnumerable<(string Value1, string? Value2)>> Result
        {
            get
            {
                IEnumerable<ICaptureGrouping2> captureGroupings = Into(SelectedGroups.Take(2));
                return captureGroupings.Select(g => g.Values);
            }
        }

        IEnumerator<IEnumerable<(string Value1, string? Value2)>> IEnumerable<IEnumerable<(string Value1, string? Value2)>>.GetEnumerator()
            => Result.GetEnumerator();
    }

    public interface ICaptureGroupingDefinition3 : ICaptureGroupingDefinition2, IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3)>>
    {
        IEnumerable<ICaptureGrouping3> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3);

        IEnumerable<IFlattenedCaptureGrouping3> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3);

        /// <summary>
        /// The grouped capture tuples  that are the result of this grouping definition. Included are Captures for the first three <see cref="Group"/>s 
        /// in the current grouping definition context (see <see cref="ICaptureGroupingDefinition.SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3)>> Result
        {
            get
            {
                IEnumerable<ICaptureGrouping3> captureGroupings = Into(SelectedGroups.Take(3));
                return captureGroupings.Select(g => g.Values);
            }
        }

        IEnumerator<IEnumerable<(string Value1, string? Value2, string? Value3)>> IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3)>>.GetEnumerator()
            => Result.GetEnumerator();
    }

    public interface ICaptureGroupingDefinition4 : ICaptureGroupingDefinition3, IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4)>>
    {
        IEnumerable<ICaptureGrouping4> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3, GroupSpecifier toInclude4);

        IEnumerable<IFlattenedCaptureGrouping4> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3, GroupSpecifier toSelect4);

        /// <summary>
        /// The grouped capture tuples  that are the result of this grouping definition. Included are Captures for the first four <see cref="Group"/>s 
        /// in the current grouping definition context (see <see cref="ICaptureGroupingDefinition.SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4)>> Result
        {
            get
            {
                IEnumerable<ICaptureGrouping4> captureGroupings = Into(SelectedGroups.Take(4));
                return captureGroupings.Select(g => g.Values);
            }
        }

        IEnumerator<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4)>> IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4)>>.GetEnumerator()
            => Result.GetEnumerator();
    }

    public interface ICaptureGroupingDefinition5 : ICaptureGroupingDefinition4, IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4, string? Value5)>>
    {
        IEnumerable<ICaptureGrouping5> Into(GroupSpecifier toInclude1, GroupSpecifier toInclude2, GroupSpecifier toInclude3, GroupSpecifier toInclude4, GroupSpecifier toInclude5);

        IEnumerable<IFlattenedCaptureGrouping5> Flatten(GroupSpecifier toSelect1, GroupSpecifier toSelect2, GroupSpecifier toSelect3, GroupSpecifier toSelect4, GroupSpecifier toSelect5);

        /// <summary>
        /// The grouped capture tuples  that are the result of this grouping definition. Included are Captures for the first five <see cref="Group"/>s 
        /// in the current grouping definition context (see <see cref="ICaptureGroupingDefinition.SelectedGroups"/>).
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4, string? Value5)>> Result
        {
            get
            {
                IEnumerable<ICaptureGrouping5> captureGroupings = Into(SelectedGroups.Take(5));
                return captureGroupings.Select(g => g.Values);
            }
        }

        IEnumerator<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4, string? Value5)>> IEnumerable<IEnumerable<(string Value1, string? Value2, string? Value3, string? Value4, string? Value5)>>.GetEnumerator()
            => Result.GetEnumerator();
    }

    public sealed class CaptureGroupingDefinition : CaptureGroupingDefinitionBase
    {
        internal CaptureGroupingDefinition(CaptureGroupingRoot captureGroupingRoot, Group groupedBy)
            : base(captureGroupingRoot, groupedBy)
        {
        }

        public sealed override CaptureGroupingDefinitionBase? Parent => null;

        public sealed override int Index => 0;

        internal override IList<CaptureRowIndexes> GetCaptureIndexesWithinGroupedBy(Group[] columns)
        {
            // this method should not be called if indexes have already been evaluated.
            Debug.Assert(!IsCaptureIndexesLoaded);

            var groupCaptures = GroupedBy.Captures;
            int nRows = groupCaptures.Count;
            var result = new CaptureRowIndexes[nRows];
            var searchStartIndexes = new int[columns.Length];
            for (var rowIndex = 0; rowIndex < nRows; rowIndex++)
            {
                result[rowIndex] = GetCaptureIndexesRow(columns, searchStartIndexes, rowIndex);
            }

            return result;
        }
    }
}

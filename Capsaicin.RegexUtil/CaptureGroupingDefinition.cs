using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public sealed class CaptureGroupingDefinition : CaptureGroupingDefinitionBase, IEnumerable<ICaptureGroup1>
    {
        internal CaptureGroupingDefinition(CaptureGroupingRoot captureGroupingRoot, Group groupedBy)
            : base(captureGroupingRoot, groupedBy)
        {
        }

        public override sealed CaptureGroupingDefinitionBase? Parent => null;

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

        public new IEnumerator<ICaptureGroup1> GetEnumerator() => base.GetEnumerator();
    }
}

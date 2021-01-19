using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// Capture group with one key capture
    /// </summary>
    public interface ICaptureGroup1 : ICaptureGroup
    {
        /// <summary>
        /// Gets the last capture in <see cref="ICaptureGroupBase.Key"/>.
        /// </summary>
        new Capture Key { get; }
    }

    public interface ICaptureGroupBase
    {
        Capture[] Key { get; }

        Capture? First(GroupSpecifier group);

        IParentCaptureGroup? Parent { get; }
    }

    public interface ICaptureGroup : ICaptureGroupBase
    {
        NestedCaptureGroupingDefinition GroupNestedBy(GroupSpecifier group);
        CaptureGrouping Into(params GroupSpecifier[] toSelect);
        IEnumerable<string> Select(GroupSpecifier toSelect);
        IEnumerable<string?[]> Select(params GroupSpecifier[] toSelect);
    }

    public interface IParentCaptureGroup : ICaptureGroupBase
    {
    }

    public abstract class CaptureGroupBase
    {
        internal CaptureGroupBase(CaptureGroupingDefinitionBase definition, Capture[] key)
        {
            Definition = definition;
            Key = key;
        }

        protected CaptureGroupingDefinitionBase Definition { get; }

        public Capture[] Key { get; }

        private IParentCaptureGroup? _Parent;
        public IParentCaptureGroup? Parent => _Parent ?? (_Parent = EvaluateParent());

        private IParentCaptureGroup? EvaluateParent()
        {
            return Definition.Parent is null 
                ? null 
                : new ParentCaptureGroup(Definition.Parent, Key[..^1]);
        }
    }

    internal class ParentCaptureGroup : CaptureGroupBase, IParentCaptureGroup
    {
        // TODO: implement ICaptureGroup? This would make interface IParentCaptureGroup obsolete

        public ParentCaptureGroup(CaptureGroupingDefinitionBase definition, Capture[] key)
            : base(definition, key)
        {
        }

        public Capture? First(GroupSpecifier group)
        {
            var root = Definition.Root;
            var groupObj = group.GetGroup(root.Match);
            var key = Key[^1];

            int max = key.Index + key.Length;

            for (var i = 0; i < groupObj.Captures.Count; i++)
            {
                var capture = groupObj.Captures[i];
                if (capture.Index > max)
                {
                    break;
                }
                else if (capture.Index>= key.Index)
                {
                    return capture;
                }
            }
            return null;
        }
    }

    public sealed class CaptureGroup : CaptureGroupBase, ICaptureGroup, ICaptureGroup1
    {
        internal CaptureGroup(CaptureGroupingDefinitionBase definition, Capture[] key, int index)
            : base(definition, key)
        {
            Index = index;
        }
        
        private Match Match => Definition.Root.Match;        

        Capture ICaptureGroup1.Key => Key[^1];

        internal int[] CaptureIndexes => Definition.GetCaptureIndexes()[Index].CaptureIndexes;

        public int Index { get; }

        public NestedCaptureGroupingDefinition GroupNestedBy(GroupSpecifier group)
        {
            var groupedBy = group.GetGroup(Match);
            return new NestedCaptureGroupingDefinition(Definition, groupedBy);
        }

        internal CaptureGrouping Into(Group[] columns, int[] captureIndexes)
        {
            var rows = GetRows(columns, captureIndexes);
            var key = Key[^1];
            return new CaptureGrouping(this, key, rows);
        }

        private IEnumerable<Capture?[]> GetRows(Group[] columns, int[] captureIndexes)
        {
            var keyCapture = Key[^1];
            int max = keyCapture.Index + keyCapture.Length;
            bool notAtEnd;
            do
            {
                notAtEnd = false;

                var row = new Capture?[columns.Length];
                for (int column = 0; column < columns.Length; column++)
                {
                    int captureIndex = captureIndexes[column];
                    if (captureIndex >= 0)
                    {
                        var group = columns[column];
                        var capture = group.Captures[captureIndex];
                        if (capture.Index >= keyCapture.Index && capture.Index + capture.Length <= max)
                        {
                            row[column] = capture;
                            notAtEnd = true;
                            captureIndex++;
                            var nextCapture = group.Captures.Count > captureIndex ? group.Captures[captureIndex] : null;
                            captureIndexes[column] = nextCapture is not null && nextCapture.Index + nextCapture.Length < max
                                ? captureIndex
                                : -1;
                        }
                    }
                }

                if (notAtEnd)
                {
                    yield return row;
                }
            } while (notAtEnd);
        }

        public IEnumerable<string> Select(GroupSpecifier toSelect)
        {
            foreach (var row in Into(toSelect).Captures)
            {
                // Nullable warning supressed because if there is only one column, the row cannot have null values
                yield return row[0]!.Value;
            }
        }

        public IEnumerable<string?[]> Select(params GroupSpecifier[] toSelect)
        {
            foreach (var row in Into(toSelect).Captures)
            {
                var item = new string?[toSelect.Length];
                for (int i = 0; i < item.Length; i++)
                {
                    item[i] = row[i]?.Value;
                }
                yield return item;
            }
        }

        public CaptureGrouping Into(params GroupSpecifier[] toSelect)
        {
            if (toSelect is null)
            {
                throw new ArgumentNullException(nameof(toSelect));
            }
            if (toSelect.Length == 0)
            {
                throw new ArgumentException("Groups is empty.", nameof(toSelect));
            }

            var root = Definition.Root;
            var columns = root.GetGroups(toSelect);
            var captureIndexes = new int[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                var globalGroupIndex = root.IndexOf(columns[i]);
                captureIndexes[i] = CaptureIndexes[globalGroupIndex];
            }

            var rows = GetRows(columns, captureIndexes);
            return new CaptureGrouping(this, Key[^1], rows);
        }

        /// <summary>
        /// Gets the first capture for the specified group within the <see cref="Key"/> capture.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public Capture? First(GroupSpecifier group)
        {
            var root = Definition.Root;
            var groupObj = group.GetGroup(root.Match);
            var groupIndex = root.IndexOf(groupObj);
            var captureIndex = CaptureIndexes[groupIndex];
            return captureIndex >= 0
                ? groupObj.Captures[captureIndex]
                : null;
        }
    }
}

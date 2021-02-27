using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGroupingRoot
    {
        ImmutableArray<Group> Groups { get; }

        protected CaptureGroupingDefinition ByCore(GroupSpecifier group);
    }

    public interface ICaptureGroupingRoot1 : ICaptureGroupingRoot
    {
        ICaptureGroupingDefinition1 By(GroupSpecifier group) => ByCore(group);
    }

    public interface ICaptureGroupingRoot2 : ICaptureGroupingRoot1
    {
        new ICaptureGroupingDefinition2 By(GroupSpecifier group) => ByCore(group);
    }

    public interface ICaptureGroupingRoot3 : ICaptureGroupingRoot2
    {
        new ICaptureGroupingDefinition3 By(GroupSpecifier group) => ByCore(group);
    }

    public interface ICaptureGroupingRoot4 : ICaptureGroupingRoot3
    {
        new ICaptureGroupingDefinition4 By(GroupSpecifier group) => ByCore(group);
    }

    public interface ICaptureGroupingRoot5 : ICaptureGroupingRoot4
    {
        new ICaptureGroupingDefinition5 By(GroupSpecifier group) => ByCore(group);
    }

    public class CaptureGroupingRoot : ICaptureGroupingRoot1, ICaptureGroupingRoot2, ICaptureGroupingRoot3, ICaptureGroupingRoot4, ICaptureGroupingRoot5
    {
        internal CaptureGroupingRoot(Match match, GroupSpecifier[] groupSpecifiers)
        {
            Match = match;
            GroupToIndexMap = new Dictionary<Group, int>(groupSpecifiers.Length);
            var groups = new Group[groupSpecifiers.Length];
            for (int i = 0; i < groupSpecifiers.Length; i++)
            {
                var group = groupSpecifiers[i].GetGroup(Match);
                groups[i] = group;
                GroupToIndexMap.Add(group, i);
            }
            Groups = groups.ToImmutableArray();
        }

        public Match Match { get; }

        public ImmutableArray<Group> Groups { get; }

        private readonly Dictionary<Group, int> GroupToIndexMap;

        internal int[] GetGroupIndexes(Group[] columns)
        {
            var result = new int[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                result[i] = IndexOf(columns[i]);
            }
            return result;
        }

        public CaptureGroupingDefinition By(GroupSpecifier group)
        {
            var groupObj = group.GetGroup(Match);
            //if (!GroupToIndexMap.ContainsKey(groupObj))
            //{
            //    throw new ArgumentException("The group is not contained in groups.", nameof(group));
            //}

            return new CaptureGroupingDefinition(this, groupObj);
        }

        CaptureGroupingDefinition ICaptureGroupingRoot.ByCore(GroupSpecifier group)
            => By(group);

        internal Group[] GetGroups(GroupSpecifier[] groups)
        {
            var result = new Group[groups.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = groups[i].GetGroup(Match);
            }
            return result;
        }

        internal int IndexOf(Group group)
            => GroupToIndexMap[group];
    }
}

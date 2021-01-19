using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public class CaptureGroupingRoot
    {
        internal CaptureGroupingRoot(Match match, GroupSpecifier[] groups)
        {
            Match = match;
            GroupToIndexMap = new Dictionary<Group, int>(groups.Length);
            Groups = new Group[groups.Length];
            for (int i = 0; i < groups.Length; i++)
            {
                var group = groups[i].GetGroup(Match);
                Groups[i] = group;
                GroupToIndexMap.Add(group, i);
            }
        }

        public Match Match { get; }
        public Group[] Groups { get; }
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

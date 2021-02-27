using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public static class MatchExt
    {
        public static ICaptureGroupingRoot1 Group(this Match match, GroupSpecifier group1)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            if (group1 is null)
            {
                throw new ArgumentNullException(nameof(group1));
            }

            return new CaptureGroupingRoot(match, new[] { group1 });
        }

        public static ICaptureGroupingRoot2 Group(this Match match, GroupSpecifier group1, GroupSpecifier group2)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            return GroupMultiple(match, new[] { group1, group2 });
        }

        public static ICaptureGroupingRoot3 Group(this Match match, GroupSpecifier group1, GroupSpecifier group2, GroupSpecifier group3)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            return GroupMultiple(match, new[] { group1, group2, group3});
        }

        public static ICaptureGroupingRoot4 Group(this Match match, GroupSpecifier group1, GroupSpecifier group2, GroupSpecifier group3, GroupSpecifier group4)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            return GroupMultiple(match, new[] { group1, group2, group3, group4 });
        }

        public static ICaptureGroupingRoot5 Group(this Match match, GroupSpecifier group1, GroupSpecifier group2, GroupSpecifier group3, GroupSpecifier group4, GroupSpecifier group5)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            return GroupMultiple(match, new[] { group1, group2, group3, group4, group5 });
        }

        public static CaptureGroupingRoot Group(this Match match, params GroupSpecifier[] groups)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            if (groups is null)
            {
                throw new ArgumentNullException(nameof(groups));
            }
            if (groups.Length == 0)
            {
                throw new ArgumentException("At least one group is required", nameof(groups));
            }

            return GroupMultiple(match, groups);
        }

        private static CaptureGroupingRoot GroupMultiple(Match match, GroupSpecifier[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] is null)
                {
                    throw new ArgumentException("Not all group specifiers are not null.");
                }
            }
            return new CaptureGroupingRoot(match, groups);
        }
    }
}

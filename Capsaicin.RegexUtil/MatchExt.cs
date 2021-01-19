using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public static class MatchExt
    {
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

            return new CaptureGroupingRoot(match, groups);
        }
    }
}

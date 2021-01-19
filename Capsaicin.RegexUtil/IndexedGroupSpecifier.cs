using System;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// Specifies a <see cref="Group"/> in a regular expression <see cref="Match"/> by index.
    /// </summary>
    public sealed class IndexedGroupSpecifier : GroupSpecifier
    {
        public IndexedGroupSpecifier(int groupIndex)
        {
            if (groupIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(groupIndex));
            }
            GroupIndex = groupIndex;
        }

        public int GroupIndex { get; }

        public override Group GetGroup(GroupCollection groups) => groups[GroupIndex];
    }
}

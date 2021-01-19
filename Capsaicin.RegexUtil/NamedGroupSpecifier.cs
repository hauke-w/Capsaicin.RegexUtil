using System;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// Specifies a <see cref="Group"/> in a regular expression <see cref="Match"/> by name.
    /// </summary>
    public sealed class NamedGroupSpecifier : GroupSpecifier
    {
        public NamedGroupSpecifier(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException($"'{nameof(groupName)}' cannot be null or empty", nameof(groupName));
            }

            GroupName = groupName;
        }

        public string GroupName { get; }

        public override Group GetGroup(GroupCollection groups) => groups[GroupName];
    }
}

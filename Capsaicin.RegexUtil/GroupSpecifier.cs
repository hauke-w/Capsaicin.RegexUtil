using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// Specifies a <see cref="Group"/> in a regular expression <see cref="Match"/>.
    /// </summary>
    /// <remarks>
    /// There are implicit conversions from string and int to this class.
    /// </remarks>
    public abstract class GroupSpecifier
    {
        private protected GroupSpecifier() { }

        public Group GetGroup(Match match) => GetGroup(match.Groups);

        /// <summary>
        /// Gets the group withing <paramref name="groups"/> that is specified by this instance.
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public abstract Group GetGroup(GroupCollection groups);

        public static implicit operator GroupSpecifier(string groupName) => new NamedGroupSpecifier(groupName);

        public static implicit operator GroupSpecifier(int groupIndex) => new IndexedGroupSpecifier(groupIndex);
    }
}

namespace Capsaicin.RegexUtil
{
    internal static class GroupSpecifiers
    {
        public static GroupSpecifier[] FromObjects(params object[] values)
        {
            var result = new GroupSpecifier[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = FromObject(values[i]);
            }
            return result;
        }

        public static GroupSpecifier FromObject(object value)
        {
            return value switch
            {
                string groupName => new NamedGroupSpecifier(groupName),
                int groupIndex => new IndexedGroupSpecifier(groupIndex)
            };
        }
    }
}

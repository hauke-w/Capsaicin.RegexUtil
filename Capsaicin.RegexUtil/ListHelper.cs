using System;
using System.Collections.Generic;
using System.Text;

namespace Capsaicin.RegexUtil
{
    internal static class ListHelper
    {
        public static T[] Slice<T>(this IList<T> list, Range range)
        {
            int start = range.Start.GetOffset(0);
            int end = range.End.GetOffset(list.Count);
            int count = end - start;
            if (count<0)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }
            var result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = list[start++];
            }
            return result;
        }
    }
}
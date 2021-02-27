using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        /// <summary>
        /// Copies the first <paramref name="n"/> items to a new array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="n">The number of items to take from the beginning of <paramref name="from"/>.</param>
        /// <returns></returns>
        public static T[] Take<T>(this IList<T> from, int n)
        {
            var result = new T[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = from[i];
            }
            return result;
        }
    }
}
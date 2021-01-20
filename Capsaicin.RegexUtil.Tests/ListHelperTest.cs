using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class ListHelperTest
    {
        [TestMethod]
        [SliceTestCases]
        public void SliceTest(string[] items, Range range)
        {
            IList<string> list = items;
            string[] expected = items[range];
            var actual = ListHelper.Slice(list, range);
            CollectionAssert.AreEqual(expected, actual);
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class SliceTestCasesAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                yield return TestCase(new[] { "a", "b", "c" }, ..^1);
                yield return TestCase(new[] { "a", "b", "c" }, 1..);
                yield return TestCase(new[] { "a", "b", "c", "d", "e", "f" }, 1..^2);
                yield return TestCase(new[] { "a", "b", "c" }, ..);
                yield return TestCase(new[] { "a", "b", "c", "d", "e", "f" }, 3..4);
            }

            private object[] TestCase(string[] items, Range range) => new object[] { items, range };

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                var sb = new StringBuilder();
                sb.Append('{');
                sb.AppendJoin(", ", (string[])data[0]);
                sb.Append("}[");
                sb.Append(data[1]);
                sb.Append(']');
                return sb.ToString();
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class CaptureGroupingDefinitionBaseTest
    {
        [TestMethod]
        public void GetCaptureIndexesRowTest()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a,b,c)(d)(e,f)");
            Assert.IsTrue(match.Success, "invalid test data");
            var testObject = MatchExt
                .Group(match, "val1, val2", "val4")
                .By("tuple");

            var columns = new[] { match.Groups["tuple"], match.Groups["val1"], match.Groups["val2"], match.Groups["val4"] };
            var searchStartIndexes = new int[columns.Length];

            var expectedRows = new int[][]
            {
                new int []{0, 0, 0, -1},
                new int []{1, 1, -1, -1},
                new int []{2, 2, 1, -1}
            };

            for (int i = 0; i < expectedRows.Length; i++)
            {
                var expected = expectedRows[i].ToArray(); // copy it!
                var actual = testObject.GetCaptureIndexesRow(columns, searchStartIndexes, i);
                var expectedKey = new[] { match.Groups["tuple"].Captures[i] };
                CollectionAssert.AreEqual(expectedKey, actual.Key);
                CollectionAssert.AreEqual(expected, actual.CaptureIndexes);
            }
        }
    }
}

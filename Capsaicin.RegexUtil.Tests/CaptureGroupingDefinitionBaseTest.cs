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

        [TestMethod]
        public void FlattenTest()
        {
            var regex = new Regex(@"^(?<object>(?<id>[a-zA-Z]\w*)\((?<property>(?<name>[a-zA-Z]\w*)=(?<value>\w*)(,(?=[a-zA-Z]))?)*\)(,(?=[a-zA-Z]))?)*$");
            var match = regex.Match("obj1(a=1,b=2),obj2(x=y)");

            var testObject = match
                .Group("id", "property", "name", "value") // for nested groups it is essential to specify all captures that are later used!
                .By("object")
                .ThenBy("property");
            var actual = testObject.Flatten("name", "value");

            Assert.IsNotNull(actual);
            var actualAsList = actual.ToList();
            Assert.AreEqual(3, actualAsList.Count);

            var propertyCaptures = match.Groups["property"].Captures;
            var nameCaptures = match.Groups["name"].Captures;
            var valueCaptures = match.Groups["value"].Captures;

            for (int i = 0; i < 3; i++)
            {
                var actualItem = actualAsList[i];
                Assert.IsNotNull(actualItem);
                Assert.AreEqual(2, actualItem.Count);
                Assert.AreEqual(2, actualItem.Captures.Length);

                Assert.AreEqual(propertyCaptures[i], actualItem.Key);
                Assert.AreEqual(nameCaptures[i], actualItem.Captures[0]);
                Assert.AreEqual(valueCaptures[i], actualItem.Captures[1]);
            }
        }
    }
}

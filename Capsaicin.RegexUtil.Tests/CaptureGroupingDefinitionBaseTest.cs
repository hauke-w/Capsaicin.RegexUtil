using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
                .Group("id", "property", "name", "value")
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

        [TestMethod]
        [DataRow("a1")]
        [DataRow("a2,b2")]
        [DataRow("a3,b3,c3")]
        [DataRow("a4,b4,c4,d4")]
        [DataRow("a5,b5,c5,d5,e5")]
        [DataRow("", true)] // no list is matched
        [DataRow(" ", true)] // one empty list is matched
        [DataRow(" |2", true)]
        [DataRow("1| |3", true)]
        [DataRow("12| ", true)]
        [DataRow("A1|A2,B2|A3,A3,C3,D3,D3|A4,B4,C4|A5,B5,C5,D5")]
        [DataRow("A1|A2,B2|A3,A3,C3,D3,D3|A4,B4,C4| |A5,B5,C5,D5", true)]
        public void IntoTest(string expression, bool allowEmpty = false)
        {
            var regex = allowEmpty
                ? new Regex(@"^(?<list>((?<empty> )|((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?))(\|(?=\w| ))?)*$")
                : new Regex(@"^(?<list>(?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?(\|(?=\w))?)*$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "Invalid test data");

            var expected = expression
                    .Split('|', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(it => it == " " ? new string?[0] : it.Split(','))
                    .ToList();

            var testObject = match
                .Group("val1", "val2", "val3", "val4", "val5")
                .By("list");

            var actual1 = testObject.Into("val1");
            Verify(actual1, 1, it => it.Values);

            var actual2 = testObject.Into("val1", "val2");
            Verify(actual2, 2, it => it.Values);

            var actual3 = testObject.Into("val1", "val2", "val3");
            Verify(actual3, 3, it => it.Values);

            var actual4 = testObject.Into("val1", "val2", "val3", "val4");
            Verify(actual4, 4, it => it.Values);

            var actual5 = testObject.Into("val1", "val2", "val3", "val4", "val5");
            Verify(actual5, 5, it => it.Values);

            void Verify<T, TValue>(IEnumerable<T> actual, int n, Func<T, IEnumerable<TValue>> valuesFunc)
                where T : ICaptureGrouping
            {
                Assert.IsNotNull(actual);
                var actualAsList = actual.ToList();
                Assert.AreEqual(expected.Count, actualAsList.Count);

                for (var i = 0; i < actualAsList.Count; i++)
                {
                    var actualGrouping = actualAsList[i];
                    var expectedGrouping = expected[i];
                    var actualCaptures = actualGrouping.Captures?.ToList();
                    Assert.IsNotNull(actualCaptures);

                    var values = valuesFunc(actualGrouping)?.ToList();
                    Assert.IsNotNull(values);

                    if (expectedGrouping.Length > 0)
                    {
                        Assert.AreEqual(1, actualCaptures!.Count);
                        var expectedCaptureValues = expectedGrouping.ToList();
                        EnsureCollectionSize(expectedCaptureValues, n);
                        var actualCaptureValues = actualCaptures[0].Select(it => it?.Value).ToList();
                        CollectionAssert.AreEqual(expectedCaptureValues, actualCaptureValues);
                        CollectionAssert.AreEqual(expectedCaptureValues, values);
                    }
                    else
                    {
                        Assert.AreEqual(0, actualCaptures!.Count);
                        Assert.AreEqual(0, values!.Count);
                    }
                }
            }

            void EnsureCollectionSize<T>(IList<T?> collection, int n)
            {
                if (collection.Count > n)
                {
                    do
                    {
                        collection.RemoveAt(collection.Count - 1);
                    } while (collection.Count > n);
                }
                else
                {
                    while (collection.Count < n)
                    {
                        collection.Add(default);
                    }
                }
            }
        }
    }
}

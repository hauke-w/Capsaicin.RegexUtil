using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class CaptureGroupingDefinitionTest
    {
        [TestMethod]
        [DataRow("(Ab)", "Ab")]
        [DataRow("(1,2)", "1", "2")]
        [DataRow("(a,b,c),(d),(e,f)", "a", "b", "c", "d", "e", "f")]
        [DataRow("")]
        public void FlattenTest(string expression, params string[] expected)
        {
            var regex = new Regex(@"^((?<tuple>\(((?<value>\w+)(,(?=\w))?)+\))(,(?=\())?)*$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "invalid test data");
            var testObject = (CaptureGroupingDefinition)MatchExt
                .Group(match, "value")
                .By("tuple");

            var actual = testObject.Flatten("value");
            Assert.IsNotNull(actual);
            var actualAsList = actual.ToList();
            CollectionAssert.AreEqual(expected, actualAsList);
        }

        [TestMethod]
        public void Flatten_EmptyTuples_Test()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a,b)(c)");
            Assert.IsTrue(match.Success, "invalid test data");

            var actual = match.Group("val1", "val2", "val3", "val4", "val5")
                .By("tuple")
                .Flatten("val3", "val4"); // there are no captures for these groups!

            Assert.IsNotNull(actual);
            var actualAsList = actual.ToList();
            Assert.AreEqual(0, actualAsList.Count);
        }

        [TestMethod]
        public void Flatten5Test()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a1,b1,c1,d1,e1)(a2,b2)(a3)(a3,b3,c3)(a5,b5,c5,d5)");
            Assert.IsTrue(match.Success, "invalid test data");

            var expected = new int[,]
            {
                { 0, 0, 0, 0, 0 },
                { 1, 1, -1, -1, -1},
                { 2, -1, -1, -1, -1},
                { 3, 2, 1, -1, -1},
                { 4, 3, 2, 1, -1}
            };

            var testObject = match.Group("val1", "val2", "val3", "val4", "val5")
                .By("tuple");
            var toSelect = new GroupSpecifier[] { "val1", "val2", "val3", "val4", "val5" };
            var actual = testObject.Flatten(toSelect[0], toSelect[1], toSelect[2], toSelect[3], toSelect[4]);

            VerifyFlattenX(match, expected, actual, toSelect);
        }

        private static void VerifyFlattenX(Match match, int[,] expected, IEnumerable<IFlattenedCaptureGrouping> actual, GroupSpecifier[] toSelect)
        {
            Assert.IsNotNull(actual);
            var actualAsList = actual.ToList();
            var expectedRowCount = expected.GetLength(0);
            var expectedColumnCount = expected.GetLength(1);
            var captures = toSelect.Select(it => it.GetGroup(match).Captures).ToList();
            var expectedRows = GetRows();

            Assert.AreEqual(expectedRowCount, actualAsList.Count);

            for (int row = 0; row < expectedRowCount; row++)
            {
                var expectedRow = expectedRows[row];
                var actualRowCaptures = actualAsList[row].Captures;
                Assert.IsNotNull(actualRowCaptures);
                CollectionAssert.AreEqual(expectedRow, actualRowCaptures);
            }

            List<List<Capture?>> GetRows()
            {
                return Enumerable.Range(0, expectedRowCount)
                    .Select(row => Enumerable.Range(0, expectedColumnCount).Select(column => GetExpectedCapture(row, column)).ToList())
                    .ToList();
            }

            Capture? GetExpectedCapture(int row, int column)
            {
                var index = expected[row, column];
                return index >= 0 ? captures[column][index] : null;
            }
        }

        [TestMethod]
        public void Flatten4Test()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a1,b1,c1)(a2)(a3,b3,b3,b4)");
            Assert.IsTrue(match.Success, "invalid test data");

            var expected = new int[,]
            {
                { 0, 0, 0, -1 },
                { 1, -1, -1, -1},
                { 2, 1, 1, 0}
            };

            var testObject = match.Group("val1", "val2", "val3", "val4")
                .By("tuple");

            var toSelect = new GroupSpecifier[] { "val1", "val2", "val3", "val4" };
            var actual = testObject.Flatten(toSelect[0], toSelect[1], toSelect[2], toSelect[3]);

            VerifyFlattenX(match, expected, actual, toSelect);
        }

        [TestMethod]
        public void Flatten3Test()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a1,b1,c1)(a2,b2,c2,d2,e2)(a3,b3,c3,d3,e3)(a4)");
            Assert.IsTrue(match.Success, "invalid test data");

            var expected = new int[,]
            {
                { 0, 0, -1 },
                { 1, 1, 0 },
                { 2, 2, 1 },
                { -1, 3, -1 }
            };

            var testObject = match.Group("val1", "val2", "val3", "val4", "val5")
                .By("tuple");

            var toSelect = new GroupSpecifier[] { "val2", "val1", "val4" };
            var actual = testObject.Flatten(toSelect[0], toSelect[1], toSelect[2]);

            VerifyFlattenX(match, expected, actual, toSelect);
        }

        [TestMethod]
        public void Flatten2Test()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a1)(a2,b2)(a3,b3,c3)(a4,b4)(a5,b5,c5)(a6)(a7,b7)(a8)");
            Assert.IsTrue(match.Success, "invalid test data");

            var expected = new int[,]
            {
                // (a1) is left out because there are no matches for val2 and val3
                { 0, -1, }, // (a2,b2)!!! 
                { 1, 0},
                { 2, -1 },
                { 3, 1  },
                // (a6) is left out because there are no matches for val2 and val3
                { 4, -1 }
                // (a8) is left out because there are no matches for val2 and val3
            };

            var testObject = match.Group("val1", "val2", "val3")
                .By("tuple");
            var toSelect = new GroupSpecifier[] { "val2", "val3" };
            var actual = testObject.Flatten(toSelect[0], toSelect[1]);

            VerifyFlattenX(match, expected, actual, toSelect);
        }

        [TestMethod]
        [DataRow("val3", "tuple", "c1", "c3", "c4")]
        [DataRow("val1", "tuple", "a1", "a2", "a3", "a4")]
        [DataRow("val4", "tuple", "d3")]
        [DataRow("val5", "tuple")]
        [DataRow("tuple", "tuple", "(a1,b1,c1)", "(a2)", "(a3,b3,c3,d3)", "(a4,b4,c4)")]
        public void Flatten1Test(string toGroup, string by, params string[] expected)
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a1,b1,c1)(a2)(a3,b3,c3,d3)(a4,b4,c4)");
            Assert.IsTrue(match.Success, "invalid test data");

            var testObject = match.Group(toGroup)
                .By(by);
            var toSelect = new GroupSpecifier[] { toGroup };
            var actual = testObject.Flatten(toSelect[0]);
            Assert.IsNotNull(actual);
            var actualAsList = actual.ToList();
            CollectionAssert.AreEqual(expected, actualAsList);
        }


        [TestMethod]
        //[Timeout(200)]
        public void ThenByTest()
        {
            var regex = new Regex(@"^(?<object>(?<id>[a-zA-Z]\w*)\((?<property>(?<name>[a-zA-Z]\w*)=(?<value>\w*)(,(?=[a-zA-Z]))?)*\)(,(?=[a-zA-Z]))?)*$");
            var match = regex.Match("obj1(a=1,b=2),obj2(x=y)");

            var root = match
                .Group("id", "property", "name", "value");
            var parent = (CaptureGroupingDefinition)root.By("object");
            var actual = parent.ThenBy("property");
            Assert.IsNotNull(actual);
            Assert.AreSame(root, actual.Root);
            Assert.AreSame(parent, actual.Parent);
            Assert.AreSame(match.Groups["property"], actual.GroupedBy);

            var objectCaptures = match.Groups["object"].Captures.ToList();
            var propertyCaptures = match.Groups["property"].Captures.ToList();
            var expectedGroupKeys = new[]
            {
                new []{ objectCaptures[0], propertyCaptures[0] },
                new []{ objectCaptures[0], propertyCaptures[1] },
                new []{ objectCaptures[1], propertyCaptures[2] }
            };
            var actualCaptureGroups = actual.CaptureGroups.ToList();
            Assert.AreEqual(expectedGroupKeys.Length, actualCaptureGroups.Count);
            for (int i = 0; i < expectedGroupKeys.Length; i++)
            {
                var captureGroup = actualCaptureGroups[i];
                Assert.IsNotNull(captureGroup);
                CollectionAssert.AreEqual(expectedGroupKeys[i], captureGroup.Key);
            }            
        }

        [TestMethod]
        public void GetCaptureIndexesWithinGroupedByTest()
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match("(a)(b,c)(d)(e,f,g)");
            Assert.IsTrue(match.Success, "invalid test data");
            var testObject = (CaptureGroupingDefinition)MatchExt
                .Group(match, "val1", "val5", "val2", "tuple")
                .By("tuple");

            var columns = new[] { match.Groups["val5"], match.Groups["val1"], match.Groups["val2"], match.Groups["tuple"] };
            var actual = testObject.GetCaptureIndexesWithinGroupedBy(columns);

            int[][] expected =
            {
                new int []{-1, 0, -1, 0},
                new int []{-1, 1, 0, 1},
                new int []{-1, 2, -1, 2},
                new int []{-1, 3, 1, 3}
            };

            Assert.AreEqual(expected.Length, actual.Count);
            for (var i = 0; i < expected.Length; i++)
            {
                CollectionAssert.AreEqual(expected[i], actual[i].CaptureIndexes);
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class FlattenedCaptureGroupingTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var regex = new Regex(@"^(?<sentence>((?<word>\w+)+( (?=\w))?)+(?<endSymbol>[\.!])( (?=\w))?)+$");
            var match = regex.Match("Hello World!");
            Assert.IsTrue(match.Success, "invalid test data");

            var key = match.Groups["sentence"];
            var definitionMock = new CaptureGroupingDefinitionBaseMock(key);
            var captureGroup = new CaptureGroup(definitionMock, new[] { key }, 0);
            var captures = match.Groups["word"].Captures.ToArray();
            Assert.AreEqual(2, captures.Length);
            var actual = new FlattenedCaptureGrouping(captureGroup, key, captures);

            Assert.AreSame(captureGroup, actual.CaptureGroup);
            Assert.AreSame(key, actual.Key);
            Assert.IsNotNull(actual.Captures);
            CollectionAssert.AreEqual(captures, actual.Captures);
            Assert.AreEqual(captures.Length, actual.Count);
        }


        private static CaptureGroupingDefinition CreateCaptureGroupingDefinition(string expression)
        {
            var regex = new Regex(@"^(?<tuple>\((?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?\))+$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "invalid test data");

            return (CaptureGroupingDefinition)match.Group("val1", "val2", "val3", "val4", "val5")
                .By("tuple");
        }

        [TestMethod]
        [DataRow("(a,b)", "val1", "val2", "a", "b")]
        [DataRow("(a,b)", "val1", "val3", "a", null)]
        [DataRow("(a,b)", "val3", "val2", null, "b")]
        [DataRow("(a,b,c,d)", "val2", "val4", "b", "d")]
        public void Deconstruct2Test(string expression, object groupSpecifier1, object groupSpecifier2, string? expected1, string? expected2)
        {
            if (expected1 is null && expected2 is null)
            {
                throw new ArgumentException($"Invalid test data: {nameof(expected1)} and {nameof(expected2)} cannot be both null.");
            }

            var group = CreateCaptureGroupingDefinition(expression);
            var list = group
                .Flatten(
                    GroupSpecifiers.FromObject(groupSpecifier1),
                    GroupSpecifiers.FromObject(groupSpecifier2))
                .ToList();

            var testObject = list[0];
            Assert.AreEqual(2, testObject.Count);

            testObject.Deconstruct(out var actual1, out var actual2);
            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        private class CaptureGroupingDefinitionBaseMock : CaptureGroupingDefinitionBase
        {
            public CaptureGroupingDefinitionBaseMock(Group groupedBy) : base(null!, groupedBy)
            {
            }

            public override int Index => throw new NotImplementedException();

            public override CaptureGroupingDefinitionBase Parent => throw new NotImplementedException();

            internal override IList<CaptureRowIndexes> GetCaptureIndexesWithinGroupedBy(Group[] columns) => throw new NotImplementedException();
        }

        [TestMethod]
        [DataRow("a", 5)]
        [DataRow("A,B", 5)]
        [DataRow("aa,bb,cc", 5)]
        [DataRow("a,b,c,d", 5)]
        [DataRow("1,2,3,4,5", 5)]
        [DataRow("ä", 1)]
        [DataRow("Ä,bbb", 4)]
        [DataRow("aa,bb,cc", 5)]
        [DataRow("a,b,c,d", 4)]
        [DataRow("1,2,3,4,5", 5)]
        public void DeconstructTest(string expression, int nExpected)
        {
            var regex = new Regex(@"^(?<list>(?<val1>\w+)(,(?<val2>\w+)(,(?<val3>\w+)(,(?<val4>\w+)(,(?<val5>\w+))?)?)?)?)$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "invalid test data");

            var expectedValues = GetExpectedValues(out int numExpectedNotNullValues);

            var key = match.Groups["tuple"];
            var definitionMock = new CaptureGroupingDefinitionBaseMock(key);
            var captureGroup = new CaptureGroup(definitionMock, new[] { key }, 0);
            IEnumerable<Group> groups = match.Groups;
            var captures = Enumerable
                .Range(1, numExpectedNotNullValues)
                .Select(i => match.Groups["val" + i])
                .ToList<Capture?>();
            while (captures.Count < nExpected)
            {
                captures.Add(null);
            }

            var testObject = new FlattenedCaptureGrouping(captureGroup, key, captures.ToArray());
            CollectionAssert.AreEqual(captures, testObject.Captures);

            string? value1, value2, value3, value4, value5;

            value1 = testObject;
            Assert.AreEqual(expectedValues[0], value1);
            Assert.AreEqual(expectedValues.Length, testObject.Count);

            IFlattenedCaptureGrouping2 flattended2 = testObject;
            if (expectedValues.Length > 1)
            {
                flattended2.Deconstruct(out value1, out value2);
                Assert.AreEqual(expectedValues.Length, flattended2.Count);
                Verify(value1, value2);
            }
            else
            {
                Assert.ThrowsException<InvalidOperationException>(() => flattended2.Deconstruct(out value1, out value2));
            }

            IFlattenedCaptureGrouping3 flattended3 = testObject;
            if (expectedValues.Length > 2)
            {
                flattended3.Deconstruct(out value1, out value2, out value3);
                Assert.AreEqual(expectedValues.Length, flattended3.Count);
                Verify(value1, value2, value3);
            }
            else
            {
                Assert.ThrowsException<InvalidOperationException>(() => flattended3.Deconstruct(out value1, out value2, out value3));
            }

            IFlattenedCaptureGrouping4 flattended4 = testObject;
            if (expectedValues.Length > 3)
            {
                flattended4.Deconstruct(out value1, out value2, out value3, out value4);
                Assert.AreEqual(expectedValues.Length, flattended4.Count);
                Verify(value1, value2, value3, value4);
            }
            else
            {
                Assert.ThrowsException<InvalidOperationException>(() => flattended4.Deconstruct(out value1, out value2, out value3, out value4));
            }

            IFlattenedCaptureGrouping5 flattended5 = testObject;
            if (expectedValues.Length > 4)
            {
                flattended5.Deconstruct(out value1, out value2, out value3, out value4, out value5);
                Assert.AreEqual(expectedValues.Length, flattended5.Count);
                Verify(value1, value2, value3, value4, value5);
            }
            else
            {
                Assert.ThrowsException<InvalidOperationException>(() => flattended5.Deconstruct(out value1, out value2, out value3, out value4, out value5));
            }

            void Verify(params string?[] actual)
            {
                CollectionAssert.AreEqual(expectedValues[0..actual.Length], actual);
            }

            string?[] GetExpectedValues(out int numPresent)
            {
                var list = expression
                    .Split(',')
                    .ToList<string?>();
                numPresent = list.Count;
                while (nExpected > list.Count)
                {
                    list.Add(null);
                }
                return list.ToArray();
            }
        }
    }
}

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

            return match.Group("val1", "val2", "val3", "val4", "val5")
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
    }
}

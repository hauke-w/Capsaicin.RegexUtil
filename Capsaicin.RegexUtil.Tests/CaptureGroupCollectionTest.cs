using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Capsaicin.RegexUtil
{

    [TestClass]
    public class CaptureGroupCollectionTest
    {
        [TestMethod]
        [DataRow("(a,b,c)", "0,1,2")]
        [DataRow("(a,b,c),(d,e),(f)", "0,1,2", "3,4", "5")]
        [DataRow("")]
        public void GroupCapturesByTest(string expression, params string[] expectedValueCaptureIndexes)
        {
            var regex = new Regex(@"^((?<tuple>\(((?<value>\w+)(,(?=\w))?)+\))(,(?=\())?)*$");
            var match = regex.Match(expression);
            var grouped = match.GroupCapturesBy("tuple");
            Assert.AreEqual(match.Groups["tuple"], grouped.Key);
            Assert.AreEqual(expectedValueCaptureIndexes.Length, grouped.Count);

            for (int i = 0; i < expectedValueCaptureIndexes.Length; i++)
            {
                var expectedValues = expectedValueCaptureIndexes[i]
                    .Split(',')
                    .Select(captureIndex => match.Groups["value"].Captures[captureIndex])
                    .ToList();
            }
            foreach (var expectedCapturesExpression in expectedCapturesExpressions)
            {
                var expected = expectedCapturesExpression.Split(',');
                CollectionAssert.AreEqual();
            }
        }

        [TestMethod]
        public void GroupBy_2Levels_Test()
        {
            var regex = new Regex();
            var match = regex.Match();
            var grouped = match
                .GroupCapturesBy("level1")
                .GroupCapturesBy("level2")
                .Select("key", "value");
        }
    }
}

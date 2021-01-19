using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class CaptureGroupingRootTest
    {
        [TestMethod]
        public void ByTest()
        {
            var regex = new Regex(@"^((?<tuple>\(((?<value>\w+)(,(?=\w))?)+\))(,(?=\())?)*$");
            var match = regex.Match("(a,b,c),(d),(e,f)");
            Assert.IsTrue(match.Success, "invalid test data");
            var testObject = MatchExt.Group(match, "value");
            var actual = testObject.By("tuple");
            Assert.IsNotNull(actual);
            Assert.IsNull(actual.Parent);
            Assert.AreSame(match.Groups["tuple"], actual.GroupedBy);
            Assert.AreSame(testObject, actual.Root);
        }
    }
}

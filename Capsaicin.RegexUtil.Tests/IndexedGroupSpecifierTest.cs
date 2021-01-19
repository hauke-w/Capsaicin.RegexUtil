using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class IndexedGroupSpecifierTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            const int groupIndex = 4;
            var actual = new IndexedGroupSpecifier(4);
            Assert.AreEqual(groupIndex, actual.GroupIndex);
        }

        [TestMethod]
        public void GetGroup_ForMatch_Test()
        {
            var regex = new Regex(@"^(?<g>\w+)$");
            var match = regex.Match("abc");
            Assert.IsTrue(match.Success, "invalid test data");

            const int groupIndex = 1;
            var testObject = new IndexedGroupSpecifier(groupIndex);
            var actual = testObject.GetGroup(match);
            Assert.AreSame(match.Groups[1], actual);
        }

        [TestMethod]
        public void GetGroup_ForGroupCollection_Test()
        {
            var regex = new Regex(@"^(?<Group1>\w+) (?<Group2>\w+)$");
            var match = regex.Match("abc de");
            Assert.IsTrue(match.Success, "invalid test data");

            const int groupIndex = 2;
            var testObject = new IndexedGroupSpecifier(groupIndex);
            var actual = testObject.GetGroup(match.Groups);
            Assert.AreSame(match.Groups["Group2"], actual);
        }
    }
}

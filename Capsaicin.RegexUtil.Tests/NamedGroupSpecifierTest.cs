using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class NamedGroupSpecifierTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            const string groupName = "Group123";
            var actual = new NamedGroupSpecifier(groupName);
            Assert.AreEqual(groupName, actual.GroupName);
        }

        [TestMethod]
        public void GetGroup_ForMatch_Test()
        {
            var regex = new Regex(@"^(?<g>\w+)$");
            var match = regex.Match("abc");
            Assert.IsTrue(match.Success, "invalid test data");

            const string groupName = "g";
            var testObject = new NamedGroupSpecifier(groupName);
            var actual = testObject.GetGroup(match);
            Assert.AreSame(match.Groups[groupName], actual);
        }

        [TestMethod]
        public void GetGroup_ForGroupCollection_Test()
        {
            var regex = new Regex(@"^(?<Group1>\w+) (?<Group2>\w+)$");
            var match = regex.Match("abc de");
            Assert.IsTrue(match.Success, "invalid test data");

            const string groupName = "Group2";
            var testObject = new NamedGroupSpecifier(groupName);
            var actual= testObject.GetGroup(match.Groups);
            Assert.AreSame(match.Groups[groupName], actual);
        }
    }
}

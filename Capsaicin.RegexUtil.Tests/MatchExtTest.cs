using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class MatchExtTest
    {
        [TestMethod]
        [DataRow("value")]
        [DataRow(1)]
        [DataRow("value", "tuple")]
        public void GroupTest(params object[] groups)
        {
            var groupSpecifiers = GroupSpecifiers.FromObjects(groups);
            var regex = new Regex(@"^((?<tuple>\(((?<value>\w+)(,(?=\w))?)+\))(,(?=\())?)*$");
            var match = regex.Match("(a,b,c),(d),(e,f)");
            Assert.IsTrue(match.Success, "invalid test data");
            var actual = MatchExt.Group(match, groupSpecifiers);
            Assert.IsNotNull(actual);
            Assert.AreSame(match, actual.Match);
            Assert.AreEqual(groups.Length, actual.Groups.Length);
            for (int i = 0; i < groups.Length; i++)
            {
                var expectedGroup = groupSpecifiers[i].GetGroup(match);
                Assert.AreSame(expectedGroup, actual.Groups[i]);
            }
        }
    }
}

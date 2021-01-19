using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    [TestClass]
    public class CaptureGroupTest
    {
        [TestMethod]
        public void FirstTest()
        {
            var regex = new Regex(@"^(?<object>(?<id>[a-zA-Z]\w*)\((?<property>(?<name>[a-zA-Z]\w*)=(?<value>\w*)(,(?=[a-zA-Z]))?)*\)(,(?=[a-zA-Z]))?)*$");
            var match = regex.Match("obj1(a=1,b=2),obj2(x=y)");

            var properties = match
                .Group("id", "property", "name", "value") // for nested groups it is essential to specify all captures that are later used!
                .By("object")
                .ThenBy("property")
                .Flatten("name", "value");

            var expected = new (int IdIndex, int NameIndex, int ValueIndex)[] 
            {
                ( 0, 0, 0 ),
                ( 0, 1, 1 ),
                ( 1, 2, 2 )
            };

            int i = 0;
            var idCaptures = match.Groups["id"].Captures;
            var nameCaptures = match.Groups["name"].Captures;
            var valueCaptures = match.Groups["value"].Captures;
            foreach (var property in properties)
            {
                var testObject1 = property.CaptureGroup;
                var actual1 = testObject1.First("name");
                var actual2 = testObject1.First("value");

                var testObject2 = property.Parent!;
                var actual3 = testObject2.First("id");

                var (idIndex, nameIndex, valueIndex) = expected[i];
                Assert.AreEqual(idCaptures[idIndex], actual3);
                Assert.AreEqual(nameCaptures[nameIndex], actual1);
                Assert.AreEqual(valueCaptures[valueIndex], actual2);
                i++;
            }
        }
    }
}

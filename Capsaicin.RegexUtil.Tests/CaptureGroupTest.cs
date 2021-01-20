using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Linq;

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
                Assert.AreSame(idCaptures[idIndex], actual3);
                Assert.AreSame(nameCaptures[nameIndex], actual1);
                Assert.AreSame(valueCaptures[valueIndex], actual2);
                i++;
            }
        }

        [TestMethod]
        [DataRow("@PROP=@NAME=URI@ENDNAME;@VALU=myValue@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;", "NAME,URI|VALU,myValue|PRMT,")]
        [DataRow("@PROP=@NAME=URI@ENDNAME;@TYPE=@ENDTYPE;@VALU=myValue@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;", "NAME,URI|TYPE,|VALU,myValue|PRMT,")]
        public void SelectTest(string expression, string expectedCaptureIndexesExpression)
        {
            var regex = new Regex(@"^(?<prop>@PROP=(?<kvp>@(?<name>\w+)=(?<value>([^@]|@(?!\k<name>))+)?@END\k<name>;)+@ENDPROP;)*$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "Invalid test data");

            var testObjects = match.Group("name", "value")
                .By("prop")
                .ToList();

            var expectedData = expectedCaptureIndexesExpression
                .Split(';')
                .Select(prop => prop
                    .Split('|')
                    .Select(kvp =>
                    {
                        var fragments = kvp.Split(',');
                        return (Name: fragments[0],
                                Value: string.IsNullOrEmpty(fragments[1]) ? null : fragments[1]);
                    }).ToList())
                .ToList();

            for (var i = 0; i < testObjects.Count; i++)
            {
                var testObject = testObjects[i];
                var actual = testObject.Select("name", "value");
                Assert.IsNotNull(actual);
                var actualAsList = actual.ToList();

                var expectedPropertyData = expectedData[i];
                Assert.AreEqual(expectedPropertyData.Count, actualAsList.Count);

                for (int j = 0; j < expectedPropertyData.Count; j++)
                {
                    var actualKvp = actualAsList[j];
                    Assert.AreEqual(2, actualKvp.Length);          
                    var actualName = actualKvp[0];
                    var actualValue = actualKvp[1];
                    var (expectedName, expectedValue) = expectedPropertyData[j];
                    Assert.AreEqual(expectedName, actualName);
                    Assert.AreEqual(expectedValue, actualValue);
                }                
            }
        }
    }
}

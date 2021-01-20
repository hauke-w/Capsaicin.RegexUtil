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
        [DataRow("@PROP=@NAME=URI@ENDNAME;@TYPE=@ENDTYPE;@VALU=2@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;", "NAME,URI|TYPE,|VALU,2|PRMT,")]
        [DataRow("@PROP=@NAME=@ENDNAME;@TYPE=String@ENDTYPE;@VALU=value3@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;@PROP=@NAME=prop2@ENDNAME;@TYPE=@ENDTYPE;@VALU=@ENDVALU;@PRMT=prmt@ENDPRMT;@ENDPROP;",
            "NAME,|TYPE,String|VALU,value3|PRMT,|NAME,prop2|TYPE,|VALU,|PRMT,prmt")]
        public void SelectTest(string expression, string expectedCaptureIndexesExpression)
        {
            var regex = new Regex(@"^(?<prop>@PROP=(?<kvp>@(?<name>\w+)=(?<value>([^@]|@(?!\k<name>))+)?@END\k<name>;)+@ENDPROP;)*$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "Invalid test data");

            var testObjects = match.Group("name", "value")
                .By("prop")
                .ThenBy("kvp")
                .ToList();

            var expectedData = expectedCaptureIndexesExpression
                .Split('|')
                .Select(kvp =>
                {
                    var fragments = kvp.Split(',');
                    return (Name: fragments[0],
                            Value: string.IsNullOrEmpty(fragments[1]) ? null : fragments[1]);
                }).ToList();

            Assert.AreEqual(expectedData.Count, testObjects.Count);

            for (var i = 0; i < testObjects.Count; i++)
            {
                var testObject = testObjects[i];
                var actual = testObject.Select("name", "value");
                Assert.IsNotNull(actual);
                var actualAsList = actual.ToList();

                var (expectedName, expectedValue) = expectedData[i];
                Assert.AreEqual(1, actualAsList.Count);
                var actualKvp = actualAsList[0];
                Assert.AreEqual(2, actualKvp.Length);
                var actualName = actualKvp[0];
                var actualValue = actualKvp[1];
                Assert.AreEqual(expectedName, actualName);
                Assert.AreEqual(expectedValue, actualValue);               
            }
        }

        [TestMethod]
        [DataRow("@PROP=@NAME=@ENDNAME;@TYPE=String@ENDTYPE;@VALU=value3@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;@PROP=@NAME=prop2@ENDNAME;@TYPE=@ENDTYPE;@VALU=@ENDVALU;@PRMT=prmt@ENDPRMT;@ENDPROP;",
            "NAME,|TYPE,String|VALU,value3|PRMT,;NAME,prop2|TYPE,|VALU,|PRMT,prmt")]
        [DataRow("@PROP=@NAME=URI@ENDNAME;@VALU=myValue@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;", "NAME,URI|VALU,myValue|PRMT,")]
        [DataRow("@PROP=@NAME=URI@ENDNAME;@TYPE=@ENDTYPE;@VALU=2@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;", "NAME,URI|TYPE,|VALU,2|PRMT,")]        
        public void GroupNestedByTest(string expression, string expectedCaptureIndexesExpression)
        {
            var regex = new Regex(@"^(?<prop>@PROP=(?<kvp>@(?<name>\w+)=(?<value>([^@]|@(?!\k<name>))+)?@END\k<name>;)+@ENDPROP;)*$");
            var match = regex.Match(expression);
            Assert.IsTrue(match.Success, "Invalid test data");

            var root = match.Group("name", "value");
            var propGroupDefinition = root.By("prop");

            var testObjects = propGroupDefinition.ToList();

            var expectedData = expectedCaptureIndexesExpression
                .Split(';')
                .Select(prop => prop
                    .Split('|')
                    .Select(kvp =>
                    {
                        var fragments = kvp.Split(',');
                        return (Name: fragments[0],
                                Value: string.IsNullOrEmpty(fragments[1]) ? null : fragments[1]);
                    }).ToList()
                ).ToList();

            Assert.AreEqual(expectedData.Count, testObjects.Count);

            int kvpCount = 0;
            for (var i = 0; i < testObjects.Count; i++)
            {
                var testObject = testObjects[i];
                var actual = testObject.GroupNestedBy("kvp");
                Assert.IsNotNull(actual);

                var flattended = actual.Flatten("name", "value")?.ToList();
                Assert.IsNotNull(flattended);
                var expectedKvps = expectedData[i];
                Assert.AreEqual(expectedKvps.Count, flattended!.Count);

                for (int j = 0; j < expectedKvps.Count; j++)
                {
                    var actualKvp = flattended[j];
                    Assert.AreSame(match.Groups["kvp"].Captures[kvpCount++], actualKvp.Key);
                    Assert.AreEqual(2, actualKvp.Count);

                    var actualName = actualKvp.Captures[0]?.Value;
                    var actualValue = actualKvp.Captures[1]?.Value;
                    var (expectedName, expectedValue) = expectedKvps[j];
                    Assert.AreEqual(expectedName, actualName);
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }
        }
    }
}

# Capsaicin.RegexUtil
This library provides utility for walking the tree of the captures of a regular expression match. It helps in following scenarios without having to deal with the position (indexes) of captures in the original string:
- A regular expression with optional groups is to be iterated with captures "aligned"
- hierarchical iteratation of the captures tree 
- flatten the capture tree into tuples

## Usage
```
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Capsaicin.RegexUtil;

//
// Example 1: simple GroupCapturesBy
//
Console.WriteLine("*** Example 1 ***");
var regex1 = new Regex(@"^((?<tuple>\(((?<value>\w+)(,(?=\w))?)+\))(,(?=\())?)*$");
var match1 = regex1.Match("(a,b,c),(d),(e,f)");
var groups = match1
    .Group("value")
    .By("tuple");

int count = 0;
foreach (var group in groups)
{
    Console.WriteLine($"Values in group #{count++}: {string.Join(", ", group.Select("value"))}");
}

//
// Example 2: nested grouping iterated using one loop (flattended)
//
Console.WriteLine();
Console.WriteLine("*** Example 2 ***");
var regex = new Regex(@"^(?<object>(?<id>[a-zA-Z]\w*)\((?<property>(?<name>[a-zA-Z]\w*)=(?<value>\w*)(,(?=[a-zA-Z]))?)*\)(,(?=[a-zA-Z]))?)*$");
var match = regex.Match("obj1(a=1,b=2),obj2(x=y)");

var properties = match
    .Group("id", "property", "name", "value") // for nested groups it is essential to specify all captures that are later used!
    .By("object")
    .ThenBy("property")
    .Flatten("name", "value");

foreach (var property in properties)
{
    var objectId = property.Parent?.First("id")?.Value ?? "<parent id not found>";
    (string? name, string? value) = property;
    Console.WriteLine($"{objectId}.{name} = {value}");
}

//
// Example 3: nested grouping iterated with nested loop
//
Console.WriteLine();
Console.WriteLine("*** Example 3 ***");
var objectGroups = match
    .Group("id", "property", "name", "value") // for nested groups it is essential to specify all captures that are later used!
    .By("object");

foreach (var objectGroup in objectGroups)
{
    Console.WriteLine($"Object {objectGroup.First("id")}");
    foreach (var (name, value) in objectGroup.GroupNestedBy("property").Flatten("name", "value"))
    {
        Console.WriteLine($"{name} = {value}");
    }
}



```

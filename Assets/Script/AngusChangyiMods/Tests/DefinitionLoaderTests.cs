// using NUnit.Framework;
// using System.Text;
// using System.Xml.Linq;
// using UnityEngine;
// using System.IO;
// using System.Linq;
// using System.Collections.Generic;
// using AngusChangyiMods.Core;


// namespace AngusChangyiMods.Tests
// {
//     public class DefinitionLoaderTests
//     {
//         private DefinitionDatabase _definitionDatabase;
//         private ModDefinitionDeserializer _deserializer;

//         [SetUp]
//         public void Setup()
//         {
//             _definitionDatabase = new DefinitionDatabase();
//             _deserializer = new ModDefinitionDeserializer(_definitionDatabase, null);
//         }

//         [Test]
//         public void Deserialize_SimpleDefinition_ShouldRegisterCorrectly()
//         {
//             // 模擬一個定義的 XML 字串
//             var xml = @"
//                 <Defs>
//                   <TestDefinition>
//                     <defName>sample_001</defName>
//                     <label>Sample Label</label>
//                     <value>42</value>
//                   </TestDefinition>
//                 </Defs>";

//             var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
//             var xdoc = XDocument.Load(stream);

//             // Act
//             _deserializer.Deserialize(xdoc, "TestDefinition");

//             // Assert
//             var defs = _definitionDatabase.GetAll();
//             Assert.AreEqual(1, defs.Count);
//             var def = defs.First();

//             Assert.AreEqual("sample_001", def.defName);
//             Assert.AreEqual("Sample Label", def.GetType().GetProperty("label")?.GetValue(def));
//             Assert.AreEqual(42, def.GetType().GetProperty("value")?.GetValue(def));
//         }

//         [Test]
//         public void Deserialize_InvalidXml_ShouldNotThrowButSkip()
//         {
//             var badXml = @"<Defs><Broken></Defs>"; // malformed

//             using var stream = new MemoryStream(Encoding.UTF8.GetBytes(badXml));
//             Assert.Throws<System.Xml.XmlException>(() =>
//             {
//                 var doc = XDocument.Load(stream);
//                 _deserializer.Deserialize(doc, "Broken");
//             });
//         }
//     }
// }

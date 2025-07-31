// using System;
// using System.Xml.Linq;
// using NUnit.Framework;
//
// namespace AngusChangyiMods.Core.DefinitionProcessing.Test
// {
//     [TestFixture]
//     [TestOf(typeof(DefinitionVarifier))]
//     public class DefinitionVarifierTest
//     {
//
//         [Test]
//         [TestCase("Common/SimpleCase.xml")]
//         public void Test_01_ShouldVerifyValidDefinitions(string fileName)
//         {
//             // Arrange
//             DefinitionVarifier varifier = new DefinitionVarifier();
//             string content = CaseReader.ReadFile(fileName);
//             XElement check = XMLUtil.ParseFirstElement(content);
//             
//             // Act
//             bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
//             
//             // Assert
//             Assert.IsTrue(isValid, "Definitions should be valid");
//             Assert.IsEmpty(errorMessage, "Error message should be empty for valid definitions");
//         }
//
//         [Test]
//         [TestCase("Varifier/LostDefName.xml")]
//         public void Test_02_ShouldReturnErrorForMissingDefName(string fileName)
//         {
//             // Arrange
//             DefinitionVarifier varifier = new DefinitionVarifier();
//             string content = CaseReader.ReadFile(fileName);
//             XElement check = XMLUtil.ParseFirstElement(content);
//             
//             // Act
//             
//             bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
//             
//             // Assert
//             Assert.IsFalse(isValid, "Should not be valid if defName is missing");
//             Assert.AreEqual(DefinitionVarifier.error_lostDefName, errorMessage, "Should return correct error message for missing defName");
//         }
//
//         [Test]
//         [TestCase("Varifier/IllegalDefName1.xml")]
//         [TestCase("Varifier/IllegalDefName2.xml")]
//         [TestCase("Varifier/IllegalDefName3.xml")]
//         public void Test_03_ShouldReturnErrorForIllegalDefName(string fileName)
//         {
//             // Arrange
//             DefinitionVarifier varifier = new DefinitionVarifier();
//             string content = CaseReader.ReadFile(fileName);
//             XElement check = XMLUtil.ParseFirstElement(content);
//            
//             // Act
//             bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
//             
//             // Assert
//             string expectedMessage = string.Format(DefinitionVarifier.error_illegalDefName, check.Element(Def.DefName)?.Value);
//             Assert.IsFalse(isValid, "Should not be valid if defName is illegal");
//             Assert.IsTrue(errorMessage.Contains(expectedMessage), 
//                 $"Should return error message containing '{expectedMessage}' for illegal defName, but got '{errorMessage}'");
//         }
//         
//     }
// }
using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionVarifier))]
    public class DefinitionVarifierTest
    {

        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Varifier), nameof(DefProcessingCase_Varifier.SimpleCase))]
        public void Test_01_ShouldVerifyValidDefinitions(string content)
        {
            // Arrange
            DefinitionVarifier varifier = new DefinitionVarifier();
            XElement check = XMLUtil.ParseFirstElement(content);
            
            // Act
            bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
            
            // Assert
            Assert.IsTrue(isValid, "Definitions should be valid");
            Assert.IsEmpty(errorMessage, "Error message should be empty for valid definitions");
        }

        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Varifier), nameof(DefProcessingCase_Varifier.LostDefName))]
        public void Test_02_ShouldReturnErrorForMissingDefName(string content)
        {
            // Arrange
            DefinitionVarifier varifier = new DefinitionVarifier();
            XElement check = XMLUtil.ParseFirstElement(content);
            
            // Act
            
            bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
            
            // Assert
            Assert.IsFalse(isValid, "Should not be valid if defName is missing");
            Assert.AreEqual(DefinitionVarifier.error_lostDefName, errorMessage, "Should return correct error message for missing defName");
        }

        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Varifier), nameof(DefProcessingCase_Varifier.IllegalDefName1))]
        [TestCaseSource(typeof(DefProcessingCase_Varifier), nameof(DefProcessingCase_Varifier.IllegalDefName2))]
        [TestCaseSource(typeof(DefProcessingCase_Varifier), nameof(DefProcessingCase_Varifier.IllegalDefName3))]
        public void Test_03_ShouldReturnErrorForIllegalDefName(string content)
        {
            // Arrange
            DefinitionVarifier varifier = new DefinitionVarifier();
            XElement check = XMLUtil.ParseFirstElement(content);
           
            // Act
            bool isValid = varifier.VerifyDefinitions(check, out string errorMessage);
            
            // Assert
            string expectedMessage = string.Format(DefinitionVarifier.error_illegalDefName, check.Element(Def.DefName)?.Value);
            Assert.IsFalse(isValid, "Should not be valid if defName is illegal");
            Assert.IsTrue(errorMessage.Contains(expectedMessage), 
                $"Should return error message containing '{expectedMessage}' for illegal defName, but got '{errorMessage}'");
        }
        
    }
}
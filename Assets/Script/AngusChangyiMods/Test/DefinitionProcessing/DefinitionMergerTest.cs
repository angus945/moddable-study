using System.Linq;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionMerger))]
    public class DefinitionMergerTest
    {
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Merger), nameof(DefProcessingCase_Merger.MergeCase))]
        public void Test_01_ShouldMergeDefinitions(XDocument[]sources, XDocument expected)
        {
            // Arrange
            IDefinitionVarifier verifier = Substitute.For<IDefinitionVarifier>();
            verifier.VerifyDefinitions(Arg.Any<XElement>(), out Arg.Any<string>()).Returns(true);
            
            DefinitionMerger merger = new DefinitionMerger(verifier);
            XDocument mergeTarget = XMLUtil.DefinitionBase;

            // Act
            string overrideMessages = string.Empty;
            foreach (var source in sources)
            {
                merger.MergeDefinitions(source, mergeTarget, out string overrideMessage);
                overrideMessages += overrideMessage;
            }
            
            //
            // Assert
            string expectedRootName = Def.Root;
            string mergeTargetRootName = mergeTarget.Root.Name.LocalName;
            int expectedNodeCount = expected.Root.Elements().Count();
            int mergeTargetNodeCount = mergeTarget.Root.Elements().Count();
            string mergedPrint = mergeTarget.ToString();
            string expectedPrint = expected.ToString();
            Assert.IsNotNull(mergeTarget, "Merge target should not be null");
            Assert.IsNotNull(mergeTarget.Root, "Merge target root should not be null");
            Assert.AreEqual(expectedRootName, mergeTargetRootName, "Merge target root name should match expected, expected: " + expectedRootName + ", actual: " + mergeTargetRootName);
            Assert.AreEqual(expectedNodeCount, mergeTargetNodeCount, "Merge target should have the same number of nodes as expected, expected: " + expectedNodeCount + ", actual: " + mergeTargetNodeCount);
            Assert.AreEqual(expectedPrint, mergedPrint, "Merged XML should match expected XML, expected: \n" + expectedPrint + ",\n actual: \n" + mergedPrint + "\nOverride messages: " + overrideMessages);
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Merger), nameof(DefProcessingCase_Merger.OverrideCase))]
        public void Test_02_ShouldOverrideDefinitions(XDocument[]sources, XDocument expected)
        {
            // Arrange
            IDefinitionVarifier verifier = Substitute.For<IDefinitionVarifier>();
            verifier.VerifyDefinitions(Arg.Any<XElement>(), out Arg.Any<string>()).Returns(true);
            
            DefinitionMerger merger = new DefinitionMerger(verifier);
            XDocument mergeTarget = XMLUtil.DefinitionBase;

            // Act
            string errorMessages = string.Empty;
            foreach (var source in sources)
            {
                merger.MergeDefinitions(source, mergeTarget, out string errorMessage);
                errorMessages += errorMessage;
            }
            
            // Assert
            string rawSources = string.Join("\n", sources.Select(s => s.ToString()));
            string mergedPrint = mergeTarget.ToString();
            string expectedPrint = expected.ToString();
            Assert.AreEqual(expectedPrint, mergedPrint, "Merged XML should match expected XML after override, expected: \n" + expectedPrint + ", actual: \n" + mergedPrint);
            Assert.IsTrue(errorMessages.Contains("Override Definition"), "Error message should contain 'Override Definition', error messages: " + errorMessages + "merged XML: " + mergedPrint + "\nRaw sources: " + rawSources);
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Merger), nameof(DefProcessingCase_Merger.IllegalCase))]
        public void Test_03_ShouldRemoveIllegalDefinitions(XDocument[]sources, XDocument expected)
        {
            // Arrange
            IDefinitionVarifier verifier = Substitute.For<IDefinitionVarifier>();
            verifier.VerifyDefinitions(Arg.Any<XElement>(), out Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var element = callInfo.ArgAt<XElement>(0);
                    var hasDefName = element.Element("defName") != null;
                    return hasDefName;
                });
            DefinitionMerger merger = new DefinitionMerger(verifier);
            XDocument mergeTarget = XMLUtil.DefinitionBase;
            
            // Act
            string errorMessages = string.Empty;
            foreach (var source in sources)
            {
                merger.MergeDefinitions(source, mergeTarget, out string errorMessage);
                errorMessages += errorMessage;
            }
            
            // Assert
            string mergedPrint = mergeTarget.ToString();
            string expectedPrint = expected.ToString();
            Assert.AreEqual(expectedPrint, mergedPrint, "Merged XML should match expected XML after removing illegal definitions, expected: \n" + expectedPrint + ", actual: \n" + mergedPrint);
            Assert.IsTrue(errorMessages.Contains("Invalid definition"), "Error message should contain 'Invalid definition', error messages: " + errorMessages);
        }

        [Test]
        public void Test_04_ReturnFalseWhenSourceInvalid()
        {
            // Arrange
            IDefinitionVarifier verifier = Substitute.For<IDefinitionVarifier>();
            DefinitionMerger merger = new DefinitionMerger(verifier);
            XDocument mergeTarget = new  XDocument();
            XDocument invalidSource = new XDocument(new XElement("InvalidRoot"));

            // Act
            bool result = merger.MergeDefinitions(invalidSource, mergeTarget, out string errorMessage);
            
            // Assert
            Assert.IsFalse(result, "Merge should return false for invalid source, error message: " + errorMessage);
            Assert.IsTrue(errorMessage.Contains("Invalid source document"), "Error message should indicate invalid source document, actual: " + errorMessage);
        }
        
        [Test]
        public void Test_05_ReturnFalseWhenMergeTargetNull()
        {
            // Arrange
            IDefinitionVarifier verifier = Substitute.For<IDefinitionVarifier>();
            DefinitionMerger merger = new DefinitionMerger(verifier);
            XDocument invalidSource = new XDocument(new XElement(Def.Root));

            // Act
            bool result = merger.MergeDefinitions(invalidSource, null, out string errorMessage);
            
            // Assert
            Assert.IsFalse(result, "Merge should return false when merge target is null, error message: " + errorMessage);
            Assert.IsTrue(errorMessage.Contains(DefinitionMerger.error_mergeTargetNull), 
                "Error message should indicate merge target is null, actual: " + errorMessage);
        }
    }
}
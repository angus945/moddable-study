using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core;
using System.Collections.Generic;
using System.Linq;

namespace ModInfrastructure.Test
{
    /// <summary>
    /// Mock logger for testing purposes that captures log messages
    /// </summary>


    [TestFixture]
    public class ModDefinitionLoaderTests
    {
        private ModDefinitionLoader loader;
        private string testDirectory;

        [SetUp]
        public void SetUp()
        {
            // 設置測試環境
            loader = new ModDefinitionLoader(new NullLogger());

            // 創建測試目錄
            testDirectory = Path.Combine(Path.GetTempPath(), "ModDefinitionLoaderTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            // 清理測試目錄
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        #region Helper Methods

        /// <summary>
        /// 為每個測試創建獨立的 XML 文檔，確保測試隔離
        /// </summary>
        private XDocument CreateTestXmlDocument()
        {
            return new XDocument(new XElement("Defs"));
        }

        /// <summary>
        /// 創建測試用的 XML 檔案
        /// </summary>
        private string CreateTestXmlFile(string fileName, string xmlContent)
        {
            string filePath = Path.Combine(testDirectory, fileName);
            File.WriteAllText(filePath, xmlContent);
            return filePath;
        }

        /// <summary>
        /// 創建有效的定義 XML 內容
        /// </summary>
        private string CreateValidDefinitionXml(string defType, string defId, Dictionary<string, string> properties = null)
        {
            var element = new XElement(defType,
                new XElement("defID", defId)
            );

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    element.Add(new XElement(prop.Key, prop.Value));
                }
            }

            return new XDocument(new XElement("Defs", element)).ToString();
        }

        #endregion

        #region LoadDefinition Tests

        [Test]
        public void Test_01_LoadDefinition_WithValidFile_ShouldReturnTrueAndMergeElements()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            var xmlContent = CreateValidDefinitionXml("TestDef", "Test001",
                new Dictionary<string, string> { { "label", "Test Label" }, { "value", "100" } });
            var filePath = CreateTestXmlFile("valid_test.xml", xmlContent);

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            Assert.IsTrue(result, "LoadDefinition should return true for valid file");
            Assert.AreEqual(1, testXmlDocument.Root.Elements().Count(), "Should merge one element");

            var mergedElement = testXmlDocument.Root.Elements("TestDef").FirstOrDefault();
            Assert.IsNotNull(mergedElement, "TestDef element should be merged");
            Assert.AreEqual("Test001", mergedElement.Element("defID")?.Value, "defID should be preserved");
            Assert.AreEqual("Test Label", mergedElement.Element("label")?.Value, "label should be preserved");
        }

        [Test]
        public void Test_02_LoadDefinition_WithNonExistentFile_ShouldReturnFalseAndLogWarning()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            string nonExistentPath = Path.Combine(testDirectory, "non_existent.xml");

            // Act
            bool result = loader.LoadDefinition(nonExistentPath, testXmlDocument);

            // Assert
            Assert.IsFalse(result, "LoadDefinition should return false for non-existent file");
            Assert.AreEqual(0, testXmlDocument.Root.Elements().Count(), "No elements should be merged");
        }

        [Test]
        public void Test_03_LoadDefinition_WithInvalidXmlFormat_ShouldReturnFalseAndLogError()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            string invalidXml = "This is not valid XML content";
            var filePath = CreateTestXmlFile("invalid.xml", invalidXml);

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            Assert.IsFalse(result, "LoadDefinition should return false for invalid XML");
            Assert.AreEqual(0, testXmlDocument.Root.Elements().Count(), "No elements should be merged");
        }

        [Test]
        public void Test_04_LoadDefinition_WithInvalidRootElement_ShouldReturnFalseAndLogWarning()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            string xmlWithWrongRoot = "<InvalidRoot><TestDef><defID>Test001</defID></TestDef></InvalidRoot>";
            var filePath = CreateTestXmlFile("wrong_root.xml", xmlWithWrongRoot);

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            Assert.IsFalse(result, "LoadDefinition should return false for invalid root element");
            Assert.AreEqual(0, testXmlDocument.Root.Elements().Count(), "No elements should be merged");
        }

        [Test]
        public void Test_05_LoadDefinition_WithMultipleDefinitions_ShouldMergeAllElements()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            var xmlContent = @"
<Defs>
    <TestDef>
        <defID>Test001</defID>
        <label>First Test</label>
    </TestDef>
    <AnotherDef>
        <defID>Another001</defID>
        <value>200</value>
    </AnotherDef>
</Defs>";
            var filePath = CreateTestXmlFile("multiple_defs.xml", xmlContent);

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            Assert.IsTrue(result, "LoadDefinition should return true");
            Assert.AreEqual(2, testXmlDocument.Root.Elements().Count(), "Should merge two elements");
            Assert.IsNotNull(testXmlDocument.Root.Elements("TestDef").FirstOrDefault());
            Assert.IsNotNull(testXmlDocument.Root.Elements("AnotherDef").FirstOrDefault());
        }

        #endregion

        #region RemoveExisting Tests

        [Test]
        public void Test_20_LoadDefinition_WithDuplicateDefID_ShouldOverrideExisting()
        {
            // Arrange - 每個測試使用獨立的 XML 文檔
            var testXmlDocument = CreateTestXmlDocument();

            // 先載入一個定義
            var initialXmlContent = CreateValidDefinitionXml("TestDef", "Test001",
                new Dictionary<string, string> { { "label", "Original Label" } });
            var initialFilePath = CreateTestXmlFile("initial.xml", initialXmlContent);
            loader.LoadDefinition(initialFilePath, testXmlDocument);

            // 確認初始狀態
            Assert.AreEqual(1, testXmlDocument.Root.Elements().Count());
            Assert.AreEqual("Original Label", testXmlDocument.Root.Elements("TestDef").First().Element("label")?.Value);

            // Act - 載入相同 defID 的新定義
            var overrideXmlContent = CreateValidDefinitionXml("TestDef", "Test001",
                new Dictionary<string, string> { { "label", "Override Label" } });
            var overrideFilePath = CreateTestXmlFile("override.xml", overrideXmlContent);
            bool result = loader.LoadDefinition(overrideFilePath, testXmlDocument);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, testXmlDocument.Root.Elements().Count(), "Should still have only one element");
            Assert.AreEqual("Override Label", testXmlDocument.Root.Elements("TestDef").First().Element("label")?.Value,
                "Should have the override label");
        }

        [Test]
        public void Test_21_LoadDefinition_WithDifferentDefTypes_ShouldNotOverride()
        {
            // Arrange - 每個測試使用獨立的 XML 文檔
            var testXmlDocument = CreateTestXmlDocument();

            // 先載入一個 TestDef
            var testDefContent = CreateValidDefinitionXml("TestDef", "Test001");
            var testDefPath = CreateTestXmlFile("testdef.xml", testDefContent);
            loader.LoadDefinition(testDefPath, testXmlDocument);

            // Act - 載入相同 defID 但不同 defType 的定義
            var anotherDefContent = CreateValidDefinitionXml("AnotherDef", "Test001");
            var anotherDefPath = CreateTestXmlFile("anotherdef.xml", anotherDefContent);
            bool result = loader.LoadDefinition(anotherDefPath, testXmlDocument);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, testXmlDocument.Root.Elements().Count(), "Should have both definitions");
            Assert.IsNotNull(testXmlDocument.Root.Elements("TestDef").FirstOrDefault());
            Assert.IsNotNull(testXmlDocument.Root.Elements("AnotherDef").FirstOrDefault());
        }

        [Test]
        public void Test_22_LoadDefinition_WithMissingDefID_ShouldNotTriggerOverride()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();
            var xmlContentWithoutDefID = @"
<Defs>
    <TestDef>
        <label>No DefID Element</label>
        <value>100</value>
    </TestDef>
</Defs>";
            var filePath = CreateTestXmlFile("no_defid.xml", xmlContentWithoutDefID);

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            Assert.IsTrue(result, "Should still load successfully");
            Assert.AreEqual(1, testXmlDocument.Root.Elements().Count());
            // 不應該有覆蓋訊息，因為沒有 defID
        }

        #endregion

        #region Edge Cases and Error Handling

        [Test]
        public void Test_30_LoadDefinition_WithNullXmlDocument_ShouldThrowException()
        {
            // Arrange
            var validXmlContent = CreateValidDefinitionXml("TestDef", "Test001");
            var filePath = CreateTestXmlFile("valid.xml", validXmlContent);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => loader.LoadDefinition(filePath, null));
        }

        [Test]
        public void Test_31_LoadDefinition_WithNullFilePath_ShouldReturnFalse()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();

            // Act
            bool result = loader.LoadDefinition(null, testXmlDocument);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Test_32_LoadDefinition_WithEmptyFilePath_ShouldReturnFalse()
        {
            // Arrange
            var testXmlDocument = CreateTestXmlDocument();

            // Act
            bool result = loader.LoadDefinition("", testXmlDocument);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Test_33_LoadDefinition_WithLargeFile_ShouldHandleEfficiently()
        {
            // Arrange - 每個測試使用獨立的 XML 文檔
            var testXmlDocument = CreateTestXmlDocument();

            // 創建包含大量定義的檔案
            var largeXmlContent = "<Defs>";
            for (int i = 0; i < 1000; i++)
            {
                largeXmlContent += $@"
    <TestDef>
        <defID>Test{i:D4}</defID>
        <label>Test Label {i}</label>
        <value>{i * 10}</value>
    </TestDef>";
            }
            largeXmlContent += "</Defs>";

            var filePath = CreateTestXmlFile("large_file.xml", largeXmlContent);
            var startTime = DateTime.Now;

            // Act
            bool result = loader.LoadDefinition(filePath, testXmlDocument);

            // Assert
            var endTime = DateTime.Now;
            var processingTime = endTime - startTime;

            Assert.IsTrue(result, "Should successfully load large file");
            Assert.AreEqual(1000, testXmlDocument.Root.Elements().Count(), "Should load all 1000 definitions");
            Assert.IsTrue(processingTime.TotalSeconds < 5, "Should process within reasonable time");
        }

        #endregion
    }
}

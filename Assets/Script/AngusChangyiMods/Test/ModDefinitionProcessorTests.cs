using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core;
using System.Collections.Generic;
using System.Linq;

namespace ModInfrastructure.Test
{
    [TestFixture]
    public class ModDefinitionProcessorTests
    {
        private ModDefinitionProcessor processor;
        private MockLogger mockLogger;
        private string testDirectory;

        [SetUp]
        public void SetUp()
        {
            // 設置測試環境
            mockLogger = new MockLogger();
            processor = new ModDefinitionProcessor(mockLogger);

            // 創建測試目錄
            testDirectory = Path.Combine(Path.GetTempPath(), "ModDefinitionProcessorTests", Guid.NewGuid().ToString());
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

            mockLogger.Clear();
        }

        #region Helper Methods

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

        #region LoadDefinitions Tests

        [Test]
        public void Test_01_LoadDefinitions_WithMultipleValidFiles_ShouldLoadAllSuccessfully()
        {
            // Arrange
            var file1Content = CreateValidDefinitionXml("TestDef", "Test001");
            var file2Content = CreateValidDefinitionXml("TestDef", "Test002");
            var file3Content = CreateValidDefinitionXml("AnotherDef", "Another001");

            var filePaths = new[]
            {
                CreateTestXmlFile("file1.xml", file1Content),
                CreateTestXmlFile("file2.xml", file2Content),
                CreateTestXmlFile("file3.xml", file3Content)
            };

            // Act
            processor.LoadDefinitions(filePaths);

            // Assert
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("Starting to load 3 definition files")));
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("success: 3/3")));
        }

        [Test]
        public void Test_02_LoadDefinitions_WithMixedValidAndInvalidFiles_ShouldLoadValidOnesOnly()
        {
            // Arrange
            var validFileContent = CreateValidDefinitionXml("TestDef", "Test001");
            var invalidFileContent = "Invalid XML";

            var filePaths = new[]
            {
                CreateTestXmlFile("valid.xml", validFileContent),
                CreateTestXmlFile("invalid.xml", invalidFileContent),
                Path.Combine(testDirectory, "non_existent.xml") // 不存在的檔案
            };

            // Act
            processor.LoadDefinitions(filePaths);

            // Assert
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("success: 1/3")));
        }

        [Test]
        public void Test_03_LoadDefinitions_WithEmptyArray_ShouldHandleGracefully()
        {
            // Arrange
            var emptyFilePaths = new string[0];

            // Act & Assert - 不應該拋出異常
            Assert.DoesNotThrow(() => processor.LoadDefinitions(emptyFilePaths));
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("Starting to load 0 definition files")));
        }

        [Test]
        public void Test_04_LoadDefinitions_WithDuplicateDefIDs_ShouldHandleOverrides()
        {
            // Arrange - 模擬模組覆蓋情境
            var baseDefContent = CreateValidDefinitionXml("WeaponDef", "BaseWeapon",
                new Dictionary<string, string> { { "damage", "10" }, { "range", "1" } });

            var modAContent = CreateValidDefinitionXml("WeaponDef", "ModA_Sword",
                new Dictionary<string, string> { { "damage", "15" }, { "range", "1" } });

            var modBContent = CreateValidDefinitionXml("WeaponDef", "ModB_Bow",
                new Dictionary<string, string> { { "damage", "12" }, { "range", "5" } });

            // ModB 覆蓋 BaseWeapon
            var modBOverrideContent = CreateValidDefinitionXml("WeaponDef", "BaseWeapon",
                new Dictionary<string, string> { { "damage", "20" }, { "range", "2" } });

            var filePaths = new[]
            {
                CreateTestXmlFile("base.xml", baseDefContent),
                CreateTestXmlFile("modA.xml", modAContent),
                CreateTestXmlFile("modB.xml", modBContent),
                CreateTestXmlFile("modB_override.xml", modBOverrideContent)
            };

            // Act
            processor.LoadDefinitions(filePaths);

            // Assert
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("success: 4/4")));
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("Overriding Definition: WeaponDef with defID: BaseWeapon")));
        }

        [Test]
        public void Test_05_LoadDefinitions_DependencyInjection_ShouldUseInjectedLogger()
        {
            // Arrange
            var testFile = CreateTestXmlFile("test.xml", CreateValidDefinitionXml("TestDef", "Test001"));

            // Act
            processor.LoadDefinitions(new[] { testFile });

            // Assert - 驗證日誌是通過注入的 logger 記錄的
            Assert.IsTrue(mockLogger.LogMessages.Count > 0, "Should have logged messages through injected logger");
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("[DefinitionLoader]")),
                "Should have tagged log messages from DefinitionLoader");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Test_90_LoadDefinitions_CompleteWorkflow_ShouldWorkCorrectly()
        {
            // Arrange - 模擬完整的模組定義載入流程
            var coreDefContent = CreateValidDefinitionXml("ItemDef", "Core_Item",
                new Dictionary<string, string> { { "label", "Core Item" }, { "value", "100" } });

            var modADefContent = CreateValidDefinitionXml("ItemDef", "ModA_Item",
                new Dictionary<string, string> { { "label", "Mod A Item" }, { "value", "150" } });

            var modBDefContent = CreateValidDefinitionXml("ItemDef", "ModB_Item",
                new Dictionary<string, string> { { "label", "Mod B Item" }, { "value", "200" } });

            // Mod B 覆蓋 Core Item
            var modBOverrideContent = CreateValidDefinitionXml("ItemDef", "Core_Item",
                new Dictionary<string, string> { { "label", "Modified Core Item" }, { "value", "250" } });

            var filePaths = new[]
            {
                CreateTestXmlFile("core.xml", coreDefContent),
                CreateTestXmlFile("modA.xml", modADefContent),
                CreateTestXmlFile("modB.xml", modBDefContent),
                CreateTestXmlFile("modB_override.xml", modBOverrideContent)
            };

            // Act
            processor.LoadDefinitions(filePaths);

            // Assert
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("Starting to load 4 definition files")));
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("success: 4/4")));
            Assert.IsTrue(mockLogger.LogMessages.Any(m => m.Contains("Overriding Definition: ItemDef with defID: Core_Item")));

            // 驗證沒有錯誤
            Assert.AreEqual(0, mockLogger.ErrorMessages.Count, "Should have no error messages");
        }

        #endregion
    }
}

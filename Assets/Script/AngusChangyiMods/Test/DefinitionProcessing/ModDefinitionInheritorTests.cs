using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core;

namespace ModInfrastructure.Test
{
  [TestFixture]
  public class ModDefinitionInheritorTests
  {
    private ModDefinitionInheritor inheritor;

    [SetUp]
    public void SetUp()
    {
      inheritor = new ModDefinitionInheritor(new NullLogger());
    }

    [TearDown]
    public void TearDown() { }

    #region Helper

    private XDocument LoadTestXml(string xml)
    {
      return XDocument.Parse(xml);
    }

    #endregion

    [TestCaseSource(typeof(ModDefinitionInheritorTestCases), nameof(ModDefinitionInheritorTestCases.SingleInheritance))]
    public void Test_01_SingleInheritance_ShouldResolveAndMergeFields(string xml)
    {
      // Arrange
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var potion = result.Root.Elements("ThingDef")
          .FirstOrDefault(e => e.Element("defID")?.Value == "HealthPotion");

      // Assert
      Assert.IsNotNull(potion);
      Assert.AreEqual("治療藥水", potion.Element("label")?.Value); // 子級覆蓋
      Assert.AreEqual("物品的基本描述", potion.Element("description")?.Value); // 父級繼承
      Assert.AreEqual("5", potion.Element("stack")?.Value); // 子級覆蓋
    }

    [TestCaseSource(typeof(ModDefinitionInheritorTestCases), nameof(ModDefinitionInheritorTestCases.DeepInheritance))]
    public void Test_02_DeepInheritance_ShouldResolveMultiLevelChain(string xml)
    {
      // Arrange
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var sword = result.Root.Elements("ThingDef")
          .FirstOrDefault(e => e.Element("defID")?.Value == "IronSword");

      // Assert
      Assert.IsNotNull(sword);
      Assert.AreEqual("1", sword.Element("stack")?.Value, "應繼承 BaseItem 的 stack");
      Assert.AreEqual("30", sword.Element("damage")?.Value, "應覆蓋 BaseWeapon 的 damage");
    }

    [TestCaseSource(typeof(ModDefinitionInheritorTestCases), nameof(ModDefinitionInheritorTestCases.AbstractDefs))]
    public void Test_03_AbstractDefs_ShouldBeRemoved(string xml)
    {
      // Arrange
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var allDefIDs = result.Root.Elements("ThingDef").Select(e => e.Element("defID")?.Value).ToList();

      // Assert
      Assert.Contains("ConcreteDef", allDefIDs);
      Assert.IsFalse(allDefIDs.Contains("AbstractDef"), "抽象定義應被移除");
    }

    [TestCaseSource(typeof(ModDefinitionInheritorTestCases), nameof(ModDefinitionInheritorTestCases.TagLists))]
    public void Test_04_TagLists_ShouldBeMergedAndDeduplicated(string xml)
    {
      // Arrange
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var tags = result.Root.Elements("ThingDef")
          .FirstOrDefault(e => e.Element("defID")?.Value == "Child")?
          .Element("tags")?
          .Elements("tag")
          .Select(t => t.Value)
          .ToHashSet();

      // Assert
      Assert.IsNotNull(tags);
      CollectionAssert.AreEquivalent(new[] { "a", "b", "c" }, tags);
    }

    [TestCaseSource(typeof(ModDefinitionInheritorTestCases), nameof(ModDefinitionInheritorTestCases.MissingParent))]
    public void Test_05_MissingParent_ShouldIgnoreAndSkip(string xml)
    {
      // Arrange
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var orphan = result.Root.Elements("ThingDef")
          .FirstOrDefault(e => e.Element("defID")?.Value == "Orphan");

      // Assert
      Assert.IsNotNull(orphan);
      Assert.AreEqual("遺失繼承", orphan.Element("label")?.Value);
    }
  }
}

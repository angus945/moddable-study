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
    private string testDirectory;

    [SetUp]
    public void SetUp()
    {
      inheritor = new ModDefinitionInheritor(new NullLogger());
      testDirectory = Path.Combine(Path.GetTempPath(), "ModDefinitionInheritorTests", Guid.NewGuid().ToString());
      Directory.CreateDirectory(testDirectory);
    }

    [TearDown]
    public void TearDown()
    {
      if (Directory.Exists(testDirectory))
        Directory.Delete(testDirectory, true);
    }

    #region Helper

    private string CreateTestXmlFile(string fileName, string xmlContent)
    {
      string filePath = Path.Combine(testDirectory, fileName);
      File.WriteAllText(filePath, xmlContent);
      return filePath;
    }

    private XDocument LoadTestXml(string xml)
    {
      return XDocument.Parse(xml);
    }

    #endregion

    [Test]
    public void Test_01_SingleInheritance_ShouldResolveAndMergeFields()
    {
      // Arrange
      var xml = @"
<Defs>
  <ThingDef isAbstract=""true"">
    <defID>BaseItem</defID>
    <label>基礎物品</label>
    <description>物品的基本描述</description>
    <stack>1</stack>
  </ThingDef>
  <ThingDef parent=""BaseItem"">
    <defID>HealthPotion</defID>
    <label>治療藥水</label>
    <stack>5</stack>
  </ThingDef>
</Defs>";

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

    [Test]
    public void Test_02_DeepInheritance_ShouldResolveMultiLevelChain()
    {
      // Arrange
      var xml = @"
<Defs>
  <ThingDef isAbstract=""true"">
    <defID>BaseItem</defID>
    <stack>1</stack>
  </ThingDef>
  <ThingDef isAbstract=""true"" parent=""BaseItem"">
    <defID>BaseWeapon</defID>
    <damage>10</damage>
  </ThingDef>
  <ThingDef parent=""BaseWeapon"">
    <defID>IronSword</defID>
    <damage>30</damage>
  </ThingDef>
</Defs>";
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

    [Test]
    public void Test_03_AbstractDefs_ShouldBeRemoved()
    {
      // Arrange
      var xml = @"
<Defs>
  <ThingDef isAbstract=""true"">
    <defID>AbstractDef</defID>
  </ThingDef>
  <ThingDef>
    <defID>ConcreteDef</defID>
  </ThingDef>
</Defs>";
      var doc = LoadTestXml(xml);

      // Act
      var result = inheritor.ProcessInheritance(doc);
      var allDefIDs = result.Root.Elements("ThingDef").Select(e => e.Element("defID")?.Value).ToList();

      // Assert
      Assert.Contains("ConcreteDef", allDefIDs);
      Assert.IsFalse(allDefIDs.Contains("AbstractDef"), "抽象定義應被移除");
    }

    [Test]
    public void Test_04_TagLists_ShouldBeMergedAndDeduplicated()
    {
      // Arrange
      var xml = @"
<Defs>
  <ThingDef isAbstract=""true"">
    <defID>Base</defID>
    <tags>
      <tag>a</tag>
      <tag>b</tag>
    </tags>
  </ThingDef>
  <ThingDef parent=""Base"">
    <defID>Child</defID>
    <tags>
      <tag>b</tag>
      <tag>c</tag>
    </tags>
  </ThingDef>
</Defs>";
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

    [Test]
    public void Test_05_MissingParent_ShouldIgnoreAndSkip()
    {
      // Arrange
      var xml = @"
<Defs>
  <ThingDef parent=""NonExistent"">
    <defID>Orphan</defID>
    <label>遺失繼承</label>
  </ThingDef>
</Defs>";
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

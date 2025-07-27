// using System;
// using System.Collections.Generic;
// using System.Linq;
// using NUnit.Framework;
// using AngusChangyiMods.Core;
// using AngusChangyiMods.Core.Utils;
// using Angus;

// namespace ModInfrastructure.Test
// {
//     /// <summary>
//     /// Unit tests for definition inheritance functionality.
//     /// </summary>
//     [TestFixture]
//     public class DefinitionInheritanceTests
//     {
//         private List<CharacterDef> testDefinitions;

//         [SetUp]
//         public void SetUp()
//         {
//             // Initialize test data before each test
//             testDefinitions = CreateTestDefinitions();
//         }

//         /// <summary>
//         /// Test single-level inheritance (direct parent-child relationship).
//         /// </summary>
//         [Test]
//         public void TestSingleLevelInheritance()
//         {
//             // Arrange: Create simple parent-child definitions
//             var parent = new CharacterDef
//             {
//                 defID = "Parent",
//                 health = 100,
//                 speed = 5,
//                 IsAbstract = true
//             };

//             var child = new CharacterDef
//             {
//                 defID = "Child",
//                 health = 0, // Should inherit from parent
//                 speed = 8,  // Override parent's speed
//                 inheritsFrom = "Parent"
//             };

//             var definitions = new List<CharacterDef> { parent, child };

//             // Act: Process inheritance
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(definitions, ApplyCharacterInheritance).ToList();

//             // Assert: Verify inheritance results
//             var processedChild = processed.FirstOrDefault(d => d.defID == "Child");
//             Assert.IsNotNull(processedChild, "Child definition should be in processed results");
//             Assert.AreEqual(100, processedChild.health, "Child should inherit health from parent");
//             Assert.AreEqual(8, processedChild.speed, "Child should keep its own speed");
//         }

//         /// <summary>
//         /// Test multi-level inheritance chain (grandparent -> parent -> child).
//         /// </summary>
//         [Test]
//         public void TestMultiLevelInheritance()
//         {
//             // Arrange: Create inheritance chain
//             var grandparent = new CharacterDef
//             {
//                 defID = "Grandparent",
//                 health = 50,
//                 speed = 3,
//                 IsAbstract = true
//             };

//             var parent = new CharacterDef
//             {
//                 defID = "Parent",
//                 health = 100, // Override grandparent's health
//                 speed = 0,    // Should inherit grandparent's speed
//                 inheritsFrom = "Grandparent",
//                 IsAbstract = true
//             };

//             var child = new CharacterDef
//             {
//                 defID = "Child",
//                 health = 0, // Should inherit parent's health (100)
//                 speed = 0,  // Should inherit through chain: parent->grandparent (3)
//                 inheritsFrom = "Parent"
//             };

//             var definitions = new List<CharacterDef> { grandparent, parent, child };

//             // Act: Process inheritance
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(definitions, ApplyCharacterInheritance).ToList();

//             // Assert: Verify multi-level inheritance
//             var processedChild = processed.FirstOrDefault(d => d.defID == "Child");
//             Assert.IsNotNull(processedChild, "Child definition should be in processed results");
//             Assert.AreEqual(100, processedChild.health, "Child should inherit health from parent (100)");
//             Assert.AreEqual(3, processedChild.speed, "Child should inherit speed through chain from grandparent (3)");
//         }

//         /// <summary>
//         /// Test the specific Knight inheritance scenario.
//         /// </summary>
//         [Test]
//         public void TestKnightInheritance()
//         {
//             // Act: Process inheritance with test definitions
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(testDefinitions, ApplyCharacterInheritance).ToList();

//             // Assert: Verify Knight inheritance
//             var knight = processed.FirstOrDefault(d => d.defID == "Knight");
//             Assert.IsNotNull(knight, "Knight should exist in processed results");
//             Assert.AreEqual(150, knight.health, "Knight should inherit health from BaseWarrior");
//             Assert.AreEqual(3, knight.speed, "Knight should keep its own speed value");
//         }

//         /// <summary>
//         /// Test the specific Wizard inheritance scenario.
//         /// </summary>
//         [Test]
//         public void TestWizardInheritance()
//         {
//             // Act: Process inheritance with test definitions
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(testDefinitions, ApplyCharacterInheritance).ToList();

//             // Assert: Verify Wizard inheritance
//             var wizard = processed.FirstOrDefault(d => d.defID == "Wizard");
//             Assert.IsNotNull(wizard, "Wizard should exist in processed results");
//             Assert.AreEqual(70, wizard.health, "Wizard should keep its own health value");
//             Assert.AreEqual(6, wizard.speed, "Wizard should inherit speed from BaseMage");
//         }

//         /// <summary>
//         /// Test that abstract definitions are excluded from final results.
//         /// </summary>
//         [Test]
//         public void TestAbstractDefinitionsExclusion()
//         {
//             // Act: Process inheritance
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(testDefinitions, ApplyCharacterInheritance).ToList();

//             // Assert: Verify abstract definitions are excluded
//             Assert.IsFalse(processed.Any(d => d.defID == "BaseCharacter"), "BaseCharacter (abstract) should not be in results");
//             Assert.IsFalse(processed.Any(d => d.defID == "BaseWarrior"), "BaseWarrior (abstract) should not be in results");
//             Assert.IsFalse(processed.Any(d => d.defID == "BaseMage"), "BaseMage (abstract) should not be in results");

//             // But concrete definitions should be included
//             Assert.IsTrue(processed.Any(d => d.defID == "Knight"), "Knight (concrete) should be in results");
//             Assert.IsTrue(processed.Any(d => d.defID == "Wizard"), "Wizard (concrete) should be in results");
//         }

//         /// <summary>
//         /// Test topological sorting ensures correct processing order.
//         /// </summary>
//         [Test]
//         public void TestTopologicalSorting()
//         {
//             // We can't directly test the internal TopologicalSort method,
//             // but we can verify that the inheritance results are correct,
//             // which implies correct processing order.

//             // Act: Process inheritance
//             var processed = DefinitionInheritanceUtils.ProcessInheritance(testDefinitions, ApplyCharacterInheritance).ToList();

//             // Assert: If topological sorting works correctly, all inheritances should be resolved
//             var knight = processed.FirstOrDefault(d => d.defID == "Knight");
//             var wizard = processed.FirstOrDefault(d => d.defID == "Wizard");

//             Assert.IsNotNull(knight, "Knight should be processed");
//             Assert.IsNotNull(wizard, "Wizard should be processed");

//             // These assertions will pass only if parents are processed before children
//             Assert.AreEqual(150, knight.health, "Knight inheritance should work (requires BaseWarrior to be processed first)");
//             Assert.AreEqual(6, wizard.speed, "Wizard inheritance should work (requires BaseMage to be processed first)");
//         }

//         /// <summary>
//         /// Test property inheritance with default values.
//         /// </summary>
//         [Test]
//         public void TestPropertyInheritanceWithDefaults()
//         {
//             // Arrange: Test numeric property inheritance
//             var parent = new CharacterDef { defID = "Parent", health = 100, speed = 5 };
//             var child = new CharacterDef { defID = "Child", health = 0, speed = 0, inheritsFrom = "Parent" };

//             // Act: Test individual property inheritance
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.health, (c, v) => c.health = v);
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.speed, (c, v) => c.speed = v);

//             // Assert: Verify property inheritance
//             Assert.AreEqual(100, child.health, "Child should inherit health from parent");
//             Assert.AreEqual(5, child.speed, "Child should inherit speed from parent");
//         }

//         /// <summary>
//         /// Test that non-default values are not overridden.
//         /// </summary>
//         [Test]
//         public void TestNonDefaultValuesNotOverridden()
//         {
//             // Arrange: Child has non-default values
//             var parent = new CharacterDef { defID = "Parent", health = 100, speed = 5 };
//             var child = new CharacterDef { defID = "Child", health = 80, speed = 7, inheritsFrom = "Parent" };

//             // Act: Try to inherit (should not override non-default values)
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.health, (c, v) => c.health = v);
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.speed, (c, v) => c.speed = v);

//             // Assert: Verify values are not overridden
//             Assert.AreEqual(80, child.health, "Child's non-default health should not be overridden");
//             Assert.AreEqual(7, child.speed, "Child's non-default speed should not be overridden");
//         }

//         /// <summary>
//         /// Helper method to create test definitions matching the real scenario.
//         /// </summary>
//         private List<CharacterDef> CreateTestDefinitions()
//         {
//             var definitions = new List<CharacterDef>();

//             // BaseCharacter (abstract)
//             definitions.Add(new CharacterDef
//             {
//                 defID = "BaseCharacter",
//                 label = "基礎角色",
//                 health = 100,
//                 speed = 5,
//                 IsAbstract = true
//             });

//             // BaseWarrior (abstract, inherits from BaseCharacter)
//             definitions.Add(new CharacterDef
//             {
//                 defID = "BaseWarrior",
//                 label = "基礎戰士",
//                 health = 150,
//                 speed = 4,
//                 inheritsFrom = "BaseCharacter",
//                 IsAbstract = true
//             });

//             // BaseMage (abstract, inherits from BaseCharacter)
//             definitions.Add(new CharacterDef
//             {
//                 defID = "BaseMage",
//                 label = "基礎法師",
//                 health = 80,
//                 speed = 6,
//                 inheritsFrom = "BaseCharacter",
//                 IsAbstract = true
//             });

//             // Knight (concrete, inherits from BaseWarrior)
//             definitions.Add(new CharacterDef
//             {
//                 defID = "Knight",
//                 label = "騎士",
//                 health = 0, // Should inherit 150 from BaseWarrior
//                 speed = 3,  // Override parent's speed
//                 inheritsFrom = "BaseWarrior"
//             });

//             // Wizard (concrete, inherits from BaseMage)
//             definitions.Add(new CharacterDef
//             {
//                 defID = "Wizard",
//                 label = "巫師",
//                 health = 70, // Override parent's health
//                 speed = 0,   // Should inherit 6 from BaseMage
//                 inheritsFrom = "BaseMage"
//             });

//             return definitions;
//         }

//         /// <summary>
//         /// Helper method to apply character-specific inheritance logic.
//         /// </summary>
//         private void ApplyCharacterInheritance(CharacterDef child, CharacterDef parent)
//         {
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.health, (c, v) => c.health = v);
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent, c => c.speed, (c, v) => c.speed = v);
//         }
//     }
// }
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModArchitecture;
using ModArchitecture.Logger;
using Angus;

/// <summary>
/// Test script to verify inheritance functionality.
/// </summary>
public class InheritanceTest : MonoBehaviour
{
    void Start()
    {
        TestInheritance();
    }

    private void TestInheritance()
    {
        ModLogger.Log("=== Starting Inheritance Test ===", "InheritanceTest");

        // Create test definitions
        var definitions = CreateTestDefinitions();

        // Debug: Show initial values
        ModLogger.Log("Initial definitions:", "InheritanceTest");
        foreach (var def in definitions)
        {
            ModLogger.Log($"  {def.defID}: health={def.health}, speed={def.speed}, inheritsFrom={def.inheritsFrom}", "InheritanceTest");
        }

        // Create inheritor and process inheritance
        var inheritor = new CharacterDefInheritor();
        var processedDefs = inheritor.ProcessInheritance(definitions.Cast<ModArchitecture.Definition.Definition>());
        var characters = processedDefs.Cast<CharacterDef>().ToList();

        // Debug: Show processed values
        ModLogger.Log("Processed definitions:", "InheritanceTest");
        foreach (var def in characters)
        {
            ModLogger.Log($"  {def.defID}: health={def.health}, speed={def.speed}, inheritsFrom={def.inheritsFrom}", "InheritanceTest");
        }

        // Find specific characters
        var knight = characters.FirstOrDefault(c => c.defID == "Knight");
        var wizard = characters.FirstOrDefault(c => c.defID == "Wizard");

        // Test results
        if (knight != null)
        {
            ModLogger.Log($"Knight - Health: {knight.health} (expected: 150), Speed: {knight.speed} (expected: 3)", "InheritanceTest");

            if (knight.health == 150)
                ModLogger.Log("✅ Knight health inheritance SUCCESS", "InheritanceTest");
            else
                ModLogger.LogError($"❌ Knight health inheritance FAILED - got {knight.health}, expected 150", "InheritanceTest");
        }
        else
        {
            ModLogger.LogError("❌ Knight not found", "InheritanceTest");
        }

        if (wizard != null)
        {
            ModLogger.Log($"Wizard - Health: {wizard.health} (expected: 70), Speed: {wizard.speed} (expected: 6)", "InheritanceTest");

            if (wizard.speed == 6)
                ModLogger.Log("✅ Wizard speed inheritance SUCCESS", "InheritanceTest");
            else
                ModLogger.LogError($"❌ Wizard speed inheritance FAILED - got {wizard.speed}, expected 6", "InheritanceTest");
        }
        else
        {
            ModLogger.LogError("❌ Wizard not found", "InheritanceTest");
        }

        ModLogger.Log("=== Inheritance Test Completed ===", "InheritanceTest");
    }

    private List<CharacterDef> CreateTestDefinitions()
    {
        // Create test definitions matching the XML structure
        var definitions = new List<CharacterDef>();

        // BaseCharacter (abstract)
        var baseCharacter = new CharacterDef
        {
            defID = "BaseCharacter",
            label = "基礎角色",
            description = "所有角色的基礎定義，包含共通屬性",
            health = 100,
            speed = 5,
            IsAbstract = true
        };
        definitions.Add(baseCharacter);

        // BaseWarrior (abstract, inherits from BaseCharacter)
        var baseWarrior = new CharacterDef
        {
            defID = "BaseWarrior",
            label = "基礎戰士",
            description = "戰士類角色的基礎定義",
            health = 150,
            speed = 4,
            inheritsFrom = "BaseCharacter",
            IsAbstract = true
        };
        definitions.Add(baseWarrior);

        // BaseMage (abstract, inherits from BaseCharacter)
        var baseMage = new CharacterDef
        {
            defID = "BaseMage",
            label = "基礎法師",
            description = "法師類角色的基礎定義",
            health = 80,
            speed = 6,
            inheritsFrom = "BaseCharacter",
            IsAbstract = true
        };
        definitions.Add(baseMage);

        // Knight (concrete, inherits from BaseWarrior)
        var knight = new CharacterDef
        {
            defID = "Knight",
            label = "騎士",
            description = "重裝戰士，擁有強大的防禦力",
            health = 0, // Should inherit 150 from BaseWarrior
            speed = 3,  // Override parent's speed
            inheritsFrom = "BaseWarrior"
        };
        definitions.Add(knight);

        // Wizard (concrete, inherits from BaseMage)
        var wizard = new CharacterDef
        {
            defID = "Wizard",
            label = "巫師",
            description = "智慧的魔法使用者",
            health = 70, // Override parent's health
            speed = 0,   // Should inherit 6 from BaseMage
            inheritsFrom = "BaseMage"
        };
        definitions.Add(wizard);

        return definitions;
    }
}

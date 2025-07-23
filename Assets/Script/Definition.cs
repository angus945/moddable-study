using System.Collections.Generic;

public abstract class Definition
{
    public string defID;
    public string label;
    public string description;
}

public class CharacterDef : Definition
{
    public int health;
    public int speed;
}

public class ThingDef : Definition
{
    public int damage;
    public int stack;
    public List<string> tags; // e.g., "flammable", "edible"

    public WeaponProperties weaponProps;

}


public class WeaponProperties
{
    public string type; // e.g., "Melee", "Ranged"
    public int damage;
    public float range;
}


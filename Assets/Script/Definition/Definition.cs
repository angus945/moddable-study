using System.Collections.Generic;

[System.Serializable]
public abstract class Definition
{
    public string defID;
    public string label;
    public string description;
}

[System.Serializable]
public class CharacterDef : Definition
{
    public int health;
    public int speed;
}

[System.Serializable]
public class ThingDef : Definition
{
    public int damage;
    public int stack;

    [System.Xml.Serialization.XmlArray("tags")]
    [System.Xml.Serialization.XmlArrayItem("tag")]
    public List<string> tags; // e.g., "flammable", "edible"

    public WeaponProperties weaponProps;

}

[System.Serializable]
public class WeaponProperties
{
    public string type; // e.g., "Melee", "Ranged"
    public int damage;
    public float range;
}


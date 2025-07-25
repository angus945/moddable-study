using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

[System.Serializable]
public class ModMetaData
{
    public string id;
    public string name;
    public string description;
    public string author;

    public string directory;
    public string[] assemblies;
    public string[] custom;
    public string[] definitions;
    public string[] sounds;
    public string[] textures;
    public string[] patches;

    public override string ToString()
    {
        string infoString = "";
        infoString += $"ID: {id}\n";
        infoString += $"Name: {name}\n";
        infoString += $"Description: {description}\n";
        infoString += $"Author: {author}\n";
        infoString += $"Folder Path: {directory}\n";
        return infoString;
    }
}

[System.Serializable]
public class ModMetaData
{
    public string id;
    public string name;
    public string description;
    public string author;

    public string folderPath;

    public override string ToString()
    {
        string infoString = "";
        infoString += $"ID: {id}\n";
        infoString += $"Name: {name}\n";
        infoString += $"Description: {description}\n";
        infoString += $"Author: {author}\n";
        infoString += $"Folder Path: {folderPath}\n";
        return infoString;
    }
}

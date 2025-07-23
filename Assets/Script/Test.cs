using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XMLLoader loader = new XMLLoader();
        string directoryPath = $"{Application.streamingAssetsPath}/Mods/";
        XDocument doc = loader.LoadAll(directoryPath);

        Debug.Log("merged: \n" + doc.PrintAsString());

        loader.Patch(directoryPath, doc);

        Debug.Log("patched: \n" + doc.PrintAsString());

        string checkFilePath = $"{Application.dataPath}/Check.xml";
        File.WriteAllText(checkFilePath, doc.PrintAsString());
    }

    // Update is called once per frame
    void Update()
    {

    }
}

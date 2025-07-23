using System.IO;
using System.Xml.Linq;
using UnityEngine;
using ModdableArchitecture;
using ModdableArchitecture.Loaders;
using ModdableArchitecture.Utils;

public class Test : MonoBehaviour
{
    void Start()
    {
        // 1. Setup the logger and the core loader
        ModdableArchitecture.Utils.ILogger logger = new UnityDebugLogger();
        IXMLLoader loader = new XMLLoader(logger);

        // 2. Decorate the loader with high-level logging
        IXMLLoader decoratedLoader = new XMLLoaderLoggerDecorator(loader);

        string directoryPath = $"{Application.streamingAssetsPath}/Mods/";

        // 3. Load all XML definitions using the fully decorated loader
        XDocument doc = decoratedLoader.LoadAll(directoryPath);

        // 4. Apply all patches
        PatchService patchService = new PatchService();
        patchService.ApplyPatches(directoryPath, doc);
        Debug.Log("Patched Defs: \n" + doc.PrintAsString());

        // 5. Save the final result for verification
        string checkFilePath = $"{Application.dataPath}/Check.xml";
        File.WriteAllText(checkFilePath, doc.PrintAsString());
        Debug.Log($"Final XML saved to {checkFilePath}");
    }
}

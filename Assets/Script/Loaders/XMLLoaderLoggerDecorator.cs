using System.Xml.Linq;
using UnityEngine;

namespace ModdableArchitecture.Loaders
{
    public class XMLLoaderLoggerDecorator : IXMLLoader
    {
        private readonly IXMLLoader _loader;

        public XMLLoaderLoggerDecorator(IXMLLoader loader)
        {
            _loader = loader;
        }

        public XDocument LoadAll(string directoryPath)
        {
            Debug.Log("[XMLLoader] Start loading all XML files...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            XDocument result = _loader.LoadAll(directoryPath);

            stopwatch.Stop();
            Debug.Log($"[XMLLoader] Finished loading in {stopwatch.ElapsedMilliseconds}ms.");
            Debug.Log("Merged Defs: \n" + result.PrintAsString());

            return result;
        }
    }
}

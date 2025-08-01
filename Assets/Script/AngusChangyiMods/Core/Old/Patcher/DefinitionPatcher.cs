// using System.Collections.Generic;
// using System.IO;
// using System.Xml.Linq;
//
//
//
// namespace AngusChangyiMods.Core
// {
//     public class ModDefinitionPatcher
//     {
//         private readonly PatchOperationFactory factory = new PatchOperationFactory();
//
//         public void ApplyPatch(string filePath, XDocument xmlDocument)
//         {
//             XDocument patchDoc = XDocument.Load(filePath);
//             foreach (XElement operationElement in patchDoc.Root.Elements("Operation"))
//             {
//                 try
//                 {
//                     IPatchOperation operation = factory.CreateOperation(operationElement);
//                     operation.Apply(xmlDocument);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     ModLogger.LogError($"Failed to create patch operation from {filePath}: {ex.Message}");
//                 }
//             }
//         }
//     }
// }
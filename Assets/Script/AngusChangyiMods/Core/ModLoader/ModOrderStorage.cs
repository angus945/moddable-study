// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
//
// namespace AngusChangyiMods.Core
// {
//     public interface IModOrderStorage
//     {
//         void Save(string filePath, List<ModContentPack> sortedMods);
//         List<ModLoadingData> Load(string filePath);
//         List<ModContentPack> Apply(List<ModContentPack> mods, List<ModLoadingData> order);
//     }
//     
//     public class FileModOrderStorage : IModOrderStorage
//     {
//         public void Save(string filePath, List<ModContentPack> sortedMods)
//         {
//             var lines = sortedMods.Select(mod => ModLoadingData.Combain(mod.Meta.PackageId, mod.Meta.RootDirectory));
//             File.WriteAllLines(filePath, lines);
//             Console.WriteLine($"[ModOrderStorage] Saved to {filePath}");
//         }
//
//         public List<ModLoadingData> Load(string filePath)
//         {
//             var result = new List<ModLoadingData>();
//
//             if (!File.Exists(filePath))
//                 return result;
//
//             foreach (var line in File.ReadAllLines(filePath))
//             {
//                 try
//                 {
//                     result.Add(ModLoadingData.Parse(line));
//                 }
//                 catch (FormatException)
//                 {
//                     Console.WriteLine($"[ModOrderStorage] Skipped invalid line: {line}");
//                 }
//             }
//
//             return result;
//         }
//
//         public List<ModContentPack> Apply(List<ModContentPack> mods, List<ModLoadingData> order)
//         {
//             var sorted = new List<ModContentPack>();
//             var used = new HashSet<ModContentPack>();
//
//             foreach (var orderData in order)
//             {
//                 var match = mods.FirstOrDefault(mod => orderData.Matches(mod));
//                 if (match != null)
//                 {
//                     sorted.Add(match);
//                     used.Add(match);
//                 }
//                 else
//                 {
//                     Console.WriteLine($"[ModOrderStorage] Missing mod: {orderData.packageId} from {orderData.rootDirectory}");
//                 }
//             }
//
//             foreach (var mod in mods)
//             {
//                 if (!used.Contains(mod))
//                 {
//                     Console.WriteLine($"[ModOrderStorage] Unordered mod: {mod.Meta.PackageId} from {mod.Meta.RootDirectory}");
//                     sorted.Add(mod);
//                 }
//             }
//
//             return sorted;
//         }
//     }
//
// }
// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace AngusChangyiMods.Core
// {
//     public interface IModSorter
//     {
//         void SetMods(List<ModContentPack> contentPacks);
//         // void SetModOrder(ModOrder order);
//
//         // ModOrder GetModOrder { get; }
//     }
//
//     public class ModSorter : IModSorter
//     {
//         // private List<ModContentPack> sortedMods;
//         // private List<ModContentPack> sortedNonRepeat;
//         //
//         // public List<ModContentPack> SortedMods => sortedNonRepeat;
//         //
//         // public void SetModOrder(List<ModContentPack> unsortedMods)
//         // {
//         //     sortedMods = Sorts(unsortedMods);
//         //     sortedNonRepeat = RemoveRepetedModPackgeId(sortedMods);
//         // }
//         // List<ModContentPack> Sorts(List<ModContentPack> unsortedMods)
//         // {
//         //     Dictionary<string, ModContentPack> latest = new();
//         //     HashSet<string> warned = new();
//         //
//         //     for (int i = unsortedMods.Count - 1; i >= 0; i--)
//         //     {
//         //         var mod = unsortedMods[i];
//         //         if (!latest.ContainsKey(mod.Meta.PackageId))
//         //         {
//         //             latest[mod.Meta.PackageId] = mod;
//         //         }
//         //         else if (!warned.Contains(mod.Meta.PackageId))
//         //         {
//         //             Console.WriteLine($"[ModSorter] Duplicate PackageId '{mod.Meta.PackageId}' detected. Only the later one will be used.");
//         //             warned.Add(mod.Meta.PackageId);
//         //         }
//         //     }
//         //     
//         //     return unsortedMods
//         //         .Where(mod => latest.TryGetValue(mod.Meta.PackageId, out var chosen) && mod == chosen)
//         //         .ToList();
//         // }
//         //
//         // List<ModContentPack> RemoveRepetedModPackgeId(List<ModContentPack> sortedMods)
//         // {
//         //     List<ModContentPack> nonRepeatMods = new List<ModContentPack>(sortedMods);
//         //     Dictionary<string, ModContentPack> registeredIds = new Dictionary<string, ModContentPack>();
//         //
//         //     foreach (var mod in nonRepeatMods)
//         //     {
//         //         if (registeredIds.TryGetValue(mod.Meta.PackageId, out ModContentPack repeated))
//         //         {
//         //             nonRepeatMods.Remove(repeated);
//         //         }
//         //    
//         //         registeredIds[mod.Meta.PackageId] = mod;
//         //     }
//         //     
//         //     return nonRepeatMods;
//         // }
//
//         Dictionary<ModContentPack, ModLoadingData> LoadingDatas = new Dictionary<ModContentPack, ModLoadingData>();
//         public void SetMods(List<ModContentPack> contentPacks, List<ModLoadingData> loadingDatas )
//         {
//             LoadingDatas.Clear();
//             foreach (var contentPack in contentPacks)
//             {
//                 var loadingData = loadingDatas.FirstOrDefault(data => data.Matches(contentPack));
//                 if (loadingData != null)
//                 {
//                     LoadingDatas[contentPack] = loadingData;
//                 }
//                 else
//                 {
//                     Console.WriteLine($"[ModSorter] No loading data found for mod: {contentPack.Meta.PackageId}");
//                 }
//             }
//             
//         }
//     }
//
//
// }
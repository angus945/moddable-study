using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    public interface IModSorter
    {
        void LoadOrder(List<ModContentPack> loadMods, List<ModSortingData> loadSorts);
        void SetOrder(List<ModSortingData> setSorts);
        public List<ModSortingData> SortedMods { get; }
    }

    public class ModSorter : IModSorter
    {
        public List<ModSortingData> SortedMods { get; private set; }

        public void LoadOrder(List<ModContentPack> loadMods, List<ModSortingData> loadSorts)
        {
            List<ModContentPack> mods = new List<ModContentPack>(loadMods);
            List<ModSortingData> sorts = new List<ModSortingData>(loadSorts);

            foreach (ModSortingData sortData in sorts)
            {
                ModContentPack related = mods.Find(mod => ModSortingData.CheckRelation(sortData, mod));

                if (related != null)
                {
                    sortData.relatedContentPack = related;
                    mods.Remove(related);
                }
            }

            foreach (ModContentPack mod in mods)
            {
                ModSortingData sort = new ModSortingData(mod.Meta.PackageId, mod.Meta.RootDirectory);
                sort.relatedContentPack = mod;
                sorts.Add(sort);
            }

            SortedMods = sorts;
        }

        public void SetOrder(List<ModSortingData> setSorts)
        {
            SortedMods = setSorts;
        }
    }
}
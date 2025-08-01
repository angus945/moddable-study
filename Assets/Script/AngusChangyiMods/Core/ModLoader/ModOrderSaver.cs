using System;
using System.Collections.Generic;
using AngusChangyiMods.Core.SaveLoad;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core
{
    public interface IModOrderSaver
    {
        List<ModSortingData> LoadModOrder();
        void SaveModOrder(List<ModSortingData> sortedMods);
    }

    public class ModOrderSaver : IModOrderSaver
    {
        public const string errorLoadingModOrder = "Failed to load mod order";
        public const string errorSavingModOrder = "Failed to save mod order";
        
        [System.Serializable]
        public class ModOrder
        {
            public List<ModSortingData> sortlist;

            public ModOrder() { }
            public ModOrder(List<ModSortingData> sortedMods)
            {
                sortlist = sortedMods;
            }
        }

        private readonly IModDataSaver saver;
        private readonly ILogger logger;

        public ModOrderSaver(IModDataSaver saver, ILogger logger = null)
        {
            this.saver = saver;
            this.logger = logger;
        }

        public List<ModSortingData> LoadModOrder()
        {
            try
            {
                if (saver.TryRead<ModOrder>(out ModOrder modOrder))
                {
                    return modOrder.sortlist;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError($"{errorLoadingModOrder}: {ex.Message}");
            }

            return new List<ModSortingData>();
        }

        public void SaveModOrder(List<ModSortingData> sortedMods)
        {
            ModOrder order = new ModOrder(sortedMods);
            
            try
            {
                saver.Write(order);
            }
            catch (Exception ex)
            {
                logger?.LogError($"{errorSavingModOrder}: {ex.Message}");
            }
        }
    }
}
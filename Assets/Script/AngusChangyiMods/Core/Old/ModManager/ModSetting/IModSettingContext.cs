using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    public interface IModSettingContext
    {
        void ReadCollection<T>(string key, List<T> collection, List<T> defaultValue);
        void ReadValue<T>(string key, ref T value, T defaultValue);
        void WriteCollection<T>(string key, List<T> collection);
        void WriteValue<T>(string key, T value);
    }
}


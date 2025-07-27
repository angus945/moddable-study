using System.Collections.Generic;
using System.Xml.Linq;
using AngusChangyiMods;
using AngusChangyiMods.Core;

namespace Angus
{
    public class CoreSettings : IModeSettings
    {
        public bool enableFeature = false; // 範例布林設定
        public float someThreshold = 1.5f; // 範例浮點設定
        public List<string> nameList = new List<string>() { "Alice", "Bob", "Charlie" }; // 範例字串列表

        public void Load(IModSettingContext context)
        {
            context.ReadValue("enableFeature", ref enableFeature, false);
            context.ReadValue("someThreshold", ref someThreshold, 1.5f);
            context.ReadCollection("nameList", nameList, new List<string>());
        }
        public void Save(IModSettingContext context)
        {
            context.WriteValue("enableFeature", enableFeature);
            context.WriteValue("someThreshold", someThreshold);
            context.WriteCollection("nameList", nameList);
        }
    }
}

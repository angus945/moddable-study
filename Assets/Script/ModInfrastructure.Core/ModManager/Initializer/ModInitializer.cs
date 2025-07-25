
using System;
using System.Collections.Generic;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace ModArchitecture
{

    public class ModInstancer
    {
        readonly Dictionary<string, IModInstance> modInstances = new Dictionary<string, IModInstance>();


        public void InstanceMod()
        {
            var instanceTypes = ReflectionUtils.GetTypesAssignableFrom<IModInstance>();

            foreach (var type in instanceTypes)
            {
                var instance = ReflectionUtils.SafeCreateInstance<IModInstance>(type);
                if (instance != null)
                {
                    modInstances[type.FullName] = instance;
                    ModLogger.Log($"Registered mod instance: {type.FullName}");
                }
            }
        }
        public void InitializeMods()
        {
            foreach (var instance in modInstances.Values)
            {
                ModLogger.Log($"Initializing mod: {instance.GetType().FullName}");
                instance.Initialize();
            }
        }
        internal void StartGame()
        {
            foreach (var instance in modInstances.Values)
            {
                instance.OnGameStart();
            }
        }
    }
}
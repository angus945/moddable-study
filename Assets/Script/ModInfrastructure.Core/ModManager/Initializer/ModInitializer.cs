
using System;
using System.Collections.Generic;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace ModArchitecture
{

    public class ModInitializer
    {
        readonly Dictionary<string, IModInitializer> modInitializers = new Dictionary<string, IModInitializer>();


        public void RegisterInitializer()
        {
            var initializerTypes = ReflectionUtils.GetTypesAssignableFrom<IModInitializer>();

            foreach (var type in initializerTypes)
            {
                var initializer = ReflectionUtils.SafeCreateInstance<IModInitializer>(type);
                if (initializer != null)
                {
                    modInitializers[type.FullName] = initializer;
                    ModLogger.Log($"Registered mod initializer: {type.FullName}");
                }
            }
        }
        public void InitializeMods()
        {
            foreach (var initializer in modInitializers.Values)
            {
                ModLogger.Log($"Initializing mod: {initializer.GetType().FullName}");
                initializer.Initialize();
            }
        }


    }
}
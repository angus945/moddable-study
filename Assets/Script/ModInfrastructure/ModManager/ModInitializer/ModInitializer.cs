
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModArchitecture.Logger;

namespace ModArchitecture
{

    public class ModInitializer
    {
        readonly Dictionary<string, IModInitializer> modInitializers = new Dictionary<string, IModInitializer>();

        public void LoadModAssembly(string[] assemblies)
        {
            foreach (var assemblyPath in assemblies)
            {
                try
                {
                    Assembly.LoadFrom(assemblyPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load assembly from path: {assemblyPath}", ex);
                }
            }
        }
        public void RegisterInitializer()
        {
            var initializerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IModInitializer).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var type in initializerTypes)
            {
                var initializer = (IModInitializer)Activator.CreateInstance(type);
                modInitializers[type.FullName] = initializer;
                ModLogger.Log($"Registered mod initializer: {type.FullName}");
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
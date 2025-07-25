
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModArchitecture;
using UnityEngine;

public class ModInitializer
{
    private readonly Dictionary<string, IModInitializer> modInitializers = new Dictionary<string, IModInitializer>();

    internal void LoadModAssembly(string[] assemblies)
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
            Debug.Log($"Registered mod initializer: {type.FullName}");
        }
    }

    public void InitializeMods()
    {
        foreach (var initializer in modInitializers.Values)
        {
            Debug.Log($"Initializing mod: {initializer.GetType().FullName}");
            initializer.Initialize();
        }
    }


}
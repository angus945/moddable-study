
using System;
using System.Collections.Generic;
using System.Linq;
using ModArchitecture;

public class ModInitializer
{
    private readonly Dictionary<string, IModInitializer> modInitializers = new Dictionary<string, IModInitializer>();

    public ModInitializer()
    {
        RegisterInitializer();
    }
    void RegisterInitializer()
    {
        var initializerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IModInitializer).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        foreach (var type in initializerTypes)
        {
            var initializer = (IModInitializer)Activator.CreateInstance(type);
            modInitializers[type.FullName] = initializer;
        }
    }

    public void InitializeMods()
    {
        foreach (var initializer in modInitializers.Values)
        {
            initializer.Initialize();
        }
    }
}
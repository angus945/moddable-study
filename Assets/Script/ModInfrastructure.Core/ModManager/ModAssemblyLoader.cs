using System;
using System.Reflection;

public class ModAssemblyLoader
{
    AppDomain modDomain;

    public ModAssemblyLoader()
    {
        modDomain = AppDomain.CreateDomain("ModDomain");
    }

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
    public void UnloadAssembly(string[] assemblies)
    {
        // foreach (var assemblyPath in assemblies)
        // {
        //     try
        //     {
        //         Assembly.UnsafeLoadFrom(assemblyPath);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Failed to unload assembly from path: {assemblyPath}", ex);
        //     }
        // }
    }
}

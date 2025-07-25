using System;
using System.Reflection;

public class ModAssemblyLoader
{
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
}

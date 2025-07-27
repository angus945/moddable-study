using System;

namespace AngusChangyiMods.Core
{
    /// <summary>
    /// Attribute to mark static classes that should be instantiated after mod assembly loading
    /// Used to automatically trigger static constructors, typically for Harmony patch initialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StaticConstructorOnStartupAttribute : Attribute
    {
    }
}

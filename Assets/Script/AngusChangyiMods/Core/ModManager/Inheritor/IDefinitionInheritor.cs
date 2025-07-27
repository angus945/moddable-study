using System;
using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    /// <summary>
    /// Interface for definition inheritance processors.
    /// </summary>
    public interface IDefinitionInheritor
    {
        /// <summary>
        /// The type this inheritor handles.
        /// </summary>
        Type HandlesType { get; }

        /// <summary>
        /// Process inheritance for a collection of definitions.
        /// </summary>
        /// <param name="definitions">Definitions to process inheritance for</param>
        /// <returns>Processed definitions with inheritance applied</returns>
        IEnumerable<DefBase> ProcessInheritance(IEnumerable<DefBase> definitions);
    }
}

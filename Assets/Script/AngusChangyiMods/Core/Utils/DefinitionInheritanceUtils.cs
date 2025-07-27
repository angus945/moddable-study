using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace AngusChangyiMods.Core.Utils
{
    /// <summary>
    /// Utility class to simplify DefinitionInheritor implementation and provide reusable inheritance operations.
    /// </summary>
    public static class DefinitionInheritanceUtils
    {
        // Delegate definitions for better readability and type safety

        /// <summary>
        /// Delegate for custom inheritance logic between parent and child definitions.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="child">Child definition to inherit to</param>
        /// <param name="parent">Parent definition to inherit from</param>
        public delegate void InheritanceAction<T>(T child, T parent) where T : AngusChangyiMods.Core.DefBase;

        /// <summary>
        /// Delegate for getting a property value from a definition.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property value</typeparam>
        /// <param name="definition">Definition to get value from</param>
        /// <returns>Property value</returns>
        public delegate TValue PropertyGetter<T, TValue>(T definition);

        /// <summary>
        /// Delegate for setting a property value on a definition.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property value</typeparam>
        /// <param name="definition">Definition to set value on</param>
        /// <param name="value">Value to set</param>
        public delegate void PropertySetter<T, TValue>(T definition, TValue value);


        public static void InheritNumericProperty<T, TValue>(T child, T parent, PropertyGetter<T, TValue> getter, PropertySetter<T, TValue> setter)
            where TValue : struct, IComparable<TValue>
        {
            TValue parentValue = getter(parent);

            setter(child, parentValue);
        }

        /// <summary>
        /// Inherit a property using custom logic to determine if inheritance should occur.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the property value</param>
        /// <param name="setter">Action to set the property value</param>
        /// <param name="shouldInherit">Function to determine if inheritance should occur</param>
        // public static void InheritProperty<T, TValue>(T child, T parent, PropertyGetter<T, TValue> getter, PropertySetter<T, TValue> setter, InheritancePredicate<TValue> shouldInherit)
        // {
        //     var childValue = getter(child);
        //     var parentValue = getter(parent);

        //     if (shouldInherit(childValue, parentValue))
        //     {
        //         setter(child, parentValue);
        //     }
        // }

    }
}

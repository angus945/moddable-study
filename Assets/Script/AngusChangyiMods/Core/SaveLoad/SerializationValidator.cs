using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace AngusChangyiMods.Core.SaveLoad
{
    public static class SerializationValidator
    {
        public static void ValidateXmlSerializable(Type type)
        {
            if (!type.IsPublic && !type.IsNestedPublic)
            {
                throw new InvalidOperationException($"Type {type.FullName} must be public for XmlSerializer.");
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException(
                    $"Type {type.FullName} must have a public parameterless constructor.");
            }

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (!field.IsPublic && !field.IsDefined(typeof(XmlIgnoreAttribute), true)) continue;

                var fieldType = field.FieldType;
                if (!IsXmlSerializableType(fieldType))
                {
                    throw new InvalidOperationException(
                        $"Field {field.Name} in {type.FullName} is of type {fieldType.FullName} which may not be serializable. Add [XmlIgnore] if needed.");
                }
            }
        }

        private static bool IsXmlSerializableType(Type type)
        {
            if (type == typeof(string) || type.IsPrimitive)
                return true;

            if (type.IsArray)
                return IsXmlSerializableType(type.GetElementType());

            if (type.IsGenericType)
                return type.GetGenericArguments().All(IsXmlSerializableType);

            // 可序列化屬性或可建構的 public class（必要條件）
            return type.IsPublic &&
                   type.GetConstructor(Type.EmptyTypes) != null &&
                   type.GetFields().All(f => f.IsPublic || f.IsDefined(typeof(XmlIgnoreAttribute), true));
        }

    }
}
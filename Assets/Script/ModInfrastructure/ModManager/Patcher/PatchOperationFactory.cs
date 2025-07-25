using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace ModArchitecture
{
    public class PatchOperationFactory
    {
        public IPatchOperation CreateOperation(XElement operationElement)
        {
            string className = operationElement.Attribute("Class")?.Value;
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentException("Operation element must have a 'Class' attribute.");
            }

            // Search for the patch operation class in all loaded assemblies
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == className && typeof(IPatchOperation).IsAssignableFrom(t));

            if (type == null)
            {
                throw new ArgumentException($"Could not find a patch operation class named '{className}'.");
            }

            IPatchOperation operation = (IPatchOperation)Activator.CreateInstance(type);

            // Populate public fields from XML elements
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                XElement fieldElement = operationElement.Element(field.Name);
                if (fieldElement != null)
                {
                    // This is a simplified deserialization. 
                    // It handles string, XElement and bool. More complex types would need more logic.
                    if (field.FieldType == typeof(string))
                    {
                        field.SetValue(operation, fieldElement.Value);
                    }
                    else if (field.FieldType == typeof(XElement))
                    {
                        field.SetValue(operation, fieldElement.FirstNode as XElement);
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(operation, bool.Parse(fieldElement.Value));
                    }
                }
            }

            return operation;
        }
    }
}

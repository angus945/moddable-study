using System;
using System.IO;
using System.Xml.Serialization;
using Script.AngusChangyiMods.Core.Util;

namespace AngusChangyiMods.Core.SaveLoad
{
    public class ModDataSaver
    {
        public const string FileExtension = ".xml";
    }
    
    public interface IModDataSaver
    {
        public bool TryRead<T>(out T data) where T : class;
        public void Write<T>(T data) where T : class;
        public T Read<T>() where T : class;
        public bool Has<T>() where T : class;
        public void Delete<T>() where T : class;
    }
    
    public class FileModDataSaver : IModDataSaver
    {
        private readonly string saveDirectory;

        public FileModDataSaver(string saveDirectory)
        {
            this.saveDirectory = saveDirectory;
        }

        public void Write<T>(T data) where T : class
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(T), ModDataSaver.FileExtension);
            string fullPath = Path.Combine(saveDirectory, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, data);
            }
        }

        public T Read<T>() where T : class
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(T), ModDataSaver.FileExtension);
            string fullPath = Path.Combine(saveDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File for type {typeof(T).FullName} not found at {fullPath}");
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                object result = serializer.Deserialize(stream);
                return result as T;
            }
        }

        public bool TryRead<T>(out T data) where T : class
        {
            try
            {
                data = Read<T>();
                return true;
            }
            catch
            {
                data = null;
                return false;
            }
        }

        public bool Has<T>() where T : class
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(T), ModDataSaver.FileExtension);
            string fullPath = Path.Combine(saveDirectory, fileName);
            return File.Exists(fullPath);
        }

        public void Delete<T>() where T : class
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(T), ModDataSaver.FileExtension);
            string fullPath = Path.Combine(saveDirectory, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
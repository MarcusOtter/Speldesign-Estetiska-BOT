using System.IO;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace SpeldesignBotCore.Storage.Implementations
{
    public class JsonStorage : IDataStorage
    {
        public bool HasObject(string key) => File.Exists($"{key}.json");

        public T RestoreObject<T>(string key)
        {
            string json = string.Empty;

            try
            {
                json = File.ReadAllText($"{key}.json");
            }
            catch (FileNotFoundException)
            {
                throw new System.ArgumentException($"The provided key '{key}' could not be found.");
            }

            return DeserializeObject<T>(json);
        }

        public void StoreObject(object obj, string key)
        {
            var filePath = $"{key}.json";
            var directoryName = Path.GetDirectoryName(filePath);

            if (directoryName == string.Empty)
            {
                throw new System.ArgumentException("Key must be at least 1 directory deep");
            }

            Directory.CreateDirectory(directoryName);
            var json = SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
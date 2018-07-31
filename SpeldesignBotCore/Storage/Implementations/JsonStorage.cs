using System.IO;
using static System.IO.Directory;
using static Newtonsoft.Json.JsonConvert;

namespace SpeldesignBotCore.Storage.Implementations
{
    public class JsonStorage : IDataStorage
    {
        public T RestoreObject<T>(string key)
        {
            var json = File.ReadAllText($"{key}.json");
            return DeserializeObject<T>(json);
        }

        public void StoreObject(object obj, string key)
        {
            var filePath = $"{key}.json";
            CreateDirectory(Path.GetDirectoryName(filePath));
            var json = SerializeObject(obj);
            File.WriteAllText(filePath, json);
        }
    }
}
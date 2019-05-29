using SpeldesignBotCore.Storage;
using SpeldesignBotCore.Storage.Implementations;
using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class DataStorageTests
    {
        [Fact]
        public void DataStorage_StorageDefaultToJsonTest()
        {
            var storage = Unity.Resolve<IDataStorage>();
            Assert.True(storage is JsonStorage);
        }

        [Fact]
        public void DataStorage_InMemoryStorage_OverrideTest()
        {
            const string key = "Config/TestKey";
            const string firstString = "I should be overriden";
            const string expected = "I should override the previous data";

            IDataStorage storage = new InMemoryStorage();

            storage.StoreObject(firstString, key);
            storage.StoreObject(expected, key);

            var actual = storage.RestoreObject<string>(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DataStorage_InMemoryStorage_RestoreObject_MissingKeyTest()
        {
            IDataStorage storage = new InMemoryStorage();
            Assert.Throws<System.ArgumentException>(() => storage.RestoreObject<string>("fake-key"));
        }

        [Fact]
        public void DataStorage_JsonStorage_RestoreObject_MissingKeyTest()
        {
            IDataStorage storage = new JsonStorage();
            Assert.Throws<System.ArgumentException>(() => storage.RestoreObject<string>("fake-key"));
        }

        [Fact]
        public void DataStorage_JsonStorage_StoreObject_InvalidKeyTest()
        {
            const string key = "configurationFile";
            const string objectToStore = "some information";

            IDataStorage storage = new JsonStorage();
            Assert.Throws<System.ArgumentException>(() => storage.StoreObject(objectToStore, key));
        }

        [Fact]
        public void DataStorage_JsonStorage_OverrideTest()
        {
            const string key = "Config/TestKey";
            const string firstString = "I should be overriden";
            const string expected = "I should override the previous data";

            IDataStorage storage = new JsonStorage();

            storage.StoreObject(firstString, key);
            storage.StoreObject(expected, key);

            var actual = storage.RestoreObject<string>(key);

            Assert.Equal(expected, actual);
        }
    }
}

﻿namespace SpeldesignBotCore.Storage
{
    public interface IDataStorage
    {
        void StoreObject(object obj, string key);
        T RestoreObject<T>(string key);
        bool HasObject(string key);
    }
}

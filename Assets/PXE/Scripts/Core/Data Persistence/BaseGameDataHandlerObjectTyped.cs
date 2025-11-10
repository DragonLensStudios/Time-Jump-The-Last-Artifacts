using System.Collections.Generic;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence
{
    public abstract class BaseGameDataHandlerObjectTyped<T> : BaseGameDataHandlerObject, IGameDataHandlerTyped<T> where T : class, IGameDataContent, new()
    {
        [Tooltip("The data for the game.")]
        [field: SerializeField] public virtual List<T> Data { get; set; }

        public virtual void SaveGameData(SerializableGuid playerID, string playerName)
        {
            var gameDataObjects = FindAllGameDataObjects();
            var aggregatedData = new List<T>();

            foreach (var gameDataObj in gameDataObjects)
            {
                if (gameDataObj is IDataPersistable dataPersistableObj)
                {
                    // Create an empty instance of T, which should be a subtype of IGameDataContent
                    T gameDataInstance = new T();

                    // Save data from the gameDataObj to the gameDataInstance
                    dataPersistableObj.SaveData(gameDataInstance);

                    // Check if data was saved successfully
                    if (gameDataInstance != null)
                    {
                        aggregatedData.Add(gameDataInstance);
                    }
                }
            }

            if (aggregatedData.Count <= 0)
            {
                Debug.LogWarning("No data to save.");
                return;
            }

            DataHandler.Save<T>(aggregatedData, playerID, playerName);
        }
        
        public virtual void LoadGameData(SerializableGuid playerID, string playerName)
        {
            var gameDataObjectsInScene = FindAllGameDataObjects();
            List<T> loadedGameDataList = DataHandler.Load<T>(playerID, playerName);

            // Check if the lists have the same count
            if (gameDataObjectsInScene.Count != loadedGameDataList.Count)
            {
                Debug.LogWarning("Mismatch between game objects in the scene and loaded game data.");
                return;
            }

            for (int i = 0; i < loadedGameDataList.Count; i++)
            {
                if (gameDataObjectsInScene[i] is IDataPersistable dataPersistable)
                {
                    T gameData = loadedGameDataList[i];
                    if (gameData != null)
                    {
                        dataPersistable.LoadData(gameData);
                    }
                    else
                    {
                        Debug.LogWarning($"No game data found for object at index {i}.");
                    }
                }
            }
        }

        public (SerializableGuid playerID, T gameData) GetMostRecentlyUpdatedPlayer<T>() where T : class, IGameDataContent, new()
        {
            return DataHandler.GetMostRecentlyUpdatedPlayer<T>();
        }

        Dictionary<SerializableGuid, List<T>> IGameDataHandler.LoadAllProfiles<T>()
        {
            return DataHandler.LoadAllProfiles<T>();
        }

        public virtual void Save(T data, SerializableGuid playerID, string playerName)
        {
            List<T> dataList = new List<T> { data };
            DataHandler.Save(dataList, playerID, playerName);
        }

        public virtual List<T> Load(SerializableGuid playerID, string playerName)
        {
            return DataHandler.Load<T>(playerID, playerName);
        }

        public virtual Dictionary<SerializableGuid, List<T>> LoadAllProfiles()
        {
            return DataHandler.LoadAllProfiles<T>();
        }
    }
}
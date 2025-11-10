using System;
using System.Collections.Generic;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Objects;
using PXE.Core.ScriptableObjects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence
{
    public abstract class BaseGameDataHandlerObject : ScriptableObjectController
    {
        [Tooltip("The path to the folder where the data will be saved.")]
        [field: SerializeField] public virtual string DataPath { get; set; }
        
        [Tooltip("The extension for the data file.")]
        [field: SerializeField] public virtual string DataFileName { get; set; } = "Save";
        
        [Tooltip("The extension for the data file.")]
        [field: SerializeField] public virtual string DataExtension { get; set; } = ".save";
        
        [Tooltip("Whether or not to encrypt the data.")]
        [field: SerializeField] public virtual bool UseEncryption { get; set; } = false;

        [Tooltip("The file data handler for the game.")]
        public virtual IFileDataHandler DataHandler { get; set; }

        public virtual void DeleteGameData(SerializableGuid playerID, string playerName)
        {
            DataHandler.Delete(playerID, playerName);
        }

        public virtual void SetDataHandler(string path, string fileName, string extension, bool useEncryption)
        {
            DataHandler = new FileDataHandler(path, fileName, extension, useEncryption);
        }

        public virtual void Delete(SerializableGuid playerID, string playerName)
        {
            DataHandler.Delete(playerID, playerName);
        }

        public virtual List<IDataPersistable> FindAllGameDataObjects()
        {
            var gameDataObjects = new List<IDataPersistable>();
            var gameDataObjectsInScene = FindObjectsOfType<ObjectController>(true);
            foreach (var gameDataObj in gameDataObjectsInScene)
            {
                var gameDataObjType = gameDataObj.GetType();
                if (IsImplementationOfType(gameDataObjType))
                {
                    gameDataObjects.Add(gameDataObj as IDataPersistable);
                }
            }

            return gameDataObjects;
        }

        public virtual bool IsImplementationOfType(Type type)
        {
            return typeof(IDataPersistable).IsAssignableFrom(type);
        }
    }
}
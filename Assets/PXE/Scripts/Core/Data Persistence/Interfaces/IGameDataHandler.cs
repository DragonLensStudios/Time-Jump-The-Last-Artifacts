using System.Collections.Generic;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IGameDataHandler
    {
        IFileDataHandler DataHandler { get; set; }
        void SaveGameData(SerializableGuid playerID, string playerName);
        void LoadGameData(SerializableGuid playerID, string playerName);
        void DeleteGameData(SerializableGuid playerID, string playerName);
        (SerializableGuid playerID, T gameData) GetMostRecentlyUpdatedPlayer<T>() where T : class, IGameDataContent, new();
        Dictionary<SerializableGuid, List<T>> LoadAllProfiles<T>() where T : class, IGameDataContent, new();
    }
}
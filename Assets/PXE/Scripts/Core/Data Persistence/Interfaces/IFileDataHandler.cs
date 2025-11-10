using System.Collections.Generic;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IFileDataHandler
    {
        string DataPath { get; set; }
        string FileName { get; set; }
        string Extension { get; set; }
        bool UseEncryption { get; set; }
        string EncryptionCodeWord { get; set; }
        string BackupExtension { get; set; }
        
        void Initialize();
        void Initialize(string path, string fileName, string extension, bool useEncryption, string encryptionCodeWord, string backupExtension);
        
        void Save<T>(List<T> data, SerializableGuid playerID, string playerName) where T : class, IGameDataContent, new();
        List<T> Load<T>(SerializableGuid playerID, string playerName, bool allowRestoreFromBackup = true) where T : class, IGameDataContent, new();
        void Delete(SerializableGuid playerID, string playerName);
        (SerializableGuid playerID, T gameData) GetMostRecentlyUpdatedPlayer<T>() where T : class, IGameDataContent, new();
        Dictionary<SerializableGuid, List<T>> LoadAllProfiles<T>() where T : class, IGameDataContent, new();
    }
}
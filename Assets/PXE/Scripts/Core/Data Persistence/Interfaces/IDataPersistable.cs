namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IDataPersistable
    {
        void LoadData<T>(T loadedGameData) where T : class, IGameDataContent, new();
        
        void SaveData<T>(T savedGameData) where T : class, IGameDataContent, new();
    }
}
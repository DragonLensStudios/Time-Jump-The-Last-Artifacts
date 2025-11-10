namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IGameDataHandlerObject
    {
        IFileDataHandler DataHandler { get; set; }
        
        void SetDataHandler(string path, string fileName, string extension, bool useEncryption);
    }
}
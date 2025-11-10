using System.Collections.Generic;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IGameDataHandlerTyped<T> : IGameDataHandler where T : class, IGameDataContent, new()
    {
        List<T> Data { get; set; }
        void Save(T data, SerializableGuid playerID, string playerName);
        List<T> Load(SerializableGuid playerID, string playerName);
    }
}
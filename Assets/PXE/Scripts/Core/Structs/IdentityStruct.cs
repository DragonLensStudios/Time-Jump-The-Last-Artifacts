using System.Collections.Generic;
using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Structs
{
    public struct IdentityStruct : IID
    {
        public string Name { get; set; }
        public SerializableGuid ID { get; set; }
        public bool IsManualID { get; set; }

        public IEnumerable<T> GetExistingIDs<T>() where T : IID
        {
            // For your Identity struct loop over your list of structs here:
            throw new System.NotImplementedException();
        }

        public IdentityStruct(string name, SerializableGuid id, bool isManualID)
        {
            Name = name;
            ID = id;
            IsManualID = isManualID;
        }
    }
}
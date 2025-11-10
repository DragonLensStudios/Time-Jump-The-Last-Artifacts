using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Interfaces
{
    public interface IObjectIdentity : IID
    {
        IEnumerable<T> GetExistingIDs<T>() where T: IID; 
        bool IsExistingID<T>(SerializableGuid id) where T: IID => IsExistingID(id, GetExistingIDs<T>());
        bool IsExistingID<T>() where T: IID => IsExistingID<T>(ID);
        bool IsExistingID<T>(SerializableGuid id, IEnumerable<T> objs) where T: IID => objs.Any(o => o.ID.Equals(id));
        bool IsExistingID<T>(SerializableGuid id, T[] objs) where T: IID => objs.Any(o => o.ID.Equals(id));
        bool IsExistingID<T>(string id) where T: IID => IsExistingID<T>(new SerializableGuid(Guid.Parse(id)));
    }
}
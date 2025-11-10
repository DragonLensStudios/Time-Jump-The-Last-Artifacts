using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PXE.Core.Utilities.GameObject
{
    public static class GameObjectUtilities
    {
        public static Dictionary<Guid, UnityEngine.GameObject> GetGameObjectsWithGameObjectID(FindObjectsInactive includeActive = FindObjectsInactive.Include)
        {
            var gos = Object.FindObjectsByType<UnityEngine.GameObject>(includeActive, FindObjectsSortMode.None);

            return gos
                .Select(g => new { GameObject = g, IID = g.GetComponent<IGameObjectIdentity>() })
                .Where(item => item.IID != null && item.IID.ID.Guid != Guid.Empty)
                .GroupBy(item => item.IID.ID.Guid)
                .ToDictionary(group => group.Key, group => group.First().GameObject);
        }

        public static UnityEngine.GameObject GetGameObjectByID(SerializableGuid id, FindObjectsInactive includeActive = FindObjectsInactive.Include)
        {
            var gos = GetGameObjectsWithGameObjectID(includeActive);
            return gos.TryGetValue(id.Guid, out var obj) ? obj : null;
        }
    }
}
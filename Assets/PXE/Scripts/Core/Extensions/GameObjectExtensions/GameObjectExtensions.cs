using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Extensions.GameObjectExtensions
{
    public static class GameObjectExtensions
    {
        // Helper method to find the root parent object
        public static UnityEngine.GameObject FindRootParent(this UnityEngine.GameObject obj)
        {
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
            }
            return obj;
        }
        
        public static SerializableGuid GetObjectParentID(this UnityEngine.GameObject go)
        {
            return go.transform.parent != null ? go.transform.parent.gameObject.GetObjectID() : SerializableGuid.Empty;
        }
        
        public static bool SetObjectActive(this UnityEngine.GameObject go, bool active)
        {
            var oc = go.GetIGameObjectController();
            if (oc != null)
            {
                oc.SetObjectActive(active);
            }
            else
            {
                go.SetActive(active);
            }

            return active;
        }

        public static bool IsObjectActive(this UnityEngine.GameObject go)
        {
            var oc = go.GetIGameObjectController();
            return oc != null ? oc.IsActive : go.activeSelf;
        }
        
        public static SerializableGuid GetObjectID(this UnityEngine.GameObject go)
        {
            if (go == null) return SerializableGuid.Empty;
            var iObject = go.GetComponent<IObject>();
            if(iObject == null) return SerializableGuid.Empty;
            if (SerializableGuid.IsEmpty(iObject.ID))
            {
                return iObject.ID = SerializableGuid.Empty;
            }
            return iObject.ID;
        }
        
        public static string GetObjectName(this UnityEngine.GameObject go)
        {
            if (go == null) return null;
            var iObject = go.GetComponent<IObject>();
            return iObject != null ? iObject.Name : go.name;
        }

        public static SerializableGuid SetObjectID(this UnityEngine.GameObject go, SerializableGuid instanceID)
        {
            if (go == null) return null;
            var iObject = go.GetComponent<IObject>();
            return iObject != null ? iObject.ID = instanceID : SerializableGuid.Empty;
        }
        
        public static string SetObjectName(this UnityEngine.GameObject go, string name)
        {
            if (go == null) return null;
            var iObject = go.GetComponent<IObject>();
            return iObject != null ? iObject.Name = name : go.name = name;
        }

        public static IGameObject GetIGameObjectController(this UnityEngine.GameObject go)
        {
            return go.GetComponent<IGameObject>();
        }

#if UNITY_EDITOR
        //TODO: Add handling for runtime checking for if is a prefab.
        public static bool IsPrefab(this UnityEngine.GameObject go)
        {
            return  UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(go) == null && UnityEditor.PrefabUtility.GetPrefabInstanceHandle(go) != null;
        }

        public static bool IsPartOfAnyPrefab(this UnityEngine.GameObject go)
        {
            return UnityEditor.PrefabUtility.IsPartOfAnyPrefab(go);
        }
#endif


    }
}
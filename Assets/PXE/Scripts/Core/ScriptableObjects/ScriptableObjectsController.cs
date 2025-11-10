using System.Collections.Generic;
using System.IO;
using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.ScriptableObjects
{
    public class ScriptableObjectController : ScriptableObject, IObject
    {
        [field: SerializeField] public virtual string Name { get; set; }
        [field: SerializeField] public virtual SerializableGuid ID { get; set; }
        [field: SerializeField] public virtual bool IsManualID { get; set; } = false;


        public void OnValidate()
        {
            if(ID == null || SerializableGuid.IsEmpty(ID))
            {
                ID = SerializableGuid.CreateNew;
            }
            
        }

        public virtual void SetScriptableObjectName()
        {
#if UNITY_EDITOR
            string soPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = Path.GetFileName(soPath);
            }
            UnityEditor.AssetDatabase.RenameAsset(soPath, Name);
#endif
            if(string.IsNullOrWhiteSpace(Name))
            {
                //TODO: Find a way to read asset filename at runtime.
                Name = SerializableGuid.CreateNew.Guid.ToString();
            }
        }
        
        public static void UpdateIdentity(ScriptableObjectController soc)
        {
            if (SerializableGuid.IsEmpty(soc.ID))
            {
                soc.ID = SerializableGuid.CreateNew;
            }
                
            if(string.IsNullOrWhiteSpace(soc.Name))
            {
                soc.SetScriptableObjectName();
            }
        }
        
        public static void UpdateAllIdentities()
        {
            //TODO: handle scriptable objects that are not a resource folder.
            var socs = Resources.FindObjectsOfTypeAll<ScriptableObjectController>();
            foreach (var soc in socs)
            {
                UpdateIdentity(soc);
            }
        }
        
        public IEnumerable<T> GetExistingIDs<T>() where T : IID
        {
            //TODO: handle scriptable objects that are not a resource folder.
            return Resources.FindObjectsOfTypeAll<ScriptableObjectController>() as IEnumerable<T>;
        }

    }
}
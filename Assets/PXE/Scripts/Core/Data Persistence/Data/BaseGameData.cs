using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Achievements.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Interfaces;
using PXE.Core.Inventory.Data;
using PXE.Core.SerializableTypes;
using PXE.Core.Time.Data;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Data
{
    public class BaseGameData : IGameDataContent
    {
        [field: SerializeField] public virtual string Name { get; set; }
        [field: SerializeField] public virtual SerializableGuid ID { get; set; }
        [field: SerializeField] public virtual bool IsManualID  { get; set; }
        [field: SerializeField] public virtual float MoveSpeed { get; set; } = 0f;
        [field: SerializeField] public virtual Vector3 Position { get; set; } = Vector3.zero;
        [field: SerializeField] public virtual Vector2 MovementDirection { get; set; } = Vector2.zero;
        [field: SerializeField] public virtual string ReferenceState { get; set; } = "";
        [field: SerializeField] public virtual string CurrentLevelName { get; set; } = "";
        [field: SerializeField] public virtual SerializableGuid CurrentLevelID { get; set; } = new(Guid.Empty);
        [field: SerializeField] public virtual DateTime LastUpdated { get; set; } = DateTime.Now;
        [field: SerializeField] public virtual List<PlayerAchievementProgress> Achievements { get; set; } = new();
        [field: SerializeField] public virtual InventoryData Inventory { get; set; } = new();
        [field: SerializeField] public virtual TimeData CurrentTime { get; set; } = new();

        public BaseGameData()
        {
            
        }
        
        public IEnumerable<T> GetExistingIDs<T>() where T : IID
        {
            var baseGameDataHandlersObjects = Resources.FindObjectsOfTypeAll<BaseGameDataHandlerObject>();
            IEnumerable<T> uniqueDatas = new List<T>();
            foreach (var bgd in baseGameDataHandlersObjects)
            {
                var profiles = bgd.DataHandler.LoadAllProfiles<BaseGameData>();
                uniqueDatas = uniqueDatas.Union(profiles.Values.SelectMany(p => p).OfType<T>());
            }

            return uniqueDatas;
        }


    }
}
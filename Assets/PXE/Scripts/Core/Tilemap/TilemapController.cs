using PXE.Core.Interfaces;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Tilemap
{
    /// <summary>
    ///  Represents the TilemapController.
    /// </summary>
    public class TilemapController : ObjectController
    {
        [field: Tooltip("The Tilemaps")]
        [field: SerializeField] public SerializableDictionary<SerializableGuid, UnityEngine.Tilemaps.Tilemap> Tilemaps { get; set; } = new();
        
        [field: Tooltip("Whether to force get the Tilemaps")]
        [field: SerializeField] public bool ForceGetTilemaps { get; set; } = false;

        
        /// <summary>
        ///  Handles the start event and gets the Tilemaps from the tilemaps in children.
        /// </summary>
        public override void Start()
        {
            base.Start();
            if (Tilemaps.Count > 0 && !ForceGetTilemaps) return;
            GetTilemaps();
        }
        
        /// <summary>
        ///  Gets the Tilemaps from the tilemaps in children.
        /// </summary>
        public virtual void GetTilemaps()
        {
            Tilemaps.Clear();
            foreach (var tilemap in GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>())
            {
                var iObject = tilemap.GetComponent<IObject>();
                if (iObject != null)
                {
                    if (!Tilemaps.TryAdd(iObject.ID, tilemap))
                    {
                        Debug.LogWarning("TilemapController: Tilemap with ID " + iObject.ID + " already exists."); 
                    }
                }
            }
        }
    }
}

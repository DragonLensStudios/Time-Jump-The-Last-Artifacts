using PXE.Core.Inventory.Items;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Inventory.Managers
{
    public class ItemDatabaseManager : ObjectController
    {
        public static ItemDatabaseManager Instance { get; private set; }
        
        [field: Tooltip("The item database object.")]
        [field: SerializeField] public ItemDatabaseObject ItemDatabase { get; set; }

        /// <summary>
        /// Ensures that only one instance of ItemDatabaseManager exists in the scene.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }
    }
}
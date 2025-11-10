using System.Linq;
using PXE.Core.ScriptableObjects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    /// <summary>
    ///  This class represents the item database.
    /// </summary>
    [CreateAssetMenu(fileName ="Item Database", menuName ="PXE/Game/Items/Item Database", order = 3)]
    public class ItemDatabaseObject : ScriptableObjectController
    {
        
        [field: Tooltip("The items in the database.")]
        [field: SerializeField] public virtual SerializableDictionary<SerializableGuid, ItemObject> Items { get; set; } = new();

        
        /// <summary>
        ///  When the object is enabled, load the items.
        /// </summary>
        public virtual void OnEnable()
        {
            if (Items.Count == 0)
            {
                LoadItems();
            }
        }
        
        /// <summary>
        /// This method sets up <see cref="Items"/> dictionary from the loaded items in resources folder.
        /// </summary>
        public virtual void LoadItems()
        {
            Items.Clear();
            var itemObjects = Resources.LoadAll<ItemObject>("Items").ToList();
            for (int i = 0; i < itemObjects.Count; i++)
            {
                var item = itemObjects[i];
                if (item != null)
                {
                    Items.TryAdd(item.ID, item);
                }
            }
        }
    }
}
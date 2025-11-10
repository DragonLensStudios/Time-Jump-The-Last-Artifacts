using PXE.Core.Enums;
using PXE.Core.Inventory.Interfaces;
using UnityEngine;

namespace PXE.Core.Inventory.Items
{
    [CreateAssetMenu(fileName ="Basic Equipment", menuName ="PXE/Game/Items/Basic Equipment", order = 2)]
    public class EquipmentObject : ItemObject, IEquipmentObject
    {
        [field: Tooltip("The equipment slot for the item." )]
        [field: SerializeField] public virtual EquipmentSlot Slot { get; set; }
        
        [field: Tooltip("is the item equipable by all?")]
        [field: SerializeField] public virtual bool EquipableByAll { get; set; } = true;
        // [field: SerializeField] public List<CharacterClassObject> AllowedCharacterClasses { get; set; } = new ();
        
    }
}
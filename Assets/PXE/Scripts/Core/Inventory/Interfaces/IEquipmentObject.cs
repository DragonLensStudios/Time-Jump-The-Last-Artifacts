using PXE.Core.Enums;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IEquipmentObject : IItemObject
    {
        EquipmentSlot Slot { get; set; }
        bool EquipableByAll { get; set; }
    }
}
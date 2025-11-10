using PXE.Core.Interfaces;
using PXE.Core.Inventory.Items;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IItemSlot : IID
    {
        ItemObject Item { get; set; }
        int Quantity  { get; set; }
    }
}
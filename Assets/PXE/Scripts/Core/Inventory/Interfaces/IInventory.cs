using PXE.Core.Inventory.Items;

namespace PXE.Core.Inventory.Interfaces
{
    public interface IInventory 
    {
        /// <summary>
        /// The Inventory for the actor.
        /// </summary>
        InventoryObject Inventory { get; set; }
    }
}
// using PXE.Core.Objects;
// using PXE.Inventory.Items;
// using UnityEngine.InputSystem;
//
// namespace PXE._TESTING
// {
//     public class CurrencyTesting : ObjectController
//     {
//         public InventoryObject PlayerInventory;
//         public InventoryObject VendorInventory;
//         public long CopperValue;
//         public InputAction AddCopperInput;
//         public InputAction SubtractCopperInput;
//         public InputAction SetCoppperInput;
//     
//         public override void OnActive()
//         {
//             base.OnActive();
//             AddCopperInput.Enable();
//             SubtractCopperInput.Enable();
//             SetCoppperInput.Enable();
//             AddCopperInput.performed += AddCopper;
//             SubtractCopperInput.performed += SubtractCopper;
//             SetCoppperInput.performed += SetCopper;
//         }
//
//         private void SetCopper(InputAction.CallbackContext obj)
//         {
//             PlayerInventory.Currency.SetCopper(CopperValue);
//         }
//
//         private void SubtractCopper(InputAction.CallbackContext obj)
//         {
//             PlayerInventory.Currency.SubtractCopperValue(CopperValue);
//         }
//
//         private void AddCopper(InputAction.CallbackContext obj)
//         {
//             PlayerInventory.Currency.AddCopperValue(CopperValue);
//         }
//
//         public override void OnInactive()
//         {
//             base.OnInactive();
//             AddCopperInput.Disable();
//             SubtractCopperInput.Disable();
//             SetCoppperInput.Disable();
//             AddCopperInput.performed -= AddCopper;
//             SubtractCopperInput.performed -= SubtractCopper;
//             SetCoppperInput.performed -= SetCopper;
//         }
//     
//     }
// }

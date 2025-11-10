using PXE.Core.Inventory.Items;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Inventory.Editor
{
    [CustomEditor(typeof(InventoryObject))]
    public class InventoryObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset Currencies"))
            {
                var inventory = (InventoryObject)target;
                if (inventory != null)
                {
                    inventory.Currency.SetUpCurrency();
                }
            }
            if(GUILayout.Button("Reset Item Slots"))
            {
                var inventory = (InventoryObject)target;
                if (inventory != null)
                {
                    inventory.Initialize();
                }
            }
        }
    }
}
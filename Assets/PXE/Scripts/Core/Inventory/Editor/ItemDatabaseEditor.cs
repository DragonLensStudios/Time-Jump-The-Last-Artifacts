using PXE.Core.Inventory.Items;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Inventory.Editor
{
    [CustomEditor(typeof(ItemDatabaseObject))]
    public class ItemDatabaseEditor : UnityEditor.Editor
    {
        private ItemDatabaseObject _itemDatabaseObject;

        private void Awake()
        {
            _itemDatabaseObject = (ItemDatabaseObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load Items"))
            {
                _itemDatabaseObject.LoadItems();;
            }
        }
    }
}
using System.IO;
using System.Linq;
using PXE.Core.Inventory.Items;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Inventory.Editor
{
    [CustomEditor(typeof(ItemObject))]
    public class ItemObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Reset Currencies"))
            {
                var item = (ItemObject)target;
                if (item != null)
                {
                    item.Cost.SetUpCurrency();
                    item.SellValue.SetUpCurrency();
                    item.BuyValue.SetUpCurrency();
                }
            }
            if (GUILayout.Button("Set Item Icon Path"))
            {
                var item = (ItemObject)target;
                if (item != null && item.ItemIcon != null)
                {
                    var fullItemPath = AssetDatabase.GetAssetPath(item.ItemIcon);

                    // Extracting the relative path after "Resources/"
                    int resourcesIndex = fullItemPath.LastIndexOf("Resources/");
                    if (resourcesIndex >= 0)
                    {
                        string relativePath = fullItemPath.Substring(resourcesIndex + 10); // 10 = "Resources/".Length

                        var filePathNoExtension = Path.GetFileNameWithoutExtension(relativePath);
                        var filePath = Path.Combine(Path.GetDirectoryName(relativePath), filePathNoExtension);
                        var sprites = Resources.LoadAll<Sprite>(filePath).ToList();

                        // If there's more than one sprite in the spritesheet, use the sprite's name
                        item.ItemSpritesheetPath = filePath;
                        if (sprites.Count > 1)
                        {
                            item.ItemIconPath = $"{filePath}.{item.ItemIcon.name}";
                        }
                        else
                        {
                            item.ItemIconPath = filePath;
                        }
                    }
                }
            }
        }
    }
}
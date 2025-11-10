using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Crafting
{
    [CreateAssetMenu(fileName ="New Crafting Recipe", menuName ="PXE/Game/Crafting/Crafting Recipe", order = 2)]
    public class CraftingRecipeObject : ScriptableObjectController
    {
        
        [field: Tooltip("The description of the crafting recipe.")]
        [field: SerializeField] public virtual string Description { get; set; }
        
        [field: Tooltip("The icon of the crafting recipe.")]
        [field: SerializeField] public virtual Sprite Icon { get; set; }
        
        [field: Tooltip("The amount of time it takes to craft the recipe.")]
        [field: SerializeField] public virtual int Amount { get; set; }
        
        [field: Tooltip("The amount of time it takes to craft the recipe.")]
        [field: SerializeField] public virtual float CraftingTime { get; set; }
        
        [field: Tooltip("if true, the recipe can be crafted.")]
        [field: SerializeField] public virtual bool IsCraftable { get; set; }
        
        [field: Tooltip("if true, the recipe is unlocked.")]
        [field: SerializeField] public virtual bool IsUnlocked { get; set; }
        
        [field: Tooltip("if true, the recipe is unlocked by default.")]
        [field: SerializeField] public virtual bool IsUnlockedByDefault { get; set; }
        
        [field: Tooltip("The crafting station required to craft the recipe.")]
        [field: SerializeField] public virtual List<ItemSlot> RequiredItems { get; set; }
        
        [field: Tooltip("The result items of the recipe.")]
        [field: SerializeField] public virtual List<ItemSlot> ResultItems { get; set; }
        
        [field: Tooltip("The required recipes to craft the recipe.")]
        [field: SerializeField] public virtual List<CraftingRecipeObject> RequiredRecipes { get; set; }
        
        [field: Tooltip("The result recipes of the recipe.")]
        [field: SerializeField] public virtual List<CraftingRecipeObject> ResultRecipes { get; set; }
        
        [field: Tooltip("The crafting station required to craft the recipe.")]
        [field: SerializeField] public virtual RequiredCraftingStations RequiredCraftingStations { get; set; }
        
        /// <summary>
        ///  Check if crafting station is available.
        /// </summary>
        /// <param name="craftingStation"></param>
        /// <returns></returns>
        public virtual bool IsCraftingStationAvailable(RequiredCraftingStations craftingStation)
        {
            return RequiredCraftingStations.HasFlag(craftingStation);
        }
        
        /// <summary>
        ///  Check if the recipe is available.
        /// </summary>
        public virtual bool IsAvailable
        {
            get
            {
                if (IsUnlocked)
                {
                    return true;
                }

                if (IsUnlockedByDefault)
                {
                    return true;
                }

                foreach (var recipe in RequiredRecipes)
                {
                    if (!recipe.IsUnlocked)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        /// <summary>
        ///  Unlock the recipe.
        /// </summary>
        public virtual void Unlock()
        {
            IsUnlocked = true;
        }
        
        /// <summary>
        /// Lock the recipe.
        /// </summary>
        public virtual void Lock()
        {
            IsUnlocked = false;
        }
        
        
    }
}
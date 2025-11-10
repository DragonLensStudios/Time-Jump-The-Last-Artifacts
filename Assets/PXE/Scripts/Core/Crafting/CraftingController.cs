using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Crafting
{
    
    public class CraftingController : ObjectController
    {
        [field: Tooltip( "The current crafting stations available to the player")]
        [field: SerializeField] public virtual RequiredCraftingStations CurrentCraftingStations { get; set; }
        
        [field: Tooltip("crafting recipes available to the player")]
        [field: SerializeField] public virtual CraftingRecipeObject[] Recipes { get; set; }
        
        [field: Tooltip( "The current inventory")]
        [field: SerializeField] public virtual InventoryObject Inventory { get; set; }
        
        [field: Tooltip( "The current crafting recipe")]
        [field: SerializeField] public virtual CraftingRecipeObject CurrentRecipe { get; set; }
        
        /// <summary>
        ///  Try to craft the current recipe.
        /// </summary>
        /// <returns>True if crafted</returns>
        public virtual bool TryCraftItem()
        {
            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!CurrentRecipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(CurrentRecipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!Inventory.ContainsItems(CurrentRecipe.RequiredItems))
            {
                return false;
            }

            Inventory.RemoveItems(CurrentRecipe.RequiredItems);
            Inventory.AddItems(CurrentRecipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft the current recipe using the specified inventory.
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(InventoryObject inventory)
        {
            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!CurrentRecipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(CurrentRecipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!inventory.ContainsItems(CurrentRecipe.RequiredItems))
            {
                return false;
            }

            inventory.RemoveItems(CurrentRecipe.RequiredItems);
            inventory.AddItems(CurrentRecipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft the specified recipe.
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(CraftingRecipeObject recipe)
        {
            if (recipe == null)
            {
                return false;
            }

            if (!recipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(recipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!Inventory.ContainsItems(recipe.RequiredItems))
            {
                return false;
            }

            Inventory.RemoveItems(recipe.RequiredItems);
            Inventory.AddItems(recipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft item using required items.
        /// </summary>
        /// <param name="requiredItems"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(ItemSlot[] requiredItems)
        {
            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!CurrentRecipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(CurrentRecipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!Inventory.ContainsItems(requiredItems.ToList()))
            {
                return false;
            }

            Inventory.RemoveItems(requiredItems.ToList());
            Inventory.AddItems(CurrentRecipe.ResultItems);
            return true;
        }
        
        /// <summary>
        /// Try to craft item using specified recipe and specified inventory.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(CraftingRecipeObject recipe, InventoryObject inventory)
        {
            if (recipe == null)
            {
                return false;
            }

            if (!recipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(recipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!inventory.ContainsItems(recipe.RequiredItems))
            {
                return false;
            }

            inventory.RemoveItems(recipe.RequiredItems);
            inventory.AddItems(recipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft item using required items and specified inventory.
        /// </summary>
        /// <param name="requiredItems"></param>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(ItemSlot[] requiredItems, InventoryObject inventory)
        {
            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!CurrentRecipe.IsAvailable)
            {
                return false;
            }
            
            if(!CurrentCraftingStations.HasFlag(CurrentRecipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!inventory.ContainsItems(requiredItems.ToList()))
            {
                return false;
            }

            inventory.RemoveItems(requiredItems.ToList());
            inventory.AddItems(CurrentRecipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft item using specified recipe and specified inventory and specified crafting stations.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="inventory"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(CraftingRecipeObject recipe, InventoryObject inventory, RequiredCraftingStations craftingStations)
        {
            if (recipe == null)
            {
                return false;
            }

            if (!recipe.IsAvailable)
            {
                return false;
            }
            
            if(!craftingStations.HasFlag(recipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!inventory.ContainsItems(recipe.RequiredItems))
            {
                return false;
            }

            inventory.RemoveItems(recipe.RequiredItems);
            inventory.AddItems(recipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft item using required items and specified inventory and specified crafting stations.
        /// </summary>
        /// <param name="requiredItems"></param>
        /// <param name="inventory"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(ItemSlot[] requiredItems, InventoryObject inventory, RequiredCraftingStations craftingStations)
        {
            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!CurrentRecipe.IsAvailable)
            {
                return false;
            }
            
            if(!craftingStations.HasFlag(CurrentRecipe.RequiredCraftingStations))
            {
                return false;
            }

            if (!inventory.ContainsItems(requiredItems.ToList()))
            {
                return false;
            }

            inventory.RemoveItems(requiredItems.ToList());
            inventory.AddItems(CurrentRecipe.ResultItems);
            return true;
        }
        
        /// <summary>
        ///  Try to craft item with provided recipe ID.
        /// </summary>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(SerializableGuid recipeID)
        {
            SetCurrentRecipe(recipeID);
            return TryCraftItem();
        }
        
        /// <summary>
        ///  Try to craft item with provided recipe ID and specified inventory.
        /// </summary>
        /// <param name="recipeID"></param>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(SerializableGuid recipeID, InventoryObject inventory)
        {
            SetCurrentRecipe(recipeID);
            return TryCraftItem(inventory);
        }

        /// <summary>
        ///  Try to craft item with provided recipe Name.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        public virtual bool TryCraftItem(string recipeName)
        {
            SetCurrentRecipe(recipeName);
            return TryCraftItem();
        }
        
        /// <summary>
        ///  Sets the current recipe to the provided recipe.
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public virtual CraftingRecipeObject SetCurrentRecipe(CraftingRecipeObject recipe)
        {
            CurrentRecipe = recipe;
            return CurrentRecipe;
        }
        
        /// <summary>
        ///  Sets the current recipe to with provided recipe name.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        public virtual CraftingRecipeObject SetCurrentRecipe(string recipeName)
        {
            CurrentRecipe = Recipes.FirstOrDefault(x=> x.Name.Equals(recipeName));
            return CurrentRecipe;
        }
        
        /// <summary>
        ///  Sets the current recipe to with provided recipe ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual CraftingRecipeObject SetCurrentRecipe(SerializableGuid id)
        {
            CurrentRecipe = Recipes.FirstOrDefault(x=> x.ID.Equals(id));
            return CurrentRecipe;
        }
        
        /// <summary>
        ///  Clears the current recipe.
        /// </summary>
        public virtual void ClearCurrentRecipe()
        {
            CurrentRecipe = null;
        }
        
        /// <summary>
        ///  Sets the current crafting stations to the provided crafting stations.
        /// </summary>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual RequiredCraftingStations SetCurrentCraftingStations(RequiredCraftingStations craftingStations)
        {
            CurrentCraftingStations = craftingStations;
            return CurrentCraftingStations;
        }
        
        /// <summary>
        ///  Adds the provided crafting stations to the current crafting stations.
        /// </summary>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual RequiredCraftingStations AddCraftingStation(RequiredCraftingStations craftingStations)
        {
            CurrentCraftingStations |= craftingStations;
            return CurrentCraftingStations;
        }
        
        /// <summary>
        ///  Removes the provided crafting stations from the current crafting stations.
        /// </summary>
        /// <param name="craftingStations"></param>
        public virtual void RemoveCraftingStation(RequiredCraftingStations craftingStations)
        {
            CurrentCraftingStations &= ~craftingStations;
        }
        
        /// <summary>
        ///  Clears the current crafting stations.
        /// </summary>
        public virtual void ClearCraftingStations()
        {
            CurrentCraftingStations = RequiredCraftingStations.None;
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the current crafting stations.
        /// </summary>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes()
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.RequiredCraftingStations.HasFlag(CurrentCraftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided crafting stations.
        /// </summary>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided crafting stations and availability.
        /// </summary>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided availability.
        /// </summary>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject item)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == item)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item and crafting stations.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject item, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == item) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item and crafting stations and availability.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject item, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == item) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item and availability.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject item, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == item) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided items.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject[] items)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => items.Contains(y.Item))).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided items and crafting stations.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject[] items, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => items.Contains(y.Item)) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided items and crafting stations and availability.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject[] items, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => items.Contains(y.Item)) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided items and availability.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemObject[] items, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => items.Contains(y.Item)) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slot.
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot itemSlot)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == itemSlot.Item && y.Quantity == itemSlot.Quantity)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slot and crafting stations.
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot itemSlot, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == itemSlot.Item && y.Quantity == itemSlot.Quantity) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slot and crafting stations and availability.
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot itemSlot, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == itemSlot.Item && y.Quantity == itemSlot.Quantity) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slot and availability.
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot itemSlot, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => y.Item == itemSlot.Item && y.Quantity == itemSlot.Quantity) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slots.
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot[] itemSlots)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => itemSlots.Any(z => z.Item == y.Item && z.Quantity == y.Quantity))).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slots and crafting stations.
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot[] itemSlots, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => itemSlots.Any(z => z.Item == y.Item && z.Quantity == y.Quantity)) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slots and crafting stations and availability.
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot[] itemSlots, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => itemSlots.Any(z => z.Item == y.Item && z.Quantity == y.Quantity)) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided item slots and availability.
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(ItemSlot[] itemSlots, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ResultItems.Any(y => itemSlots.Any(z => z.Item == y.Item && z.Quantity == y.Quantity)) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe.
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject recipe)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x == recipe).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe and crafting stations.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject recipe, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x == recipe && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe and crafting stations and availability.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject recipe, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x == recipe && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe and availability.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject recipe, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x == recipe && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by provided recipes.
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject[] recipes)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> recipes.Contains(x)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering provided recipes and crafting stations.
        /// </summary>
        /// <param name="recipes"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject[] recipes, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> recipes.Contains(x) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering provided recipes and crafting stations and availability.
        /// </summary>
        /// <param name="recipes"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject[] recipes, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> recipes.Contains(x) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipes available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering provided recipes and availability.
        /// </summary>
        /// <param name="recipes"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(CraftingRecipeObject[] recipes, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> recipes.Contains(x) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        /// Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe name.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string recipeName)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.Name == recipeName).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe name and crafting stations.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string recipeName, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.Name == recipeName && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe name and crafting stations and availability.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string recipeName, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.Name == recipeName && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe name and availability.
        /// </summary>
        /// <param name="recipeName"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string recipeName, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.Name == recipeName && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string[] names)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> names.Contains(x.Name)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe names and crafting stations.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string[] names, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> names.Contains(x.Name) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe names and crafting stations and availability.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="craftingStations"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string[] names, RequiredCraftingStations craftingStations, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> names.Contains(x.Name) && x.RequiredCraftingStations.HasFlag(craftingStations) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe names and availability.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(string[] names, bool isAvailable)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> names.Contains(x.Name) && x.IsAvailable == isAvailable).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(SerializableGuid id)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ID.Equals(id)).ToArray();
            return Recipes.ToList();
        }
        
        /// <summary>
        ///  Gets the recipe available to the player by loading all crafting recipes from the Crafting Recipes folder and filtering by the provided recipe ID and crafting stations.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="craftingStations"></param>
        /// <returns></returns>
        public virtual List<CraftingRecipeObject> GetRecipes(SerializableGuid id, RequiredCraftingStations craftingStations)
        {
            Recipes = Resources.LoadAll<CraftingRecipeObject>("Crafting Recipes").Where(x=> x.ID.Equals(id) && x.RequiredCraftingStations.HasFlag(craftingStations)).ToArray();
            return Recipes.ToList();
        }

        /// <summary>
        ///  On trigger enter 2D event handler sets the player's crafting controller to this crafting controller and sets the player's inventory to this crafting controller's inventory.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            if(player == null) return;
            player.CraftingController = this;
            Inventory = player.Inventory;
        }
    }
}

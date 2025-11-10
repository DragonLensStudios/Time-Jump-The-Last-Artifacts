namespace PXE.Core.Enums
{
    /// <summary>
    /// Represents the ItemType
    /// </summary>
    [System.Flags]
    public enum ItemType
    {
        Consumable = 1,
        Reusable = 2,
        Equipment = 3,
        Weapon = 4,
        Ammo = 5,
        Material = 6,
        Quest = 7,
        Key = 8,
        Currency = 9,
        Junk = 10,
        Recipe = 11,
        CraftingStation = 12,
        Building = 13,
        Tool = 14,
        Resource = 15,
        Animal = 16,
        Food = 17,
        Drink = 18,
        Container = 19,
    }
}
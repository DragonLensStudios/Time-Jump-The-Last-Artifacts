namespace PXE.Core.Enums
{
    [System.Flags]
    public enum CombatType
    {
        None = 0,
        Melee = 1 << 1,
        Ranged = 1 << 2,
        Magic = 1 << 3
    }
}
namespace PXE.Core.Enums
{
    [System.Flags]
    public enum HitType
    {
        None = 0,
        HitOwner = 1 << 1,
        HitTeam = 1 << 2,
        HitDodging = 1 << 3,
        MissFlying = 1 << 4,
        Healing = 1 << 5,
        Lightning = 1 << 6,
        Fire = 1 << 7,
        Ice = 1 << 8,
        Earth = 1 << 9,
        Water = 1 << 10,
        Air = 1 << 11,
        Poison = 1 << 12
    }
}
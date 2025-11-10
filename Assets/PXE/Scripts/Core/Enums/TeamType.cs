namespace PXE.Core.Enums
{
    [System.Flags]
    public enum TeamType
    {
        Neutrals = 0,
        Players = 1 << 1,
        Allies = 1 << 2,
        Enemies = 1 << 3
    }
}
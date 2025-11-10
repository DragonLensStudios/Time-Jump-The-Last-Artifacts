namespace PXE.Core.Enums
{
    
    [System.Flags]
    public enum MessageChannels
    {
        None = 0,
        Dialogue = 1 << 0,
        GameFlow = 1 << 1,
        Gameplay = 1 << 2,
        UI = 1 << 3,
        Audio = 1 << 4,
        Saves = 1 << 5,
        Player = 1 << 6,
        Level = 1 << 7,
        Achievement = 1 << 8,
        Spawning = 1 << 9,
        Time = 1 << 10,
        Items = 1 << 11,
        Lighting = 1 << 12,
        Object = 1 << 13
    }

}
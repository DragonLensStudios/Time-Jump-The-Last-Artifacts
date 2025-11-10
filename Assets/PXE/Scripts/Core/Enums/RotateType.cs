namespace PXE.Core.Enums
{
    // not Flags
    public enum RotateType
    {
        None = 0,
        
        Parent = 1 << 1,
        Moving = 1 << 2,
        Target = 1 << 3,
        //     = 1 << 4,
        
        ParentUpDown = 1 << 5,
        MovingUpDown = 1 << 6,
        TargetUpDown = 1 << 7,
        //           = 1 << 8,
        
        ParentLeftRight = 1 << 9,
        MovingLeftRight = 1 << 10,
        TargetLeftRight = 1 << 11,
        //              = 1 << 12,

        Parent4Directions = 1 << 13,
        Moving4Directions = 1 << 14,
        Target4Directions = 1 << 15,
        //                = 1 << 16,

        Parent8Directions = 1 << 17,
        Moving8Directions = 1 << 18,
        Target8Directions = 1 << 19
        //                = 1 << 20,
    }

}
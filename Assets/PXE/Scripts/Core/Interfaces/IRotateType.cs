using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Interfaces
{
    public interface IRotateType
    {
        RotateType RotateType { get; set; }
        Transform transform { get; }
    }
}
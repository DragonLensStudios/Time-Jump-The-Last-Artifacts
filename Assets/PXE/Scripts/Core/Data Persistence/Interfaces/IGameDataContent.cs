using System;
using PXE.Core.Interfaces;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Interfaces
{
    public interface IGameDataContent : IGameObjectIdentity
    {
        float MoveSpeed { get; set; }
        Vector3 Position { get; set; }
        Vector2 MovementDirection { get; set; }
        string ReferenceState { get; set; }
        string CurrentLevelName { get; set; }
        SerializableGuid CurrentLevelID { get; set; }
        DateTime LastUpdated { get; set; }
    }
}
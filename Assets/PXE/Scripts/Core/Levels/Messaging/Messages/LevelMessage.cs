using PXE.Core.Enums;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Levels.Messaging.Messages
{
    public struct LevelMessage
    {
        public SerializableGuid LevelID { get; }
        public string LevelName { get; }
        public LevelState LevelState { get; }
        public Vector2 Position { get; }
        
/// <summary>
/// Executes the LevelMessage method.
/// Handles the LevelMessage functionality.
/// </summary>
        public LevelMessage(SerializableGuid levelID, string levelName, LevelState levelState, Vector2 position)
        {
            LevelID = levelID;
            LevelName = levelName;
            LevelState = levelState;
            Position = position;
        }
    }
}
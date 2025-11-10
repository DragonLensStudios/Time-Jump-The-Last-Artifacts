using System.Collections.Generic;
using PXE.Core.Achievements.Data;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Messaging.Messages
{
    public struct PlayerDataMessage
    {
        public SerializableGuid PLayerID { get; }
        public string PlayerName { get; }
        public Vector3 PlayerPosition { get; }
        public float PlayerSpeed { get; }
        public SerializableGuid CurrentLevelID { get; }
        public string CurrentLevelName { get; }

        public List<PlayerAchievementProgress> AchievementProgressList { get; }
        

/// <summary>
/// Executes the PlayerDataMessage method.
/// Handles the PlayerDataMessage functionality.
/// </summary>
        public PlayerDataMessage(SerializableGuid pLayerID, string playerName, Vector3 playerPosition, float playerSpeed, SerializableGuid currentLevelID, string currentLevelName, List<PlayerAchievementProgress> achievementProgressList)
        {
            PLayerID = pLayerID;
            PlayerName = playerName;
            PlayerPosition = playerPosition;
            PlayerSpeed = playerSpeed;
            CurrentLevelID = currentLevelID;
            CurrentLevelName = currentLevelName;
            AchievementProgressList = achievementProgressList;
        }
    }
}
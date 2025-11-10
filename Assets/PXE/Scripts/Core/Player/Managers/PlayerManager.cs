using System.Collections.Generic;
using PXE.Core.Achievements.Data;
using PXE.Core.Data_Persistence.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Player.Managers
{
    public class PlayerManager : ObjectController
    {
        public static PlayerManager Instance { get; private set; }

        [field: Tooltip("The player.")]
        [field: SerializeField] public virtual PlayerController Player { get; private set; }

        /// <summary>
        ///  This method registers the PlayerManager for the PlayerDataMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<PlayerDataMessage>(MessageChannels.Player, UpdatePlayerData);

        }

        /// <summary>
        /// This method unregisters the PlayerManager for the PlayerDataMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<PlayerDataMessage>(MessageChannels.Player, UpdatePlayerData);
        }

        /// <summary>
        ///  Singleton pattern for the player manager and sets the player.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
            
            if (Player == null)
            {
                Player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            }
        }
        
        /// <summary>
        ///  This methods updates the player data, if the player is null it returns false.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="playerName"></param>
        /// <param name="playerPosition"></param>
        /// <param name="playerMovementSpeed"></param>
        /// <param name="currentLevelName"></param>
        /// <param name="playerAchievementProgresses"></param>
        /// <returns></returns>
        public virtual bool UpdatePlayer(SerializableGuid playerId, string playerName, Vector3 playerPosition, float playerMovementSpeed, string currentLevelName, List<PlayerAchievementProgress> playerAchievementProgresses)
        {
            if(Player == null) return false;
            Player.ID = playerId;
            Player.Name = playerName;
            Player.transform.position = playerPosition;
            Player.MoveSpeed = playerMovementSpeed;
            Player.CurrentLevelName = currentLevelName;
            Player.AchievementProgressList = playerAchievementProgresses;
            UpdateIdentity(Player.gameObject);
            return true;
        }
        
        /// <summary>
        ///  This method updates the player data if the message has a value and calls the UpdatePlayer method.
        /// </summary>
        /// <param name="message"></param>
        public virtual void UpdatePlayerData(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PlayerDataMessage>().HasValue) return;
            var data = message.Message<PlayerDataMessage>().GetValueOrDefault();
            UpdatePlayer(data.PLayerID, data.PlayerName, data.PlayerPosition, data.PlayerSpeed, data.CurrentLevelName, data.AchievementProgressList);
        }
    }
}
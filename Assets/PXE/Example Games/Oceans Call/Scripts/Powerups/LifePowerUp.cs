using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Example_Games.Oceans_Call.Messages;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Powerups
{
    public class LifePowerUp : ObjectController
    {
        [field: Tooltip("The number of lives to add.")]
        [field: SerializeField] public virtual int Lives { get; set; } = 1;
        
        [field: Tooltip("The sound effect.")]
        [field: SerializeField] public virtual AudioObject Sfx { get; set; }

        /// <summary>
        ///  Handles the trigger enter event sends a message to the player to add lives and plays the sound effect.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            var playerID = other.gameObject.GetObjectID();
            if(Sfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(Sfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new PlayerLifePowerUpMessage(playerID, Lives));
            // MessageSystem.MessageManager.SendImmediate(MessageChannels.Achievement, new AchievementMessage("2", AchievementOperator.Unlock));
            SetObjectActive(false);
        }
    }
}

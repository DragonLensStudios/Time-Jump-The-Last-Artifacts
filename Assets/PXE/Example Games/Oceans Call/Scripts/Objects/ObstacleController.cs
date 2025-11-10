using PXE.Core.Actor;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Player;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Objects
{
   public class ObstacleController : PatrolObjectController 
   {
      [field: Tooltip("The amount of damage the obstacle does to the player.")]
      [field: SerializeField] public virtual int Damage { get; set; } = 1;
      
      [field: Tooltip("The AudioObject to play when the obstacle damages the player.")]
      [field: SerializeField] public virtual AudioObject DamageSfx { get; set; }

      /// <summary>
      ///  Checks if the player has collided with the obstacle.
      /// </summary>
      /// <param name="other"></param>
      public virtual void OnCollisionEnter2D(Collision2D other) => CheckPlayer(other.gameObject);
      public virtual void OnTriggerEnter2D(Collider2D col) => CheckPlayer(col.gameObject);

      public virtual void CheckPlayer(GameObject other)
      {
         if (!other.CompareTag("Player")) return;
         var player = other.GetComponent<PlayerController>();
         if (player == null) return;

         if (DamageSfx != null)
         {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(DamageSfx, AudioOperation.Play, AudioChannel.SoundEffects));
         }

         MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new TargetDamageMessage(player.ID, Damage));
      }
   }
}
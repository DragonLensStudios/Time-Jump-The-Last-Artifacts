using System.Collections;
using PXE.Core.Actor;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Enemy
{
    public class EnemyActorController : HandleTargetActorController
    {
        [field: Tooltip("The amount of damage this enemy deals to the target.")]
        [field: SerializeField] public virtual int Damage { get; set; } = 1;
        
        [field: Tooltip("The audio clip to play when this enemy damages the target.")]
        [field: SerializeField] public virtual AudioObject MeleeDamageSfx { get; set; }
        
        [field: Tooltip("The amount of time to wait after playing the damage sfx before damaging the target.")]
        [field: SerializeField] public virtual float TimeDelayAfterSfx { get; set; } = 0.25f;
        
        [field: SerializeField] public virtual float HitDelay { get; set; } = 0.25f;
    
        protected float hitTimer = 0.0f;

        protected bool isAttacking = false;

        /// <summary>
        ///  Called on collision with another object. If the object is the player, damage them.
        /// </summary>
        /// <param name="other"></param>
            public virtual void OnCollisionStay2D(Collision2D other)
            {
                if (other.collider.isTrigger) return;
                if (IsDisabled) return;
                if (isAttacking) return;
                if (!other.gameObject.CompareTag("Player")) return;
                var playerId = other.gameObject.GetObjectID();
                StartCoroutine(DamageTarget(playerId));
            }

        /// <summary>
        ///  Damages the target with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IEnumerator DamageTarget(SerializableGuid id)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            if(MeleeDamageSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(MeleeDamageSfx, AudioOperation.Play, AudioChannel.SoundEffects));
                yield return new WaitForSeconds(TimeDelayAfterSfx);
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new TargetDamageMessage(id, Damage));
            yield return new WaitForSeconds(HitDelay);
            isAttacking = false;
        }

    }
}
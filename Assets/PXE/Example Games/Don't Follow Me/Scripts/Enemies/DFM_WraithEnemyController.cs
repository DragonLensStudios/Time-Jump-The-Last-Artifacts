using System.Collections;
using System.Collections.Generic;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enemy;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.SerializableTypes;
using UnityEngine;

public class DFM_WraithEnemyController : EnemyActorController
{
    protected Coroutine attackCoroutine;
    public override void OnActive()
    {
        base.OnActive();
        Spawn();
    }

    public virtual void Spawn()
    {
        anim.SetTrigger("Spawn");
    }

    /// <summary>
    ///  Handles the OnTriggerEnter2D functionality and sends a GameObjectInteractionMessage.
    /// </summary>
    /// <param name="col"><see cref="Collider2D"/></param>
    public void OnTriggerStay2D(Collider2D col)
    {
        if (IsDisabled) return;
        if (isAttacking) return;
        if (!col.gameObject.CompareTag("Player")) return;
        var playerId = col.gameObject.GetObjectID();
        attackCoroutine = StartCoroutine(DamageTarget(playerId));
    }

    /// <summary>
    ///  Damages the target with the specified id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override IEnumerator DamageTarget(SerializableGuid id)
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
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}

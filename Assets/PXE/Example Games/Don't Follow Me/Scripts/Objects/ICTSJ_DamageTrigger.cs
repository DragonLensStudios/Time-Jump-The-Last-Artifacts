using System.Collections;
using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using UnityEngine;

public class ICTSJ_DeathTrigger : ObjectController
{
    [field: SerializeField] public int Damage { get; set; } = 999999999;
    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        var playerId = col.gameObject.GetObjectID();
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new TargetDamageMessage(playerId, Damage));
        
    }
}


using System.Collections;
using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages;
using UnityEngine;

public class ICTSJ_PhaseableObject : ObjectController
{
    protected Collider2D[] colliders;
    public override void Awake()
    {
        base.Awake();
        colliders = GetComponents<Collider2D>();
    }

    /// <summary>
    ///  Registers the ObjectController for the TransformPositionMessage message.
    /// </summary>
    public override void OnActive()
    {
        base.OnActive();
        MessageSystem.MessageManager.RegisterForChannel<ICTSJ_PhaseMessage>(MessageChannels.Player, PhaseMessageHandler);
    }

    public virtual void PhaseMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<ICTSJ_PhaseMessage>().HasValue) return;
        var data = message.Message<ICTSJ_PhaseMessage>().GetValueOrDefault();
        foreach (var collider in colliders)
        {
            if(collider == null) continue;
            if(collider.isTrigger) continue;
            collider.enabled = !data.IsPhasing;
        }
    }

    /// <summary>
    ///  Unregisters the ObjectController for the TransformPositionMessage message.
    /// </summary>
    public override void OnInactive()
    {
        base.OnInactive();
        MessageSystem.MessageManager.UnregisterForChannel<ICTSJ_PhaseMessage>(MessageChannels.Player, PhaseMessageHandler);
    }


}

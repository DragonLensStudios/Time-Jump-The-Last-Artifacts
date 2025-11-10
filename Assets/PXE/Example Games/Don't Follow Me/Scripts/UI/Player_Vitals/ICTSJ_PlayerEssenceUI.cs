using System;
using System.Collections;
using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages;
using UnityEngine;
using UnityEngine.UI;

public class ICTSJ_PlayerEssenceUI : ObjectController
{
    [field: SerializeField] public virtual Image EssenceBar { get; set; }
    [field: SerializeField] public virtual int CurrentEssence { get; set; }
    [field: SerializeField] public virtual int MaxEssence { get; set; }

    public override void OnActive()
    {
        base.OnActive();
        UpdateEssenceBar();
        MessageSystem.MessageManager.RegisterForChannel<ICTSJ_EssenceMessage>(MessageChannels.Player, EssenceMessageHandler);
    }

    public virtual void EssenceMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if (!message.Message<ICTSJ_EssenceMessage>().HasValue) return;
        var data = message.Message<ICTSJ_EssenceMessage>().GetValueOrDefault();
        MaxEssence = data.MaxEssencevalue;
        switch (data.Operator)
        {
            case Operator.Add:
                CurrentEssence += data.EssenceValue;
                if (CurrentEssence > MaxEssence) CurrentEssence = MaxEssence;
                break;
            case Operator.Subtract:
                CurrentEssence -= data.EssenceValue;
                break;
            case Operator.Multiply:
                CurrentEssence *= data.EssenceValue;
                if (CurrentEssence > MaxEssence) CurrentEssence = MaxEssence;
                break;
            case Operator.Divide:
                if (data.EssenceValue == 0) return;
                CurrentEssence /= data.EssenceValue;
                break;
            case Operator.Set:
                CurrentEssence = data.EssenceValue;
                if (CurrentEssence > MaxEssence) CurrentEssence = MaxEssence;
                break;
        }
    }

    public override void OnInactive()
    {
        base.OnInactive();
        MessageSystem.MessageManager.UnregisterForChannel<ICTSJ_EssenceMessage>(MessageChannels.Player, EssenceMessageHandler);

    }

    public void UpdateEssenceBar()
    {
        EssenceBar.fillAmount = (float)CurrentEssence / MaxEssence;
    }
    
    public override void Update()
    {
        UpdateEssenceBar();
    }
}

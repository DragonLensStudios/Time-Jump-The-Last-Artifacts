using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Time;
using PXE.Example_Games.Oceans_Call.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Example_Games.Oceans_Call.UI
{
    public class PlayerInfoUI : ObjectController
    {
        [field: SerializeField] public Sprite CrabHeartFull { get; set; }
        [field: SerializeField] public Sprite CrabHeartEmpty { get; set; }
        [field: SerializeField] public List<Image> LivesImages { get; set; }
        [field: SerializeField] public TMP_Text MetersTraveledText { get; set; }
        [field: SerializeField] public TMP_Text CurrentTimeText { get; set; }
        [field: SerializeField] public GameTimeObject CurrentTime { get; set; }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<PlayerInfoMessage>(MessageChannels.UI, PlayerInfoMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<PlayerInfoMessage>(MessageChannels.UI, PlayerInfoMessageHandler);
        }
        
        private void PlayerInfoMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PlayerInfoMessage>().HasValue) return;
            var data = message.Message<PlayerInfoMessage>().GetValueOrDefault();
            MetersTraveledText.text = $"Depth {Mathf.Max(0, Mathf.FloorToInt(data.MetersTraveled))} Meters";
            for (var i = 0; i < LivesImages.Count; i++)
            {
                // if(data.Lives < 0 || data.Lives > LivesImages.Count) continue;
                if(LivesImages[i] == null) continue;
                LivesImages[i].sprite = data.Lives > i ? CrabHeartFull : CrabHeartEmpty;
            }
        }

        public override void Update()
        {
            base.Update();
            if (CurrentTime == null) return;
            CurrentTimeText.text = $"{(int)CurrentTime.Hour:D2}:{(int)CurrentTime.Minute:D2}:{(int)CurrentTime.Second:D2}";
        }
    }
}

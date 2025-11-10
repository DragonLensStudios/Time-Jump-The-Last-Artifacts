using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Audio
{
    public class BGJOnGameStartAudio : ObjectController
    {

        [field: SerializeField] public AudioObject GameStartAudio { get; set; }
    
        public override void Start()
        {
            base.Start();
            if (GameStartAudio == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(GameStartAudio, AudioOperation.Play, AudioChannel.Music));
        }
    }
}

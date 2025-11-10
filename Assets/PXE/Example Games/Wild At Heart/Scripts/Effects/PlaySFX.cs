using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Example_Games.Wild_At_Heart.Scripts.Effects
{
    public class PlaySFX : ObjectController
    {
        [field: Tooltip("The audio clip to play after the triggering event.")]
        [field: SerializeField] public virtual AudioObject Sfx { get; set; }
        [field: Tooltip("Another prefab to instantly instantiate.")]
        [field: SerializeField] public virtual ObjectController InstantPrefab { get; set; }

        public override void Start()
        {
            base.Start();
            if (Sfx != null) MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(Sfx, AudioOperation.Play, AudioChannel.SoundEffects));
            if (InstantPrefab != null) Instantiate(InstantPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

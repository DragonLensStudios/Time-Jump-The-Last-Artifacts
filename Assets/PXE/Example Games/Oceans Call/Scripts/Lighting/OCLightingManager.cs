using PXE.Core.Enums;
using PXE.Core.Lighting.Managers;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Example_Games.Oceans_Call.Messages;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Lighting
{
    //TODO: Fix it so that it inherits from the Core.LightningManager
    public class OCLightingManager : LightingManager
    {
        
        [field: Tooltip("The depth lighting threshold.")]
        [field: SerializeField] public virtual float DepthLightingThreshold { get; set; } = 10f;
        
        /// <summary>
        ///  When the lighting manager is enabled it registers for the depth changed message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<DepthChangedMessage>(MessageChannels.Lighting, DepthChangedMessageHandler);
        }

        /// <summary>
        ///  When the lighting manager is disabled it unregisters for the depth changed message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<DepthChangedMessage>(MessageChannels.Lighting, DepthChangedMessageHandler);
        }
        
        /// <summary>
        ///  Handles the depth changed message and sets the global light intensity.
        /// </summary>
        /// <param name="message"></param>
        public virtual void DepthChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<DepthChangedMessage>().HasValue) return;
            var data = message.Message<DepthChangedMessage>().GetValueOrDefault();
            GlobalLight.intensity = CalculateLightingIntensity(data.Depth);
        }         
        
        /// <summary>
        ///  Calculates the lighting intensity based on the depth
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public virtual float CalculateLightingIntensity(float depth)
        {
            return GlobalLightIntensityMin + (GlobalLightIntensityMax - GlobalLightIntensityMin) * Mathf.Exp(-depth/DepthLightingThreshold);
        }
    }
}
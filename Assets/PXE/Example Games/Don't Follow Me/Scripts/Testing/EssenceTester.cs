using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages;
using UnityEngine.InputSystem;

namespace PXE.Example_Games.Don_t_Follow_Me.Scripts.Testing
{
    public class EssenceTester : ObjectController
    {
        public int EssenceValue = 1;
        public int MaxEssenceValue = 10;
        public override void Update()
        {
            base.Update();
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                MessageSystem.MessageManager.SendImmediate(new ICTSJ_EssenceMessage(SerializableGuid.CreateNew, EssenceValue, MaxEssenceValue, Operator.Add), MessageChannels.Player);
            }
            
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                MessageSystem.MessageManager.SendImmediate(new ICTSJ_EssenceMessage(SerializableGuid.CreateNew, EssenceValue, MaxEssenceValue, Operator.Subtract), MessageChannels.Player);
            }
        }
    }
}
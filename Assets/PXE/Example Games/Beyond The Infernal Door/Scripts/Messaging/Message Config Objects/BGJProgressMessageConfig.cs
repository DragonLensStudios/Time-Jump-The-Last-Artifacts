using System.Threading.Tasks;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Message_Config_Objects;
using PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Messaging.Messages;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Messaging.Message_Config_Objects
{
    [CreateAssetMenu(fileName = "BGJProgressMessageConfig", menuName = "PXE/BGJ/Messaging/Message Config Objects/BGJProgressMessageConfig")]
    public class BGJProgressMessageConfig : MessageConfigBaseObject
    {
        [field: SerializeField] public virtual bool IsGameOver { get; set; }
        
        public override void SendImmediate(MessageChannels channel)
        {
            MessageSystem.MessageManager.SendImmediate(channel, new BGJProgressMessage(IsGameOver));
        }

        public override void Send(MessageChannels channel)
        {
            MessageSystem.MessageManager.Send(channel, new BGJProgressMessage(IsGameOver));
        }

        public override Task SendImmediateAsync(MessageChannels channel)
        {
            return MessageSystem.MessageManager.SendImmediateAsync(channel, new BGJProgressMessage(IsGameOver));
        }

        public override Task SendAsync(MessageChannels channel)
        {
            return MessageSystem.MessageManager.SendAsync(channel, new BGJProgressMessage(IsGameOver));
        }

        public override void BroadcastImmediate()
        {
            MessageSystem.MessageManager.BroadcastImmediate(new BGJProgressMessage(IsGameOver));
        }

        public override void Broadcast()
        {
            MessageSystem.MessageManager.Broadcast(new BGJProgressMessage(IsGameOver));
        }

        public override Task BroadcastImmediateAsync()
        {
            return MessageSystem.MessageManager.BroadcastImmediateAsync(new BGJProgressMessage(IsGameOver));
        }

        public override Task BroadcastAsync()
        {
            return MessageSystem.MessageManager.BroadcastAsync(new BGJProgressMessage(IsGameOver));
        }
    }
}
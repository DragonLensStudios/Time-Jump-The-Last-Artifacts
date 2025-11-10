using System.Threading.Tasks;
using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Messaging.Message_Config_Objects
{
    public abstract class MessageConfigBaseObject : ScriptableObject
    {
        // Define the method to send a message immediately.
        public abstract void SendImmediate(MessageChannels channel);

        // Define the method to send a message to be processed later.
        public abstract void Send(MessageChannels channel);

        // Define the asynchronous version of the immediate send method.
        public abstract Task SendImmediateAsync(MessageChannels channel);

        // Define the asynchronous version of the send method for processing later.
        public abstract Task SendAsync(MessageChannels channel);

        // Define the method for broadcasting a message immediately.
        public abstract void BroadcastImmediate();

        // Define the method for broadcasting a message to be processed later.
        public abstract void Broadcast();

        // Define the asynchronous version of the broadcast immediate method.
        public abstract Task BroadcastImmediateAsync();

        // Define the asynchronous version of the broadcast for later processing method.
        public abstract Task BroadcastAsync();
    }
}
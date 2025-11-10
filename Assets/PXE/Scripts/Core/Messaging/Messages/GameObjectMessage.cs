using UnityEngine;

namespace PXE.Core.Messaging.Messages
{
    public struct GameObjectMessage
    {
        public GameObject GameObject { get;}

/// <summary>
/// Executes the GameObjectMessage method.
/// Handles the GameObjectMessage functionality.
/// </summary>
        public GameObjectMessage(GameObject gameObject)
        {
            GameObject = gameObject;
        }
    }
}
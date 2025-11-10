using UnityEngine;

namespace PXE.Core.Messaging.Messages
{
    public struct GameObjectInteractionMessage
    {
        public GameObject SourceGameObject { get; }
        public GameObject TargetGameObject { get; }

        public GameObjectInteractionMessage(GameObject sourceGameObject, GameObject targetGameObject)
        {
            SourceGameObject = sourceGameObject;
            TargetGameObject = targetGameObject;
        }
    }
}
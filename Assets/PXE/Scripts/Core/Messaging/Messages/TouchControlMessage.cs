using JetBrains.Annotations;
using UnityEngine;

namespace PXE.Core.Messaging.Messages
{
    public struct TouchControlMessage
    {
        public bool TouchControlActive { get; }
        [CanBeNull] public GameObject Layout { get; }

        public TouchControlMessage(bool touchControlActive, GameObject layout = null)
        {
            TouchControlActive = touchControlActive;
            Layout = layout;
        }
    }
}
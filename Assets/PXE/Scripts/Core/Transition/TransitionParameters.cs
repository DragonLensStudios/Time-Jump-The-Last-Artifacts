using System;
using PXE.Core.Enums;
using UnityEngine.Events;

namespace PXE.Core.Transition
{
    /// <summary>
    ///   Parameters for a transition
    /// </summary>
    [Serializable]
    public class TransitionParameters
    {
        public TransitionType transitionType;
        public Direction slideDirection = Direction.None;
        public float animationDurationInSeconds = 1;
        public UnityEvent endEvent;
    }
}
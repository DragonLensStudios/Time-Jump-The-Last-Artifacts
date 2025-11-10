using PXE.Core.Enums;
using UnityEngine.Events;

namespace PXE.Core.Transition.Messaging.Messages
{
    public struct TransitionMessage
    {
        public TransitionType TransitionType { get;}
        public Direction SlideDirection { get; }
        public float AnimationDurationInSeconds { get; }
        public UnityEvent EndEvent { get; }

/// <summary>
/// Executes the TransitionMessage method.
/// Handles the TransitionMessage functionality.
/// </summary>
        public TransitionMessage(TransitionType transitionType, Direction slideDirection = Direction.None, float animationDurationInSeconds = 1, UnityEvent endEvent = null)
        {
            TransitionType = transitionType;
            SlideDirection = slideDirection;
            AnimationDurationInSeconds = animationDurationInSeconds;
            EndEvent = endEvent;
        }
    }
}
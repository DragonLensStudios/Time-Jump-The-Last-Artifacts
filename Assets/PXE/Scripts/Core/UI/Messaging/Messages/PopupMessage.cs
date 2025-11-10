using PXE.Core.Enums;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PXE.Core.UI.Messaging.Messages
{
    
    public struct PopupMessage
    {
        public string Message { get; }
        public PopupType PopupType { get; }
        public PopupPosition PopupPosition { get; }
        public float DisplayTime { get; }
        public UnityAction OkAction { get; }
        public UnityAction CancelAction { get; }
        public UnityAction ConfirmAction { get; }
        public Slider Slider { get; }

        /// <summary>
        /// Executes the PopupMessage method.
        /// Handles the PopupMessage functionality.
        /// </summary>
        public PopupMessage(string message, PopupType popupType, PopupPosition position = PopupPosition.Bottom, float displayTime = 0f, UnityAction okAction = null, UnityAction cancelAction = null, UnityAction confirmAction = null, Slider slider = null)
        {
            Message = message;
            PopupType = popupType;
            PopupPosition = position;
            DisplayTime = displayTime;
            OkAction = okAction;
            CancelAction = cancelAction;
            ConfirmAction = confirmAction;
            Slider = slider;
        }
    }
}
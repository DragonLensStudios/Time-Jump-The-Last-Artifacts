using PXE.Core.Enums;

namespace PXE.Core.UI.Messaging.Messages
{
    public struct HidePopupMessage
    {
        public PopupType PopupType { get; }

/// <summary>
/// Executes the HidePopupMessage method.
/// Handles the HidePopupMessage functionality.
/// </summary>
        public HidePopupMessage(PopupType popupType)
        {
            PopupType = popupType;
        }
    }
}
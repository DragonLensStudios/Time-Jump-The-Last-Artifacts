using System.Collections;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.UI.Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PXE.Core.UI.Managers
{
/// <summary>
/// Represents the PopupDisplayUI.
/// The PopupDisplayUI class provides functionality related to popupdisplayui management.
/// This class contains methods and properties that assist in managing and processing popupdisplayui related tasks.
/// </summary>
    public class PopupDisplayUIManager : ObjectController
    {
        public static PopupDisplayUIManager Instance { get; private set; }
        
        [SerializeField] private GameObject confirmDialog, textDialog, notificationPopup;
        [SerializeField] private TMP_Text confimPopupText, textPopupText, notificationPopupText;
        [SerializeField] private Button confirmButton, cancelButton, okButton;

        private EventSystem eventSystem;
        private Coroutine notificationCoroutine;

        private ObjectController notificationPopupOc, textDialogOc, confirmDialogOc, confirmButtonOc, cancelButtonOc, okButtonOc;

        
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<PopupMessage>(MessageChannels.UI, PopupMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<HidePopupMessage>(MessageChannels.UI, HidePopupMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<PopupMessage>(MessageChannels.UI, PopupMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<HidePopupMessage>(MessageChannels.UI, HidePopupMessageHandler);
        }

        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
            eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
            notificationPopupOc = notificationPopup.GetComponent<ObjectController>();
            textDialogOc = textDialog.GetComponent<ObjectController>();
            confirmDialogOc = confirmDialog.GetComponent<ObjectController>();
            confirmButtonOc = confirmButton.GetComponent<ObjectController>();
            cancelButtonOc = cancelButton.GetComponent<ObjectController>();
            okButtonOc = okButton.GetComponent<ObjectController>();

        }
        
        public void ShowNotification(string text, float displayTime = 0f, PopupPosition position = PopupPosition.Bottom)
        {
            if (notificationPopupOc != null)
            {
                notificationPopupOc.SetObjectActive(true);
            }
            else
            {
                if (notificationPopup != null)
                {
                    notificationPopup.SetActive(true);
                    
                }
            }
            SetPopupPosition(notificationPopup, position);
            notificationPopupText.text = text;

            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }

            if (displayTime > 0f && notificationPopup.IsObjectActive())
            {
                StartCoroutine(HideAfterDelay(notificationPopup, displayTime));
            }

        }
        
        public void ShowTextPopup(string text, UnityAction okAction = null, float displayTime = 0f, PopupPosition position = PopupPosition.Middle)
        {
            if (textDialogOc != null)
            {
                textDialogOc.SetObjectActive(true);
            }
            else
            {
                textDialog.gameObject.SetActive(true);
            }
            SetPopupPosition(textDialog, position);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
            textPopupText.text = text;
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(okButton.gameObject);

            okButton.onClick.RemoveAllListeners();
            if (okAction != null)
            {
                okButton.onClick.AddListener(okAction);
                okButton.onClick.AddListener(HideTextDialog);
            }

            if (displayTime > 0f)
            {
                StartCoroutine(HideAfterDelay(textDialog, displayTime));
            }
        }
        
        public void ShowConfirmPopup(string text, UnityAction confirmAction = null, UnityAction cancelAction = null, float displayTime = 0f, PopupPosition position = PopupPosition.Middle)
        {
            if (confirmDialogOc != null)
            {
                confirmDialogOc.SetObjectActive(true);
            }
            else
            {
                confirmDialog.gameObject.SetActive(true);
            }
            SetPopupPosition(confirmDialog, position);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
            confimPopupText.text = text;
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(confirmButton.gameObject);

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            if (confirmAction != null)
            {
                confirmButton.onClick.AddListener(confirmAction);
                confirmButton.onClick.AddListener(HideConfrimDialog);
                if (confirmButtonOc != null)
                {
                    confirmButtonOc.SetObjectActive(true);   
                }
                else
                {
                    confirmButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (confirmButtonOc != null)
                {
                    confirmButtonOc.SetObjectActive(false);   
                }
                else
                {
                    confirmButton.gameObject.SetActive(false);
                }
            }

            if (cancelAction != null)
            {
                cancelButton.onClick.AddListener(cancelAction);
                cancelButton.onClick.AddListener(HideConfrimDialog);
                if (cancelButtonOc != null)
                {
                    cancelButtonOc.SetObjectActive(true);   
                }
                else
                {
                    cancelButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (cancelButtonOc != null)
                {
                    cancelButtonOc.SetObjectActive(false);   
                }
                else
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }

            if (displayTime > 0f)
            {
                StartCoroutine(HideAfterDelay(confirmDialog, displayTime));
            }
        }

        private IEnumerator HideAfterDelay(GameObject dialog, float delay)
        {
            yield return new WaitForSeconds(delay);

            var dialogOc = dialog.GetComponent<ObjectController>();
            if (dialogOc != null)
            {
                dialogOc.SetObjectActive(false);
            }
            else
            {
                if (dialog != null)
                {
                    dialog.SetActive(false);
                }
            }
        }
        
        public void HideAllDialogs()
        {
            HideConfrimDialog();
            HideTextDialog();
            HideNotificationPopup();
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public void HideConfrimDialog()
        {
            if (confirmDialogOc != null)
            {
                confirmDialogOc.SetObjectActive(false);
            }
            else
            {
                if (confirmDialog != null)
                {
                    confirmDialog.SetActive(false);
                }
            }
            if (confirmButtonOc != null)
            {
                confirmButtonOc.SetObjectActive(false);
            }
            else
            {
                if (confirmButton != null)
                {
                    confirmButton.gameObject.SetActive(false);
                }
            }
            if (cancelButtonOc != null)
            {
                cancelButtonOc.SetObjectActive(false);
            }
            else
            {
                if (cancelButton != null)
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public void HideTextDialog()
        {
            if (textDialogOc != null)
            {
                textDialogOc.SetObjectActive(false);
            }
            else
            {
                if (textDialog != null)
                {
                    textDialog.SetActive(false);
                }
            }
            if (okButtonOc != null)
            {
                okButtonOc.SetObjectActive(false);
            }
            else
            {
                if (okButton != null)
                {
                    okButton.gameObject.SetActive(false);
                }
            }
            okButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }
        
        public void HideNotificationPopup()
        {
            if (notificationPopupOc != null)
            {
                notificationPopupOc.SetObjectActive(false);
            }
            else
            {
                if (notificationPopup != null)
                {
                    notificationPopup.SetActive(false);
                }
            }
        }
        
        private void PopupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PopupMessage>().HasValue) return;
            var data = message.Message<PopupMessage>().GetValueOrDefault();
            switch (data.PopupType)
            {
                case PopupType.Confirm:
                    ShowConfirmPopup(data.Message,data.ConfirmAction, data.CancelAction, data.DisplayTime, data.PopupPosition);
                    break;
                case PopupType.Message:
                    ShowTextPopup(data.Message, data.OkAction, data.DisplayTime,data.PopupPosition);
                    break;
                case PopupType.Notification:
                    ShowNotification(data.Message, data.DisplayTime, data.PopupPosition);
                    break;
            }
        }
        
        private void HidePopupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<HidePopupMessage>().HasValue) return;
            var data = message.Message<HidePopupMessage>().GetValueOrDefault();

            switch (data.PopupType)
            {
                case PopupType.Confirm:
                    HideConfrimDialog();
                    break;
                case PopupType.Message:
                    HideTextDialog();
                    break;
                case PopupType.Notification:
                    HideNotificationPopup();
                    break;
            }

        }
        
        private void SetPopupPosition(GameObject popup, PopupPosition position)
        {
            RectTransform rectTransform = popup.GetComponent<RectTransform>();
            RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
    
            switch (position)
            {
                case PopupPosition.Top:
                    rectTransform.anchoredPosition = new Vector2(0, parentRectTransform.rect.height / 2 - rectTransform.rect.height / 2);
                    break;
                case PopupPosition.Middle:
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case PopupPosition.Bottom:
                    rectTransform.anchoredPosition = new Vector2(0, -parentRectTransform.rect.height / 2 + rectTransform.rect.height / 2);
                    break;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnInactive();
        }
    }
}

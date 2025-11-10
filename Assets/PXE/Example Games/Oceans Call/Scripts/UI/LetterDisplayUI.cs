using System.Collections;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Example_Games.Oceans_Call.Messages;
using TMPro;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.UI
{
    public class LetterDisplayUI : ObjectController
    {
        [field: SerializeField] public GameObject LetterDisplay { get; set; }
        [field: SerializeField] public TMP_Text LetterDisplayText { get; set; }

        private Coroutine showMessageCoroutine = null;

        public override void Awake()
        {
            base.Awake();
            if (LetterDisplay == null)
            {
                LetterDisplay = gameObject.transform.GetChild(0).gameObject;
            }
            
            if (LetterDisplayText == null)
            {
                LetterDisplayText = LetterDisplay.GetComponentInChildren<TMP_Text>();
            }
        }

        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<LetterMessage>(MessageChannels.UI, LetterMessageHandler);

        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<LetterMessage>(MessageChannels.UI, LetterMessageHandler);
        }
        
        private void LetterMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LetterMessage>().HasValue) return;
            var data = message.Message<LetterMessage>().GetValueOrDefault();
            if (LetterDisplayText != null)
            {
                LetterDisplayText.text = data.Message;
            }
            if (showMessageCoroutine != null)
            {
                StopCoroutine(showMessageCoroutine);
                showMessageCoroutine = null;
            }
            showMessageCoroutine = StartCoroutine(DisplayMessage(data.TimeToDisplay));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this,true));
        }
        
        public void HideMessage()
        {
            if(LetterDisplay == null) return;
            var letterDisplayOc = LetterDisplay.GetComponent<ObjectController>();
            if (letterDisplayOc != null)
            {
                if (letterDisplayOc.IsActive)
                {
                    letterDisplayOc.SetObjectActive(false);
                }
            }
            else
            {
                if (LetterDisplay.activeSelf)
                {
                    LetterDisplay.SetActive(false);
                }
            }

            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
        }

        public IEnumerator DisplayMessage(float timeToDisplay)
        {
            if (LetterDisplay == null) yield break;
            var letterDisplayOc = LetterDisplay.GetComponent<ObjectController>();
            if (letterDisplayOc != null)
            {
                letterDisplayOc.SetObjectActive(true);
            }
            else
            {
                LetterDisplay.SetActive(true);
            }
            yield return new WaitForSeconds(timeToDisplay);
            HideMessage();
        }
        
    }
}

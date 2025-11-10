using System.Collections;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Dialogue;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Dialogue.UI;
using PXE.Core.Enums;
using PXE.Core.Inventory.Items;
using PXE.Core.Levels;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.UI.Messaging.Messages;
using PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Enums;
using PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Door
{
    public class BGJDoorController : ObjectController
    {
        [field: SerializeField] public virtual bool IsOpen { get; set; }
        [field: SerializeField] public virtual bool IsLocked { get; set; }
        [field: SerializeField] public virtual bool IsInteractable { get; set; }
        [field: SerializeField] public virtual Button OpenButton { get; set; }
        [field: SerializeField] public virtual AudioObject DoorOpenSfx { get; set; }
        [field: SerializeField] public virtual LevelsOfHell Level { get; set; }
        [field: SerializeField] public virtual ItemObject Key { get; set; }
    
        [field: SerializeField] public BGJDoorManager DoorManager { get; set; }

        protected Animator anim;
    
        protected bool doorSelected;
    
        protected static readonly int OpenAnim = Animator.StringToHash("Open");

        public override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            DoorManager = FindObjectOfType<BGJDoorManager>();
        }

        public override void OnActive()
        {
            base.OnActive();
            DoorManager.RegisterDoor(this);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            DoorManager.UnregisterDoor(this);
        }

        public virtual void OpenDialogue(DialogueGraph graph)
        {
            if (IsLocked)
            {
                if(Key != null)
                {
                    if (BGJGameProgressManager.Instance.PlayerInventory.ContainsItem(Key))
                    {
                        IsLocked = false;
                        BGJGameProgressManager.Instance.PlayerInventory.RemoveItem(Key);
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"You unlock the door with the {Key.Name}", PopupType.Notification, PopupPosition.Middle, 2f));
                    }
                    else
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("This door is locked.", PopupType.Notification, PopupPosition.Middle, 2f));
                        return;
                    }
                }
            }
            if (IsOpen) return;
            IsOpen = true;
            anim.SetTrigger(OpenAnim);
            IsInteractable = false;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(DoorOpenSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            StartCoroutine(OpenDoorDialogue(graph));
        }

        public virtual void OpenLevel(LevelObject levelObject)
        {
            if (IsLocked)
            {
                if(Key != null)
                {
                    if (BGJGameProgressManager.Instance.PlayerInventory.ContainsItem(Key))
                    {
                        IsLocked = false;
                        BGJGameProgressManager.Instance.PlayerInventory.RemoveItem(Key);
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"You unlock the door with the {Key.Name}", PopupType.Notification, PopupPosition.Middle, 2f));
                    }
                    else
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("This door is locked.", PopupType.Notification, PopupPosition.Middle, 2f));
                        return;
                    }
                }
            }
            if (IsOpen) return;
            IsOpen = true;
            anim.SetTrigger(OpenAnim);
            IsInteractable = false;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(DoorOpenSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            StartCoroutine(OpenDoorLevel(levelObject));
        }

        public virtual IEnumerator OpenDoorLevel(LevelObject levelObject)
        {
            yield return new WaitForSeconds(1f);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(levelObject.ID, levelObject.Name, LevelState.Loading, Vector2.zero));
        }
    
        public virtual IEnumerator OpenDoorDialogue(DialogueGraph graph)
        {
            yield return new WaitForSeconds(1f);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueMessage(DialogueState.Start, string.Empty, graph, null, null, ID));
        }

        public override void Update()
        {
            base.Update();
            if(OpenButton == null) return;
            if(DialogueUi.Instance == null) return;
            IsInteractable = !DialogueUi.Instance.IsDialogueOpen && !IsOpen;
            OpenButton.interactable = IsInteractable;
        }
    }
}

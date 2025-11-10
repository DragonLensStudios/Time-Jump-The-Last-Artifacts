using System.Collections;
using System.Collections.Generic;
using PXE.Core.Dialogue.UI;
using PXE.Core.Objects;
using PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Managers;
using UnityEngine;

namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Door
{
    public class BGJDoorManager : ObjectController
    {
        [field: SerializeField] public virtual List<BGJDoorController> Doors { get; set; } = new();
        protected bool wasDialogueOpen = false;

        public override void Update()
        {
            base.Update();
            bool isDialogueOpen = DialogueUi.Instance != null && DialogueUi.Instance.IsDialogueOpen;
            if (wasDialogueOpen && !isDialogueOpen)
            {
                // Dialogue has just closed; trigger door selection with a slight delay
                StartCoroutine(DelayedDoorSelection());
            
            }
            wasDialogueOpen = isDialogueOpen;
        }
        public virtual IEnumerator DelayedDoorSelection()
        {
            if(DialogueUi.Instance == null)
            {
                yield break;
            }
            yield return new WaitUntil(()=>!DialogueUi.Instance.IsDialogueOpen);
            wasDialogueOpen = false; // Reset flag
            // Short delay to ensure UI has been updated
            SelectNextAvailableDoor();
        }

        public virtual void RegisterDoor(BGJDoorController door)
        {
            if (!Doors.Contains(door))
            {
                Doors.Add(door);
            }
            SelectNextAvailableDoor();
        }
    
        public virtual void UnregisterDoor(BGJDoorController door)
        {
            if (Doors.Contains(door))
            {
                Doors.Remove(door);
            }
        }

        public virtual void DoorOpened()
        {
            SelectNextAvailableDoor();
        }

        public virtual void SelectNextAvailableDoor()
        {
            // Check if the Dialogue UI is active/open; if so, return without selecting a door
            if (DialogueUi.Instance != null && DialogueUi.Instance.IsDialogueOpen || BGJGameProgressManager.Instance.IsGameOver)
            {
                return;
            }

            for (int i = 0; i < Doors.Count; i++)
            {
                if (Doors[i].IsInteractable && !Doors[i].IsOpen && Doors[i].IsActive)
                {
                    Doors[i].OpenButton.Select();
                    return; // Exit after selecting the next door
                }
            }
        }

    }
}
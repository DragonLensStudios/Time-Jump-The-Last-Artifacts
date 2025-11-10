using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Utilities.Helpers.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PXE.Core.UI
{
    /// <summary>
    /// Represents the Page.
    /// The Page class provides functionality related to page management.
    /// This class contains methods and properties that assist in managing and processing page related tasks.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class Page : ObjectController
    {
        public bool ExitOnNewPagePush = false;
        [SerializeField]
        private GameObject firstFocusItem;
        [SerializeField]
        private float AnimationSpeed = 1f;
        [SerializeField]
        private AudioObject EntrySfx, ExitSfx;
        [SerializeField]
        private EntryMode EntryMode = EntryMode.Slide;
        [SerializeField]
        private Direction EntryDirection = Direction.Left;
        [SerializeField]
        private EntryMode ExitMode = EntryMode.Slide;
        [SerializeField]
        private Direction ExitDirection = Direction.Left;
        [SerializeField]
        private UnityEvent PrePushAction;
        [SerializeField]
        private UnityEvent PostPushAction;
        [SerializeField]
        private UnityEvent PrePopAction;
        [SerializeField]
        private UnityEvent PostPopAction;

        private RectTransform RectTransform;
        private CanvasGroup CanvasGroup;
        private Coroutine AnimationCoroutine;
        // private EventSystem _eventSystem;


        public override void Awake()
        {
            base.Awake();
            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
            // _eventSystem = FindFirstObjectByType<EventSystem>();

            // if (_eventSystem != null)
            // {
                // _eventSystem.SetSelectedGameObject(null);
                // _eventSystem.SetSelectedGameObject(firstFocusItem);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstFocusItem);
            // }
        }

        /// <summary>
        /// Executes the Enter method.
        /// Handles the Enter functionality.
        /// </summary>
        public virtual void Enter(bool PlayAudio)
        {
            SetObjectActive(true);

            // SelectedNavigationInit();

            if (firstFocusItem != null)
            {
                // if (_eventSystem != null)
                // {
                    // _eventSystem.SetSelectedGameObject(null);
                    // _eventSystem.SetSelectedGameObject(firstFocusItem);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(firstFocusItem);
                // }
            }
            PrePushAction?.Invoke();

            switch(EntryMode)
            {
                case EntryMode.None:
                    PlayEntryClip(PlayAudio);
                    break;
                case EntryMode.Slide:
                    SlideIn(PlayAudio);
                    break;
                case EntryMode.Zoom:
                    ZoomIn(PlayAudio);
                    break;
                case EntryMode.Fade:
                    FadeIn(PlayAudio);
                    break;
            }
        }

        // /// <summary>
        // /// Executes the SelectedNavigationInit method.
        // /// Manages linier menus.
        // /// </summary>
        // public virtual void SelectedNavigationInit()
        // {
            // Debug.Log("Fixing Buttons");
            
//             var selectables = GetComponentsInChildren<Selectable>();
//             for (int i = 0; i < selectables.Length; i++)
//             {
//                 // Debug.Log(selectables[i].name + ": " + (selectables[i].GetComponent<ObjectController>().IsActive ? "Enabled" : "Disabled"));
// //                 selectables[i].GetComponent<Button>();
// //                 if (selectables[i].GetComponent<Button>() != null)
// //                 {
// // Debug.Log("Found Button");                    
// //                 }
                
//                 var selectable = selectables[i].GetComponent<Button>(); //selectables[i] as Button;
//                 var _navigation = (selectable)?.navigation;
//                 if (_navigation != null)
//                 {
//                     Navigation navigation = (Navigation)_navigation;
//                     // Debug.Log("Up Old: " + navigation.selectOnUp);

//                     //selectables[i].
//                     navigation.selectOnUp = selectables[WalkArray2(selectables, i, false)];
                    
//                     // Debug.Log("Up New: " + navigation.selectOnUp);
//                     Debug.Log("Down Old: " + navigation.selectOnDown);
//                     //selectables[i].
//                     navigation.selectOnDown = selectables[WalkArray2(selectables, i, true)];
//                     Debug.Log("Down New: " + navigation.selectOnDown);
//                 }
//             };
        // }

        // /// <summary>
        // /// Walks an Array of GameObjects for the next Active GameObject.
        // /// </summary>
        // public virtual int WalkArray2<T>(T[] array, int i, bool inc) where T : Selectable
        // {
        //     int len = array.Length;
        //     if (i < 0 || i >= len) return i;
        //     int step = inc ? 1 : -1;
        //     int done = i;
        //     if (inc)
        //     {
        //         for (i += step; i < len; i += step)
        //         {
        //             var temp = array[i].GetComponent<ObjectController>();
        //             if (temp.IsActive) return i;
        //         }
        //         for (i = 0; i < done; i += step)
        //         {
        //             var temp = array[i].GetComponent<ObjectController>();
        //             if (temp.IsActive) return i;
        //         }
        //     }
        //     else
        //     {
        //         for (i += step; i >= 0; i += step)
        //         {
        //             var temp = array[i].GetComponent<ObjectController>();
        //             if (temp.IsActive) return i;
        //         }
        //         for (i = len - 1; i > done; i += step)
        //         {
        //             var temp = array[i].GetComponent<ObjectController>();
        //             if (temp.IsActive) return i;
        //         }
        //     }
        //     return done;
        // }

        // /// <summary>
        // /// Walks an Array of GameObjects for the next Active GameObject.
        // /// </summary>
        // public virtual int WalkArray<T>(T[] array, int i, bool inc) where T : ObjectController
        // {
        //     int len = array.Length;
        //     if (i < 0 || i >= len) return i;
        //     int step = inc ? 1 : -1;
        //     int done = i;
        //     for (i += step; i < len; i += step) {
        //         if (array[i].GetObjectActiveState()) return i;
        //     }
        //     for (i = 0; i < done; i += step) {
        //         if (array[i].GetObjectActiveState()) return i;
        //     }
        //     return done;
        // }

        /// <summary>
        /// Executes the Exit method.
        /// Handles the Exit functionality.
        /// </summary>
        public virtual void Exit(bool PlayAudio)
        {
            PrePopAction?.Invoke();

            switch (ExitMode)
            {
                case EntryMode.None:
                    PlayExitClip(PlayAudio);
                    break;
                case EntryMode.Slide:
                    SlideOut(PlayAudio);
                    break;
                case EntryMode.Zoom:
                    ZoomOut(PlayAudio);
                    break;
                case EntryMode.Fade:
                    FadeOut(PlayAudio);
                    break;
            }
            
            SetObjectActive(false);
        }

        public void SetSelectedGameObject(GameObject obj)
        {
            // if (_eventSystem == null) return;
            // _eventSystem.SetSelectedGameObject(null);
            // _eventSystem.SetSelectedGameObject(obj);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(obj);
        }

        private void SlideIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.SlideIn(RectTransform, EntryDirection, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void SlideOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.SlideOut(RectTransform, ExitDirection, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }
    
        private void ZoomIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.ZoomIn(RectTransform, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void ZoomOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.ZoomOut(RectTransform, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }

        private void FadeIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.FadeIn(CanvasGroup, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void FadeOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.FadeOut(CanvasGroup, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }

        private void PlayEntryClip(bool PlayAudio)
        {
            if (!PlayAudio || EntrySfx == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(EntrySfx, AudioOperation.Play, AudioChannel.SoundEffects));
        }
    
        private void PlayExitClip(bool PlayAudio)
        {
            if (!PlayAudio || ExitSfx == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ExitSfx, AudioOperation.Play, AudioChannel.SoundEffects));
        }
    }
}

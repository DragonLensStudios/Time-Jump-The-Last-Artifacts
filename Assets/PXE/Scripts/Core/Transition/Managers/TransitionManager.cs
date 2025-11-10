using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Transition.Messaging.Messages;
using PXE.Core.Utilities.Helpers.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PXE.Core.Transition.Managers
{
    /// <summary>
    /// Represents the TransitionManager.
    /// The TransitionManager class provides functionality related to transitionmanager management.
    /// This class contains methods and properties that assist in managing and processing transitionmanager related tasks.
    /// </summary>
    public class TransitionManager : ObjectController
    {
        public static TransitionManager Instance { get; private set; }

        [field: Tooltip("The transition type.")]
        [field: SerializeField] public TransitionType TransitionType { get; set; }
        
        [field: Tooltip("The slide direction.")]
        [field: SerializeField] public Direction SlideDirection { get; set; }
        
        [field: Tooltip("The animation duration in seconds.")]
        [field: SerializeField] public float AnimationDurationInSeconds { get; set; } = 1f;
        
        [field: Tooltip("The on end event.")]
        [field: SerializeField] public UnityEvent OnEnd { get; set; }
        
        [field: Tooltip("The rect transform.")]
        [field: SerializeField] public RectTransform RectTransform  { get; set; }
        
        [field: Tooltip("The canvas group.")]
        [field: SerializeField] public CanvasGroup CanvasGroup  { get; set; }
        
        [field: Tooltip("The transition image.")]
        [field: SerializeField] public Image TransitionImage  { get; set; }


        /// <summary>
        ///  Singleton pattern for the transition manager and sets the transition image, canvas group, and rect transform.
        /// </summary>
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
            if (CanvasGroup == null)
            {
                CanvasGroup = GetComponentInChildren<CanvasGroup>();
            }

            if (TransitionImage == null)
            {
                TransitionImage = GetComponentInChildren<Image>(true);
            }

            if (RectTransform == null)
            {
                RectTransform =TransitionImage.GetComponent<RectTransform>();
            }
            

        }
        
        /// <summary>
        ///  This method registers the TransitionManager for the TransitionMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<TransitionMessage>(MessageChannels.UI, HandleTransition);

        }

        /// <summary>
        ///  This method unregisters the TransitionManager for the TransitionMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<TransitionMessage>(MessageChannels.UI, HandleTransition);

        }

        /// <summary>
        ///  This method handles the transition message and sets the transition type, direction, and speed from the message.
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandleTransition(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<TransitionMessage>().HasValue) return;
            var data = message.Message<TransitionMessage>().GetValueOrDefault();

            // Set the transition type, direction, and speed from the message
            TransitionType = data.TransitionType;
            SlideDirection = data.SlideDirection;
            AnimationDurationInSeconds = data.AnimationDurationInSeconds;
            
            OnEnd = new UnityEvent();
            OnEnd.AddListener(() => 
            {
                data.EndEvent.Invoke();
                
                var transitionImageOc = TransitionImage.GetComponent<ObjectController>();
                // Enable the transition image
                if (transitionImageOc != null)
                {
                    transitionImageOc.SetObjectActive(false);
                }
                else
                {
                    TransitionImage.gameObject.SetActive(false);
                }
            });
            
            // Start the transition
            StartTransition();
            
        }
        
        /// <summary>
        ///  This method starts the transition based on the transition type.
        /// </summary>
        public virtual void StartTransition()
        {
            TransitionImage.gameObject.SetObjectActive(true);

            switch (TransitionType)
            {
                case TransitionType.None:
                    break;
                case TransitionType.ZoomIn:
                    StartCoroutine(AnimationHelper.ZoomIn(RectTransform, AnimationDurationInSeconds, OnEnd));
                    break;
                case TransitionType.ZoomOut:
                    StartCoroutine(AnimationHelper.ZoomOut(RectTransform, AnimationDurationInSeconds, OnEnd));
                    break;
                case TransitionType.FadeIn:
                    StartCoroutine(AnimationHelper.FadeOut(CanvasGroup, AnimationDurationInSeconds, OnEnd));
                    break;
                case TransitionType.FadeOut:
                    StartCoroutine(AnimationHelper.FadeIn(CanvasGroup, AnimationDurationInSeconds, OnEnd));
                    break;
                case TransitionType.SlideIn:
                    StartCoroutine(AnimationHelper.SlideIn(RectTransform, SlideDirection, AnimationDurationInSeconds, OnEnd));
                    break;
                case TransitionType.SlideOut:
                    StartCoroutine(AnimationHelper.SlideOut(RectTransform, SlideDirection, AnimationDurationInSeconds, OnEnd));
                    break;
            }
        }
    }
}

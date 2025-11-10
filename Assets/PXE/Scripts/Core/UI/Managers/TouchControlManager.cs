using System.Linq;
using JetBrains.Annotations;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.UI.Managers
{
    public class TouchControlManager : ObjectController
    {
        private static TouchControlManager instance;
        
        public static TouchControlManager Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindFirstObjectByType<TouchControlManager>(FindObjectsInactive.Include);

                if (instance != null) return instance;
                GameObject managerObject = new GameObject("Touch Control Manager");
                instance = managerObject.AddComponent<TouchControlManager>();

                return instance;
            }
        }
        [Header("Touch Controller Settings")]
        [field: SerializeField] public GameObject[] layouts;
    
        [field: SerializeField] public bool TouchControllerIsActive { get; set; }
    
        [Tooltip("The canvas for the touch controller")]
        [field: SerializeField] public ObjectController TouchControllerCanvas { get; set; }

        [Tooltip("The default layout of the touch controller")]
        [field: SerializeField] public GameObject DefaultTouchControllerLayout { get; set; }
    
        [Tooltip("The current layout of the touch controller")]
        [field: SerializeField] public GameObject CurrentTouchControllerLayout { get; set; }
    
        [Tooltip("The currently active layout of the touch controller in the scene")]
        [field: SerializeField] public GameObject ActiveTouchControllerLayout { get; set; }
    

        public override void OnActive()
        {
            base.OnActive();
        
            if (layouts.Length == 0)
            {
                layouts = Resources.LoadAll<GameObject>("Touch Controller Layouts").ToArray();
            }
            MessageSystem.MessageManager.RegisterForChannel<TouchControlMessage>(MessageChannels.UI, TouchControlMessageHandler);
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<TouchControlMessage>(MessageChannels.UI, TouchControlMessageHandler);
        }

        public override void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }

        /// <summary>
        /// Sets the default layout of the touch controller. If none is provided, the default will be provided from
        /// index 0 of the layouts array.
        /// </summary>
        /// <param name="layout"></param>
        private void SetDefaultControllerLayout([CanBeNull] GameObject layout)
        {
            DefaultTouchControllerLayout = !layout ? layouts[0] : layout;
            Debug.Log(DefaultTouchControllerLayout.name);
        }

        /// <summary>
        /// Sets the current layout of the touch controller. If the layout provided matches the one already being used,
        /// it will return out of the method with no change.
        /// </summary>
        /// <param name="layout"></param>
        private void SetCurrentControllerLayout([CanBeNull] GameObject layout)
        {
            if (layout == CurrentTouchControllerLayout && CurrentTouchControllerLayout != null) return;
            if (ActiveTouchControllerLayout != null) Destroy(ActiveTouchControllerLayout);
            // In with the new
            CurrentTouchControllerLayout = !layout ? DefaultTouchControllerLayout : layout;

        }

        private void SetActiveControllerLayout()
        {
            // If the layout doesn't change, just return.
            if (ActiveTouchControllerLayout == CurrentTouchControllerLayout) return;
            if (CurrentTouchControllerLayout == null) return;
            if(TouchControllerCanvas == null) return;
            ActiveTouchControllerLayout = Instantiate(CurrentTouchControllerLayout, TouchControllerCanvas.transform);
        }
    
        /// <summary>
        /// First, if the DefaultTouchControllerLayout is null, it gets set to whatever the first layout sent to it is.
        /// 
        /// Second, if the CurrentTouchControllerLayout is null, set it to either the data.Layout or default (if data.Layout is null)
        /// Otherwise, set it to the data.Layout to change the layout.
        /// 
        /// Next, check if the ActiveTouchControllerLayout is null. If it is:
        ///     Destroy the current ActiveTouchControllerLayout
        ///     Set it to CurrentTouchControllerLayout
        ///     Instantiate the new ActiveTouchControllerLayout.
        /// 
        /// Finally, set the TouchControllerCanvas object to Active.
        /// </summary>
        /// <param name="message"></param>
        private void TouchControlMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(TouchControllerCanvas == null) return;
            if (!message.Message<TouchControlMessage>().HasValue) return;
            var data = message.Message<TouchControlMessage>().GetValueOrDefault();

            TouchControllerIsActive = data.TouchControlActive;
        
            if (TouchControllerIsActive)
            {
                // Set the default layout if it is null.
                if (!DefaultTouchControllerLayout)
                {
                    SetDefaultControllerLayout(data.Layout);
                }
                // Set the current layout
                SetCurrentControllerLayout(data.Layout);

                // Set the active layout
                SetActiveControllerLayout();
            
                TouchControllerCanvas.SetObjectActive(TouchControllerIsActive);
            }
            else
            {
                TouchControllerCanvas.SetObjectActive(TouchControllerIsActive);
                Destroy(ActiveTouchControllerLayout);
            }
        }
    }
}

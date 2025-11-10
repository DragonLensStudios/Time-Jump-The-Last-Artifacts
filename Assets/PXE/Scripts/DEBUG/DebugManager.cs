using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Levels;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.UI.Messaging.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.DEBUG
{
    public class DebugManager : ObjectController
    {
        public static DebugManager Instance { get; private set; }
        
        [field: Tooltip("The binding for the debug menu.")]
        [field: SerializeField] public InputActionReference DebugMenuOpen { get; set; }
        
        [field: Tooltip("The debug canvas object.")]
        [field: SerializeField] public ObjectController DebugCanvas { get; set; }
        
        [field: Tooltip("The list of levels.")]
        [field: SerializeField] public List<LevelObject> Levels { get; set; } = new();
        
        [field: Tooltip("The debug menu message display time.")]
        [field: SerializeField] public float DebugMenuMessageDisplayTime { get; set; } = 2f;
        
        public override void Start()
        {
            base.Start();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("DEBUG MODE ENABLED\nPress F1 (Keyboard) or Select (Controller) to access the debug tools.", PopupType.Notification, PopupPosition.Top, DebugMenuMessageDisplayTime));
        }

        public override void Awake()
        {
#if !DEBUG
            Destroy(gameObject);
            return;
#endif
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
        }

        public override void OnActive()
        {
            base.OnActive();
            if (DebugMenuOpen == null) return;
            DebugMenuOpen.action.Enable();
            DebugMenuOpen.action.performed += DebugMenuOpenOnperformed;
        }

        public override void OnInactive()
        {
            base.OnInactive();
            if (DebugMenuOpen == null) return;
            DebugMenuOpen.action.Disable();
            DebugMenuOpen.action.performed -= DebugMenuOpenOnperformed;
        }

        public virtual void ShowDebugMenu(bool isActive)
        {
            if (DebugCanvas != null)
            {
                DebugCanvas.SetObjectActive(isActive);
            }
        }
        
        public virtual void DebugMenuOpenOnperformed(InputAction.CallbackContext input)
        {
            ShowDebugMenu(!DebugCanvas.IsActive);
        }
    }
}

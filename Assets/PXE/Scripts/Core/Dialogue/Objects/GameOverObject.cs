using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using UnityEngine;

namespace PXE.Core.Dialogue.Objects
{
    [RequireComponent(typeof(Collider2D))]
    public class GameOverObject : ObjectController
    {
        [field: Tooltip("The gizmo color.")]
        [field: SerializeField]
        public virtual Color GizmoColor { get; set; } = Color.red;

        [field: Tooltip("The collider.")]
        [field: SerializeField]
        public virtual Collider2D Col { get; set; }

        private void Reset()
        {
            Col = GetComponent<Collider2D>();
            Col.isTrigger = true;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow,
                new Messaging.Messages.EndLevelMessage(false));
            // I want to trigger the game over state from the state system
        }
    }
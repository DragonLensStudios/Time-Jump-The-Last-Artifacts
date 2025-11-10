using PXE.Core.Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PXE.Core.UI
{
    public class ScrollRectNavigator : ObjectController
    {
        [field: SerializeField] public virtual float ScrollSpeed { get; set; } = 10f;
        [field: SerializeField] public virtual InputActionReference ScrollAction { get; set; }

        protected GameObject lastSelectedObject;
        protected ScrollRect scrollRect;

        public override void Start()
        {
            base.Start();
            scrollRect = GetComponent<ScrollRect>();
        }

        public override void OnActive()
        {
            base.OnActive();
            if (ScrollAction == null || ScrollAction.action == null) return;
            ScrollAction.action.Enable();
            ScrollAction.action.performed += OnScroll;
            ScrollAction.action.canceled += OnScroll;
        }

        public override void OnInactive()
        {
            base.OnInactive();
            if (ScrollAction == null || ScrollAction.action == null) return;
            ScrollAction.action.Disable();
            ScrollAction.action.performed -= OnScroll;
            ScrollAction.action.canceled -= OnScroll;
        }
        
        protected virtual void OnScroll(InputAction.CallbackContext input)
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject == null) return;
            if (selectedObject.transform.IsChildOf(scrollRect.content))
            {
                RectTransform selectedRectTransform = selectedObject.GetComponent<RectTransform>();
                EnsureVisible(selectedRectTransform);
            }

            lastSelectedObject = selectedObject;
        }
        
        protected virtual void EnsureVisible(RectTransform selectedRectTransform)
        {
            Canvas.ForceUpdateCanvases();

            // Calculate the bounds of the selected item in the space of the viewport
            Bounds selectedItemBoundsInViewport = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, selectedRectTransform);

            // Get the lower and upper bounds of the selected item
            float itemLowerEdge = selectedItemBoundsInViewport.min.y;
            float itemUpperEdge = selectedItemBoundsInViewport.max.y;

            // Calculate the difference from the edges of the viewport
            float lowerDifference = scrollRect.viewport.rect.min.y - itemLowerEdge;
            float upperDifference = itemUpperEdge - scrollRect.viewport.rect.max.y;

            // Determine if we need to scroll up or down
            if (upperDifference > 0)
            {
                // Item is above the viewport
                ScrollToPosition(upperDifference);
            }
            else if (lowerDifference > 0)
            {
                // Item is below the viewport
                ScrollToPosition(-lowerDifference);
            }
        }

        protected virtual void ScrollToPosition(float delta)
        {
            // Calculate the delta in terms of the content's height
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            float normalizedDelta = delta / (contentHeight - viewportHeight);
    
            // Apply the delta to the current normalized position, clamping to ensure it stays within valid bounds
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + normalizedDelta);
        }
    }
}

using PXE.Core.Objects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PXE.Core.UI
{
    public class TabButton : ObjectController, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public virtual TabGroup TabGroup { get; set; }
        [field: SerializeField] public virtual ObjectController TabContent { get; set; }
        [field: SerializeField] public virtual UnityEvent OnTabSelected { get; set; }
        [field: SerializeField] public virtual UnityEvent OnTabDeselected { get; set; }
        [field: SerializeField] public virtual Image Background { get; set; }

        public override void Start()
        {
            base.Start();
            if (TabGroup == null)
            {
                TabGroup = transform.GetComponentInParent<TabGroup>();
            }
            TabGroup.Subscribe(this); // Automatically add this button to the group.
            Background.color = TabGroup.TabIdleColor; // Initial color
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabSelected(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            TabGroup.OnTabExit(this);
        }

        public virtual void Select()
        {
            OnTabSelected?.Invoke();
        }

        public virtual void Deselect()
        {
            OnTabDeselected?.Invoke();
        }
    }
}
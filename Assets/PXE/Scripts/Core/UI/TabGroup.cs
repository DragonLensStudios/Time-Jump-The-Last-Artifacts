using System.Collections.Generic;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.UI
{
    public class TabGroup : ObjectController
    {
        [field: SerializeField] public virtual Color TabIdleColor { get; set; }
        [field: SerializeField] public virtual Color TabHoverColor { get; set; }
        [field: SerializeField] public virtual Color TabActiveColor { get; set; }
        [field: SerializeField] public virtual List<TabButton> TabButtons { get; set; } = new();
        [field: SerializeField] public virtual TabButton SelectedTab { get; set; }
        [field: SerializeField] public virtual TabButton DefaultTab { get; set; } // Added DefaultTab field


        public override void Start()
        {
            base.Start();
            // Automatically get all buttons and their associated contents.
            foreach (Transform child in transform)
            {
                TabButton tabButton = child.GetComponentInChildren<TabButton>();
                if (!tabButton) continue;
                TabButtons.Add(tabButton);
                if (tabButton.TabContent == null) continue;
                tabButton.TabContent.SetObjectActive(false);
            }
        }

        public override void OnActive()
        {
            base.OnActive();
            // If a DefaultTab is assigned, select it on Start.
            if (DefaultTab)
            {
                OnTabSelected(DefaultTab);
            }
        }

        public virtual void Subscribe(TabButton button)
        {
            TabButtons ??= new List<TabButton>();

            if (!TabButtons.Contains(button))
            {
                TabButtons.Add(button);
            }
        }

        public virtual void OnTabSelected(TabButton button)
        {
            SelectedTab = button;
            ResetTabs();
            button.Select();
            button.TabContent.SetObjectActive(true); // Show the associated content.
            button.Background.color = TabActiveColor; // Set the active color
        }

        public virtual void OnTabEnter(TabButton button)
        {
            // Change color on hover only if it's not the selected tab.
            if (SelectedTab != button)
            {
                button.Background.color = TabHoverColor;
            }
        }

        public virtual void OnTabExit(TabButton button)
        {
            // Revert to idle or active color based on selection.
            button.Background.color = SelectedTab != button ? TabIdleColor : TabActiveColor;
        }

        public virtual void ResetTabs()
        {
            foreach (TabButton button in TabButtons)
            {
                if (SelectedTab != null && button == SelectedTab) { continue; }
                button.Deselect();
                button.TabContent.SetObjectActive(false); // Hide all other content.
                button.Background.color = TabIdleColor; // Reset the button to idle color.
            }
        }
    }
}
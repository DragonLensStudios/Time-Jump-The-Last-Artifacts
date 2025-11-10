using System.Linq;
using PXE.Core.Achievements.Data;
using PXE.Core.Achievements.Managers;
using PXE.Core.Achievements.Messaging.Messages;
using PXE.Core.Achievements.ScriptableObjects;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PXE.Core.Achievements.UI
{
    /// <summary>
    /// Add list of achievements to screen
    /// Represents the AchievenmentListIngame.
    /// The AchievenmentListIngame class provides functionality related to achievenmentlistingame management.
    /// This class contains methods and properties that assist in managing and processing achievenmentlistingame related tasks.
    /// </summary>
    public class AchievenmentListIngame : ObjectController
    {
        [field: Tooltip("The settings for the achievement manager")]
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        
        [field: Tooltip("The ScrollContent object in the UI")]
        [field: SerializeField] public GameObject ScrollContent { get; set; }
        
        [field: Tooltip("The prefab for achievements")]
        [field: SerializeField] public GameObject Prefab { get; set; }
        
        [field: Tooltip("The UI object for the achievement menu")]
        [field: SerializeField] public GameObject Menu { get; set; }
        
        [field: Tooltip("The dropdown menu for filtering achievements")]
        [field: SerializeField] public TMP_Dropdown Filter { get; set; }
        
        [field: Tooltip("The text that displays the number of achievements")]
        [field: SerializeField] public TMP_Text CountText { get; set; }
        
        [field: Tooltip("The text that displays the percentage of achievements")]
        [field: SerializeField] public TMP_Text CompleteText { get; set; }
        
        [field: Tooltip("The scrollbar for the achievement menu")]
        [field: SerializeField] public Scrollbar Scrollbar { get; set; }
        
        [field: Tooltip("Should the achievements list be opened with the input?")]
        [field: SerializeField] public bool UseInputToOpenAchievementsList { get; set; } = true;
        
        protected PlayerInputActions playerInput;

        public override void Awake()
        {
            base.Awake();
            playerInput = new PlayerInputActions();
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            playerInput.Enable();
            playerInput.Player.OpenAchievementMenu.performed += OpenAchievementMenuOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<AchievementMenuMessage>(MessageChannels.UI, MenuMessageHandler);
        
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            playerInput.Disable();
            playerInput.Player.OpenAchievementMenu.performed -= OpenAchievementMenuOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<AchievementMenuMessage>(MessageChannels.UI, MenuMessageHandler);
        }

        private bool MenuOpen = false;
        
        /// <summary>
        /// Adds all achievements to the UI based on a filter
        /// </summary>
        /// Executes the AddAchievements method.
        /// Handles the AddAchievements functionality.
        /// </summary>
        public void AddAchievements(string Filter)
        {  
            foreach (Transform child in ScrollContent.transform)
            {
                Destroy(child.gameObject);
            }
            int AchievedCount = AchievementManager.Instance.GetAchievedCount();

            CountText.text = "" + AchievedCount + " / " + Manager.AchievementList.Count(x => !x.Key.Equals(Manager.FinalAchievementKey));
            CompleteText.text = "Complete (" + AchievementManager.Instance.GetAchievedPercentage() + "%)";

            for (int i = 0; i < Manager.AchievementList.Count; i ++)
            {
                var achievement = Manager.AchievementList[i];
                var achievementProgress = AchievementManager.Instance.PlayerAchievementProgress.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
                if((Filter.Equals("All")) || (Filter.Equals("Achieved") && achievementProgress.Achieved) || (Filter.Equals("Unachieved") && !achievementProgress.Achieved))
                {
                    AddAchievementToUI(Manager.AchievementList[i]);
                }
            }
            Scrollbar.value = 1;
        }

        /// <summary>
        /// Executes the AddAchievementToUI method.
        /// Handles the AddAchievementToUI functionality.
        /// </summary>
        public void AddAchievementToUI(Achievement Achievement)
        {
            UIAchievement UIAchievement = Instantiate(Prefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<UIAchievement>();
            UIAchievement.Set(Achievement);
            UIAchievement.transform.SetParent(ScrollContent.transform);
        }
        /// <summary>
        /// Filter out a set of locked or unlocked achievements
        /// </summary>
        public void ChangeFilter ()
        {
            AddAchievements(Filter.options[Filter.value].text);
        }

        /// <summary>
        /// Closes the UI window.
        /// Executes the CloseWindow method.
        /// Handles the CloseWindow functionality.
        /// </summary>
        public void CloseWindow()
        {
            MenuOpen = false;
            var menuOc = Menu.GetComponent<ObjectController>();
            if (menuOc != null)
            {
                menuOc.SetObjectActive(MenuOpen);
            }
            else
            {
                Menu.SetActive(MenuOpen);
            }
        }
        /// <summary>
        /// Opens the UI window.
        /// Executes the OpenWindow method.
        /// Handles the OpenWindow functionality.
        /// </summary>
        public void OpenWindow()
        {
            MenuOpen = true;
            var menuOc = Menu.GetComponent<ObjectController>();
            if (menuOc != null)
            {
                menuOc.SetObjectActive(MenuOpen);
            }
            else
            {
                Menu.SetActive(MenuOpen);
            }
            AddAchievements("All");
        }
        /// <summary>
        /// Toggles the state of the UI window open or closed
        /// Executes the ToggleWindow method.
        /// Handles the ToggleWindow functionality.
        /// </summary>
        public void ToggleWindow()
        {
            if (MenuOpen){
                CloseWindow();
            }
            else{
                OpenWindow();
            }
        }
     
        /// <summary>
        ///  Handles the OpenAchievementMenuOnperformed functionality.
        /// </summary>
        /// <param name="input"></param>
        private void OpenAchievementMenuOnperformed(InputAction.CallbackContext input)
        {
            if (!UseInputToOpenAchievementsList || GameManager.Instance.CurrentState is MainMenuState) return;
            ToggleWindow();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, MenuOpen));
        }
        
        /// <summary>
        /// Handles the MenuMessageHandler functionality.
        /// </summary>
        /// <param name="message"><see cref="AchievementMenuMessage"/></param>
        private void MenuMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AchievementMenuMessage>().HasValue) return;
            var data = message.Message<AchievementMenuMessage>().GetValueOrDefault();
            if(data.ShowMenu) OpenWindow();
            else CloseWindow();
        }
    }
}

using System.Collections.Generic;
using PXE.Core.Achievements.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Achievements.UI
{
    /// <summary>
    /// Controls the display of achievements on the screen
/// Represents the AchievementStack.
/// The AchievementStack class provides functionality related to achievementstack management.
/// This class contains methods and properties that assist in managing and processing achievementstack related tasks.
/// </summary>
//TODO: Convert this to an ObjectController
    public class AchievementStack : MonoBehaviour
    {
        [field: Tooltip("The settings for the achievement manager")]
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        
        [field: Tooltip("The panels where achievements will be spawned")]
        [field: SerializeField] public RectTransform[] StackPanels { get; set; }
        
        [field: Tooltip("The achievements that are waiting to be displayed")]
        [field: SerializeField] public List<UIAchievement> BackLog { get; set; } = new ();
        
        //TODO: Convert AchievementTemplate to an ObjectController
        [field: Tooltip("The template for achievements")]
        [field: SerializeField] public GameObject AchievementTemplate { get; set; }
        

        /// <summary>
        /// Add an achievement to screen if it fits, otherwise, add to the backlog list
        /// </summary>
        /// <param name="Index">Index of achievement to add</param>
        public void ScheduleAchievementDisplay (int Index)
        {
            var Spawned = Instantiate(AchievementTemplate).GetComponent<UIAchievement>();
            Spawned.AS = this;
            Spawned.Set(Manager.AchievementList[Index]);
        
            //If there is room on the screen
            if (GetCurrentStack().childCount < Manager.NumberOnScreen)
            {
                Spawned.transform.SetParent(GetCurrentStack(), false);
                Spawned.StartDeathTimer();
            }
            else
            {
                Spawned.gameObject.SetActive(false);
                BackLog.Add(Spawned);
            }
        }

        /// <summary>
        /// Find the box where achievements should be spawned
        /// </summary>
        public Transform GetCurrentStack () => StackPanels[(int)Manager.StackLocation].transform;

        /// <summary>
        /// Add one achievement from the backlog to the screen
        /// </summary>
        public void CheckBackLog ()
        {
            if (BackLog.Count <= 0) return;
            BackLog[0].transform.SetParent(GetCurrentStack(), false);
            BackLog[0].gameObject.SetActive(true);
            BackLog[0].StartDeathTimer();
            BackLog.RemoveAt(0);
        }
    }
}

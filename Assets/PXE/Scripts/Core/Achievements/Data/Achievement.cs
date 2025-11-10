using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Achievements.Data
{
    /// <summary>
    /// Represents the Achievement.
    /// The Achievement class provides functionality related to achievement management.
    /// This class contains methods and properties that assist in managing and processing achievement related tasks.
    /// </summary>
    [System.Serializable, CreateAssetMenu(fileName = "Achievement", menuName = "PXE/Game/Achievements/Achievement", order = 1)]
    public class Achievement : ScriptableObjectController
    {
        [field: Tooltip("Name used to unlock/set achievement progress")]
        [field: SerializeField] public string Key { get; set; }
    
        [field: Tooltip("The display name for an achievement. Shown to the user on the UI.")]
        [field: SerializeField] public string DisplayName { get; set; }
    
        [field: Tooltip("Description for an achievement. Shown to the user on the UI.")]
        [field: SerializeField] public string Description { get; set; }
        
        [field: Tooltip("If true, the lock/achieved icon will be displayed")]
        [field: SerializeField] public bool UseIcon { get; set; }
    
        [field: Tooltip("The icon which will be displayed when the achievement is locked")]
        [field: SerializeField] public Sprite LockedIcon { get; set; }
    
        [field: Tooltip("The icon which will be displayed when the achievement is  Achieved")]
        [field: SerializeField] public Sprite AchievedIcon { get; set; }
    
        [field: Tooltip("Treat the achievement as a spoiler for the game. Hidden from player until unlocked.")]
        [field: SerializeField] public bool Spoiler { get; set; }
    
        [field: Tooltip("If true, this achievement will count to a certain amount before unlocking. E.g. race a total of 500 km, collect 10 coins or reach a high score of 25.")]
        [field: SerializeField] public bool Progression { get; set; }
    
        [field: Tooltip("The goal which must be reached for the achievement to unlock.")]
        [field: SerializeField] public float ProgressGoal { get; set; }
        
        [field: Tooltip("The rate that progress updates will be displayed on the screen e.g. Progress goal = 100 and Notification Frequency = 25. In this example, the progress will be displayed at 25,50,75 and 100.")]
        [field: SerializeField] public float NotificationFrequency { get; set; }
    
        [field: Tooltip("A string which will be displayed with a progress achievement e.g. $, KM, Miles etc")]
        [field: SerializeField] public string ProgressSuffix { get; set; }
    
        [field: Tooltip("The sound which plays when an achievement is unlocked is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string AchievedSound { get; set; }
    
        [field: Tooltip("The sound which plays when a progress update is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string ProgressMadeSound { get; set; }
    }
}
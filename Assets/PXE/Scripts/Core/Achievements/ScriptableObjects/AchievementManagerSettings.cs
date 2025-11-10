using System.Collections.Generic;
using PXE.Core.Achievements.Data;
using PXE.Core.Enums;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Achievements.ScriptableObjects
{
    /// <summary>
    ///  The settings for the achievement manager.
    /// </summary>
    [System.Serializable, CreateAssetMenu(fileName = "Achievement Manager Settings", menuName = "PXE/Game/Achievements/Achievement Manager Settings", order = 0)]
    public class AchievementManagerSettings : ScriptableObjectController
    {
        [field: SerializeField] public List<Achievement> AchievementList { get; set; }= new ();
    
        [field: Tooltip("The number of seconds an achievement will stay on the screen after being unlocked or progress is made.")]
        [field: SerializeField] public float DisplayTime { get; set; } = 3;
    
        [field: Tooltip("The total number of achievements which can be on the screen at any one time.")]
        [field: SerializeField] public int NumberOnScreen { get; set; } = 3;
    
        [field: Tooltip("If true, progress notifications will display their exact progress. If false it will show the closest bracket.")]
        [field: SerializeField] public bool ShowExactProgress { get; set; } = false;
    
        [field: Tooltip("If true, achievement unlocks/progress update notifications will be displayed on the player's screen.")]
        [field: SerializeField] public bool DisplayAchievements { get; set; }
    
        [field: Tooltip("The location on the screen where achievement notifications should be displayed.")]
        [field: SerializeField] public AchievementStackLocation StackLocation{ get; set; }

        [field: Tooltip("The message which will be displayed on the UI if an achievement is marked as a spoiler.")]
        [field: SerializeField] public string SpoilerAchievementMessage { get; set; } = "Hidden";
    
        [field: Tooltip("The sound which plays when an achievement is unlocked is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string DefaultAchievedSound { get; set; }
    
        [field: Tooltip("The sound which plays when a progress update is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string DefaultProgressMadeSound { get; set; }
    
        [field: Tooltip("If true, one achievement will be automatically unlocked once all others have been completed")]
        [field: SerializeField] public bool UseFinalAchievement { get; set; } = false;
    
        [field: Tooltip("The key of the final achievement")]
        [field: SerializeField] public string FinalAchievementKey { get; set; }
    
    }
}
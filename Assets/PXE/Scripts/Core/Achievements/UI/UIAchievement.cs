using System.Collections;
using System.Linq;
using PXE.Core.Achievements.Data;
using PXE.Core.Achievements.ScriptableObjects;
using PXE.Core.Objects;
using PXE.Core.Player.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Core.Achievements.UI
{
    /// <summary>
    /// Defines the logic behind a single achievement on the UI
/// Represents the UIAchievement.
/// The UIAchievement class provides functionality related to uiachievement management.
/// This class contains methods and properties that assist in managing and processing uiachievement related tasks.
/// </summary>
    public class UIAchievement : ObjectController
    {
        [field: Tooltip("The settings for the achievement manager")]
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        
        [field: Tooltip("The text that displays the title of the achievement")]
        [field: SerializeField] private TMP_Text Title { get; set; }
        
        [field: Tooltip("The text that displays the description of the achievement")]
        [field: SerializeField] private TMP_Text Description { get; set; }
        
        [field: Tooltip("The text that displays the percentage of the achievement")]
        [field: SerializeField] private TMP_Text Percent { get; set; }
        
        [field: Tooltip("The icon that displays the achievement")]
        [field: SerializeField] private Image OverlayIcon { get; set; }
        
        [field: Tooltip("The progress bar that displays the achievement")]
        [field: SerializeField] private Image ProgressBar { get; set; }
        
        [field: Tooltip("The overlay that displays the spoiler message")]
        [field: SerializeField] private GameObject SpoilerOverlay { get; set; }
        
        [field: Tooltip("The text that displays the spoiler message")]
        [field: SerializeField] private TMP_Text SpoilerText { get; set; }
        
        [HideInInspector] public AchievementStack AS;

        /// <summary>
        /// Destroy object after a certain amount of time
        /// </summary>
        public void StartDeathTimer ()
        {
            StartCoroutine(Wait());
        }

        /// <summary>
        /// Add information  about an Achievement to the UI elements
        /// </summary>
        public void Set (Achievement achievement)
        {
            var spoilerOverlayOc = SpoilerOverlay.GetComponent<ObjectController>();
            var overlayIconOc = OverlayIcon.gameObject.GetComponent<ObjectController>();
            var achievementProgress  = PlayerManager.Instance.Player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress == null) return;
            if(achievement.Spoiler && !achievementProgress.Achieved)
            {
                if (spoilerOverlayOc != null)
                {
                    spoilerOverlayOc.SetObjectActive(true);
                }
                else
                {
                    SpoilerOverlay.SetActive(true);
                }
                SpoilerText.text = Manager.SpoilerAchievementMessage;
            }
            else
            {
                if (!achievement.Spoiler)
                {
                    if (spoilerOverlayOc != null)
                    {
                        spoilerOverlayOc.SetObjectActive(false);
                    }
                }
                Title.text = achievement.DisplayName;
                Description.text = achievement.Description;

                if (achievement.UseIcon && !achievementProgress.Achieved)
                {
                    if (achievement.LockedIcon == null) return;
                    if (overlayIconOc != null)
                    {
                        overlayIconOc.SetObjectActive(true);
                    }
                    else
                    {
                        OverlayIcon.gameObject.SetActive(true);
                    }
                    OverlayIcon.sprite = achievement.LockedIcon;
                }
                else if (achievement.UseIcon && achievementProgress.Achieved)
                {
                    if (achievement.AchievedIcon == null) return;
                    if (overlayIconOc != null)
                    {
                        overlayIconOc.SetObjectActive(true);
                    }
                    else
                    {
                        OverlayIcon.gameObject.SetActive(true);
                    }
                    OverlayIcon.sprite = achievement.AchievedIcon;
                }

                if (!achievement.UseIcon)
                {
                    if (overlayIconOc != null)
                    {
                        overlayIconOc.SetObjectActive(false);
                    }
                    else
                    {
                        OverlayIcon.gameObject.SetActive(false);
                    }
                }

                if (achievement.Progression)
                {
                    float CurrentProgress = Manager.ShowExactProgress ? achievementProgress.Progress : (achievementProgress.LastProgressUpdate * achievement.NotificationFrequency);
                    float DisplayProgress = achievementProgress.Achieved ? achievement.ProgressGoal : CurrentProgress;

                    if (achievementProgress.Achieved)
                    {
                        Percent.text = achievement.ProgressGoal + achievement.ProgressSuffix + " / " + achievement.ProgressGoal + achievement.ProgressSuffix + " (Achieved)";
                    }
                    else
                    {
                        Percent.text = DisplayProgress + achievement.ProgressSuffix +  " / " + achievement.ProgressGoal + achievement.ProgressSuffix;
                    }

                    ProgressBar.fillAmount = DisplayProgress / achievement.ProgressGoal;
                }
                else //Single Time
                {
                    ProgressBar.fillAmount = achievementProgress.Achieved ? 1 : 0;
                    Percent.text = achievementProgress.Achieved ? "(Achieved)" : "(Locked)";
                }
            }
        }

        private IEnumerator Wait ()
        {
            yield return new WaitForSeconds(Manager.DisplayTime);
            GetComponent<Animator>().SetTrigger("ScaleDown");
            yield return new WaitForSeconds(0.1f);
            AS.CheckBackLog();
            Destroy(gameObject);
        }
    }

}

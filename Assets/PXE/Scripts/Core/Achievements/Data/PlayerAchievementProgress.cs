using System.Linq;
using PXE.Core.Achievements.Managers;
using UnityEngine;

namespace PXE.Core.Achievements.Data
{
    [System.Serializable]
    public class PlayerAchievementProgress
    {
        [field: Tooltip("The Achievement key to match")]
        [field: SerializeField] public string AchievementKey { get; set; }
        
        [field: Tooltip("The progress for the achievement")]
        [field: SerializeField] public float Progress { get; set; } = 0f;   
        
        [field: Tooltip("The last progress for the achievement")]
        [field: SerializeField] public float LastProgressUpdate { get; set; } = 0f;
        
        [field: Tooltip("When the achievement is achieved this is true.")]
        [field: SerializeField] public bool Achieved { get; set; } = false;
        
        public PlayerAchievementProgress(string key)
        {
            AchievementKey = key;
        }
        
        public void AddProgress(float value)
        {
            var achievement = AchievementManager.Instance.Manager.AchievementList.FirstOrDefault(x => x.Key.Equals(AchievementKey));
            if(achievement == null) return;
            Progress += value;
            if(Progress >= achievement.ProgressGoal)
            {
                Unlock();
            }
        }
        
        public void SubtractProgress(float value)
        {
            Progress -= value;
            if (Progress < 0)
            {
                Progress = 0;
            }
        }
        
        public void SetProgress(float value)
        {
            var achievement = AchievementManager.Instance.Manager.AchievementList.FirstOrDefault(x => x.Key.Equals(AchievementKey));
            if(achievement == null) return;
            Progress = value;
            if (Progress >= achievement.ProgressGoal)
            {
                Unlock();
            }
        }
        
        public void Unlock()
        {
            Achieved = true;
        }
        
        public void Lock()
        {
            Achieved = false;
            Progress = 0;
        }
    }
}
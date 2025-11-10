using System.Collections.Generic;
using System.Linq;
using PXE.Core.Achievements.Data;
using PXE.Core.Achievements.Messaging.Messages;
using PXE.Core.Achievements.ScriptableObjects;
using PXE.Core.Achievements.UI;
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.Player.Managers;
using UnityEngine;

namespace PXE.Core.Achievements.Managers
{
    [System.Serializable]
    public class AchievementManager : ObjectController
    {
        public static AchievementManager Instance = null;
        
        [field: Tooltip("The player achievement progress list.")]
        [field: SerializeField] public virtual List<PlayerAchievementProgress> PlayerAchievementProgress { get; set; }
        
        [field: Tooltip("The achievement manager settings.")]
        [field: SerializeField] public virtual AchievementManagerSettings Manager { get; set; }
        
        [field: Tooltip("The achievement stack.")]
        [field: SerializeField] public virtual AchievementStack AchievementStack { get; set; }
        
        protected PlayerController player;

        /// <summary>
        ///  Ensures that there is only one instance of the AchievementManager and loads the achievements.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            base.Awake();
            AchievementStack = GetComponentInChildren<AchievementStack>();
            LoadAchievements();
        }

        /// <summary>
        ///  Checks if the player is not null and sets the player achievement progress list.
        /// </summary>
        public override void Start()
        {
            base.Start();
            player = PlayerManager.Instance.Player;
            if(player == null) return;
            PlayerAchievementProgress = player.AchievementProgressList;
        }

        /// <summary>
        ///  Registers for the achievement message channel.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<AchievementMessage>(MessageChannels.Achievement, HandleAchievementMessage);

        }

        /// <summary>
        ///  Unregisters for the achievement message channel.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<AchievementMessage>(MessageChannels.Achievement, HandleAchievementMessage);
        }
        
        /// <summary>
        ///  Displays the unlock for the achievement provided the index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void DisplayUnlock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if(achievement == null) return;;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress == null) return;
            
            if ((!Manager.DisplayAchievements || achievement.Spoiler) && !achievementProgress.Achieved) return;

            //If not achieved
            if (achievement.Progression && achievementProgress.Progress < achievement.ProgressGoal)
            {
                int Steps = (int)achievement.ProgressGoal / (int)achievement.NotificationFrequency;

                //Loop through all notification point backwards from last possible option
                for (int i = Steps; i > achievementProgress.LastProgressUpdate; i--)
                {
                    //When it finds the largest valid notification point
                    if (achievementProgress.Progress >= achievement.NotificationFrequency * i)
                    {
                        PlaySfx(!string.IsNullOrEmpty(achievement.ProgressMadeSound)
                            ? achievement.ProgressMadeSound
                            : Manager.DefaultProgressMadeSound);
                        
                        achievementProgress.LastProgressUpdate = i;
                        AchievementStack.ScheduleAchievementDisplay(index);
                        return;
                    }
                }
            }
            else
            {
                PlaySfx(!string.IsNullOrEmpty(achievement.AchievedSound)
                    ? achievement.AchievedSound
                    : Manager.DefaultAchievedSound);
                AchievementStack.ScheduleAchievementDisplay(index);
            }
        }
        
#region Unlock and Progress
        /// <summary>
        ///  Unlocks the achievement provided the key.
        /// </summary>
        /// <param name="key"></param>
        public virtual void Unlock(string key)
        {
            Unlock(FindAchievementIndex(key));
        }

        /// <summary>
        ///  Unlocks the achievement provided the index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void Unlock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress == null) return;
            if (achievementProgress.Achieved) return;
            achievementProgress.Progress = achievement.ProgressGoal;
            achievementProgress.Achieved = true;
            DisplayUnlock(index);

            if (!Manager.UseFinalAchievement) return;
            var allCompleted = player.AchievementProgressList.All(x => !x.AchievementKey.Equals(Manager.FinalAchievementKey) && x.Achieved);
            if (allCompleted)
            {
                Unlock(Manager.FinalAchievementKey);
            }
            
        }
        
        /// <summary>
        ///  Locks the achievement provided the key.
        /// </summary>
        /// <param name="key"></param>
        public virtual void Lock(string key)
        {
            Lock(FindAchievementIndex(key));
        }

        /// <summary>
        ///  Locks the achievement provided the index.
        /// </summary>
        /// <param name="index"></param>
        public virtual void Lock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress is not { Achieved: true }) return;
            achievementProgress.Progress = 0;
            achievementProgress.Achieved = false;
        }

        /// <summary>
        ///  Sets the achievement progress provided the key and progress.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="progress"></param>
        public virtual void SetAchievementProgress(string Key, float progress)
        {
            SetAchievementProgress(FindAchievementIndex(Key), progress);
        }

        /// <summary>
        ///  Sets the achievement progress provided the index and progress.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="progress"></param>
        public virtual void SetAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));

            if (achievementProgress.Progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
            else
            {
                achievementProgress.Progress = progress;
                DisplayUnlock(index);

            }
        }

        /// <summary>
        ///  Adds the achievement progress provided the key and progress.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progress"></param>
        public virtual void AddAchievementProgress(string key, float progress)
        {
            AddAchievementProgress(FindAchievementIndex(key), progress);
        }

        /// <summary>
        ///  Adds the achievement progress provided the index and progress.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="progress"></param>
        public virtual void AddAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
           if(achievementProgress == null) return;
            if (!achievement.Progression) return;
            if (achievementProgress.Progress + progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
            else
            {
                achievementProgress.Progress += progress;
                DisplayUnlock(index);
            }
        }
        
        /// <summary>
        ///  Subtracts the achievement progress provided the key and progress.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progress"></param>
        public virtual void SubtractAchievementProgress(string key, float progress)
        {
            SubtractAchievementProgress(FindAchievementIndex(key), progress);
        }
        
        /// <summary>
        ///  Subtracts the achievement progress provided the index and progress.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="progress"></param>
        public virtual void SubtractAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            achievementProgress.Progress -= progress;
            DisplayUnlock(index);
            if (achievementProgress.Progress - progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
        }
#endregion

# region Miscellaneous

        /// <summary>
        ///  Checks if the achievement exists provided the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool AchievementExists(string key)
        {
            return Manager.AchievementList.Exists(x => x.Key.Equals(key));
        }

        /// <summary>
        ///  Gets the player achieved count based on the player achievement progress list.
        /// </summary>
        /// <returns></returns>
        public virtual int GetAchievedCount()
        {
            return player == null ? 0 : player.AchievementProgressList.Count(x => x.Achieved);
        }
        
        /// <summary>
        ///  Gets the achieved percentage based on the player achievement progress list.
        /// </summary>
        /// <returns></returns>
        public virtual float GetAchievedPercentage()
        {
            if (player == null) return 0;
            if (player.AchievementProgressList.Count(x => x.Achieved) == 0) return 0;
            return (float)GetAchievedCount() / Manager.AchievementList.Count * 100f;
        }
        #endregion

        #region Persistence
        
        /// <summary>
        ///  Loads the achievements from the resources folder.
        /// </summary>
        public virtual void LoadAchievements()
        {
            if(Manager == null) return;
            if (Manager.AchievementList.Count <= 0)
            {
                Manager.AchievementList = Resources.LoadAll<Achievement>("Achievements").ToList();
            }
        }

        /// <summary>
        /// Resets the achievement provided the key.
        /// </summary>
        /// <param name="key"></param>
        public virtual void ResetAchievement(string key)
        {
            if (player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(key));
            if (achievementProgress == null) return;
            achievementProgress.Progress = 0;
            achievementProgress.LastProgressUpdate = 0;
            achievementProgress.Achieved = false;
        }
        
        /// <summary>
        ///  Resets all the achievements.
        /// </summary>
        public virtual void ResetAllAchievements()
        {
            if (player == null) return;
            var achievements = player.AchievementProgressList;
            for (int i = 0; i < achievements.Count; i++)
            {
                achievements[i].Progress = 0;
                achievements[i].LastProgressUpdate = 0;
                achievements[i].Achieved = false;
            }
        }
        #endregion
        
        /// <summary>
        ///  Finds the achievement index provided the key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual int FindAchievementIndex(string Key)
        {
            return Manager.AchievementList.FindIndex(x => x.Key.Equals(Key));
        }

        /// <summary>
        ///  Plays the sound provided the sound name.
        /// </summary>
        /// <param name="soundName"></param>
        public virtual void PlaySfx (string soundName)
        {
            if(!string.IsNullOrWhiteSpace(soundName))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(soundName, AudioOperation.Play, AudioChannel.SoundEffects));
            }
        }
        
        /// <summary>
        ///  Plays the sound provided the audio object.
        /// </summary>
        /// <param name="audioObject"></param>
        public virtual void PlaySfx (AudioObject audioObject)
        {
            if(audioObject != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(audioObject, AudioOperation.Play, AudioChannel.SoundEffects));
            }
        }
        
        /// <summary>
        ///  Handles the achievement message provided handing the achievement operator for Add, Subtract, Set, Unlock, and Lock.
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandleAchievementMessage(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AchievementMessage>().HasValue) return;
            var data = message.Message<AchievementMessage>().GetValueOrDefault();
            switch (data.Operator)
            {
                case AchievementOperator.Add:
                    AddAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Subtract:
                    SubtractAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Set:
                    SetAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Unlock:
                    Unlock(data.AchievementKey);
                    break;
                case AchievementOperator.Lock:
                    Lock(data.AchievementKey);
                    break;
            }
        }
    }
}
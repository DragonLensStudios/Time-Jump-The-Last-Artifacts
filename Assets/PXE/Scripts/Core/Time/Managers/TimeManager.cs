using System;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.Time.Data;
using PXE.Core.Time.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Time.Managers
{
    /// <summary>
/// Represents the TimeManager.
/// The TimeManager class provides functionality related to Time Manager management.
/// This class contains methods and properties that assist in managing and processing Time Manager related tasks.
/// </summary>
    public class TimeManager : ObjectController, IDataPersistable
    {
        /// <summary>
        ///  Singleton instance for the TimeManager.
        /// </summary>
        public static TimeManager Instance { get; private set; }
        
        [field: Tooltip("The current time object.")]
        [field: SerializeField] public virtual GameTimeObject CurrentTimeObject { get; set; }
        
        [field: Tooltip("is the game paused?")]
        [field: SerializeField] public virtual bool IsPaused { get; set; }


        /// <summary>
        ///  Singleton pattern for the time manager.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
        }

        /// <summary>
        ///  This method registers the TimeManager for the TimeMessage message and the LevelResetMessage message and the PauseMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<TimeMessage>(MessageChannels.Time, HandleTimeMessage);
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }

        /// <summary>
        ///  This method unregisters the TimeManager for the TimeMessage message and the LevelResetMessage message and the PauseMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<TimeMessage>(MessageChannels.Time, HandleTimeMessage);
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }
            
        /// <summary>
        ///  Updates the current time object if it exists and the game is in the playing state and the game is not paused.
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (CurrentTimeObject == null || !GameManager.Instance.IsCurrentState<GamePlayingState>() || IsPaused) return;
            CurrentTimeObject.StartTime();
        }

        /// <summary>
        ///  Resets the current time object if it exists on application quit.
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            if (CurrentTimeObject != null)
            {
                CurrentTimeObject.ResetFullDate();
            }
        }
        
        /// <summary>
        ///  Handles the time message and based on the operation sets, adds, subtracts, multiplies, or divides the time.
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandleTimeMessage(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<TimeMessage>().HasValue) return;
            if(CurrentTimeObject == null) return;
            var data = message.Message<TimeMessage>().GetValueOrDefault();

            switch (data.Operation)
            {
                case TimeOperation.Set:
                    if (data.Year.HasValue) CurrentTimeObject.Year = data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month = data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week = data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day = data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour = data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute = data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second = data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond = data.MilliSecond.Value;
                    break;

                case TimeOperation.Add:
                    if (data.Year.HasValue) CurrentTimeObject.Year += data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month += data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week += data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day += data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour += data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute += data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second += data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond += data.MilliSecond.Value;
                    break;

                case TimeOperation.Subtract:
                    if (data.Year.HasValue) CurrentTimeObject.Year -= data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month -= data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week -= data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day -= data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour -= data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute -= data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second -= data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond -= data.MilliSecond.Value;
                    break;

                case TimeOperation.Multiply:
                    if (data.Year.HasValue) CurrentTimeObject.Year *= data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month *= data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week *= data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day *= data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour *= data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute *= data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second *= data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond *= data.MilliSecond.Value;
                    break;

                case TimeOperation.Divide:
                    if (data.Year.HasValue && data.Year.Value != 0) CurrentTimeObject.Year /= data.Year.Value;
                    if (data.Month.HasValue && data.Month.Value != 0) CurrentTimeObject.Month /= data.Month.Value;
                    if (data.Week.HasValue && data.Week.Value != 0) CurrentTimeObject.Week /= data.Week.Value;
                    if (data.Day.HasValue && data.Day.Value != 0) CurrentTimeObject.Day /= data.Day.Value;
                    if (data.Hour.HasValue && data.Hour.Value != 0) CurrentTimeObject.Hour /= data.Hour.Value;
                    if (data.Minute.HasValue && data.Minute.Value != 0) CurrentTimeObject.Minute /= data.Minute.Value;
                    if (data.Second.HasValue && data.Second.Value != 0) CurrentTimeObject.Second /= data.Second.Value;
                    if (data.MilliSecond.HasValue && data.MilliSecond.Value != 0) CurrentTimeObject.MilliSecond /= data.MilliSecond.Value;
                    break;
            }
            CurrentTimeObject.ValidateTime();
        }
        /// <summary>
        ///  Handles the level reset message and resets the current time object.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LevelResetMessage>().HasValue) return;
            // CurrentTimeObject.ResetFullDate();
        }
        
        /// <summary>
        ///  Handles the pause message and sets the is paused property.
        /// </summary>
        /// <param name="message"></param>
        public virtual void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            IsPaused = data.IsPaused;
        }

        public void LoadData<T>(T loadedGameData) where T : class, IGameDataContent, new()
        {
            if(loadedGameData is BaseGameData gameData)
            {
                if(!gameData.ID.Equals(ID)) return;
                CurrentTimeObject.Load(gameData.CurrentTime);
            }
        }

        public void SaveData<T>(T savedGameData) where T : class, IGameDataContent, new()
        {
            if (savedGameData is BaseGameData gameData)
            {
                gameData.ID = ID;
                gameData.Name = Name;
                gameData.CurrentTime = CurrentTimeObject.Save();
                gameData.LastUpdated = DateTime.Now;
            }

        }
        
    }
}
using PXE.Core.Audio;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.State_System
{
    [System.Serializable]
    public abstract class GameState : ScriptableObjectController
    {
        [field: Tooltip("The Enter sound effect." )]
        [field: SerializeField] public AudioObject EnterSfx { get; set; }
        
        [field: Tooltip("The Exit sound effect." )]
        [field: SerializeField] public AudioObject ExitSfx { get; set; }
        
        [field: Tooltip("The Enter background music." )]
        [field: SerializeField] public AudioObject EnterBgm { get; set; }
        
        [field: Tooltip("The Exit background music." )]
        [field: SerializeField] public AudioObject ExitBgm { get; set; }
        
        /// <summary>
        ///  Executes the Enter method checks for EnterSfx and EnterBgm and plays them.
        /// </summary>
        public virtual void Enter()
        {
            if (EnterSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(EnterSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            
            if (EnterBgm != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(EnterBgm, AudioOperation.Play, AudioChannel.SoundEffects));
            }
        }
        
        /// <summary>
        ///  Executes the Exit method checks for ExitSfx and ExitBgm and plays them.
        /// </summary>
        public virtual void Exit()
        {
            if (ExitSfx != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ExitSfx, AudioOperation.Play, AudioChannel.SoundEffects));
            }
            
            if (ExitBgm != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(ExitBgm, AudioOperation.Play, AudioChannel.SoundEffects));
            }
        }
        
        /// <summary>
        ///  Executes the Update method.
        /// </summary>
        public abstract void Update();


    }
}
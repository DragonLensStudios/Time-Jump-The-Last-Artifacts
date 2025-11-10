using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PXE.Core.UI.Options
{
/// <summary>
/// Represents the OptionsMenuController.
/// The OptionsMenuController class provides functionality related to optionsmenucontroller management.
/// This class contains methods and properties that assist in managing and processing optionsmenucontroller related tasks.
/// </summary>
    public class OptionsMenuController : MonoBehaviour
    {
        [field: SerializeField] public Scrollbar MasterVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text MasterVolumeText { get; set; }
        [field: SerializeField] public Scrollbar SFXVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text SFXVolumeText { get; set; }
        [field: SerializeField] public Scrollbar BGMVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text BGMVolumeText { get; set; }
        private void Start()
        {
            var masterVolume = Mathf.Pow(10, PlayerPrefs.GetFloat("masterVolume") / 20f);
            var sfxVolume = Mathf.Pow(10, PlayerPrefs.GetFloat("effectsVolume") / 20f);
            var bgmVolume = Mathf.Pow(10, PlayerPrefs.GetFloat("musicVolume") / 20f);
            MasterVolumeScrollbar.value = masterVolume;
            SFXVolumeScrollbar.value = sfxVolume;
            BGMVolumeScrollbar.value = bgmVolume;        
        }
/// <summary>
/// Executes the SetMasterVolume method.
/// Handles the SetMasterVolume functionality.
/// </summary>
        public void SetMasterVolume(float volume)
        {
            MasterVolumeText.text = $"Master Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, AudioChannel.Master,null, null, volume));
        }
        
/// <summary>
/// Executes the SetBGMVolume method.
/// Handles the SetBGMVolume functionality.
/// </summary>
        public void SetBGMVolume(float volume)
        {
            BGMVolumeText.text = $"BGM Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, AudioChannel.Music,null,null, volume));
        }
        
/// <summary>
/// Executes the SetSFXVolume method.
/// Handles the SetSFXVolume functionality.
/// </summary>
        public void SetSFXVolume(float volume)
        {
            SFXVolumeText.text = $"SFX Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, AudioChannel.SoundEffects, null, null, volume));
        }
    }
}

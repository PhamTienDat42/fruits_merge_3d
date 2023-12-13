using Services;
using UnityEngine;

using System.Collections.Generic;

namespace Audio
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] private List<Sound> sounds;

        private AudioService audioService;
        // Cache
        private readonly Dictionary<string, AudioSource> audioSources = new();

        private void Awake()
        {
            foreach (var sound in sounds)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.AudioClip;
                audioSource.playOnAwake = false;

                audioSources.Add(sound.Name, audioSource);
            }
        }

        private void OnDisable()
        {
            audioService.OnSoundChanged -= AudioService_OnSoundChanged;
        }

        private void AudioService_OnSoundChanged(bool isOn)
        {
            if (isOn == false)
            {
                foreach (var sound in audioSources)
                {
                    sound.Value.Stop();
                }
            }
        }

        public void PlaySound(string name)
        {
            if (audioSources.ContainsKey(name))
            {
                audioSources[name].Play();
            }
            else
            {
                Logger.Warning($"Sound: {name} not found!");
            }
        }

        public void StopSound(string name)
        {
            if (audioSources.ContainsKey(name))
            {
                audioSources[name].Stop();
            }
            else
            {
                Logger.Warning($"Sound: {name} not found!");
            }
        }

        public void Initialized(AudioService audioService)
        {
            this.audioService = audioService;

            audioService.OnSoundChanged += AudioService_OnSoundChanged;
        }
    }

}

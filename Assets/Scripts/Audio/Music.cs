using Services;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Audio
{
    public class Music : MonoBehaviour
    {
        [SerializeField] private List<Sound> musics;

        private AudioService audioService;

        // Cache
        private readonly Dictionary<string, AudioSource> audioSources = new();

        private void Awake()
        {
            foreach (var music in musics)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = music.AudioClip;
                audioSource.playOnAwake = false;

                audioSources.Add(music.Name, audioSource);
            }
        }

        private void OnDisable()
        {
            audioService.OnMusicChanged -= AudioService_OnMusicChanged;
        }

        private void AudioService_OnMusicChanged(bool isOn)
        {
            if (isOn == false)
            {
                foreach (var music in audioSources)
                {
                    music.Value.Stop();
                }
            }
        }

        public void PlayMusic(string name)
        {
            if (audioSources.ContainsKey(name))
            {
                audioSources[name].Play();
                audioSources[name].loop = true;
            }
            else
            {
                Logger.Warning($"Music: {name} not found!");
            }
        }

        public void FadeMusic(string name, float time)
        {
            if (audioSources.ContainsKey(name))
            {
                AudioSource audioSource = audioSources[name];
                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }
                StartCoroutine(FadeMusicCoroutine(audioSource, time));
            }
            else
            {
                Logger.Warning($"Music: {name} not found!");
            }
        }

        private IEnumerator FadeMusicCoroutine(AudioSource audioSource, float time)
        {
            float deltaT = 0.02f;
            float volumeTemp = audioSource.volume;
            float step = time / deltaT;
            float stepVolume = volumeTemp / step;

            for (int i = 0; i < 150; i++)
            {
                yield return new WaitForSeconds(0.01f);
                audioSource.volume -= stepVolume;
            }

            audioSource.Stop();
            audioSource.volume = volumeTemp;
        }

        public void StopMusic(string name)
        {
            if (audioSources.ContainsKey(name))
            {
                audioSources[name].Stop();
            }
            else
            {
                Logger.Warning($"Music: {name} not found!");
            }
        }

        public bool IsMusicPlaying(string name)
        {
            if (audioSources.ContainsKey(name))
            {
                if (audioSources[name].isPlaying == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            Logger.Warning($"Music: {name} not found!");
            return false;
        }

        public void Initialized(AudioService audioService)
        {
            this.audioService = audioService;

            audioService.OnMusicChanged += AudioService_OnMusicChanged;
        }
    }
}

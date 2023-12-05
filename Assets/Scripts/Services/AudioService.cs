using System;
using System.Collections.Generic;

using UnityEngine;

using Audio;

namespace Services
{
    public class AudioService
    {
        public event Action<bool> OnSoundChanged;
        public event Action<bool> OnMusicChanged;

        private bool soundOn;
        private bool musicOn;
        private bool vibrateOn;

        private readonly Music music;
        private readonly Dictionary<string, AudioSource> soundAudioSources;

        public AudioService(Music music, List<Sound> sounds, GameObject soundObject)
        {
            this.music = music;
            this.music.Initialized(this);

            soundAudioSources = new Dictionary<string, AudioSource>();
            foreach (var sound in sounds)
            {
                AudioSource soundSource = soundObject.AddComponent<AudioSource>();
                soundSource.clip = sound.AudioClip;
                soundSource.volume = sound.Volume;
                soundSource.playOnAwake = false;
                soundAudioSources.Add(sound.Name, soundSource);
            }
        }

        // Play Sound
        public void PlaySfx(string name)
        {
            if (soundOn == true)
            {
                var audioSouce = soundAudioSources[name];
                audioSouce.Play();
            }
        }

        // Play Music
        public void PlayMusic(string name)
        {
            if (musicOn == true)
            {
                music.PlayMusic(name);
            }
        }

        // Fade Music
        public void FadeMusic(float time)
        {
            if (musicOn == true)
            {
                music.FadeMusic(Constants.BgMusicName, time);
            }
        }

        // End
        public void StopAllSound()
        {
            foreach (var audioSource in soundAudioSources)
            {
                audioSource.Value.Stop();
            }
        }

        public void StopMusic(string name)
        {
            music.StopMusic(name);
        }

        public bool IsMusicPlaying()
        {
            return music.IsMusicPlaying(Constants.BgMusicName);
        }

        public void SetVibrate(bool isOn)
        {
            VibrateOn = isOn;
        }

        public void Vibrate()
        {
            if (vibrateOn == true)
            {
                Handheld.Vibrate();
            }
        }

        public bool SoundOn
        {
            get { return soundOn; }
            set
            {
                soundOn = value;
                if (soundOn == false)
                {
                    StopAllSound();
                }
                OnSoundChanged?.Invoke(soundOn);
            }
        }

        public bool MusicOn
        {
            get { return musicOn; }
            set
            {
                musicOn = value;
                if (musicOn == true)
                {
                    music.PlayMusic(Constants.BgMusicName);
                }
                else
                {
                    music.StopMusic(Constants.BgMusicName);
                }
                OnMusicChanged?.Invoke(musicOn);
            }
        }

        public bool VibrateOn
        {
            get { return vibrateOn; }
            set
            {
                vibrateOn = value;
            }
        }
    }
}

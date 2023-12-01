using UnityEngine;

namespace Services
{
    public class PlayerServices
    {
		public bool Sound { get; set; }
		public bool Effect { get; set; }
		public bool Haptic { get; set; }

		public PlayerServices()
		{
			Sound = PlayerPrefs.GetInt(Constants.SoundKey, 1) == 1;
			Effect = PlayerPrefs.GetInt(Constants.EffectKey, 1) == 1;
			Haptic = PlayerPrefs.GetInt(Constants.HapticKey, 1) == 1;
		}

		public void SettingsSave()
		{
			PlayerPrefs.SetInt(Constants.SoundKey, Sound ? 1 : 0);
			PlayerPrefs.SetInt(Constants.EffectKey, Effect ? 1 : 0);
			PlayerPrefs.SetInt(Constants.HapticKey, Haptic ? 1 : 0);

			PlayerPrefs.Save();
		}
	}
}

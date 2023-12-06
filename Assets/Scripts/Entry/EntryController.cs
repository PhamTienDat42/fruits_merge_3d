using System.Collections.Generic;
using Audio;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		[SerializeField] private EntryModel entryModel;
		[SerializeField] private Music music;
		[SerializeField] private List<Sound> sounds;

		private GameServices gameServices = null;

		private void Awake()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
				GameObject gameServicesObject = new(nameof(GameServices))
				{
					tag = Constants.ServicesTag
				};
				gameServices = gameServicesObject.AddComponent<GameServices>();
			}

			DontDestroyOnLoad(music.gameObject);
			GameObject soundObject = new(Constants.SoundObjectName);
			DontDestroyOnLoad(soundObject);

			gameServices.AddService(new PlayerServices());
			gameServices.AddService(new ParamServices());
			gameServices.AddService(new AudioService(music, sounds, soundObject));

			//---------------------GetServices----------------------------------------
			var playerService = gameServices.GetService<PlayerServices>();
			var audioService = gameServices.GetService<AudioService>();
			// --------------------------- Audio ---------------------------------
			audioService.SoundOn = playerService.Effect;
			audioService.MusicOn = playerService.Sound;
			audioService.VibrateOn = playerService.Haptic;
		}

		private void Start()
		{
			Application.targetFrameRate = Constants.TargetFrameRate;
			gameServices.GetService<AudioService>().PlayMusic(Constants.BgMusicName);
			SceneManager.LoadScene(Constants.HomeScene);
		}
	}
}

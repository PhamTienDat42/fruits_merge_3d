using System.Collections.Generic;
using Audio;
using Services;
using TransitionNS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		[SerializeField] private EntryModel entryModel;
		[SerializeField] private Transition transition;
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
			DontDestroyOnLoad(transition.gameObject);
			GameObject soundObject = new(Constants.SoundObjectName);
			DontDestroyOnLoad(soundObject);

			gameServices.AddService(new PlayerServices());
			gameServices.AddService(new ParamServices());
			gameServices.AddService(new AudioService(music, sounds, soundObject));
			gameServices.AddService(new TransitionService(transition));

			//---------------------GetServices----------------------------------------
			var playerService = gameServices.GetService<PlayerServices>();
			var audioService = gameServices.GetService<AudioService>();
			// --------------------------- Audio ---------------------------------
		}

		private void Start()
		{
			Application.targetFrameRate = Constants.TargetFrameRate;
			gameServices.GetService<AudioService>().PlayMusic(Constants.BgMusicName);
			SceneManager.LoadScene(Constants.HomeScene);
		}
	}
}

using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace Home
{
	public class HomeView : MonoBehaviour
	{
		[SerializeField] private HomeController homeController;
		[SerializeField] private TMP_Text highScoreTMP;

		[Space(8.0f)]
		[Header("Toggle")]
		[SerializeField] private Toggle musicToggle;
		[SerializeField] private Toggle sfxToggle;
		[SerializeField] private Toggle hapticToggle;

		private ParamServices paramServices;
		private PlayerServices playerServices;
		private AudioService audioService;

		private void Start()
		{
			paramServices = homeController.GameServices.GetService<ParamServices>();
			playerServices = homeController.GameServices.GetService<PlayerServices>();
			audioService = homeController.GameServices.GetService<AudioService>();

			audioService.SoundOn = playerServices.Effect;
			audioService.MusicOn = playerServices.Sound;
			audioService.VibrateOn = playerServices.Haptic;

			paramServices.CameraSize = Constants.DesignCamSize;
			ShowToggleValueStart();
			highScoreTMP.text = $"{PlayerPrefs.GetInt(Constants.HighScore, 0)}";
		}

		public void OnPlayButtonClick()
		{
			PlayButtonSfx();
			Time.timeScale = 1.0f;
			paramServices.IsContinue = false;
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnBilliardThemeButtonClick()
		{
			PlayButtonSfx();
			PlayerPrefs.SetInt(Constants.Theme, 0);
		}

		public void OnSportThemeButtonClick()
		{
			PlayButtonSfx();
			PlayerPrefs.SetInt(Constants.Theme, 1);
		}

		public void OnFruitThemeButtonClick()
		{
			PlayButtonSfx();
			PlayerPrefs.SetInt(Constants.Theme, 2);
		}

		public void OnContinueButtonClick()
		{
			PlayButtonSfx();
			Time.timeScale = 1.0f;
			paramServices.IsContinue = true;
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnRankingButtonClick()
		{
			PlayButtonSfx();
			PopupHelpers.Show(Constants.RankingPopup);
		}

		public void OnSoundToggleValueChanged()
		{
			PlayButtonSfx();
			playerServices.Sound = !musicToggle.isOn;
			audioService.MusicOn = !musicToggle.isOn;
		}

		public void OnSfxToggleValueChanged()
		{
			PlayButtonSfx() ;
			playerServices.Effect = !sfxToggle.isOn;
			audioService.SoundOn = !sfxToggle.isOn;
		}

		public void OnHapticToggleValueChanged()
		{
			PlayButtonSfx() ;
			playerServices.Haptic = !hapticToggle.isOn;
			audioService.VibrateOn = !hapticToggle.isOn;
		}

		public void OnSettingSave()
		{
			playerServices.SettingsSave();
		}

		public void PlayButtonSfx()
		{
			audioService.PlaySfx(Constants.ButtonSfxName);
		}

		public void ShowToggleValueStart()
		{
			musicToggle.isOn = !playerServices.Sound;
			sfxToggle.isOn = !playerServices.Effect;
			hapticToggle.isOn = !playerServices.Haptic;
		}
	}
}

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

		private void Start()
		{
			paramServices = homeController.GameServices.GetService<ParamServices>();
			playerServices = homeController.GameServices.GetService<PlayerServices>();

			ShowToggleValueStart();
			highScoreTMP.text = $"{PlayerPrefs.GetInt(Constants.HighScore, 0)}";
		}

		public void OnPlayButtonClick()
		{
			Time.timeScale = 1.0f;
			paramServices.IsContinue = false;
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnBilliardThemeButtonClick()
		{
			PlayerPrefs.SetInt(Constants.Theme, 0);
		}

		public void OnSportThemeButtonClick()
		{
			PlayerPrefs.SetInt(Constants.Theme, 1);
		}

		public void OnContinueButtonClick()
		{
			Time.timeScale = 1.0f;
			paramServices.IsContinue = true;
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnRankingButtonClick()
		{
			PopupHelpers.Show(Constants.RankingPopup);
		}

		public void OnSoundToggleValueChanged()
		{
			playerServices.Sound = !musicToggle.isOn;
			playerServices.SettingsSave();
		}

		public void OnSfxToggleValueChanged()
		{
			playerServices.Effect = !sfxToggle.isOn;
			playerServices.SettingsSave();
		}

		public void OnHapticToggleValueChanged()
		{
			playerServices.Haptic = !hapticToggle.isOn;
			playerServices.SettingsSave();
		}

		public void OnSettingSave()
		{
			playerServices.SettingsSave();
		}

		public void ShowToggleValueStart()
		{
			musicToggle.isOn = !playerServices.Sound;
			sfxToggle.isOn = !playerServices.Effect;
			hapticToggle.isOn = !playerServices.Haptic;
		}
	}
}

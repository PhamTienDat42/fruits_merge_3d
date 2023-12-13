using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace SettingPopup
{
	public class SettingView : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;
		[SerializeField] private TMP_Text confirmButtonTMP;
		[SerializeField] private TMP_Text highScoreTMP;

		private ParamServices paramServices;
		private AudioService audioService;
		private TransitionService transitionService;

		private event Action OnConfirmButtonClicked;

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				var gameServices = gameServiceObj.GetComponent<GameServices>();
				paramServices = gameServices.GetService<ParamServices>();
				audioService = gameServices.GetService<AudioService>();
				transitionService = gameServices.GetService<TransitionService>();
			}
			else
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}
		}

		private void Start()
		{
			if (paramServices.CameraSize != Constants.DesignCamSize)
			{
				mainCamera.orthographicSize = paramServices.CameraSize;
			}

			highScoreTMP.text = $"{PlayerPrefs.GetInt(Constants.HighScore, 0)}";

			if (paramServices.PopupTypeParam == PopupType.SettingPopup)
			{
				OnConfirmButtonClicked = OnContinueButtonClick;
				confirmButtonTMP.text = "Continue";
			}
			else
			{
				OnConfirmButtonClicked = OnPlayAgainButtonClick;
				confirmButtonTMP.text = "Play Again";
			}
		}


		public void OnConfirmButtonClick()
		{
			audioService.PlaySfx(Constants.ButtonSfxName);
			OnConfirmButtonClicked?.Invoke();
		}

		private void OnPlayAgainButtonClick()
		{
			Time.timeScale = 1.0f;
			audioService.PlaySfx(Constants.ButtonSfxName);
			SceneManager.LoadScene(Constants.GameScene);
		}

		private void OnContinueButtonClick()
		{
			Time.timeScale = 1.0f;
			audioService.PlaySfx(Constants.ButtonSfxName);
			PopupHelpers.Close();
		}

		public void OnHomeButtonClick()
		{
			Time.timeScale = 1.0f;
			audioService.PlaySfx(Constants.ButtonSfxName);
			transitionService.PlayEndTransition(Constants.HomeScene);
		}

		public void OnRankingButtonClick()
		{
			Time.timeScale = 1.0f;
			audioService.PlaySfx(Constants.ButtonSfxName);
			PopupHelpers.Show(Constants.RankingPopup);
		}
	}
}

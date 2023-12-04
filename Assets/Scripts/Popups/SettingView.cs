using Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace SettingPopup
{
	public class SettingView : MonoBehaviour
	{
		[SerializeField] private TMP_Text confirmButtonTMP;

		private ParamServices paramServices;
		private event Action OnConfirmButtonClicked;

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				var gameServices = gameServiceObj.GetComponent<GameServices>();
				paramServices = gameServices.GetService<ParamServices>();
			}
			else
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}
		}

		private void Start()
		{
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
			OnConfirmButtonClicked?.Invoke();
		}

		private void OnPlayAgainButtonClick()
		{
			Time.timeScale = 1.0f;
			SceneManager.LoadScene(Constants.GameScene);
		}

		private void OnContinueButtonClick()
		{
			Time.timeScale = 1.0f;
			PopupHelpers.Close();
		}

		public void OnHomeButtonClick()
		{
			Time.timeScale = 1.0f;
			SceneManager.LoadScene(Constants.HomeScene);
		}
	}
}

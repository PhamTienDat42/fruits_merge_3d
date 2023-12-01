using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameController gameController;
		[SerializeField] private GameModel gameModel;

		[Space(8.0f)]
		[Header("TMP")]
		[SerializeField] private TMP_Text currentScore;

		private ParamServices paramServices;

		private void Start()
		{
			paramServices = gameController.GameServices.GetService<ParamServices>();
		}

		public void UpdateCurrentScore()
		{
			currentScore.text = $"Score {gameModel.CurrentScore}";
		}

		public void OnEscButtonClick()
		{
			SceneManager.LoadScene(Constants.HomeScene);
		}

		public void OnPlayAgainButtonClick()
		{
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnSettingButtonClick()
		{
			Time.timeScale = 0.0f;
			paramServices.PopupTypeParam = PopupType.SettingPopup;
			PopupHelpers.Show(Constants.Popup);
		}

		public void ShowGameOverPopup()
		{
			Time.timeScale = 0.0f;
			paramServices.PopupTypeParam = PopupType.GameOverPopup;
			PopupHelpers.Show(Constants.Popup);
		}
	}
}

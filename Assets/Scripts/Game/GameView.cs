using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameController gameController;
		[SerializeField] private GameModel gameModel;

		[Space(8.0f)]
		[Header("TMP")]
		[SerializeField] private TMP_Text currentScore;

		public void UpdateCurrentScore()
		{
			currentScore.text = $"Score: {gameModel.CurrentScore}";
		}

		public void OnEscButtonClick()
		{
			SceneManager.LoadScene(Constants.HomeScene);
		}

		public void OnPlayAgainButtonClick()
		{
			SceneManager.LoadScene(Constants.GameScene);
		}
	}
}

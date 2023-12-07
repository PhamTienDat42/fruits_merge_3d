using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using static UnityEngine.ParticleSystem;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameController gameController;
		[SerializeField] private GameModel gameModel;
		[SerializeField] private ParticleSystem mergeParticle;

		[Space(8.0f)]
		[Header("TMP")]
		[SerializeField] private TMP_Text currentScore;

		private ParamServices paramServices;
		private AudioService audioService;

		private void Start()
		{
			paramServices = gameController.GameServices.GetService<ParamServices>();
			audioService = gameController.GameServices.GetService<AudioService>();

			if (paramServices.IsContinue == true)
			{
				currentScore.text = $"{PlayerPrefs.GetInt(Constants.OldScore, 0)}";
			}
		}

		public void UpdateCurrentScore()
		{
			currentScore.text = $"{gameModel.CurrentScore}";
		}

		public void OnEscButtonClick()
		{
			PlayButtonSfx();
			SceneManager.LoadScene(Constants.HomeScene);
		}

		public void OnPlayAgainButtonClick()
		{
			PlayButtonSfx();
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnSettingButtonClick()
		{
			PlayButtonSfx();
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

		public void PlayButtonSfx()
		{
			audioService.PlaySfx(Constants.ButtonSfxName);
		}

		public void PlayMergeSfx()
		{
			audioService.PlaySfx(Constants.MergeSfxName);
		}

		public void PlayDropSfx()
		{
			audioService.PlaySfx(Constants.DropSfxName);
		}

		public void PlayCollisionSfx()
		{
			audioService.PlaySfx(Constants.CollisionSfxName);
		}

		public void OnShakeBoosterClick()
		{
			gameController.BoolShake = true;
		}

		public void PlayMergeParticle(float xParticle, float yParticle)
		{
			mergeParticle.gameObject.SetActive(true);
			mergeParticle.transform.position = new Vector3(xParticle, yParticle, -2.0f);
			mergeParticle.Play();
		}
	}
}

using System.Collections;
using Pools;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameController gameController;
		[SerializeField] private FruitPoolManager fruitManager;
		[SerializeField] private GameModel gameModel;
		[SerializeField] private ParticleSystem mergeParticle;
		[SerializeField] private ParticleSystem explosionParticles;
		[SerializeField] private Camera mainCamera;

		[Space(8.0f)]
		[Header("TMP")]
		[SerializeField] private TMP_Text currentScore;

		[Space(8.0f)]
		[Header("Button")]
		[SerializeField] private Button zoomOutButton;

		private ParamServices paramServices;
		private AudioService audioService;
		private TransitionService transitionService;

		private void Start()
		{
			paramServices = gameController.GameServices.GetService<ParamServices>();
			audioService = gameController.GameServices.GetService<AudioService>();
			transitionService = gameController.GameServices.GetService<TransitionService>();

			if (paramServices.IsContinue == true)
			{
				currentScore.text = $"{PlayerPrefs.GetInt(Constants.OldScore, 0)}";
			}
			transitionService.PlayStartTransition();
			zoomOutButton.interactable = paramServices.CameraSize == Constants.DesignCamSize;
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

		public void PlayBilliardMergeSfx()
		{
			audioService.PlaySfx(Constants.CollisionSfxName);
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

		public void PlayMergeParticle(Vector3 position, int multiplyNum)
		{
			mergeParticle.gameObject.SetActive(true);
			mergeParticle.transform.position = position;
			var scaleNum = 0.25f + 0.15f * multiplyNum;
			mergeParticle.transform.localScale = new Vector3(scaleNum, scaleNum, scaleNum);
			mergeParticle.Play();
		}

		public void OnZoomOutCameraBooster()
		{
			paramServices.CameraSize = mainCamera.orthographicSize * 1.25f;
			paramServices.IsContinue = true;
			StartCoroutine(IPlayExplosionParticleTransition());
		}

		private IEnumerator IPlayExplosionParticleTransition()
		{
			transitionService.PlayEndTransition(Constants.GameScene);
			explosionParticles.gameObject.SetActive(true);
			explosionParticles.Play();
			yield return new WaitForSeconds(0.25f);
			fruitManager.ReturnToPool();
			//SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnKnifeBoosterClick()
		{
			gameController.IsClickable = false;
			gameController.IsKnife = true;
		}

		public void PlayGameOverSfx()
		{
			audioService.PlaySfx(Constants.LoseSfxName);
		}

		public void PlayCutBallSfx()
		{
			audioService.PlaySfx(Constants.CutBallSfxName);
		}

		public ParamServices ParamServices => paramServices;
	}
}

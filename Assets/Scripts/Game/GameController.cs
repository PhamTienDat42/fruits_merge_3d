using Fruits;
using Pools;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private Mesh fruitMesh;
		[SerializeField] private Transform fruitForDistanceTransform;

		[SerializeField] private Camera mainCamera;
		[SerializeField] private FruitManager fruitManager;
		[SerializeField] private GameModel gameModel;
		[SerializeField] private GameView gameView;

		[SerializeField] private SpriteRenderer bgSpriteRenderer;

		[Space(8.0f)]
		[Header("Bound-Collider2D")]
		[SerializeField] private BoxCollider2D topCollider;
		[SerializeField] private BoxCollider2D leftCollider;
		[SerializeField] private BoxCollider2D rightCollider;

		[Space(8.0f)]
		[Header("Fruit Score")]
		[SerializeField] private List<float> fruitComboIndex;

		private Vector3 startPos;
		private bool isClickable = false;
		private Fruits.Fruit2D nextFruit;

		private bool isDrag = false;
		public GameServices GameServices { get; set; }

		private int fruitCombo = 0;
		private readonly float isComboTime = 2.0f;

		private float lastCombineTime = float.MaxValue;

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				GameServices = gameServiceObj.GetComponent<GameServices>();
			}
			else
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}

			Application.targetFrameRate = 60;
		}

		private void Start()
		{
			isClickable = true;

			var startY = mainCamera.orthographicSize - fruitMesh.bounds.size.y * fruitForDistanceTransform.localScale.x / 1.5f;
			startPos = new Vector3(0f, startY, 0f);
			nextFruit = fruitManager.GetNewFruitForShow(startPos);
			SetBoxBound2D(mainCamera);
			ScaleBackground(bgSpriteRenderer);

			fruitManager.OnFruitCombinedFromPool += OnFruitCombined2;
		}

		private void Update()
		{
			//Game
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				DragFruits();
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Space) && isDrag == false)
			{
				isDrag = true;
				StartCoroutine(DropFruitEverySecond());
			}
			else if (Input.GetKeyDown(KeyCode.Space) && isDrag == true)
			{
				isDrag = false;
			}
#endif
		}

		private void DragFruits()
		{
			if (Input.GetMouseButtonDown(0) && isClickable == true)
			{
				var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				var pos = new Vector3(mousePos.x, startPos.y, 0f);
				StartCoroutine(Drag(pos));
			}
		}

		private IEnumerator DropFruitEverySecond()
		{
			//Time.timeScale = 2.0f;
			while (isDrag)
			{
				yield return new WaitForSeconds(1.0f);
				var newPosX = Random.Range(-2f, 2f);
				var newPos = new Vector3(newPosX, startPos.y, 0f);
				nextFruit.gameObject.SetActive(false);
				fruitManager.GetFruitForDrop(nextFruit.FruitIndex, newPos);
				nextFruit = fruitManager.GetNewFruitForShow(startPos);
			}
		}

		public void OnDragFruitButtonClick()
		{
			isDrag = true;
			StartCoroutine(DropFruitEverySecond());
		}

		public void OnStopDropFruitButtonClick()
		{
			isDrag = false;
		}

		private IEnumerator Drag(Vector3 pos)
		{
			isClickable = false;
			nextFruit.gameObject.SetActive(false);
			fruitManager.GetFruitForDrop(nextFruit.FruitIndex, pos);
			yield return new WaitForSeconds(1.5f);
			fruitManager.SaveCombinePool();
			nextFruit = fruitManager.GetNewFruitForShow(startPos);
			isClickable = true;
		}

		private void SetBoxBound2D(Camera mainCamera)
		{
			var screenHeight = mainCamera.orthographicSize * 2f;
			var screenWidth = screenHeight * mainCamera.aspect;
			SetBoundPosition2D(topCollider, new Vector2(screenWidth, 0.01f), new Vector3(0f, screenHeight / 2f - fruitMesh.bounds.size.y * fruitForDistanceTransform.localScale.x, 0f));
			SetBoundPosition2D(leftCollider, new Vector2(0.01f, screenHeight), new Vector3(-screenWidth / 2f, 0f, 0f));
			SetBoundPosition2D(rightCollider, new Vector2(0.01f, screenHeight), new Vector3(screenWidth / 2f, 0f, 0f));
		}

		private void SetBoundPosition2D(BoxCollider2D collider2D, Vector2 size, Vector3 localPos)
		{
			collider2D.size = size;
			collider2D.transform.localPosition = localPos;
		}

		public void IncreaseScore(int fruitPoint)
		{
			if (fruitCombo > 9)
			{
				fruitCombo = 9;
			}
			gameModel.CurrentScore += Mathf.CeilToInt(fruitPoint * fruitComboIndex[fruitCombo]);
			PlayerPrefs.SetInt(Constants.OldScore, gameModel.CurrentScore);
			var highScore = PlayerPrefs.GetInt(Constants.HighScore, 0);
			if (gameModel.CurrentScore > highScore)
			{
				PlayerPrefs.SetInt(Constants.HighScore, gameModel.CurrentScore);
			}
			PlayerPrefs.Save();
			gameView.UpdateCurrentScore();
		}

		private void OnFruitCombined2(Fruit2D fruit)
		{
			float elapsedTimeSinceCombine = Time.time - lastCombineTime;
			fruitCombo = elapsedTimeSinceCombine < isComboTime ? ++fruitCombo : 0;
			IncreaseScore(fruit.FruitPoint);
			lastCombineTime = Time.time;
		}

		private void ScaleBackground(SpriteRenderer bgSpiteRenderer)
		{
			var cameraHeight = 2.0f * mainCamera.orthographicSize + 0.05f;
			var cameraWidth = cameraHeight * mainCamera.aspect + 0.05f;
			var scaleX = cameraWidth / bgSpiteRenderer.bounds.size.x;
			var scaleY = cameraHeight / bgSpiteRenderer.bounds.size.y;
			bgSpiteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
		}

		public void SaveGamePlay()
		{
			fruitManager.SaveCombinePool();
		}

		public void GameOver()
		{
			string filePath = Path.Combine(Application.persistentDataPath, "fruitData.json");
			File.WriteAllText(filePath, string.Empty);
			PlayerPrefs.SetInt(Constants.OldScore, 0);
			gameView.ShowGameOverPopup();
		}
	}
}

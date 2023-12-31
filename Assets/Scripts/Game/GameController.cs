using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fruits;
using Pools;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;
		[SerializeField] private FruitPoolManager fruitManager;
		[SerializeField] private GameModel gameModel;
		[SerializeField] private GameView gameView;

		[SerializeField] private SpriteRenderer bgSpriteRenderer;
		[SerializeField] private GameObject groundObject;

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

		private bool boolShake = false;
		private readonly float shakeThreshold = 3f;

		private float lastCombineTime = float.MaxValue;
		private bool isTouching = false;
		private bool isKnife = false;

		private Mesh fruitMesh;
		private Transform fruitForDistanceTransform;
		private float fruitLocalscaleX = 0.0f;

		private ParamServices paramServices;

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				GameServices = gameServiceObj.GetComponent<GameServices>();
				paramServices = GameServices.GetService<ParamServices>();
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

			if (paramServices.CameraSize != Constants.DesignCamSize)
			{
				mainCamera.orthographicSize = paramServices.CameraSize;
			}

			GetDistanceToShowBall();
			var startY = mainCamera.orthographicSize - fruitMesh.bounds.size.y * fruitForDistanceTransform.localScale.x * fruitLocalscaleX / 1.5f;
			startPos = new Vector3(0f, startY, 0f);
			nextFruit = fruitManager.GetNewBallNoPhysicOn(startPos);
			SetBoxBound2D(mainCamera);
			ScaleBackground(bgSpriteRenderer);
			groundObject.transform.localPosition = new Vector3(0.0f, -mainCamera.orthographicSize, 0.0f);

			fruitManager.OnFruitCombinedFromPool += OnFruitCombined;
		}

		private void GetDistanceToShowBall()
		{
			var lastFruit = fruitManager.GetLastBallFromPrefabs();
			fruitMesh = lastFruit.GetComponentInChildren<MeshFilter>().sharedMesh;
			fruitForDistanceTransform = lastFruit.GetComponent<Transform>().GetChild(0);
			fruitLocalscaleX = lastFruit.GetComponent<Transform>().localScale.x;
		}

		private void Update()
		{
			//ShakePhone
			if (Input.acceleration.sqrMagnitude >= shakeThreshold * shakeThreshold && boolShake == true)
			{
				StartCoroutine(ShakePhone());
			}

			//KnifeBooster
			if (isKnife == true && isClickable == false)
			{
				StartCoroutine(ICutBallWithKnifeBooster());
			}

			//Game
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				DragFruits();
			}

			if (Input.GetKeyDown(KeyCode.Space) && isDrag == false)
			{
				isDrag = true;
				StartCoroutine(DropFruitEverySecond());
			}
			else if (Input.GetKeyDown(KeyCode.Space) && isDrag == true)
			{
				isDrag = false;
			}
		}

		private void DragFruits()
		{
			var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			UpdateFruitShowPosition(mousePos.x);
#if UNITY_EDITOR
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0) && isClickable == true)
				{
					isTouching = true;
				}

				if (Input.GetMouseButtonUp(0) && isTouching == true)
				{
					isTouching = false;
					var dragPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
					var pos = new Vector3(dragPos.x, startPos.y, 0f);
					StartCoroutine(Drag(pos));
				}
			}
#elif UNITY_ANDROID
			if (isClickable == true && Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
				{
					if (touch.phase == TouchPhase.Began)
					{
						isTouching = true;
					}

					if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isTouching == true)
					{
						isTouching = false;
						var touchPos = mainCamera.ScreenToWorldPoint(touch.position);
						var pos = new Vector3(touchPos.x, startPos.y, 0.0f);
						StartCoroutine(Drag(pos));
					}
				}
			}
#endif
		}

		private void UpdateFruitShowPosition(float posX)
		{
#if UNITY_EDITOR
			if (!EventSystem.current.IsPointerOverGameObject() && isTouching == true)
			{
				var newFruitPos = new Vector3(posX, startPos.y, 0.0f);
				nextFruit.transform.localPosition = newFruitPos;
			}
#elif UNITY_ANDROID
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (isTouching == true && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
				{
					var currentTouchPos = mainCamera.ScreenToWorldPoint(touch.position);
					var newFruitPos = new Vector3(currentTouchPos.x, startPos.y, 0.0f);
					nextFruit.transform.localPosition = newFruitPos;
				}
			}
#endif
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
				fruitManager.GetNewPhysicBallHas(nextFruit.FruitIndex, newPos);
				nextFruit = fruitManager.GetNewBallNoPhysicOn(startPos);
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
			gameView.PlayDropSfx();
			isClickable = false;
			nextFruit.gameObject.SetActive(false);
			fruitManager.GetNewPhysicBallHas(nextFruit.FruitIndex, pos);
			yield return new WaitForSeconds(1.5f);
			fruitManager.SaveCombinePool();
			nextFruit = fruitManager.GetNewBallNoPhysicOn(startPos);
			isClickable = true;
		}

		private void SetBoxBound2D(Camera mainCamera)
		{
			var screenHeight = mainCamera.orthographicSize * 2f;
			var screenWidth = screenHeight * mainCamera.aspect;
			var topPosX = screenHeight / 2f - fruitMesh.bounds.size.y * fruitForDistanceTransform.localScale.x * fruitLocalscaleX;
			SetBoundPosition2D(topCollider, new Vector2(screenWidth, 0.01f), new Vector3(0f, topPosX, 0f));
			SetBoundPosition2D(leftCollider, new Vector2(0.01f, screenHeight), new Vector3(-screenWidth / 2f, 0f, 0f));
			SetBoundPosition2D(rightCollider, new Vector2(0.01f, screenHeight), new Vector3(screenWidth / 2f, 0f, 0f));
		}

		private void SetBoundPosition2D(BoxCollider2D collider2D, Vector2 size, Vector3 localPos)
		{
			collider2D.size = size;
			collider2D.transform.localPosition = localPos;
		}

		private void OnFruitCombined(Fruit2D fruit)
		{
			float elapsedTimeSinceCombine = Time.time - lastCombineTime;
			fruitCombo = elapsedTimeSinceCombine < isComboTime ? ++fruitCombo : 0;
			fruitCombo = fruitCombo > 9 ? 9 : fruitCombo;
			IncreaseScore(fruit.FruitPoint);
			lastCombineTime = Time.time;
		}

		private void IncreaseScore(int fruitPoint)
		{
			BonusScore = Mathf.CeilToInt(fruitPoint * fruitComboIndex[fruitCombo]);
			gameModel.CurrentScore += BonusScore;
			PlayerPrefs.SetInt(Constants.OldScore, gameModel.CurrentScore);
			CheckAndSaveHighScore();
			gameView.UpdateCurrentScore();
		}

		private void CheckAndSaveHighScore()
		{
			if (IsHighScore() == true)
			{
				PlayerPrefs.SetInt(Constants.HighScore, gameModel.CurrentScore);
			}
			PlayerPrefs.Save();
		}

		private bool IsHighScore()
		{
			return gameModel.CurrentScore > PlayerPrefs.GetInt(Constants.HighScore, 0);
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

		public IEnumerator ShakePhone()
		{
			boolShake = false;
			topCollider.isTrigger = false;
			fruitManager.ApplyShakeForceToPoolBall();
			yield return new WaitForSeconds(2.0f);
			topCollider.isTrigger = true;
		}

		public Vector3 ReturnBallPositionOnZoomOutBooster(Vector3 fruitPos)
		{
			var posY = fruitPos.y;
			var posX = fruitPos.x;

			if (paramServices.CameraSize != Constants.DesignCamSize)
			{
				posY += paramServices.CameraSize - Constants.DesignCamSize;
				var bonusPosX = (paramServices.CameraSize - Constants.DesignCamSize) * mainCamera.aspect;
				posX += posX > 0.0f ? -bonusPosX : bonusPosX;
			}
			var pos = new Vector3(posX, posY, fruitPos.z);
			return pos;
		}

		public IEnumerator ICutBallWithKnifeBooster()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

				if (hit.collider != null)
				{
					hit.collider.gameObject.SetActive(false);
					gameView.PlayCutBallSfx();
					isKnife = false;
					yield return new WaitForSeconds(0.5f);
					isClickable = true;
				}
			}
		}

		public bool BoolShake { get => boolShake; set => boolShake = value; }
		public int BonusScore { get; private set; }
		public bool IsClickable { get => isClickable; set => isClickable = value; }
		public bool IsKnife { get => isKnife; set => isKnife = value; }
	}
}

using System.Collections;
using Pools;
using Services;
using UnityEngine;
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

		[Space(8.0f)]
		[Header("Bound-Collider2D")]
		[SerializeField] private BoxCollider2D topCollider;
		[SerializeField] private BoxCollider2D leftCollider;
		[SerializeField] private BoxCollider2D rightCollider;

		private Vector3 startPos;
		private bool isClickable = false;
		private Fruits.Fruit nextFruit;

		private bool isDrag = false;
		public GameServices GameServices { get; set; }
		//private ParamServices paramServices;

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				GameServices = gameServiceObj.GetComponent<GameServices>();
				//paramServices = GameServices.GetService<ParamServices>();
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
		}

		private void Update()
		{
			//Game
			DragFruits();
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
			Time.timeScale = 2.0f;
			while (isDrag)
			{
				yield return new WaitForSeconds(1.0f);
				var newPosX = Random.Range(-2f, 2f);
				var newPos = new Vector3(newPosX, startPos.y, 0f);
				nextFruit.gameObject.SetActive(false);
				fruitManager.GetFruitForDrop(nextFruit.FruitPoint, newPos);
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
			fruitManager.GetFruitForDrop(nextFruit.FruitPoint, pos);
			yield return new WaitForSeconds(1.5f);
			nextFruit = fruitManager.GetNewFruitForShow(startPos);
			isClickable = true;
		}

		private void SetBoxBound2D(Camera mainCamera)
		{
			float screenHeight = mainCamera.orthographicSize * 2f;
			float screenWidth = screenHeight * mainCamera.aspect;
			SetBoundPosition2D(topCollider, new Vector2(screenWidth, 0.01f), new Vector3(0f, screenHeight / 2f - fruitMesh.bounds.size.y * fruitForDistanceTransform.localScale.x, 0f));
			SetBoundPosition2D(leftCollider, new Vector2(0.01f, screenHeight), new Vector3(-screenWidth / 2f, 0f, 0f));
			SetBoundPosition2D(rightCollider, new Vector2(0.01f, screenHeight), new Vector3(screenWidth / 2f, 0f, 0f));
		}

		private void SetBoundPosition2D(BoxCollider2D collider2D, Vector2 size, Vector3 localPos)
		{
			collider2D.size = size;
			collider2D.transform.localPosition = localPos;
		}
	}
}

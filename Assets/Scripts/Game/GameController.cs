using System.Collections;
using System.Collections.Generic;
using Pools;
using UnityEngine;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private List<Material> fruitMaterials;
		[SerializeField] private List<Mesh> fruitMesh;
		[SerializeField] private Camera mainCamera;
		[SerializeField] private FruitManager fruitManager;

		private Vector3 startPos;
		private bool isClickable = false;
		private Fruits.Fruit nextFruit;

		private void Awake()
		{
			Application.targetFrameRate = 60;
		}

		private void Start()
		{
			isClickable = true;

			var startY = mainCamera.orthographicSize - fruitMesh[^1].bounds.size.y;
			startPos = new Vector3(0f, startY, 0f);
			nextFruit = fruitManager.GetNewFruitForDrag(startPos);
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

		private IEnumerator Drag(Vector3 pos)
		{
			isClickable = false;
			nextFruit.gameObject.SetActive(false);
			fruitManager.GetFruitForCombine(nextFruit.FruitPoint, pos);
			yield return new WaitForSeconds(1.5f);
			nextFruit = fruitManager.GetNewFruitForDrag(startPos);
			isClickable = true;
		}
		public List<Material> FruitMaterials => fruitMaterials;
	}
}

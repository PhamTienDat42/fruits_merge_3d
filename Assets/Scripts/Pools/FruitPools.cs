using System.Collections.Generic;
using UnityEngine;

using Game;
using Fruits;

namespace Pools
{
	public class FruitPools : MonoBehaviour
	{
		[SerializeField] private List<Fruit> fruitPrefabs;
		[SerializeField] private int poolSize;
		[SerializeField] private GameObject fruitsParents;
		[SerializeField] private GameController controller;
		[SerializeField] private GameView gameView;

		private readonly List<Fruit> fruitPools = new();
		private int lastDeactivatedFruitIndex = -1;

		private void Awake()
		{
			InitializePool(poolSize);
		}

		private void InitializePool(int size)
		{
			for (int i = 0; i < size; i++)
			{
				Fruit obj = Instantiate(GetRandomFruitPrefab());
				obj.InstantiateFruits(this, controller, gameView);
				obj.transform.parent = fruitsParents.transform;
				obj.gameObject.SetActive(false);
				fruitPools.Add(obj);
			}
		}

		public Fruit GetFruitFromPoolNew(Vector3 position)
		{
			var isFull = true;
			var d = 0;

			for (int i = 0; i < fruitPools.Count; i++)
			{
				if (!fruitPools[i].gameObject.activeInHierarchy)
				{
					d++;
					if (d > 2)
					{
						isFull = false;
						break;
					}
				}
			}

			if (isFull == true)
			{
				InitializePool(poolSize);
			}

			for (int i = 0; i < fruitPools.Count; i++)
			{
				Debug.Log(lastDeactivatedFruitIndex);
				int indexToCheck = (lastDeactivatedFruitIndex + i + 1) % fruitPools.Count;
				Fruit fruit = fruitPools[indexToCheck];

				if (!fruit.gameObject.activeInHierarchy)
				{
					fruit.transform.position = position;
					fruit.gameObject.SetActive(true);
					lastDeactivatedFruitIndex = indexToCheck;
					return fruit;
				}
			}
			return null;
		}

		//public IEnumerator ReturnActiveFruitsAndScoreWithDelay(float delay)
		//{
		//	foreach (Fruit fruit in fruitPools)
		//	{
		//		if (fruit.gameObject.activeSelf)
		//		{
		//			fruit.Rb.bodyType = RigidbodyType2D.Static;
		//		}
		//	}

		//	foreach (Fruit fruit in fruitPools)
		//	{
		//		if (fruit.gameObject.activeSelf)
		//		{
		//			controller.Model.CurrentScore += fruit.FruitPoint;
		//			gameView.SetScore();
		//			yield return new WaitForSeconds(delay);
		//			ReturnFruitToPool(fruit);
		//		}
		//	}
		//	var highScore = PlayerPrefs.GetInt(Constants.HighScore, 0);
		//	if (controller.Model.CurrentScore > highScore)
		//	{
		//		highScore = controller.Model.CurrentScore;
		//		controller.Model.SetHighScore(highScore);
		//	}
		//}

		public void ReturnFruitToPool(Fruit fruit)
		{
			fruit.gameObject.SetActive(false);
		}

		public void ReturnFruitToPoolRandom(Fruit fruit)
		{
			var randomFruit = GetRandomFruitPrefab();
			fruit.MeshRenderer.materials[0] = randomFruit.GetComponent<MeshRenderer>().materials[0];
			fruit.FruitPoint = randomFruit.FruitPoint;
			//fruit.sphereCollider.radius = randomFruit.GetComponent<SphereCollider>().radius;
			fruit.gameObject.SetActive(false);
		}

		private Fruit GetRandomFruitPrefab()
		{
			return fruitPrefabs[UnityEngine.Random.Range(0, 5)];
		}
	}
}

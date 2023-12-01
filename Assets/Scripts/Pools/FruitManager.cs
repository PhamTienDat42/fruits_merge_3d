using System;
using System.Collections;
using System.Collections.Generic;
using Fruits;
using Game;
using UnityEngine;

namespace Pools
{
	public class FruitManager : MonoBehaviour
	{
		[SerializeField] private List<Fruit2D> fruitPrefabs;
		[SerializeField] private List<Fruit2D> combineFruitPrefabs;
		[SerializeField] private GameObject parentFruitPools;
		[SerializeField] private GameObject parentFruitCombinePools;

		[SerializeField] private GameController gameController;
		[SerializeField] private GameView gameView;
		[SerializeField] private GameModel gameModel;

		private Dictionary<int, ObjectPool<Fruit2D>> fruitPools;
		private Dictionary<int, ObjectPool<Fruit2D>> combinePools;

		private readonly int poolSizeForCombinePool = 10;
		private readonly int poolSizeForDrag = 1;

		public event Action<Fruit2D> OnFruitCombinedFromPool;

		private int totalRandomScore = 300;
		private List<int> randomStartScores = new();
		[SerializeField] private List<int> randomScores;
		[SerializeField] private List<int> bonusRandomScores;

		private void Awake()
		{
			//fruitPools = new Dictionary<int, ObjectPool<Fruit>>();
			//for (int i = 1; i <= Constants.FruitTypeCount; i++)
			//{
			//	ObjectPool<Fruit> pool = new(() => Instantiate(fruitPrefabs[i - 1]), poolSizeForDrag, parentFruitPools);
			//	fruitPools.Add(i, pool);
			//}

			fruitPools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.FruitTypeCount; i++)
			{
				ObjectPool<Fruit2D> pool = new(() => Instantiate(fruitPrefabs[i - 1]), poolSizeForDrag, parentFruitPools);
				fruitPools.Add(i, pool);
			}

			combinePools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= Constants.FruitTypeCount; i++)
			{
				ObjectPool<Fruit2D> objectPool = new(() => DIFruit(i - 1), poolSizeForCombinePool, parentFruitCombinePools);
				combinePools.Add(i, objectPool);
			}
			randomStartScores.AddRange(randomScores);
		}

		private Fruit2D DIFruit(int index)
		{
			Fruit2D fruit = Instantiate(combineFruitPrefabs[index]);
			fruit.InstantiateFruits(this, gameController, gameView, gameModel);
			return fruit;
		}

		private int RandomFruitToDrop()
		{
			var score = UnityEngine.Random.Range(0, totalRandomScore);
			var rateStep = 0;
			var fruitIndex = 0;

			for (var i = 0; i < randomScores.Count; ++i)
			{
				rateStep += randomScores[i];
				if (score < rateStep && fruitIndex == 0)
				{
					fruitIndex = i + 1;
					var temp = randomScores[i] - randomStartScores[i];
					randomScores[i] = randomStartScores[i];
					totalRandomScore -= temp;
					continue;
				}

				randomScores[i] += bonusRandomScores[i];
				totalRandomScore += bonusRandomScores[i];
			}
			return fruitIndex;
		}

		public Fruit2D GetNewFruitForShow(Vector3 pos)
		{
			int randomPoints = RandomFruitToDrop();
			Fruit2D newFruit = fruitPools[randomPoints].GetObject(pos);
			return newFruit;
		}

		public Fruit2D GetFruitForDrop(int index, Vector3 pos)
		{
			ObjectPool<Fruit2D> combinePool = combinePools[index];
			int deactiveCount = 0;

			foreach (Fruit2D fruit in combinePool.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}

			if (deactiveCount < 2)
			{
				Fruit2D newObj = Instantiate(combineFruitPrefabs[index - 1], parentFruitCombinePools.transform);
				newObj.InstantiateFruits(this, gameController, gameView, gameModel);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit2D combineFruit = combinePools[index].GetObject(pos);
			combineFruit.OnFruitCombined += OnFruitCombinedFromPool;
			Transform childTranform = combineFruit.GetComponent<Transform>().GetChild(0);
			StartCoroutine(RandomRotateFruits(childTranform, 3.0f));
			return combineFruit;
		}

		private IEnumerator RandomRotateFruits(Transform fruitTransform, float duration)
		{
			float t = 0.0f;
			float randomX = UnityEngine.Random.Range(0.0f, 360.0f);
			float randomY = UnityEngine.Random.Range(0.0f, 360.0f);

			Quaternion startRot = fruitTransform.localRotation;
			Quaternion endRot = Quaternion.Euler(randomX, randomY, 0.0f);

			while (t < duration)
			{
				fruitTransform.localRotation = Quaternion.Lerp(startRot, endRot, t / duration);
				t += Time.deltaTime;
				yield return null;
			}
			fruitTransform.localRotation = endRot;
		}
	}
}

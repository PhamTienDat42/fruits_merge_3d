using System.Collections;
using System.Collections.Generic;
using Fruits;
using UnityEngine;

namespace Pools
{
	public class FruitManager : MonoBehaviour
	{
		[SerializeField] private List<Fruit> fruitPrefabs;
		[SerializeField] private List<Fruit2D> combineFruitPrefabs;
		[SerializeField] private GameObject parentFruitPools;
		[SerializeField] private GameObject parentFruitCombinePools;

		private Dictionary<int, ObjectPool<Fruit>> fruitPools;
		private Dictionary<int, ObjectPool<Fruit2D>> combinePools;

		private readonly int poolSizeForCombinePool = 10;
		private readonly int poolSizeForDrag = 1;

		private void Awake()
		{
			fruitPools = new Dictionary<int, ObjectPool<Fruit>>();
			for (int i = 1; i <= 9; i++)
			{
				ObjectPool<Fruit> pool = new(() => Instantiate(fruitPrefabs[i - 1]), poolSizeForDrag, parentFruitPools);
				fruitPools.Add(i, pool);
			}

			combinePools = new Dictionary<int, ObjectPool<Fruit2D>>();
			for (int i = 1; i <= 9; i++)
			{
				ObjectPool<Fruit2D> pool = new(() => Instantiate(combineFruitPrefabs[i - 1]), poolSizeForCombinePool, parentFruitCombinePools);
				combinePools.Add(i, pool);
			}
		}

		public Fruit GetNewFruitForShow(Vector3 pos)
		{
			int randomPoints = UnityEngine.Random.Range(1, 6);
			Fruit newFruit = fruitPools[randomPoints].GetObject(pos);
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
				newObj.gameObject.SetActive(true);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit2D combineFruit = combinePools[index].GetObject(pos);
			Transform childTranform = combineFruit.GetComponent<Transform>().GetChild(0);
			StartCoroutine(RandomRotateFruits(childTranform, 5.0f));
			return combineFruit;
		}

		private IEnumerator RandomRotateFruits(Transform fruitTransform, float duration)
		{
			float t = 0.0f;
			float randomX = Random.Range(0.0f, 360.0f);
			float randomY = Random.Range(0.0f, 360.0f);

			Quaternion startRot = fruitTransform.localRotation;
			Quaternion endRot = Quaternion.Euler(randomX, randomY, 0.0f);

			while(t < duration)
			{
				fruitTransform.localRotation = Quaternion.Lerp(startRot, endRot, t / duration);
				t += Time.deltaTime;
				yield return null;
			}
			fruitTransform.localRotation = endRot;
		}
	}
}

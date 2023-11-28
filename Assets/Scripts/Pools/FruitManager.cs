using System.Collections;
using System.Collections.Generic;
using Fruits;
using UnityEngine;

namespace Pools
{
	public class FruitManager : MonoBehaviour
	{
		[SerializeField] private List<Fruit> fruitPrefabs;
		[SerializeField] private List<Fruit> combineFruitPrefabs;
		[SerializeField] private GameObject parentFruitPools;
		[SerializeField] private GameObject parentFruitCombinePools;

		private Dictionary<int, ObjectPool<Fruit>> fruitPools;
		private Dictionary<int, ObjectPool<Fruit>> combinePools;

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

			combinePools = new Dictionary<int, ObjectPool<Fruit>>();
			for (int i = 1; i <= 9; i++)
			{
				ObjectPool<Fruit> pool = new(() => Instantiate(combineFruitPrefabs[i - 1]), poolSizeForCombinePool, parentFruitCombinePools);
				combinePools.Add(i, pool);
			}
		}

		public Fruit GetNewFruitForShow(Vector3 pos)
		{
			int randomPoints = UnityEngine.Random.Range(1, 6);
			Fruit newFruit = fruitPools[randomPoints].GetObject(pos);
			return newFruit;
		}

		public Fruit GetFruitForDrag(int index, Vector3 pos)
		{
			ObjectPool<Fruit> combinePool = combinePools[index];
			int deactiveCount = 0;

			foreach (Fruit fruit in combinePool.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}

			if (deactiveCount < 2)
			{
				Fruit newObj = Instantiate(combineFruitPrefabs[index - 1], parentFruitCombinePools.transform);
				newObj.gameObject.SetActive(true);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit combineFruit = combinePools[index].GetObject(pos);
			//StartCoroutine(GradualExpansionColliderRadius(combineFruit, 0.1f));
			return combineFruit;
		}

		public Fruit GetFruitForCombine(int index, Vector3 pos)
		{
			ObjectPool<Fruit> combinePool = combinePools[index];
			int deactiveCount = 0;

			foreach (Fruit fruit in combinePool.GetObjectList())
			{
				if (!fruit.gameObject.activeSelf)
				{
					deactiveCount++;
				}
			}

			if (deactiveCount < 2)
			{
				Fruit newObj = Instantiate(combineFruitPrefabs[index - 1], parentFruitCombinePools.transform);
				newObj.gameObject.SetActive(true);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit combineFruit = combinePools[index].GetObject(pos);
			//StartCoroutine(GradualExpansionColliderRadius(combineFruit, 0.5f));
			return combineFruit;
		}

		private IEnumerator GradualExpansionColliderRadius(Fruit fruit, float duration)
		{
			Time.timeScale = 2.0f;
			float t = 0.0f;
			while (t < duration)
			{
				fruit.GetComponent<SphereCollider>().radius = Mathf.Lerp(0.001f, 0.005f, t / duration);
				t += Time.deltaTime;
				yield return null;
			}
			fruit.GetComponent<SphereCollider>().radius = 0.005f;
			Time.timeScale = 1.0f;
		}
	}
}

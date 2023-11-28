using System.Collections.Generic;
using Fruits;
using UnityEngine;

namespace Pools
{
	public class FruitManager : MonoBehaviour
	{
		[SerializeField] private List<Fruit> fruitPrefabs;
		[SerializeField] private List<Fruit> combineFruitPrefabs;
		//[SerializeField] private int poolSizePerType = 10;

		private Dictionary<int, ObjectPool<Fruit>> fruitPools;
		private Dictionary<int, ObjectPool<Fruit>> combinePools;

		private int poolSizeForCombinePool = 10;
		private int poolSizeForDrag = 1;

		private void Awake()
		{
			fruitPools = new Dictionary<int, ObjectPool<Fruit>>();
			for (int i = 1; i <= 9; i++)
			{
				ObjectPool<Fruit> pool = new(() => Instantiate(fruitPrefabs[i-1]), poolSizeForDrag);
				fruitPools.Add(i, pool);
			}

			combinePools = new Dictionary<int, ObjectPool<Fruit>>();
			for (int i = 1; i <= 9; i++)
			{
				ObjectPool<Fruit> pool = new(() => Instantiate(combineFruitPrefabs[i - 1]), poolSizeForCombinePool);
				combinePools.Add(i, pool);
			}
		}

		public Fruit GetNewFruitForDrag(Vector3 pos)
		{
			int randomPoints = UnityEngine.Random.Range(1, 6);
			Fruit newFruit = fruitPools[randomPoints].GetObject(pos);
			return newFruit;
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
				Fruit newObj = Instantiate(combineFruitPrefabs[index - 1]);
				newObj.gameObject.SetActive(true);
				newObj.transform.position = pos;
				combinePool.AddToPool(newObj);
			}

			Fruit combineFruit = combinePools[index].GetObject(pos);
			return combineFruit;
		}

		public Dictionary<int, ObjectPool<Fruit>> CombinePools => combinePools;
	}
}

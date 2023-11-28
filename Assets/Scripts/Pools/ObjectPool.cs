using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
	public class ObjectPool<T> where T : MonoBehaviour
	{
		private List<T> objectPool = new();
		private Func<T> objectFactory;

		public ObjectPool(Func<T> factory, int initialPoolSize)
		{
			objectFactory = factory;

			for (int i = 0; i < initialPoolSize; i++)
			{
				T obj = CreateObject();
				objectPool.Add(obj);
			}
		}

		private T CreateObject()
		{
			T newObj = objectFactory();
			newObj.gameObject.SetActive(false);
			return newObj;
		}

		public T GetObject(Vector3 position)
		{

			foreach (T obj in objectPool)
			{
				if (!obj.gameObject.activeSelf)
				{
					obj.gameObject.SetActive(true);
					obj.transform.position = position;
					return obj;
				}
			}

			// If all objects are active, create a new one
			T newObj = CreateObject();
			newObj.gameObject.SetActive(true);
			newObj.transform.position = position;
			objectPool.Add(newObj);
			return newObj;
		}

		public void ReturnObjectToPool(T obj)
		{
			obj.gameObject.SetActive(false);
		}

		public int ObjectCount
		{
			get { return objectPool.Count; }
		}

		public void AddToPool(T obj)
		{
			obj.gameObject.SetActive(false);
			objectPool.Add(obj);
		}
	}
}

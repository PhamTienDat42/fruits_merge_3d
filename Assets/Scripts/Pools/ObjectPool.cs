using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pools
{
	public class ObjectPool<T> where T : MonoBehaviour
	{
		private readonly List<T> objectPool = new();
		private readonly Func<T> objectFactory;

		public ObjectPool(Func<T> factory, int initialPoolSize, GameObject parent)
		{
			objectFactory = factory;

			for (int i = 0; i < initialPoolSize; i++)
			{
				T obj = CreateObject();
				if (obj.GetComponent<TMP_Text>() != null)
				{
					obj.transform.SetParent(parent.transform);
				}
				else
				{
					obj.transform.parent = parent.transform;
				}
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
			return null;
		}

		public T GetObject()
		{

			foreach (T obj in objectPool)
			{
				if (!obj.gameObject.activeSelf)
				{
					obj.gameObject.SetActive(true);
					return obj;
				}
			}
			return null;
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

		public List<T> GetObjectList()
		{
			return objectPool;
		}
	}
}

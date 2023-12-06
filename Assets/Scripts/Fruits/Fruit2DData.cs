using System;
using UnityEngine;

namespace Fruits
{
	[Serializable]
	public class Fruit2DData
	{
		[SerializeField] private int fruitIndex;
		[SerializeField] private int fruitPoint;
		[SerializeField] private float[] position;

		public Fruit2DData()
		{

		}

		public Fruit2DData(Vector3 position)
		{
			this.Position = new float[] { position.x, position.y, position.z };
		}

		public Vector3 GetPosition()
		{
			return new Vector3(position[0], position[1], position[2]);
		}

		public int FruitIndex { get => fruitIndex; set => fruitIndex = value; }
		public int FruitPoint { get => fruitPoint; set => fruitPoint = value; }
		public float[] Position { get => position; set => position = value; }
	}
}

using System;
using UnityEngine;

namespace Fruits
{
	[Serializable]
	public class Fruit2DData
	{
		private int fruitIndex;
		private int fruitPoint;
		private float[] position;

		public static Fruit2DData FromFruit(Fruit2D fruit2D)
		{
			Fruit2DData fruit2DData = new();
			fruit2DData.fruitIndex = fruit2D.FruitIndex;
			fruit2DData.fruitPoint = fruit2D.FruitPoint;

			fruit2DData.position = new float[3];
			fruit2DData.position[0] = fruit2D.transform.localPosition.x;
			fruit2DData.position[1] = fruit2D.transform.localPosition.y;
			fruit2DData.position[2] = fruit2D.transform.localPosition.z;

			return fruit2DData;
		}

		public Fruit2DData()
		{

		}

		public Fruit2DData(Vector3 position)
		{
			this.position = new float[] { position.x, position.y, position.z };
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

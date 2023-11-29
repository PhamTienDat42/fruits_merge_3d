using UnityEngine;

namespace Fruits
{
	public class Fruit : MonoBehaviour
	{
		[SerializeField] private int fruitPoint;

		public int FruitPoint { get => fruitPoint; set => fruitPoint = value; }
	}
}

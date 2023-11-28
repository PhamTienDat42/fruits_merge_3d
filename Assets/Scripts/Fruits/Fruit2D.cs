using Game;
using Pools;
using UnityEngine;

namespace Fruits
{
	public class Fruit2D : MonoBehaviour
	{
		[SerializeField] private int fruitPoint;
		private GameController controller;
		private GameView gameView;
		private FruitManager fruitManager;

		private Rigidbody2D rb;

		private const string FruitManagerTag = "FruitManager";

		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
			var fruitManagerObj = GameObject.FindGameObjectWithTag(FruitManagerTag);
			fruitManager = fruitManagerObj.GetComponent<FruitManager>();
		}

		//public void InstantiateFruits(FruitPools fruitPools, GameController gameController, GameView gameView)
		//{
		//	this.fruitPools = fruitPools;
		//	this.controller = gameController;
		//	this.gameView = gameView;
		//}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Fruit2D otherFruit = collision.gameObject.GetComponent<Fruit2D>();

			if (otherFruit != null)
			{
				if (CanCombine(otherFruit))
				{
					CombineFruit(otherFruit);
				}
			}
		}

		private bool CanCombine(Fruit2D otherFruit)
		{
			return fruitPoint == otherFruit.fruitPoint;
		}

		private void CombineFruit(Fruit2D otherFruit)
		{
			Fruit2D higherFruit = (transform.position.y > otherFruit.transform.position.y) ? this : otherFruit;
			var newVelocity = higherFruit.gameObject.GetComponent<Rigidbody2D>().velocity;
			Vector3 newFruitPos = higherFruit.transform.position;
			var newPoints = fruitPoint + 1;

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}

			this.rb.velocity = otherFruit.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			this.gameObject.SetActive(false);
			otherFruit.gameObject.SetActive(false);

			if (fruitPoint == 9)
			{
				return;
			}

			Fruit2D newFruit = fruitManager.GetFruitForDrop(newPoints, newFruitPos);
			newFruit.GetComponent<Rigidbody2D>().velocity = newVelocity;
			//if (Math.Abs(newVelocity.y) < 0.5f && Math.Abs(newVelocity.x) < 0.1f)
			//{
			//	newFruit.GetComponent<Rigidbody>().AddForce(Vector2.up, ForceMode.Impulse);
			//}
		}

		public int FruitPoint { get => fruitPoint; set => fruitPoint = value; }
	}
}

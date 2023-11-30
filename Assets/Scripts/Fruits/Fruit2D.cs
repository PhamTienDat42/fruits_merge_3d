using System;
using Game;
using Pools;
using UnityEngine;

namespace Fruits
{
	public class Fruit2D : MonoBehaviour
	{
		[SerializeField] private int fruitPoint;
		[SerializeField] private int index;

		private GameController gameController;
		private GameView gameView;
		private GameModel gameModel;
		private FruitManager fruitManager;
		private Rigidbody2D rb;

		public event Action<Fruit2D> OnFruitCombined;

		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		public void InstantiateFruits(FruitManager fruitManager, GameController gameController, GameView gameView, GameModel gameModel)
		{
			this.fruitManager = fruitManager;
			this.gameController = gameController;
			this.gameView = gameView;
			this.gameModel = gameModel;
		}

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
			OnFruitCombined?.Invoke(this);
			Fruit2D higherFruit = (transform.position.y > otherFruit.transform.position.y) ? this : otherFruit;
			var newVelocity = higherFruit.gameObject.GetComponent<Rigidbody2D>().velocity;
			Vector3 newFruitPos = higherFruit.transform.position;
			var newIndex = index + 1;

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}

			this.rb.velocity = otherFruit.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			this.gameObject.SetActive(false);
			otherFruit.gameObject.SetActive(false);

			if (index == Constants.FruitTypeCount)
			{
				return;
			}

			Fruit2D newFruit = fruitManager.GetFruitForDrop(newIndex, newFruitPos);
			newFruit.GetComponent<Rigidbody2D>().velocity = newVelocity;
			//if (Math.Abs(newVelocity.y) < 0.5f && Math.Abs(newVelocity.x) < 0.1f)
			//{
			//	newFruit.GetComponent<Rigidbody>().AddForce(Vector2.up, ForceMode.Impulse);
			//}
		}

		private void OnDisable()
		{
			OnFruitCombined = null;
		}

		public int FruitPoint => fruitPoint;
	}
}

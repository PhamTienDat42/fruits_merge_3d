using System;
using Game;
using Pools;
using UnityEngine;

namespace Fruits
{
	public class Fruit : MonoBehaviour
	{
		[SerializeField] private int fruitPoint;
		private GameController controller;
		private FruitPools fruitPools;
		private GameView gameView;
		private FruitManager fruitManager;

		private Rigidbody rb;
		private Vector2 velocity;

		private const string FruitManagerTag = "FruitManager";

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			var fruitManagerObj = GameObject.FindGameObjectWithTag(FruitManagerTag);
			fruitManager = fruitManagerObj.GetComponent<FruitManager>();
		}

		public void InstantiateFruits(FruitPools fruitPools, GameController gameController, GameView gameView)
		{
			this.fruitPools = fruitPools;
			this.controller = gameController;
			this.gameView = gameView;
		}
		private void OnCollisionEnter(Collision collision)
		{
			Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

			if (otherFruit != null)
			{
				if (CanCombine(otherFruit))
				{
					CombineFruit(otherFruit);
				}
			}
		}

		private bool CanCombine(Fruit otherFruit)
		{
			return fruitPoint == otherFruit.fruitPoint;
		}

		private void CombineFruit(Fruit otherFruit)
		{
			Fruit higherFruit = (transform.position.y > otherFruit.transform.position.y) ? this : otherFruit;
			var newVelocity =higherFruit.gameObject.GetComponent<Rigidbody>().velocity;
			Vector3 newFruitPos = higherFruit.transform.position;
			var newPoints = fruitPoint + 1;

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}

			this.rb.velocity = otherFruit.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.gameObject.SetActive(false);
			otherFruit.gameObject.SetActive(false);

			Fruit newFruit = fruitManager.GetFruitForCombine(newPoints, newFruitPos);
			newFruit.GetComponent<Rigidbody>().velocity = newVelocity;
			if (Math.Abs(newVelocity.y) < 0.5f && Math.Abs(newVelocity.x) < 0.1f)
			{
				newFruit.GetComponent<Rigidbody>().AddForce(Vector2.up, ForceMode.Impulse);
			}
		}

		public int FruitPoint { get => fruitPoint; set => fruitPoint = value; }
	}
}

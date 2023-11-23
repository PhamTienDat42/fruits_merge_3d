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
		private MeshRenderer meshRenderer;
		private new SphereCollider collider;
		private Rigidbody rb;
		private Vector2 velocity;

		private void Start()
		{
			meshRenderer = GetComponent<MeshRenderer>();
			rb = GetComponent<Rigidbody>();
			collider = GetComponent<SphereCollider>();
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
					CombineObjects(otherFruit);
				}
			}
		}

		private bool CanCombine(Fruit otherFruit)
		{
			return fruitPoint == otherFruit.fruitPoint;
		}

		public void SpawnNewFruit(int newPoints, Vector3 pos, Vector2 velocity)
		{
			Fruit newFruit = fruitPools.GetFruitFromPoolNew(pos);
			newFruit.fruitPoint = newPoints;
			//newFruit.GetComponent<MeshRenderer>().sprite = controller.FruitSprites[(newPoints - 1) % 9];
			//float spriteRadius = newFruit.GetComponent<MeshRenderer>().material.bounds.size.x / 2f;
			//newFruit.GetComponent<SphereCollider>().radius = spriteRadius;
			newFruit.GetComponent<Rigidbody>().velocity = velocity;
			//newFruit.GetComponent<Rigidbody>().gravityScale = 1.0f;
			if (Math.Abs(velocity.y) < 0.5f && Math.Abs(velocity.x) < 0.1f)
			{
				newFruit.GetComponent<Rigidbody>().AddForce(Vector2.up, ForceMode.Impulse);
			}
			newFruit.gameObject.SetActive(true);
		}

		private void CombineObjects(Fruit otherFruit)
		{
			Fruit higherFruit = (transform.position.y > otherFruit.transform.position.y) ? this : otherFruit;
			velocity = (transform.position.y > otherFruit.transform.position.y) ? this.rb.velocity : otherFruit.gameObject.GetComponent<Rigidbody>().velocity;

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}
			//controller.Model.CurrentScore += fruitPoint;
			//var highScore = PlayerPrefs.GetInt(Constants.HighScore, 0);
			//if (controller.Model.CurrentScore > highScore)
			//{
			//	highScore = controller.Model.CurrentScore;
			//	controller.Model.SetHighScore(highScore);
			//}
			//gameView.SetScore();

			var newPoints = fruitPoint + 1;
			fruitPools.ReturnFruitToPoolRandom(this);
			fruitPools.ReturnFruitToPoolRandom(otherFruit);

			//if (newPoints == Constants.MaxPoint)
			//{
			//	controller.Model.SetWatermelonCount();
			//	gameView.SetWatermelonCount();
			//}
			//else
			//{
			//	SpawnNewFruit(newPoints, higherFruit.transform.position, velocity / 2f);
			//}
			SpawnNewFruit(newPoints, higherFruit.transform.position, velocity / 2f);

			transform.localPosition = otherFruit.gameObject.transform.localPosition = Vector3.zero;
			transform.localRotation = otherFruit.gameObject.transform.localRotation = Quaternion.identity;
			//rb.gravityScale = otherFruit.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
		}

		public int FruitPoint { get => fruitPoint; set => fruitPoint = value; }
		public Rigidbody Rb => rb;
		public MeshRenderer MeshRenderer { get => meshRenderer; set => meshRenderer = value; }
		public SphereCollider sphereCollider { get => collider; set => collider = value; }
	}
}

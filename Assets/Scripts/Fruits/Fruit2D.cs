using System;
using Game;
using Pools;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fruits
{
	public class Fruit2D : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private int fruitPoint;
		[SerializeField] private int fruitIndex;

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
			if(fruitManager.BilliardThemeIndex == true)
			{
				gameView.PlayBilliardMergeSfx();
			}
			else
			{
				gameView.PlayMergeSfx();
			}
			OnFruitCombined?.Invoke(this);

			var higherFruit = (transform.position.y > otherFruit.transform.position.y) ? this : otherFruit;
			var newVelocity = higherFruit.gameObject.GetComponent<Rigidbody2D>().velocity;
			var newFruitPos = higherFruit.transform.position;
			var newIndex = fruitIndex + 1;

			var yParticle = (transform.position.y + otherFruit.transform.position.y) / 2.0f;
			var xParticle = (transform.position.x + otherFruit.transform.position.x) / 2.0f;
			gameView.PlayMergeParticle(xParticle, yParticle);

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}

			this.rb.velocity = otherFruit.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			this.gameObject.SetActive(false);
			otherFruit.gameObject.SetActive(false);

			if (fruitIndex == Constants.FruitTypeCount)
			{
				return;
			}

			var newBonusScorePos = new Vector3(newFruitPos.x - 0.2f, newFruitPos.y, -5.0f);
			fruitManager.ShowBonusScore(gameController.BonusScore, newBonusScorePos);

			var newFruit = fruitManager.GetFruitForDrop(newIndex, newFruitPos);
			newFruit.GetComponent<Rigidbody2D>().velocity = newVelocity;
		}

		private void OnDisable()
		{
			OnFruitCombined = null;
		}

		public Fruit2DData ToData()
		{
			var posF = gameController.ReturnFruitPositionOnZoomOutBooster(this.transform.position);

			Fruit2DData fruitData = new(posF)
			{
				FruitPoint = fruitPoint,
				FruitIndex = fruitIndex
			};
			return fruitData;
		}

		public void FromData(Fruit2DData data)
		{
			fruitPoint = data.FruitPoint;
			fruitIndex = data.FruitIndex;
			transform.position = data.GetPosition();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if(gameController.IsClickable == false && gameController.IsKnife == true)
			{
				gameView.PlayCutBallSfx();
				this.gameObject.SetActive(false);
				gameController.IsKnife = false;
				gameController.IsClickable = true;
			}
		}

		public int FruitPoint => fruitPoint;
		public int FruitIndex => fruitIndex;
		public Rigidbody2D Rb => rb;
	}
}

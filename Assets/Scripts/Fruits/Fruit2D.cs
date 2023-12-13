using System;
using Game;
using Pools;
using UnityEngine;

namespace Fruits
{
	public class Fruit2D : MonoBehaviour
	{
		[SerializeField] private int fruitPoint;
		[SerializeField] private int fruitIndex;

		private GameController gameController;
		private GameView gameView;
		private GameModel gameModel;
		private FruitPoolManager fruitManager;
		private Rigidbody2D rb;

		public event Action<Fruit2D> OnFruitCombined;

		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		public void InstantiateFruits(FruitPoolManager fruitManager, GameController gameController, GameView gameView, GameModel gameModel)
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
					CombineBall(otherFruit);
				}
			}
		}

		private bool CanCombine(Fruit2D otherFruit)
		{
			return fruitIndex == otherFruit.fruitIndex;
		}

		private void CombineBall(Fruit2D otherFruit)
		{
			OnFruitCombined?.Invoke(this);
			var newCombineBallPos = SetCombineBallBetweenPosition(this.transform.position, otherFruit.transform.position);
			var particlePosition = new Vector3(newCombineBallPos.x, newCombineBallPos.y, -5.0f);

			gameView.PlayMergeParticle(particlePosition, fruitIndex);
			PlayMergeSfx();

			if (!gameObject.activeSelf && !otherFruit.gameObject.activeSelf)
			{
				return;
			}

			ReturnOldBallsToPool(this, otherFruit);

			ShowBonusScoreOn(newCombineBallPos);

			if (fruitIndex == Constants.MaxFruitType)
			{
				return;
			}

			fruitManager.GetNewCombineBall(fruitIndex + 1, newCombineBallPos);
		}

		private void PlayMergeSfx()
		{
			if (fruitManager.BilliardThemeIndex == true)
			{
				gameView.PlayBilliardMergeSfx();
			}
			else
			{
				gameView.PlayMergeSfx();
			}
		}

		private Vector3 SetCombineBallBetweenPosition(Vector3 currentPos, Vector3 otherPos)
		{
			return new Vector3((currentPos.x + otherPos.x) / 2f, (currentPos.y + otherPos.y) / 2f, (currentPos.x + otherPos.x) / 2f);
		}

		private void ShowBonusScoreOn(Vector3 newCombineBallPos)
		{
			var bonusPos = UnityEngine.Random.Range(-0.25f, 0.25f);
			var newBonusScorePos = new Vector3(newCombineBallPos.x + bonusPos, newCombineBallPos.y + bonusPos, -5.0f);
			fruitManager.ShowBonusScore(gameController.BonusScore, newBonusScorePos);
		}

		private void ReturnOldBallsToPool(Fruit2D currentBall, Fruit2D otherBall)
		{
			currentBall.rb.velocity = otherBall.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			currentBall.gameObject.SetActive(false);
			otherBall.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			OnFruitCombined = null;
		}

		public Fruit2DData ToData()
		{
			var posF = gameController.ReturnBallPositionOnZoomOutBooster(this.transform.position);

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

		public int FruitPoint => fruitPoint;
		public int FruitIndex => fruitIndex;
		public Rigidbody2D Rb => rb;
	}
}

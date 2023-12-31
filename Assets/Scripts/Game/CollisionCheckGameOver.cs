using Game;
using UnityEngine;

namespace GamePlay
{
	public class CollisionCheckGameOver : MonoBehaviour
	{
		[SerializeField] private GameController controller;
		[SerializeField] private GameView gameView;

		private bool isColliding = false;
		private readonly float collisionDuration = 2.0f;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag(Constants.FruitTag))
			{
				isColliding = true;
				StartCoroutine(CheckCollisionDuration());
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag(Constants.FruitTag))
			{
				isColliding = false;
			}
		}

		private System.Collections.IEnumerator CheckCollisionDuration()
		{
			float startTime = Time.time;

			while (isColliding)
			{
				if (Time.time - startTime > collisionDuration)
				{
					Logger.Debug("GameOverrrrrrrrrrrrr");
					gameView.PlayGameOverSfx();
					controller.GameOver();
					yield break;
				}
				yield return null;
			}
		}
	}
}

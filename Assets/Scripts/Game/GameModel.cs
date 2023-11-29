using UnityEngine;

namespace Game
{
	public class GameModel : MonoBehaviour
	{
		private int currentScore;
		private void Start()
		{
			currentScore = 0;
		}

		public int CurrentScore { get => currentScore; set { currentScore = value;} }
	}
}

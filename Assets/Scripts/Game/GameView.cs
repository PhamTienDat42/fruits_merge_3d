using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		public void OnEscButtonClick()
		{
			SceneManager.LoadScene(Constants.HomeScene);
		}
	}
}

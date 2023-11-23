using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
	public class HomeView : MonoBehaviour
	{
		public void OnPlayButtonClick()
		{
			SceneManager.LoadScene(Constants.GameScene);
		}
	}
}

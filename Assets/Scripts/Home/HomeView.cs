using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
	public class HomeView : MonoBehaviour
	{
		public void OnPlayButtonClick()
		{
			Time.timeScale = 1.0f;
			SceneManager.LoadScene(Constants.GameScene);
		}

		public void OnBilliardThemeButtonClick()
		{
			PlayerPrefs.SetInt(Constants.Theme, 0);
		}

		public void OnSportThemeButtonClick()
		{
			PlayerPrefs.SetInt(Constants.Theme, 1);
		}
	}
}

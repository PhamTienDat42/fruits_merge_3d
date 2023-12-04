using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
	public class HomeView : MonoBehaviour
	{
		[SerializeField] private HomeController homeController;

		private ParamServices paramServices;

		private void Start()
		{
			paramServices = homeController.GameServices.GetService<ParamServices>();
		}

		public void OnPlayButtonClick()
		{
			Time.timeScale = 1.0f;
			paramServices.IsContinue = false;
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

		public void OnContinueButtonClick()
		{
			Time.timeScale = 1.0f;
			paramServices.IsContinue = true;
			SceneManager.LoadScene(Constants.GameScene);
		}
	}
}

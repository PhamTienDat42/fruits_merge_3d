using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
	public class HomeController : MonoBehaviour
	{
		public GameServices GameServices { get; set; }

		private void Awake()
		{
			var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
			if (gameServiceObj != null)
			{
				GameServices = gameServiceObj.GetComponent<GameServices>();
			}
			else
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}
		}
	}
}

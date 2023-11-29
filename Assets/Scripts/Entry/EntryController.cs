using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		[SerializeField] private EntryModel entryModel;

		private GameServices gameServices = null;

		private void Awake()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
				GameObject gameServicesObject = new(nameof(GameServices))
				{
					tag = Constants.ServicesTag
				};
				gameServices = gameServicesObject.AddComponent<GameServices>();
			}

			gameServices.AddService(new PlayerServices());
			gameServices.AddService(new ParamServices());

			//---------------------GetServices----------------------------------------
			var playerServices = gameServices.GetService<PlayerServices>();
		}

		private void Start()
		{
			Application.targetFrameRate = Constants.TargetFrameRate;
			SceneManager.LoadScene(Constants.HomeScene);
		}
	}
}

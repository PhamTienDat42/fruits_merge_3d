using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class PopupHelpers
    {
        public static void Show(string name)
        {
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive).completed += _ => {
				SetSceneActive(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
			};
        }

        public static void Close()
        {
			SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).completed += _ => {
				SetSceneActive(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
			};
        }

        public static void Close(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            SceneManager.UnloadSceneAsync(scene).completed += _ => {
                SetSceneActive(SceneManager.GetActiveScene());
            };
        }

        private static void SetSceneActive(Scene scene)
        {
            foreach (var raycaster in Object.FindObjectsOfType<BaseRaycaster>())
            {
                raycaster.enabled = raycaster.gameObject.scene == scene;
            }
            SceneManager.SetActiveScene(scene);
        }
    }
}

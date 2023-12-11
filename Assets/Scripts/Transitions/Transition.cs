using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Services;

namespace TransitionNS
{
    [RequireComponent(typeof(Animator))]
    public class Transition : MonoBehaviour
    {
        [SerializeField] private Animator transitionAnimator;

        private TransitionService transitionService;

        private const float TransitionTime = 0.5f;
        private const string EndTrantition = "End";
        private const string StartTrantition = "Start";

        public void EndLevel(string sceneName)
        {
            StartCoroutine(IStartLevel(sceneName));
        }

        public void StartLevel()
        {
            StartCoroutine(IStartTransition());
        }

        public IEnumerator IStartLevel(string sceneName)
        {
            transitionAnimator.SetTrigger(EndTrantition);
            yield return new WaitForSeconds(TransitionTime);
            SceneManager.LoadScene(sceneName);
        }

        public IEnumerator IStartTransition()
        {
            transitionAnimator.SetTrigger(StartTrantition);
            yield return new WaitForSeconds(TransitionTime);
        }

        public void Initialized(TransitionService transitionService)
        {
            this.transitionService = transitionService;
        }
    }
}
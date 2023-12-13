using TransitionNS;

namespace Services
{
    public class TransitionService
    {
        private readonly Transition transition;

        public TransitionService(Transition transition)
        {
            this.transition = transition;
            this.transition.Initialized(this);
        }

        public void PlayEndTransition(string sceneName)
        {
            transition.EndLevel(sceneName);
        }

        public void PlayStartTransition()
        {
            transition.StartLevel();
        }
    }
}

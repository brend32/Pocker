using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("ChooseDifficulty")]
    public class ChooseDifficultyScreen : PageScript
    {
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
        }

        public void Close()
        {
            HidePage();
        }
    }
}

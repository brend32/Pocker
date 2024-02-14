using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("Menu")]
    public class MenuScreen : PageScript
    {
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
        }
    }
}

using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("Menu")]
    public partial class MenuScreen : PageScript
    {
        [Dependency] private GameManager _gameManager;
        
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
            
            _gameManager.GameStarted += GameStarted;
        }

        private void GameStarted()
        {
            _gameManager.GameStarted -= GameStarted;
            HidePage();
        }

        public void OpenDifficultyChooseScreen()
        {
            PageSystem.Load<ChooseDifficultyScreen>();
        }
    }
}

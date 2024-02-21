using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("ChooseDifficulty")]
    public partial class ChooseDifficultyScreen : PageScript
    {
        [Dependency] private GameManager _gameManager;
        
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
        }

        public void Close()
        {
            HidePage();
        }

        public void StartGame()
        {
            _gameManager.StartGame(new GameSettings()
            {
                PlayersCount = 3,
                StartingCash = 200
            });
            HidePage();
        }
    }
}

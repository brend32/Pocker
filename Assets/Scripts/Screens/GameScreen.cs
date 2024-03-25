using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using Poker.Gameplay.Views;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("Game")]
    public partial class GameScreen : PageScript
    {
        [SerializeField] private PlayersView _players;

        [Dependency] private GameManager _gameManager;
        
        protected override void BeforeInit()
        {
            Context.Global.Resolve(this);
            CustomInitializer.StartInitialization(gameObject.scene);
        }

        public void OpenDifficultyChooseScreen()
        {
            PageSystem.Load<ChooseDifficultyScreen>();
        }

        public void StartGame(GameState gameState)
        {
            _players.Bind();
        }

        public void RestartGame(GameState gameState)
        {
            
        }

        public void EndGame()
        {
            _gameManager.EndGame();
            PageSystem.Load<MenuScreen>(menu =>
            {
                Unload();
            });
        }
    }
}

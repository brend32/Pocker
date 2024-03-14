using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using Poker.UI.ChooseDifficulty;
using Poker.UI.Common;
using UnityEngine;

namespace Poker.Screens
{
    [SceneInitScript("ChooseDifficulty")]
    public partial class ChooseDifficultyScreen : PageScript
    {
        [SerializeField] private ToggleGroup _difficulty;
        [SerializeField] private ToggleGroup _playersCount;
        [SerializeField] private StartingCash _startingCash;
        
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
                PlayersCount = ((PlayersCountButton)_playersCount.Current).Players,
                StartingCash = _startingCash.Value,
                Difficulty = GetDifficulty()
            });
            HidePage();
        }

        private Difficulty GetDifficulty()
        {
            if (_difficulty.Current is DifficultyCardButton button)
            {
                return button.Difficulty;
            }

            return Difficulty.Normal;
        }
    }
}

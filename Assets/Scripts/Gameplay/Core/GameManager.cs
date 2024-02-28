using System;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core.States;
using Poker.Gameplay.Core.Statistics;
using Poker.Screens;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public partial class GameManager
    {
        public event Action GameStarted;
        public event Action GameEnded;
        public event Action TimeScaleChanged;

        public ITimeSource TimeSource { get; }
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public float DeltaTime { get; set; }

        public float TimeScale
        {
            get => _timeScale;
            set
            {
                _timeScale = value;
                TimeScaleChanged?.Invoke();
            }
        }
        
        public GameStatistics Statistics { get; private set; }
        public GameState State { get; private set; } 
        public GameController Controller { get; private set; } 
        public GameSettings Settings { get; private set; }

        private readonly PageSystem _pageSystem;

        private GameScreen _gameScreen;
        private float _timeScale = 1;
        
        private class WaitGameTime : CustomYieldInstruction
        {
            private float _waitUntil = -1f;
            private readonly float _duration;
            private readonly GameManager _gameManager;

            public override bool keepWaiting
            {
                get
                {
                    var time = _gameManager.State?.PlayTime ?? -1;
                    if (time < 0)
                        return false;
                    
                    if (_waitUntil < 0.0)
                        _waitUntil = time + _duration;
                    
                    var wait = time < _waitUntil;
                    
                    if (wait == false)
                        Reset();
                    
                    return wait;
                }
            }
            
            public WaitGameTime(GameManager gameManager, float time)
            {
                _gameManager = gameManager;
                _duration = time;
            }

            public override void Reset() => _waitUntil = -1f;
        }
        
        [Dependency]
        public GameManager(PageSystem pageSystem)
        {
            _pageSystem = pageSystem;

            TimeSource = new GameTimeSource(this);
        }

        public void StartGame(GameSettings gameSettings, Action loadingScreenFullyVisible = null)
        {
            IsPlaying = true;

            SetupGame(gameSettings);
            
            if (_gameScreen == null)
            {
                _pageSystem.Load<GameScreen>(gamePage =>
                {
                    _gameScreen = gamePage;
                    gamePage.StartGame(State);
                    GameStarted?.Invoke();
                    Controller.StartGame().Forget();
                });
            }
            else
            {
                _gameScreen.RestartGame(State);
                GameStarted?.Invoke();
                Controller.StartGame().Forget();
            }
        }

        private void SetupGame(GameSettings gameSettings)
        {
            TimeScale = 1;
            IsPaused = false;

            Settings = gameSettings;

            CreateNewStates(gameSettings);
        }

        private void CreateNewStates(GameSettings gameSettings)
        {
            Statistics = new GameStatistics();
            State = new GameState(Statistics);
            Controller = new GameController(State);
            
            State.AddMe(PlayerState.CreatePlayer(gameSettings, "Me"));
            for (int i = 0; i < gameSettings.PlayersCount - 1; i++)
            {
                State.AddPlayer(PlayerState.CreateBotPlayer(gameSettings, $"Player {i + 1}"));
            }
        }

        public void EndGame()
        {
            IsPlaying = false;

            State = null;
            Statistics = null;
            Settings = null;
            try
            {
                GameEnded?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void PauseGame()
        {
            if (IsPaused)
                return;
            
            IsPaused = true;
            TimeScale = 0;
        }

        public void ContinueGame(bool resetTimeScale = true)
        {
            if (IsPaused == false)
                return;
            
            IsPaused = false;
            if (resetTimeScale)
                TimeScale = 1;
        }

        public CustomYieldInstruction Wait(float time)
        {
            return new WaitGameTime(this, time);
        }
    }
}
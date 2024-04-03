using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AurumGames.Animation;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.BotLogic;
using Poker.Gameplay.Core.States;
using Poker.Gameplay.Core.Statistics;
using Poker.Screens;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public partial class GameManager
    {
        public event Action GameStarted;
        public event Action<PlayerState> GameEnded;
        public event Action TimeScaleChanged;

        public bool BotsGame { get; private set; }
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
        private CancellationTokenSource _tokenSource;
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

        private sealed class GameDelayPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<GameDelayPromise>
        {
            private static TaskPool<GameDelayPromise> _pool;
            private GameDelayPromise _nextNode;
            private GameManager _gameManager;
            public ref GameDelayPromise NextNode => ref _nextNode;

            static GameDelayPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(GameDelayPromise), () => _pool.Size);
            }

            private int _initialFrame;
            private float _delayTimeSpan;
            private float _elapsed;
            private CancellationToken _cancellationToken;
            private CancellationTokenRegistration _cancellationTokenRegistration;

            private UniTaskCompletionSourceCore<object> _core;

            private GameDelayPromise(GameManager gameManager)
            {
                _gameManager = gameManager;
            }

            public static IUniTaskSource Create(GameManager gameManager, float seconds, CancellationToken cancellationToken,
                out short token, bool cancelImmediately = false)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new GameDelayPromise(gameManager);
                }

                result._elapsed = 0.0f;
                result._delayTimeSpan = seconds;
                result._cancellationToken = cancellationToken;
                result._initialFrame = PlayerLoopHelper.IsMainThread ? Environment.TickCount : -1;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result._cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (GameDelayPromise)state;
                        promise._core.TrySetCanceled(promise._cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, result);

                token = result._core.Version;
                return result;
            }

            public void GetResult(short token)
            {
                try
                {
                    _core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            public UniTaskStatus GetStatus(short token)
            {
                return _core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return _core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return false;
                }

                if (_gameManager == null || _gameManager.IsPlaying == false)
                {
                    _core.TrySetResult(null);
                    return false;
                }
                
                if (_elapsed == 0.0f)
                {
                    if (_initialFrame == Environment.TickCount)
                    {
                        return true;
                    }
                }

                _elapsed += _gameManager.DeltaTime;
                if (_elapsed >= _delayTimeSpan)
                {
                    _core.TrySetResult(null);
                    return false;
                }

                return true;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _delayTimeSpan = default;
                _elapsed = default;
                _cancellationToken = default;
                _cancellationTokenRegistration.Dispose();
                return _pool.TryPush(this);
            }
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
            BotsGame = false;

            _tokenSource = new CancellationTokenSource();
            SetupGame(gameSettings);
            CreateNewStates(gameSettings);
            
            _pageSystem.LoadWithLoading<LoadingScreen, GameScreen>(gamePage =>
            {
                _gameScreen = gamePage;
                gamePage.StartGame(State);
                GameStarted?.Invoke();
                Controller.StartGame(_tokenSource.Token);
            });
        }
        
        public Task StartBotGame(GameSettings gameSettings, BotState[] bots)
        {
            lock (this)
            {
                IsPlaying = true;
                BotsGame = true;
                DeltaTime = 1;

                _tokenSource = new CancellationTokenSource();
                SetupGame(gameSettings);
                CreateNewStates(gameSettings, bots);

                GameStarted?.Invoke();
                return Controller.StartGame(_tokenSource.Token);
            }
        }

        private void SetupGame(GameSettings gameSettings)
        {
            CombinationOdds.Prepare();
            
            TimeScale = 1;
            IsPaused = false;

            Settings = gameSettings;
        }

        private void CreateNewStates(GameSettings gameSettings, BotState[] bots = null)
        {
            Statistics = new GameStatistics();
            State = new GameState(Statistics);
            Controller = new GameController(this, State);

            if (bots == null)
            {
                State.AddMe(PlayerState.CreatePlayer(gameSettings, "Me"));
                for (int i = 0; i < gameSettings.PlayersCount - 1; i++)
                {
                    State.AddPlayer(BotState.CreateBotPlayer(gameSettings, $"Player {i + 1}"));
                }
            }
            else
            {
                foreach (BotState bot in bots)
                {
                    State.AddPlayer(bot);
                }
            }
        }

        public void EndGame(PlayerState playerState)
        {
            lock (this) // Use synchronization to avoid multi thread write
            {
                if (IsPlaying == false)
                    return;
                
                IsPlaying = false;

                State = null;
                Statistics = null;
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
                try
                {
                    GameEnded?.Invoke(playerState);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
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
        
        public UniTask DelayAsync(float time, CancellationToken cancellationToken = default, bool ignoreOnBotGame = true)
        {
            if (BotsGame && ignoreOnBotGame)
                return UniTask.CompletedTask;
            
            return new UniTask(GameDelayPromise.Create(this, time / 1000f, cancellationToken, out var token), token);
        }
    }
}
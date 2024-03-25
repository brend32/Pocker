using System;
using System.Collections.Generic;
using System.Threading;
using AurumGames.Animation;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Cysharp.Threading.Tasks;
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
                result._initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;

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

                if (_elapsed == 0.0f)
                {
                    if (_initialFrame == Time.frameCount)
                    {
                        return true;
                    }
                }

                if (_gameManager == null || _gameManager.IsPlaying == false)
                {
                    _core.TrySetResult(null);
                    return false;
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
            Controller = new GameController(this, State);
            
            State.AddMe(PlayerState.CreatePlayer(gameSettings, "Me"));
            for (int i = 0; i < gameSettings.PlayersCount - 1; i++)
            {
                State.AddPlayer(BotState.CreateBotPlayer(this, gameSettings, State, $"Player {i + 1}"));
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
        
        public UniTask DelayAsync(float time, CancellationToken cancellationToken = default)
        {
            return new UniTask(GameDelayPromise.Create(this, time / 1000f, cancellationToken, out var token), token);
        }
    }
}
using System;
using System.Collections;
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Group initialization
    /// </summary>
    public sealed class InitializationGroup
    {
        /// <summary>
        /// Initialization ended successfully 
        /// </summary>
        public event Action InitializationEnd;
        /// <summary>
        /// Initialization failed
        /// </summary>
        public event Action InitializationError;
        
        /// <summary>
        /// Initialization scope
        /// </summary>
        public Context Scope { get; set; } = Context.Global;

        private int _endedCount;
        private bool _initializationEnded;
        private Coroutine _timeoutLoop;
        private readonly int _timeout;
        private readonly MonoBehaviour _mono;
        private readonly IInitializer[] _initializers;

        public InitializationGroup(MonoBehaviour mono, int timeout, params IInitializer[] initializers)
        {
            _mono = mono;
            _timeout = timeout;
            _initializers = initializers;
        }

        /// <summary>
        /// Begin initialization process 
        /// </summary>
        public void StartInitialization()
        {
            _timeoutLoop = _mono.StartCoroutine(TimeoutLoop());
            foreach (IInitializer initializer in _initializers)
            {
                void InitEnd(IInitializer sender)
                {
                    sender.InitializationEnd -= InitEnd;
                    _endedCount++;

                    if (_initializers.Length == _endedCount && _initializationEnded == false)
                    {
                        if (_timeoutLoop != null) _mono.StopCoroutine(_timeoutLoop);

                        InitializationEnd?.Invoke();
                        _initializationEnded = true;
                    }
                }

                initializer.InitializationEnd += InitEnd;
                Scope.Resolve(initializer);
                initializer.Initialize(Scope);
            }
        }

        /// <summary>
        /// Get initialization status
        /// </summary>
        /// <returns>True if initialized</returns>
        public bool IsInitializationEnded()
        {
            return _initializationEnded;
        }

        private IEnumerator TimeoutLoop()
        {
            var expireTime = Time.unscaledTime + _timeout;
            while (expireTime > Time.unscaledTime)
            {
                yield return null;
            }

            _endedCount = -1;
            Debug.Log("Timeout");
            InitializationError?.Invoke();
        }
    }
}
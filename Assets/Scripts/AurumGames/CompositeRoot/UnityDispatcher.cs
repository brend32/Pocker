using System;
using System.Collections.Generic;
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Call actions in main thread
    /// </summary>
    public sealed class UnityDispatcher : MonoBehaviour
    {
        private static UnityDispatcher _instance;
        private static readonly Queue<Action> ExecutionQueue = new();

        private void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0)
                {
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
            {
                _instance = this;
            }
        }

        /// <summary>
        /// Add action to queue
        /// </summary>
        /// <param name="action">Action</param>
        public void Enqueue(Action action)
        {
            lock (ExecutionQueue)
            {
                ExecutionQueue.Enqueue(() => { action?.Invoke(); });
            }
        }

        private static bool Exists()
        {
            return _instance != null;
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <returns>Instance</returns>
        /// <exception cref="Exception">No instance found</exception>
        public static UnityDispatcher Instance()
        {
            if (!Exists())
            {
                throw new Exception(
                    "UnityDispatcher could not find the UnityDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
            }

            return _instance;
        }
    }
}


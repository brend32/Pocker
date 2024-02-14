using System;
using UnityEngine;

namespace AurumGames.CompositeRoot
{
    internal sealed class LazyMonoEarlyExecution : MonoBehaviour
    {
        [SerializeField] private bool _instant;

        internal bool _shouldManuallyCallDestroy = true;
        internal LazyMonoBehaviour[] _lazyMonos;
        
        internal void Awake()
        {
            _shouldManuallyCallDestroy = false;
            _lazyMonos = GetComponents<LazyMonoBehaviour>();
            foreach (LazyMonoBehaviour mono in _lazyMonos)
            {
                if (_instant)
                {
                    mono.InstantInit();
                }
                else
                {
                    mono.InternalAwake();
                }
            }
        }

        internal void Destroy()
        {
            foreach (LazyMonoBehaviour mono in _lazyMonos)
            {
                mono.OnDestroy();
            }
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}
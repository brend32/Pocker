using System;
using System.Collections.Generic;

namespace AurumGames.CompositeRoot
{
    internal class CustomInitializationGroup
    {
        private readonly List<LazyMonoBehaviour> _initWaiters = new();
        private bool _used;
        
        internal void Wait(LazyMonoBehaviour mono)
        {
            if (_used)
            {
                throw new Exception("CustomInitializationGroup is already used");
            }
            
            _initWaiters.Add(mono);
        }

        public void StartInitialization(Context context)
        {
            if (_used)
                return;
            
            foreach (LazyMonoBehaviour waiter in _initWaiters)
            {
                if (waiter is IFastResolvable)
                {
                    Context resolveContext = context;
                    resolveContext ??= waiter.CurrentContext;
                    resolveContext.Resolve(waiter);
                }
                waiter.AfterResolve();
            }
            foreach (LazyMonoBehaviour waiter in _initWaiters)
            {
                waiter.ContinueInit();
            }

            _used = true;
            _initWaiters.Clear();
        }
    }
}
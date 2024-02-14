using System;
using System.Collections.Generic;

namespace AurumGames.CompositeRoot
{
    public struct IndependentEvent
    {
        private List<Action> _actions;
        private readonly Action<Exception> _errorHandler;

        public IndependentEvent(Action<Exception> errorHandler)
        {
            _actions = new List<Action>();
            _errorHandler = errorHandler;
        }


        public void Add(Action action)
        {
            _actions ??= new List<Action>();
            _actions.Add(action);
        }

        public void Remove(Action action)
        {
            _actions ??= new List<Action>();
            _actions.Remove(action);
        }
        
        public void Free()
        {
            _actions?.Clear();
        }

        public void Invoke()
        {
            _actions ??= new List<Action>();

            foreach (var action in _actions)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    _errorHandler?.Invoke(e);
                }
            }
        }
    }
    
    public struct IndependentEvent<T>
    {
        private List<Action<T>> _actions;
        private readonly Action<Exception> _errorHandler;

        public IndependentEvent(Action<Exception> errorHandler)
        {
            _actions = new List<Action<T>>();
            _errorHandler = errorHandler;
        }


        public void Add(Action<T> action)
        {
            _actions ??= new List<Action<T>>();
            _actions.Add(action);
        }

        public void Remove(Action<T> action)
        {
            _actions ??= new List<Action<T>>();
            _actions.Remove(action);
        }

        public void Free()
        {
            _actions?.Clear();
        }

        public void Invoke(T arg1)
        {
            _actions ??= new List<Action<T>>();

            foreach (var action in _actions)
            {
                try
                {
                    action(arg1);
                }
                catch (Exception e)
                {
                    _errorHandler?.Invoke(e);
                }
            }
        }
    }
    
    public struct IndependentEvent<T1, T2>
    {
        private List<Action<T1, T2>> _actions;
        private readonly Action<Exception> _errorHandler;

        public IndependentEvent(Action<Exception> errorHandler)
        {
            _actions = new List<Action<T1, T2>>();
            _errorHandler = errorHandler;
        }


        public void Add(Action<T1, T2> action)
        {
            _actions ??= new List<Action<T1, T2>>();
            _actions.Add(action);
        }

        public void Remove(Action<T1, T2> action)
        {
            _actions ??= new List<Action<T1, T2>>();
            _actions.Remove(action);
        }
        
        public void Free()
        {
            _actions.Clear();
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            _actions ??= new List<Action<T1, T2>>();

            foreach (var action in _actions)
            {
                try
                {
                    action(arg1, arg2);
                }
                catch (Exception e)
                {
                    _errorHandler?.Invoke(e);
                }
            }
        }
    }
}
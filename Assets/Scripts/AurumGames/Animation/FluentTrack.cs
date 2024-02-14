using System;
using UnityEngine;

namespace AurumGames.Animation
{
    public interface IFluentTrack
    {
        bool IsReachedEnd();
        void EvaluateNext(float t);
        void End();
    }

    /// <summary>
    /// Track for FluentAnimationPlayer
    /// </summary>
    /// <typeparam name="T">Track data type</typeparam>
    public abstract class FluentTrack<T> : IFluentTrack
    {
        public bool IgnoreDelayIfRunning { get; set; }
        
        private Transition _defaultTransition;
        private Transition _transition;
        private T _from;
        private T _to;
        private float _position;
        private bool _reachedEnd = true;
        
        protected FluentTrack(Transition defaultTransition)
        {
            _defaultTransition = _transition = defaultTransition;
        }
        
        /// <summary>
        /// Set target value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="options">Transition options</param>
        /// <returns>Track</returns>
        public void Set(T value, TransitionOptions options = default)
        {
            _from = Current();
            _to = value;
            _transition = options.Transition ?? _defaultTransition;

            if (options.IgnoreDelay.HasValue)
            {
                var delay = _transition.Delay;
                if (options.Delay.HasValue)
                {
                    delay = options.Delay.Value;
                }
                
                _position = options.IgnoreDelay == true ? delay : 0;
            }
            else if (IgnoreDelayIfRunning && _reachedEnd == false)
            {
                _position = _transition.Delay;
            }
            else if (options.Delay.HasValue)
            {
                _position = _transition.Delay - options.Delay.Value;
            }
            else
            {
                _position = 0;
            }
            _reachedEnd = false;
        }

        public void EvaluateNext(float t)
        {
            if (_reachedEnd)
                return;
            
            _position += t;
            if (Intersecting(_position))
            {
                Apply(_from, _to, EasedTimePercentage(_position));
            }
            else
            {
                End();
            }
        }

        public bool IsReachedEnd()
        {
            return _reachedEnd;
        }

        public void End()
        {
            Apply(_from, _to, 1);
            _reachedEnd = true;
        }
        
        private bool Intersecting(float time)
        {
            return _transition.Duration >= time;
        }

        private float EasedTimePercentage(float time)
        {
            return _transition.Ease(Mathf.Clamp01((time - _transition.Delay) / _transition.Time));
        }

        protected abstract T Current();
        protected abstract void Apply(T from, T to, float t);
    }
}
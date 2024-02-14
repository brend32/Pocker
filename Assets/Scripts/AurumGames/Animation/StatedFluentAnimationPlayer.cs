using System;
using System.Collections.Generic;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Stated animation using FluentAnimationPlayer
    /// </summary>
    /// <typeparam name="T">State type</typeparam>
    public sealed class StatedFluentAnimationPlayer<T>
    {
        public event StateChangedEventArgs StateChanged;
        public event AnimationEndedEventArgs AnimationEnded;

        public delegate void StateChangedEventArgs(T previous, T current, TransitionOptions options);
        public delegate void AnimationEndedEventArgs(T previous, T current);

        public T CurrentState { get; private set; }
        public T PreviousState { get; private set; }
        public ITimeSource TimeSource
        {
            get => _player.TimeSource;
            set => _player.TimeSource = value;
        }
        public bool IsPlaying => _player.IsPlaying;

        private readonly Dictionary<T, Dictionary<T, Action>> _transitionCallback = new();
        private readonly FluentAnimationPlayer _player;
        private readonly GameObject _playerOwner;

        /// <summary>
        /// Stated animation using FluentAnimationPlayer
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="tracks">Tracks</param>
        /// <exception cref="Exception">Minimum 1 track required</exception>
        public StatedFluentAnimationPlayer(MonoBehaviour mono, params IFluentTrack[] tracks)
        {
            if (tracks.Length < 1)
                throw new Exception("Tracks count must be greeter than 0");

            _playerOwner = mono.gameObject;
            _player = new FluentAnimationPlayer(mono, tracks);
            _player.Ended += () =>
            {
                AnimationEnded?.Invoke(PreviousState, CurrentState);

                if (!_transitionCallback.ContainsKey(PreviousState)) return;
                
                if (_transitionCallback[PreviousState].ContainsKey(CurrentState))
                {
                    _transitionCallback[PreviousState][CurrentState]?.Invoke();
                }
            };
        }

        /// <summary>
        /// Set callback when state changes from <paramref name="from"/> to <paramref name="to"/> state
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="callback"></param>
        public void SetTransitionCallback(T from, T to, Action callback)
        {
            if (_transitionCallback.ContainsKey(from) == false)
            {
                _transitionCallback.Add(from, new Dictionary<T, Action>());
            }

            if (_transitionCallback[from].ContainsKey(to) == false)
            {
                _transitionCallback[from].Add(to, callback);
            }
            else
            {
                _transitionCallback[from][to] = callback;
            }
        }

        /// <summary>
        /// Play animation
        /// </summary>
        public void Update()
        {
            _player.Update();
        }

        /// <summary>
        /// Change values instantly
        /// </summary>
        public void UpdateInstant()
        {
            _player.UpdateInstant();
        }

        /// <summary>
        /// Change state
        /// </summary>
        /// <param name="state">New state</param>
        /// <param name="options">Transition options</param>
        public void SetState(T state, TransitionOptions options = default)
        {
            ChangeState(state, options);
            if (_playerOwner.activeInHierarchy)
            {
                Update();
            }
            else
            {
                UpdateInstant();
            }
        }
        
        /// <summary>
        /// Change state instantly
        /// </summary>
        public void SetStateInstant(T state)
        {
            ChangeState(state, default);
            UpdateInstant();
        }

        /// <summary>
        /// Change state without animation
        /// </summary>
        /// <param name="state">New state</param>
        public void SetStateWithoutUpdate(T state)
        {
            ChangeState(state, default);
        }

        private void ChangeState(T state, TransitionOptions options)
        {
            PreviousState = CurrentState;
            CurrentState = state;
            StateChanged?.Invoke(PreviousState, CurrentState, options);
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        public void Stop()
        {
            _player.Stop();
        }
    }
}
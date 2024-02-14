using System;
using System.Collections.Generic;
using AurumGames.Animation.Tracks;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Animation that depends on current state
    /// </summary>
    /// <typeparam name="T">State type</typeparam>
    public sealed class StatedAnimationPlayer<T>
    {
        /// <summary>
        /// State changed
        /// </summary>
        public event StatedEventArgs StateChanged;
        /// <summary>
        /// Animation ended
        /// </summary>
        public event StatedEventArgs AnimationEnded;
        /// <summary>
        /// Called on every animation tick
        /// </summary>
        public event Action<float> Step;

        public delegate void StatedEventArgs(T previous, T current);

        public T CurrentState { get; private set; }
        public T PreviousState { get; private set; }
        /// <summary>
        /// Time source
        /// </summary>
        public ITimeSource TimeSource
        {
            get => _player.TimeSource;
            set => _player.TimeSource = value;
        }
        /// <summary>
        /// Is animation playing
        /// </summary>
        public bool IsPlaying => _player.IsPlaying;

        private readonly ITrackEvaluatorProxy _tracksSource; 
            
        private readonly AnimationPlayer _player;
        private readonly GameObject _playerOwner;

        public interface ITrackEvaluatorProxy
        {
            TracksEvaluator TrackFromState(T state);
        }
        
        private class EvaluatorProxyMap : ITrackEvaluatorProxy
        {
            private readonly Dictionary<T, TracksEvaluator> _tracks;

            public EvaluatorProxyMap(Dictionary<T, TracksEvaluator> tracks)
            {
                _tracks = tracks;
            }

            public TracksEvaluator TrackFromState(T state)
            {
                return _tracks[state];
            }
        }
        
        /// <summary>
        /// Animation that depends on current state
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="tracks">Tracks map</param>
        /// <exception cref="Exception">Minimum 2 states required</exception>
        public StatedAnimationPlayer(MonoBehaviour mono, ITrackEvaluatorProxy proxy)
        {
            _tracksSource = proxy;
            _playerOwner = mono.gameObject;
            _player = new AnimationPlayer(mono, new VoidTrack());
            _player.Ended += () =>
            {
                AnimationEnded?.Invoke(PreviousState, CurrentState);
            };
            _player.Step += (time) =>
            {
                Step?.Invoke(time);
            };
        }

        /// <summary>
        /// Animation that depends on current state
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="tracks">Tracks map</param>
        /// <exception cref="Exception">Minimum 2 states required</exception>
        public StatedAnimationPlayer(MonoBehaviour mono, Dictionary<T, TracksEvaluator> tracks) : this(mono, new EvaluatorProxyMap(tracks))
        {
            if (tracks.Count < 2)
                throw new Exception("Tracks count must be greeter than 1");
        }

        /// <summary>
        /// Change state
        /// </summary>
        /// <param name="state">New state</param>
        /// <param name="update">False: change without playing animation</param>
        public void SetState(T state, bool update = true)
        {
            if (update)
            {
                if (_playerOwner.activeInHierarchy == false)
                {
                    ChangeState(state);
                    UpdateTracks();
                    _player.JumpEnd();
                    return;
                }
            
                _player.End();
                ChangeState(state);
                UpdateTracks();
                _player.Play();
            }
            else
            {
                ChangeState(state);
            }
        }
        
        /// <summary>
        /// Change state instantly 
        /// </summary>
        /// <param name="state">New state</param>
        public void SetStateInstant(T state)
        {
            ChangeState(state);
            UpdateTracks();
            _player.JumpEnd();
        }

        private void UpdateTracks()
        {
            TracksEvaluator tracksEvaluator = _tracksSource.TrackFromState(CurrentState);
            _player.ReplaceEvaluator(tracksEvaluator);
        }

        private void ChangeState(T state)
        {
            PreviousState = CurrentState;
            CurrentState = state;
            StateChanged?.Invoke(PreviousState, CurrentState);
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
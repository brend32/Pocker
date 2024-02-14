using System;
using System.Collections;
using System.Collections.Generic;
using AurumGames.Animation.Tracks;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Animation from current state to target state
    /// </summary>
    public sealed class FluentAnimationPlayer
    {
        /// <summary>
        /// Animation ended
        /// </summary>
        public event Action Ended;

        /// <summary>
        /// Time source
        /// </summary>
        public ITimeSource TimeSource { get; set; } = TimeSources.Scaled;
        /// <summary>
        /// Is animation playing
        /// </summary>
        public bool IsPlaying => _coroutine != null;

        private IFluentTrack[] _tracks;
        private Coroutine _coroutine;
            
        private readonly MonoBehaviour _owner;

        /// <summary>
        /// Animation from current state to target state
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="tracks">Tracks</param>
        public FluentAnimationPlayer(MonoBehaviour mono, params IFluentTrack[] tracks)
        {
            _owner = mono;
            _tracks = tracks;
        }

        /// <summary>
        /// Start the animation
        /// Note: You must set a value to track before calling update
        /// </summary>
        public void Update()
        {
            if (IsPlaying)
                return;

            _coroutine = _owner.StartCoroutine(Iterator());
        }

        /// <summary>
        /// Instantly change values
        /// Note: You must set a value to track before calling update
        /// </summary>
        public void UpdateInstant()
        {
            End();
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        public void Stop()
        {
            StopCoroutine();
            Ended?.Invoke();
        }
        
        /// <summary>
        /// Stop animation and jumps to end
        /// </summary> 
        public void End()
        {
            StopCoroutine();
            foreach (IFluentTrack fluentTrack in _tracks)
            {
                if (fluentTrack.IsReachedEnd())
                    continue;
                
                fluentTrack.End();
            }
            
            Ended?.Invoke();
        }
        
        private IEnumerator Iterator()
        {
            var lastStepTime = TimeSource.Time;

            bool hasActiveTracks;
            do
            {
                yield return null;
                
                var stepTime = TimeSource.Time;
                var timeDelta = (stepTime - lastStepTime) * 1000f;

                hasActiveTracks = false;
                foreach (IFluentTrack fluentTrack in _tracks)
                {
                    if (fluentTrack.IsReachedEnd())
                        continue;

                    fluentTrack.EvaluateNext(timeDelta);
                    hasActiveTracks |= fluentTrack.IsReachedEnd() ^ true;
                }

                lastStepTime = stepTime;
            } while (hasActiveTracks);
            
            _coroutine = null;
            Ended?.Invoke();
        }

        private void StopCoroutine()
        {
            if (_coroutine != null)
            {
                _owner.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}
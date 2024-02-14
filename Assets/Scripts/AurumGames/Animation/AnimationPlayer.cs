using System;
using System.Collections;
using UnityEngine;

namespace AurumGames.Animation
{
    /// <summary>
    /// Animation based on tracks and keyframes
    /// </summary>
    public sealed class AnimationPlayer
    {
        /// <summary>
        /// Animation ended
        /// </summary>
        public event Action Ended;
        /// <summary>
        /// Animation started
        /// </summary>
        public event Action Started;
        /// <summary>
        /// Called on every animation tick
        /// </summary>
        public event Action<float> Step;

        /// <summary>
        /// Time source
        /// </summary>
        public ITimeSource TimeSource { get; set; } = TimeSources.Scaled;
        /// <summary>
        /// Is animation playing
        /// </summary>
        public bool IsPlaying => _coroutine != null;
        /// <summary>
        /// Start again after finish
        /// </summary>
        public bool Loop { get; set; }
        
        private readonly MonoBehaviour _owner;

        private TracksEvaluator _evaluator;
        private bool _isStopped;
        private Coroutine _coroutine;

        /// <summary>
        /// Animation based on tracks and keyframes
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="tracks">Animation tracks</param>
        public AnimationPlayer(MonoBehaviour mono, params ITrack[] tracks) : this(mono, new TracksEvaluator(tracks))
        {
        }
        
        /// <summary>
        /// Animation based on tracks and keyframes
        /// </summary>
        /// <param name="mono">Owner</param>
        /// <param name="evaluator">Tracks group</param>
        public AnimationPlayer(MonoBehaviour mono, TracksEvaluator evaluator)
        {
            _evaluator = evaluator;
            _owner = mono;
        }

        /// <summary>
        /// Set new tracks and reset state
        /// </summary>
        /// <param name="tracks">New tracks</param>
        public void SetTracks(params ITrack[] tracks)
        {
            _evaluator.SetTracks(tracks);
            Reset();
        }

        internal void ReplaceEvaluator(TracksEvaluator tracksEvaluator)
        {
            _evaluator = tracksEvaluator;
            Reset();
        }

        /// <summary>
        /// Start or continue animation forward
        /// </summary>
        public void Play()
        {
            _evaluator.PlayBackwards = false;
            if (_isStopped == false)
            {
                Reset();
            }

            if (_owner.gameObject.activeInHierarchy)
                StartCoroutine();
        }
        
        /// <summary>
        /// Stop animation
        /// </summary>
        public void End()
        {
            if (AnimationIsPlaying() == false)
                return;

            StopCoroutine();
            _evaluator.EvaluateEnd();
            Ended?.Invoke();
        }

        /// <summary>
        /// Start or continue animation backwards
        /// </summary>
        public void PlayBackwards()
        {
            _evaluator.PlayBackwards = true;
            if (_isStopped == false)
            {
                Reset();
            }
            
            if (_owner.gameObject.activeInHierarchy)
                StartCoroutine();
        }

        /// <summary>
        /// Resets state and start animation from the first frame
        /// </summary>
        public void PlayFromStart()
        {
            _isStopped = false;
            Play();
        }
        
        /// <summary>
        /// Resets state and start animation from the last frame
        /// </summary>
        public void PlayFromEnd()
        {
            _isStopped = false;
            PlayBackwards();
        }

        /// <summary>
        /// Instantly jumps to the end
        /// </summary>
        public void JumpEnd()
        {
            Reset();
            _evaluator.EvaluateEnd();
            Ended?.Invoke();
        }

        /// <summary>
        /// Instantly jumps to the start
        /// </summary>
        public void JumpStart()
        {
            Reset();
            _evaluator.EvaluateStart();
            Ended?.Invoke();
        }
        
        private void StartCoroutine()
        {
            StopCoroutine();
            _isStopped = false;
            _coroutine = _owner.StartCoroutine(Iterator());
            Started?.Invoke();
        }

        /// <summary>
        /// Stops animation
        /// </summary>
        public void Stop()
        {
            StopCoroutine();
            _isStopped = true;
        }

        private void StopCoroutine()
        {
            if (_coroutine == null) 
                return;
            
            _owner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        /// <summary>
        /// Resets state (required for TriggerTrack)
        /// </summary>
        public void Reset()
        {
            StopCoroutine();
            _evaluator.Reset();
            _isStopped = false;
        }

        private IEnumerator Iterator()
        {
            var lastStepTime = TimeSource.Time;

            while (AnimationIsPlaying())
            {
                var stepTime = TimeSource.Time;
                Step?.Invoke(_evaluator.Position);
                _evaluator.EvaluateNext((stepTime - lastStepTime) * 1000f);
                lastStepTime = stepTime;
                
                yield return null;
            } 
            

            if (_isStopped)
                yield break;
            
            _evaluator.EvaluateEnd();
            _coroutine = null;
            if (Loop)
            {
                if (_evaluator.PlayBackwards)
                {
                    PlayFromEnd();
                }
                else
                {
                    PlayFromStart();
                }
            }
            Ended?.Invoke();
        }

        private bool AnimationIsPlaying()
        {
            return !_isStopped && _evaluator.IsReachEnd() == false;
        }
    }
}
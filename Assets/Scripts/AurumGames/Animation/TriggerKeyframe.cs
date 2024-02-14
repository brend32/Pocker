using System;

namespace AurumGames.Animation
{
    /// <summary>
    /// Event keyframe
    /// </summary>
    public sealed class TriggerKeyFrame
    {
        public readonly float Time;
        
        private readonly Action _callback;
        
        private bool _isWasCalled;
        
        /// <summary>
        /// Event keyframe
        /// </summary>
        /// <param name="time">Fire time</param>
        /// <param name="callback">Action</param>
        public TriggerKeyFrame(float time, Action callback)
        {
            Time = time;
            _callback = callback;
        }

        /// <summary>
        /// Reset state
        /// </summary>
        public void ResetState()
        {
            _isWasCalled = false;
        }

        /// <summary>
        /// Try to fire action
        /// </summary>
        /// <param name="time">Current time</param>
        /// <param name="isBackwards">Is moving backwards</param>
        public void TryCall(float time, bool isBackwards)
        {
            if (_isWasCalled)
                return;

            if (isBackwards)
            {
                if (time > Time) return;
                
                _callback?.Invoke();
                _isWasCalled = true;
            }
            else
            {
                if (time < Time) return;
                
                _callback?.Invoke();
                _isWasCalled = true;
            }
        }
    }
}
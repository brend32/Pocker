using System.Linq;

namespace AurumGames.Animation
{
    public interface ITrack
    {
        /// <summary>
        /// Time from 0 to the last frame
        /// </summary>
        float FullDuration { get; }
        /// <summary>
        /// Calculate value at the <paramref name="time"/>
        /// </summary>
        /// <param name="time">Time</param>
        /// <param name="isBackwards">Move backwards</param>
        void Evaluate(float time, bool isBackwards);
        /// <summary>
        /// Reset state of the track
        /// </summary>
        void ResetState();
    }
    
    /// <summary>
    /// Track represents value in time
    /// </summary>
    /// <typeparam name="T">Track data type</typeparam>
    public abstract class Track<T> : ITrack
    {
        public float FullDuration { get; }

        private readonly KeyFramePair<T>[] _pairs;
        private readonly float _startTime;

        protected Track(KeyFrame<T>[] keyFrames)
        {
            _pairs = KeyFramePair<T>.FromFrames(keyFrames);
            FullDuration = keyFrames[^1].Time;

            _startTime = keyFrames[0].Time;
        }

        public void Evaluate(float time, bool isBackwards = false)
        {
            if (time >= _startTime && time <= FullDuration)
            {
                foreach (var pair in _pairs)
                {
                    if (pair.Intersecting(time) == false)
                        continue;

                    CallApply(pair, time);
                    return;
                }
            }

            InfinityTimeLineEvaluation(time);
        }

        private void InfinityTimeLineEvaluation(float time)
        {
            CallApply(time < _startTime ? _pairs[0] : _pairs[^1], time);
        }

        private void CallApply(KeyFramePair<T> pair, float time)
        {
            Apply(pair.From.Value, pair.To.Value, pair.EasedTimePercentage(time));
        }

        public virtual void ResetState()
        {
            
        }

        protected abstract void Apply(T from, T to, float t);
    }
}
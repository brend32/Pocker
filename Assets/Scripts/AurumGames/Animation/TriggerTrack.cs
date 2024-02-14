using System.Linq;

namespace AurumGames.Animation
{
    /// <summary>
    /// Events track
    /// </summary>
    public sealed class TriggerTrack : ITrack
    {
        public float FullDuration { get; }

        private readonly TriggerKeyFrame[] _keyFrames;

        /// <summary>
        /// Events track
        /// </summary>
        /// <param name="keyFrames">Keyframes</param>
        public TriggerTrack(TriggerKeyFrame[] keyFrames)
        {
            _keyFrames = keyFrames;
            FullDuration = keyFrames[^1].Time;
        }

        public void Evaluate(float time, bool isBackwards)
        {
            foreach (TriggerKeyFrame keyFrame in _keyFrames)
            {
                keyFrame.TryCall(time, isBackwards);
            }
        }

        public void ResetState()
        {
            foreach (TriggerKeyFrame keyFrame in _keyFrames)
            {
                keyFrame.ResetState();
            }
        }
    }
}
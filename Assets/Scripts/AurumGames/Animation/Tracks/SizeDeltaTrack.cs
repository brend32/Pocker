using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class SizeDeltaTrack : Track<Vector2>
    {
        private readonly RectTransform _target;
        
        public SizeDeltaTrack(RectTransform target, KeyFrame<Vector2>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.sizeDelta = Vector2.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentSizeDeltaTrack : FluentTrack<Vector2>
    {
        private readonly RectTransform _target;
        
        public FluentSizeDeltaTrack(RectTransform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector2 Current()
        {
            return _target.sizeDelta;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.sizeDelta = Vector2.LerpUnclamped(from, to, t);
        }
    }
}
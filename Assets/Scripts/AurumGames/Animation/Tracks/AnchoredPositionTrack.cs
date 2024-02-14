using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class AnchoredPositionTrack : Track<Vector2>
    {
        private readonly RectTransform _target;
        
        public AnchoredPositionTrack(RectTransform target, KeyFrame<Vector2>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentAnchoredPositionTrack : FluentTrack<Vector2>
    {
        private readonly RectTransform _target;
        
        public FluentAnchoredPositionTrack(RectTransform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector2 Current()
        {
            return _target.anchoredPosition;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
        }
    }
}
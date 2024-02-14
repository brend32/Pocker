using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class AnchorMaxTrack : Track<Vector2>
    {
        private readonly RectTransform _target;
        
        public AnchorMaxTrack(RectTransform target, KeyFrame<Vector2>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchorMax = Vector2.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentAnchorMaxTrack : FluentTrack<Vector2>
    {
        private readonly RectTransform _target;
        
        public FluentAnchorMaxTrack(RectTransform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector2 Current()
        {
            return _target.anchorMax;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchorMax = Vector2.LerpUnclamped(from, to, t);
        }
    }
}
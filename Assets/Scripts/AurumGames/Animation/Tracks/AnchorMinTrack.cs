using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class AnchorMinTrack : Track<Vector2>
    {
        private readonly RectTransform _target;
        
        public AnchorMinTrack(RectTransform target, KeyFrame<Vector2>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchorMin = Vector2.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentAnchorMinTrack : FluentTrack<Vector2>
    {
        private readonly RectTransform _target;
        
        public FluentAnchorMinTrack(RectTransform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector2 Current()
        {
            return _target.anchorMin;
        }

        protected override void Apply(Vector2 from, Vector2 to, float t)
        {
            _target.anchorMin = Vector2.LerpUnclamped(from, to, t);
        }
    }
}
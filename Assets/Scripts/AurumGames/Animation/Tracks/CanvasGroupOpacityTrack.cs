using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class CanvasGroupOpacityTrack : Track<float>
    {
        private readonly CanvasGroup _target;
        
        public CanvasGroupOpacityTrack(CanvasGroup target, KeyFrame<float>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(float from, float to, float t)
        {
            _target.alpha = Mathf.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentCanvasGroupOpacityTrack : FluentTrack<float>
    {
        private readonly CanvasGroup _target;
        
        public FluentCanvasGroupOpacityTrack(CanvasGroup target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override float Current()
        {
            return _target.alpha;
        }

        protected override void Apply(float from, float to, float t)
        {
            _target.alpha = Mathf.LerpUnclamped(from, to, t);
        }
    }
}
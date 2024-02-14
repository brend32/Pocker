using UnityEngine;
using UnityEngine.UI;

namespace AurumGames.Animation.Tracks
{
    public sealed class ImageAlphaTrack : Track<float>
    {
        private readonly Image _target;
        
        public ImageAlphaTrack(Image target, KeyFrame<float>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(float from, float to, float t)
        {
            Color color = _target.color;
            color.a = Mathf.LerpUnclamped(from, to, t);
            _target.color = color;
        }
    }
    
    public sealed class FluentImageAlphaTrack : FluentTrack<float>
    {
        private readonly Image _target;
        
        public FluentImageAlphaTrack(Image target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override float Current()
        {
            return _target.color.a;
        }

        protected override void Apply(float from, float to, float t)
        {
            Color color = _target.color;
            color.a = Mathf.LerpUnclamped(from, to, t);
            _target.color = color;
        }
    }
}
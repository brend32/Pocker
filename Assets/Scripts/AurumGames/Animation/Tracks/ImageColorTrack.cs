using UnityEngine;
using UnityEngine.UI;

namespace AurumGames.Animation.Tracks
{
    public sealed class ImageColorTrack : Track<Color>
    {
        private readonly Image _target;
        
        public ImageColorTrack(Image target, KeyFrame<Color>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Color from, Color to, float t)
        {
            _target.color = Color.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentImageColorTrack : FluentTrack<Color>
    {
        private readonly Image _target;
        
        public FluentImageColorTrack(Image target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Color Current()
        {
            return _target.color;
        }

        protected override void Apply(Color from, Color to, float t)
        {
            _target.color = Color.LerpUnclamped(from, to, t);
        }
    }
}
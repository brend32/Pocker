using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class CameraBackgroundColorTrack : Track<Color>
    {
        private readonly Camera _target;
        
        public CameraBackgroundColorTrack(Camera target, KeyFrame<Color>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Color from, Color to, float t)
        {
            _target.backgroundColor = Color.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentCameraBackgroundColorTrack : FluentTrack<Color>
    {
        private readonly Camera _target;
        
        public FluentCameraBackgroundColorTrack(Camera target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Color Current()
        {
            return _target.backgroundColor;
        }

        protected override void Apply(Color from, Color to, float t)
        {
            _target.backgroundColor = Color.LerpUnclamped(from, to, t);
        }
    }
}
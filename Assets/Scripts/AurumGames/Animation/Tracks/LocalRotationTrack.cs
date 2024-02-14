using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class LocalRotationTrack : Track<Quaternion>
    {
        private readonly Transform _target;
        
        public LocalRotationTrack(Transform target, KeyFrame<Quaternion>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Quaternion from, Quaternion to, float t)
        {
            _target.localRotation = Quaternion.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentLocalRotationTrack : FluentTrack<Quaternion>
    {
        private readonly Transform _target;
        
        public FluentLocalRotationTrack(Transform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Quaternion Current()
        {
            return _target.localRotation;
        }

        protected override void Apply(Quaternion from, Quaternion to, float t)
        {
            _target.localRotation = Quaternion.LerpUnclamped(from, to, t);
        }
    }
}
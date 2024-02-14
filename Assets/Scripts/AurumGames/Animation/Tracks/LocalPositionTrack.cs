using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class LocalPositionTrack : Track<Vector3>
    {
        private readonly Transform _target;
        
        public LocalPositionTrack(Transform target, KeyFrame<Vector3>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localPosition = Vector3.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentLocalPositionTrack : FluentTrack<Vector3>
    {
        private readonly Transform _target;
        
        public FluentLocalPositionTrack(Transform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector3 Current()
        {
            return _target.localPosition;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localPosition = Vector3.LerpUnclamped(from, to, t);
        }
    }
}
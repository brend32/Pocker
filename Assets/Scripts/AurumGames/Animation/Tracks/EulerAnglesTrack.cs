using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class EulerAnglesTrack : Track<Vector3>
    {
        private readonly Transform _target;
        
        public EulerAnglesTrack(Transform target, KeyFrame<Vector3>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.eulerAngles = Vector3.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentEulerAnglesTrack : FluentTrack<Vector3>
    {
        private readonly Transform _target;
        
        public FluentEulerAnglesTrack(Transform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector3 Current()
        {
            return _target.eulerAngles;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.eulerAngles = Vector3.LerpUnclamped(from, to, t);
        }
    }
}
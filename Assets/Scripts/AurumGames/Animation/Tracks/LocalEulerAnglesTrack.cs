using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class LocalEulerAnglesTrack : Track<Vector3>
    {
        private readonly Transform _target;
        
        public LocalEulerAnglesTrack(Transform target, KeyFrame<Vector3>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localEulerAngles = Vector3.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentLocalEulerAnglesTrack : FluentTrack<Vector3>
    {
        private readonly Transform _target;
        
        public FluentLocalEulerAnglesTrack(Transform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector3 Current()
        {
            return _target.localEulerAngles;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localEulerAngles = Vector3.LerpUnclamped(from, to, t);
        }
    }
}
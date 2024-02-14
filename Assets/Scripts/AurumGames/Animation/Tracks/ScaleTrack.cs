using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class ScaleTrack : Track<Vector3>
    {
        private readonly Transform _target;
        
        public ScaleTrack(Transform target, KeyFrame<Vector3>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localScale = Vector3.LerpUnclamped(from, to, t);
        }

        public static KeyFrame<Vector3>[] KeyFramesFrom0To(Vector3 to, TransitionStruct transition)
        {
            return transition.GetKeyFrames(Vector3.one * 0.01f, to);
        }
        
        public static KeyFrame<Vector3>[] KeyFramesFromTo0(Vector3 from, TransitionStruct transition)
        {
            return transition.GetKeyFrames(from, Vector3.one * 0.01f);
        }
        
        public static KeyFrame<Vector3>[] KeyFramesFromTo1(Vector3 from, TransitionStruct transition)
        {
            return transition.GetKeyFrames(from, Vector3.one);
        }
    }
    
    public sealed class FluentScaleTrack : FluentTrack<Vector3>
    {
        private readonly Transform _target;
        
        public FluentScaleTrack(Transform target, Transition defaultTransition) : base(defaultTransition)
        {
            _target = target;
        }

        protected override Vector3 Current()
        {
            return _target.localScale;
        }

        protected override void Apply(Vector3 from, Vector3 to, float t)
        {
            _target.localScale = Vector3.LerpUnclamped(from, to, t);
        }
    }
}
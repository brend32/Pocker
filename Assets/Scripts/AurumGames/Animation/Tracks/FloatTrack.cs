using System;
using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class FloatTrack : Track<float>
    {
        private readonly Action<float> _target;
        
        public FloatTrack(Action<float> target, KeyFrame<float>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(float from, float to, float t)
        {
            _target?.Invoke(Mathf.LerpUnclamped(from, to, t));
        }

        public static KeyFrame<float>[] KeyFrames10(TransitionStruct transition)
        {
            return transition.GetKeyFrames<float>(1, 0);
        }
        
        public static KeyFrame<float>[] KeyFrames01(TransitionStruct transition)
        {
            return transition.GetKeyFrames<float>(0, 1);
        }
    }
    
    public sealed class FluentFloatTrack : FluentTrack<float>
    {
        private readonly Action<float> _apply;
        private float _current;
        
        public FluentFloatTrack(float initial, Action<float> apply, Transition defaultTransition) : base(defaultTransition)
        {
            _current = initial;
            _apply = apply;
        }

        protected override float Current()
        {
            return _current;
        }

        protected override void Apply(float from, float to, float t)
        {
            _current = Mathf.LerpUnclamped(from, to, t);
            _apply?.Invoke(Mathf.LerpUnclamped(from, to, t));
        }
    }
}
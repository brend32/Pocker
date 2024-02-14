using TMPro;
using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class TextMeshProOpacityTrack : Track<float>
    {
        private readonly TMP_Text _target;
        
        public TextMeshProOpacityTrack(TMP_Text target, KeyFrame<float>[] keyFrames) : base(keyFrames)
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
    
    public sealed class FluentTextMeshProOpacityTrack : FluentTrack<float>
    {
        private readonly TMP_Text _target;
        
        public FluentTextMeshProOpacityTrack(TMP_Text target, Transition defaultTransition) : base(defaultTransition)
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
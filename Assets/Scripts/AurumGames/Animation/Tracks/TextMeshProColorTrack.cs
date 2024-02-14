using TMPro;
using UnityEngine;

namespace AurumGames.Animation.Tracks
{
    public sealed class TextMeshProColorTrack : Track<Color>
    {
        private readonly TMP_Text _target;
        
        public TextMeshProColorTrack(TMP_Text target, KeyFrame<Color>[] keyFrames) : base(keyFrames)
        {
            _target = target;
        }

        protected override void Apply(Color from, Color to, float t)
        {
            _target.color = Color.LerpUnclamped(from, to, t);
        }
    }
    
    public sealed class FluentTextMeshProColorTrack : FluentTrack<Color>
    {
        private readonly TMP_Text _target;
        
        public FluentTextMeshProColorTrack(TMP_Text target, Transition defaultTransition) : base(defaultTransition)
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
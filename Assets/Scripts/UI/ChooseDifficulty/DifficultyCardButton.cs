using System;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using Poker.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Button = Poker.UI.Common.Button;
using ToggleGroup = Poker.UI.Common.ToggleGroup;

namespace Poker.UI.ChooseDifficulty
{
    public class DifficultyCardButton : Button, IToggleOption
    {
        private RectTransform _transform;
        [SerializeField] private Image _shadow;
        [SerializeField] private Image _selected;
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _normalColor;
        [SerializeField] private ToggleGroup _toggleGroup;

        private StatedFluentAnimationPlayer<bool> _selectedAnimation;
        
        protected override void InitInnerState()
        {
            _transform = (RectTransform)transform;
            _shadow.color = _normalColor;
            _toggleGroup.SelectionChanged += SelectionChanged;
            OnClick.AddListener(Select);
            base.InitInnerState();
        }
        
        protected override void Initialized()
        {
            
        }

        private void SelectionChanged(IToggleOption current)
        {
            if (current is DifficultyCardButton cardButton)
            {
                _selectedAnimation.SetState(cardButton == this);
            }
        }

        protected override StatedFluentAnimationPlayer<State> SetupAnimations()
        {
            var selectedColorTrack = new FluentImageAlphaTrack(_selected, new Transition(240, Easing.QuadOut));
            _selectedAnimation = new StatedFluentAnimationPlayer<bool>(this, selectedColorTrack);
            _selectedAnimation.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case true:
                        selectedColorTrack.Set(1, options);
                        break;
                    
                    case false:
                        selectedColorTrack.Set(0, options);
                        break;
                }
            };
            _selectedAnimation.SetStateInstant(false);
            
            var shadowColorTrack = new FluentImageColorTrack(_shadow, new Transition(240, Easing.QuadOut));
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(240, Easing.QuadOut));

            var player = new StatedFluentAnimationPlayer<State>(this, shadowColorTrack, scaleTrack);
            player.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case State.Normal:
                        shadowColorTrack.Set(_normalColor, options);
                        scaleTrack.Set(Vector3.one, options);
                        break;
                    
                    case State.Hover:
                    case State.Pressed:
                        shadowColorTrack.Set(_hoverColor, options);
                        if (current == State.Pressed)
                        {
                            scaleTrack.Set(Vector3.one * 0.93f, options);
                        }
                        else
                        {
                            scaleTrack.Set(Vector3.one, options);
                        }
                        break;
                }
            };
            
            return player;
        }

        private void Select()
        {
            _toggleGroup.Current = this;
        }

        private void OnValidate()
        {
            _shadow.color = _normalColor;
        }
    }
}

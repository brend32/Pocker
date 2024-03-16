using System;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using Poker.Gameplay.Core;
using Poker.UI.Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = Poker.UI.Common.Button;
using ToggleGroup = Poker.UI.Common.ToggleGroup;

namespace Poker.UI.ChooseDifficulty
{
    public class DifficultyCardButton : Button, IToggleOption
    {
        public Difficulty Difficulty;
        public RectTransform Transform => _transform;
        public Vector3 Scale => _optionSelected ? _selectedScale : Vector3.one;
        
        private RectTransform _transform;
        [SerializeField] private Image _shadow;
        [SerializeField] private Image _selected;
        [SerializeField] private Image _image;
        [SerializeField] private Vector3 _selectedScale;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectedColor;
        [FormerlySerializedAs("_hoverColor")] [SerializeField] private Color _hoverShadowColor;
        [FormerlySerializedAs("_normalColor")] [SerializeField] private Color _normalShadowColor;
        [SerializeField] private ToggleGroup _toggleGroup;

        private StatedFluentAnimationPlayer<State> _animation;
        private bool _optionSelected;
        
        protected override void InitInnerState()
        {
            _transform = (RectTransform)transform;
            _shadow.color = _normalShadowColor;
            _image.color = _normalColor;
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
                _optionSelected = cardButton == this;
                _animation.SetState(_animation.CurrentState);
            }
        }

        protected override StatedFluentAnimationPlayer<State> SetupAnimations()
        {
            var selectedShadowAlphaTrack = new FluentImageAlphaTrack(_selected, new Transition(240, Easing.QuadOut));
            var colorTrack = new FluentImageColorTrack(_image, new Transition(240, Easing.QuadOut));
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(240, Easing.QuadOut));
            var shadowColorTrack = new FluentImageColorTrack(_shadow, new Transition(240, Easing.QuadOut));
            
            var player = new StatedFluentAnimationPlayer<State>(this, shadowColorTrack, scaleTrack, colorTrack, selectedShadowAlphaTrack);
            player.StateChanged += (previous, current, options) =>
            {
                selectedShadowAlphaTrack.Set(_optionSelected ? 1 : 0);
                
                switch (current)
                {
                    case State.Normal:
                        shadowColorTrack.Set(_normalShadowColor, options);
                        scaleTrack.Set(_optionSelected ? _selectedScale : Vector3.one, options);
                        colorTrack.Set(_optionSelected ? _selectedColor : _normalColor);
                        break;
                    
                    case State.Hover:
                    case State.Pressed:
                        shadowColorTrack.Set(_hoverShadowColor, options);
                        colorTrack.Set(_optionSelected ? _selectedColor : _hoverColor);
                        
                        if (current == State.Pressed)
                        {
                            scaleTrack.Set(Vector3.one * 0.93f, options);
                        }
                        else
                        {
                            scaleTrack.Set(_optionSelected ? _selectedScale : Vector3.one, options);
                        }
                        break;
                }
            };
            _animation = player;
            
            return player;
        }

        private void Select()
        {
            _toggleGroup.Current = this;
        }

        private void OnValidate()
        {
            _shadow.color = _normalShadowColor;
        }
    }
}

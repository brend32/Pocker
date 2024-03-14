using System;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using Poker.Gameplay.Core;
using Poker.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = Poker.UI.Common.Button;
using ToggleGroup = Poker.UI.Common.ToggleGroup;

namespace Poker.UI.ChooseDifficulty
{
    public class PlayersCountButton : Button, IToggleOption
    {
        public int Players;
        private RectTransform _transform;
        [SerializeField] private Image _selectedShadow;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _normalTextColor;
        [SerializeField] private Color _selectedTextColor;
        [SerializeField] private ToggleGroup _toggleGroup;

        private StatedFluentAnimationPlayer<bool> _selectedAnimation;
        
        protected override void InitInnerState()
        {
            _transform = (RectTransform)transform;
            _image.color = _normalColor;
            _text.color = _normalTextColor;
            _text.text = Players.ToString();
            _toggleGroup.SelectionChanged += SelectionChanged;
            OnClick.AddListener(Select);
            base.InitInnerState();
        }
        
        protected override void Initialized()
        {
            
        }

        private void SelectionChanged(IToggleOption current)
        {
            if (current is PlayersCountButton button)
            {
                _selectedAnimation.SetState(button == this);
            }
        }

        protected override StatedFluentAnimationPlayer<State> SetupAnimations()
        {
            var selectedImageColorTrack = new FluentImageColorTrack(_image, new Transition(240, Easing.QuadOut));
            var selectedTextColorTrack = new FluentTextMeshProColorTrack(_text, new Transition(240, Easing.QuadOut));
            var selectedShadowTrack = new FluentImageAlphaTrack(_selectedShadow, new Transition(240, Easing.QuadOut));
            _selectedAnimation = new StatedFluentAnimationPlayer<bool>(this, selectedImageColorTrack, selectedTextColorTrack, selectedShadowTrack);
            _selectedAnimation.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case true:
                        selectedImageColorTrack.Set(_selectedColor, options);
                        selectedTextColorTrack.Set(_selectedTextColor, options);
                        selectedShadowTrack.Set(1, options);
                        break;
                    
                    case false:
                        selectedImageColorTrack.Set(_normalColor, options);
                        selectedTextColorTrack.Set(_normalTextColor, options);
                        selectedShadowTrack.Set(0, options);
                        break;
                }
            };
            _selectedAnimation.SetStateInstant(false);
            
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(240, Easing.QuadOut));

            var player = new StatedFluentAnimationPlayer<State>(this, scaleTrack);
            player.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case State.Normal:
                        scaleTrack.Set(Vector3.one, options);
                        break;
                    
                    case State.Hover:
                    case State.Pressed:
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
            _image.color = _normalColor;
            _text.color = _normalTextColor;
            _text.text = Players.ToString();
        }
    }
}

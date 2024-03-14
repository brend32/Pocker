using System;
using System.Collections;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = Poker.UI.Common.Button;

namespace Poker.Menu.UI
{
    public class MenuCardButton : Button
    {
        private RectTransform _transform;
        private Color _shadowColor;
        [SerializeField] private float _offset;
        [SerializeField] private Image _shadow;
        [SerializeField] private Color _shadowHoverColor;

        protected override void InitInnerState()
        {
            _transform = (RectTransform)transform;
            _shadowColor = _shadow.color;
            base.InitInnerState();
        }

        protected override void Initialized()
        {
            
        }

        protected override StatedFluentAnimationPlayer<State> SetupAnimations()
        {
            var anchoredPositionTrack = new FluentAnchoredPositionTrack(_transform, new Transition(240, Easing.QuadOut));
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(240, Easing.QuadOut));
            var shadowColorTrack = new FluentImageColorTrack(_shadow, new Transition(240, Easing.QuadOut));

            var player = new StatedFluentAnimationPlayer<State>(this, anchoredPositionTrack, scaleTrack, shadowColorTrack);
            player.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case State.Normal:
                        anchoredPositionTrack.Set(Vector2.zero, options);
                        scaleTrack.Set(Vector3.one, options);
                        shadowColorTrack.Set(_shadowColor);
                        break;
                    
                    case State.Hover:
                    case State.Pressed:
                        anchoredPositionTrack.Set(new Vector2(0, _offset), options);
                        shadowColorTrack.Set(_shadowHoverColor);
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
    }
}

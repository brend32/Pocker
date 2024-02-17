using System;
using System.Collections;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Poker.UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Poker.Menu.UI
{
    public class MenuCardButton : Button
    {
        private RectTransform _transform;
        [SerializeField] private float _offset;

        protected override void InitInnerState()
        {
            _transform = (RectTransform)transform;
            base.InitInnerState();
        }

        protected override void Initialized()
        {
            
        }

        protected override StatedFluentAnimationPlayer<State> SetupAnimations()
        {
            var anchoredPositionTrack = new FluentAnchoredPositionTrack(_transform, new Transition(240, Easing.QuadOut));
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(240, Easing.QuadOut));

            var player = new StatedFluentAnimationPlayer<State>(this, anchoredPositionTrack, scaleTrack);
            player.StateChanged += (previous, current, options) =>
            {
                switch (current)
                {
                    case State.Normal:
                        anchoredPositionTrack.Set(Vector2.zero, options);
                        scaleTrack.Set(Vector3.one, options);
                        break;
                    
                    case State.Hover:
                    case State.Pressed:
                        anchoredPositionTrack.Set(new Vector2(0, _offset), options);
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

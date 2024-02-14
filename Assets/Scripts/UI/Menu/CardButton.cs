using System;
using System.Collections;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Poker.Menu.UI
{
    public class CardButton : LazyMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private float _offset;

        private StatedFluentAnimationPlayer<State> _animation;
        private bool _hovered;
        private int _pointerId = -1;

        private enum State
        {
            Normal,
            Hover,
            Pressed
        }
        
        protected override void InitInnerState()
        {
            SetupAnimations();
        }

        protected override void Initialized()
        {
            
        }

        private void SetupAnimations()
        {
            var anchoredPositionTrack = new FluentAnchoredPositionTrack(_transform, new Transition(300, Easing.QuadOut));
            var scaleTrack = new FluentScaleTrack(_transform, new Transition(300, Easing.QuadOut));

            _animation = new StatedFluentAnimationPlayer<State>(this, anchoredPositionTrack, scaleTrack);
            _animation.StateChanged += (previous, current, options) =>
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
                            scaleTrack.Set(Vector3.one * 0.85f, options);
                        }
                        else
                        {
                            scaleTrack.Set(Vector3.one, options);
                        }
                        break;
                }
            };
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerId = eventData.pointerId;
            
            _animation.SetState(State.Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId)
                return;

            _pointerId = -1;

            if (_hovered)
            {
                _animation.SetState(State.Hover);
            }
            else
            {
                _animation.SetState(State.Normal);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId && _pointerId != -1)
                return;

            _hovered = true;

            if (_animation.CurrentState == State.Normal)
            {
                _animation.SetState(State.Hover);
            } 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId && _pointerId != -1)
                return;
            
            _hovered = false;

            if (_animation.CurrentState == State.Hover)
            {
                _animation.SetState(State.Normal);
            } 
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked");
        }
    }
}

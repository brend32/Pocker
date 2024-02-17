using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Poker.UI.Common
{
	public abstract class Button : LazyMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent OnClick;
        public State CurrentState => _animation.CurrentState;

        private StatedFluentAnimationPlayer<State> _animation;
        private bool _hovered;
        private int _pointerId = -1;

        public enum State
        {
            Normal,
            Hover,
            Pressed
        }
        
        protected override void InitInnerState()
        {
            _animation = SetupAnimations();
            _animation.SetStateInstant(State.Normal);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerId = eventData.pointerId;
            
            _animation.SetState(State.Pressed);
            
            OnDown();
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
            
            OnUp();
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
            
            OnEnter();
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
            
            OnExit();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }

        protected virtual void OnEnter()
        {
            
        }

        protected virtual void OnExit()
        {
            
        }

        protected virtual void OnDown()
        {
            
        }

        protected virtual void OnUp()
        {
            
        }
        
        protected abstract StatedFluentAnimationPlayer<State> SetupAnimations();
    }
}
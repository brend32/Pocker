using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.CustomLayout;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public class BetView : LazyMonoBehaviour
	{
		[SerializeField] private RectTransform _transform;
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private CustomLayoutBase _layout;

		private StatedAnimationPlayer<Visibility> _animation;
		
		protected override void InitInnerState()
		{
			Rect rect = _transform.rect;
			Vector2 position = _transform.anchoredPosition;
			
			var show = new TracksEvaluator(new ITrack[]
			{
				new AnchoredPositionTrack(_transform, new []
				{
					new KeyFrame<Vector2>(0, new Vector2(rect.width * -1, position.y), Easing.QuintOut),
					new KeyFrame<Vector2>(450, new Vector2(0, position.y), Easing.QuintOut),
				})
			});

			var hide = new TracksEvaluator(new ITrack[]
			{
				new AnchoredPositionTrack(_transform, new []
				{
					new KeyFrame<Vector2>(0, new Vector2(0, position.y), Easing.QuintIn),
					new KeyFrame<Vector2>(450, new Vector2(rect.width * -1, position.y), Easing.QuintOut),
				})
			});

			_animation = new StatedAnimationPlayer<Visibility>(this, new Dictionary<Visibility, TracksEvaluator>()
			{
				{ Visibility.Visible, show },
				{ Visibility.Hidden, hide },
			});
			_animation.SetStateInstant(Visibility.Hidden);
		}

		protected override void Initialized()
		{
			
		}

		public void Show()
		{
			if (_animation.CurrentState == Visibility.Hidden)
				_animation.SetState(Visibility.Visible);
		}

		public void Hide()
		{
			if (_animation.CurrentState == Visibility.Visible)
				_animation.SetState(Visibility.Hidden);
		}

		public void SetBet(int value)
		{
			_text.text = $"Bet\n<b>${value}";
			_layout.UpdateLayout();
		} 
	}
}
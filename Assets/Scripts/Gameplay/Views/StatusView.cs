using System.Collections.Generic;
using System.Linq;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.CustomLayout;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class StatusView : LazyMonoBehaviour
	{
		[SerializeField] private RectTransform _transform;
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private CustomLayoutBase _layout;

		[Dependency] private GameManager _gameManager;
		private TableState TableState => _gameManager.State.Table;
		
		private PlayerState _player;
		private StatedAnimationPlayer<Visibility> _animation;
		
		protected override void InitInnerState()
		{
			Rect rect = _transform.rect;
			Vector2 position = _transform.anchoredPosition;
			
			var show = new TracksEvaluator(new ITrack[]
			{
				new AnchoredPositionTrack(_transform, new []
				{
					new KeyFrame<Vector2>(0, new Vector2(position.x, rect.height), Easing.QuintOut),
					new KeyFrame<Vector2>(450, new Vector2(position.x, -10), Easing.QuintOut),
				})
			});

			var hide = new TracksEvaluator(new ITrack[]
			{
				new AnchoredPositionTrack(_transform, new []
				{
					new KeyFrame<Vector2>(0, new Vector2(position.x, -10), Easing.QuintIn),
					new KeyFrame<Vector2>(450, new Vector2(position.x, rect.height), Easing.QuintOut),
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
			_animation.TimeSource = _gameManager.TimeSource;
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

		public void SetText(string text)
		{
			_text.text = text;
			_layout.UpdateLayout();
		} 
	}
}
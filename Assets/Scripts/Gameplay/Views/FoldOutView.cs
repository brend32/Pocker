using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.CustomLayout;
using Poker.Gameplay.Core;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class FoldOutView : LazyMonoBehaviour
	{
		[SerializeField] private RectTransform _transform;
		[SerializeField] private CanvasGroup _canvasGroup;

		[Dependency] private GameManager _gameManager;
		
		private StatedAnimationPlayer<Visibility> _animation;
		
		protected override void InitInnerState()
		{
			var show = new TracksEvaluator(new ITrack[]
			{
				new ScaleTrack(_transform, ScaleTrack.KeyFramesFrom0To(Vector3.one, new TransitionStruct(350, Easing.OutBack, 100))),
				new CanvasGroupOpacityTrack(_canvasGroup, FloatTrack.KeyFrames01(new TransitionStruct(300, Easing.QuadOut, 100)))
			});

			var hide = new TracksEvaluator(new ITrack[]
			{
				new ScaleTrack(_transform, ScaleTrack.KeyFramesFromTo0(Vector3.one, new TransitionStruct(240, Easing.QuintIn))),
				new CanvasGroupOpacityTrack(_canvasGroup, FloatTrack.KeyFrames10(new TransitionStruct(300, Easing.QuintIn)))
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
	}
}
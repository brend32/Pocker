using System;
using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Poker.Gameplay.Core;
using Poker.Screens;
using UnityEngine;

namespace Poker.UI.Game
{
	public partial class PauseScreen : WindowView
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private GameScreen _gameScreen;
		
		[Dependency] private GameManager _gameManager;

		private StatedAnimationPlayer<Visibility> _animation;
		
		protected override void InitInnerState()
		{
			var show = new TracksEvaluator(new ITrack[]
			{
				new CanvasGroupOpacityTrack(_canvasGroup, FloatTrack.KeyFrames01(new TransitionStruct(300, Easing.QuadOut)))
			});
			
			var hide = new TracksEvaluator(new ITrack[]
			{
				new CanvasGroupOpacityTrack(_canvasGroup, FloatTrack.KeyFrames10(new TransitionStruct(300, Easing.QuadOut)))
			});

			_animation = new StatedAnimationPlayer<Visibility>(this, new Dictionary<Visibility, TracksEvaluator>()
			{
				{ Visibility.Visible, show },
				{ Visibility.Hidden, hide },
			});
			_animation.StateChanged += (previous, current) =>
			{
				if (current == Visibility.Hidden)
					_canvasGroup.blocksRaycasts = false;
			};
			_animation.AnimationEnded += (previous, current) =>
			{
				if (current == Visibility.Visible)
					_canvasGroup.blocksRaycasts = true;
			};
			_animation.SetStateInstant(Visibility.Hidden);
		}

		protected override void Initialized()
		{
			
		}

		protected override void BeginShow(bool fromBlockedState)
		{
			if (fromBlockedState)
				return;
			
			_animation.SetState(Visibility.Visible);
		}

		protected override void BeginHide()
		{
			_animation.SetState(Visibility.Hidden);
		}

		protected override void BeginBlocked()
		{
			
		}

		private void Update()
		{
			if (IsBackPressed())
				Close();
		}

		public void Open()
		{
			_gameManager.PauseGame();
			Show();
		}

		public void Close()
		{
			SafeClick(() =>
			{
				_gameManager.ContinueGame();
				Hide();
			});
		}

		public void Exit()
		{
			SafeClick(() =>
			{
				_gameScreen.ExitToMenu();
			});
		}
	}
}
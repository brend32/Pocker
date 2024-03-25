using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class BalanceView : LazyMonoBehaviour
	{
		[SerializeField] private string _format = "{0}$";
		[SerializeField] private TextMeshPro _text;

		[Dependency] private GameManager _gameManager;
		
		private StatedFluentAnimationPlayer<int> _animation;
		
		protected override void InitInnerState()
		{
			var track = new FluentFloatTrack(0, v =>
			{
				_text.text = string.Format(_format, (int)v);
			}, new Transition(400, Easing.QuadOut));
			
			_animation = new StatedFluentAnimationPlayer<int>(this, track);
			_animation.StateChanged += (previous, current, options) =>
			{
				track.Set(current, options);
			};
			_animation.UpdateInstant();
		}

		protected override void Initialized()
		{
			_animation.TimeSource = _gameManager.TimeSource;
		}

		public void SetValue(int balance)
		{
			_animation.SetState(balance);
		}
	}
}
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class TableCardsView : LazyMonoBehaviour
	{
		[SerializeField] private CardView[] _cardViews;

		[Dependency] private GameManager _gameManager;

		private AnimationPlayer _animation;
		
		protected override void InitInnerState()
		{
			var tracks = new ITrack[_cardViews.Length];
			for (int i = 0; i < tracks.Length; i++)
			{
				var transition = new TransitionStruct(600, Easing.QuintInOut, 200 * i);
				tracks[i] = new ScaleTrack(_cardViews[i].transform, ScaleTrack.KeyFramesFrom0To(Vector3.one, transition));
				_cardViews[i].transform.localScale = Vector3.zero;
			}

			_animation = new AnimationPlayer(this, tracks);
		}

		protected override void Initialized()
		{
			_gameManager.State.Table.NewCardRevealed += NewCardRevealed;
			_gameManager.Controller.Round.RoundStarted += RoundStarted;
		}
		
		private void RoundStarted()
		{
			UpdateView();
			_animation.Play();
		}

		private void NewCardRevealed()
		{
			UpdateView();
		}

		[EasyButtons.Button]
		public void UpdateView()
		{
			TableState state = _gameManager.State.Table;
			
			for (var i = 0; i < _cardViews.Length; i++)
			{
				CardView cardView = _cardViews[i];
				cardView.Bind(state.Cards[i]);
				cardView.Revealed = i < state.CardsRevealed;
			}
		}
	}
}
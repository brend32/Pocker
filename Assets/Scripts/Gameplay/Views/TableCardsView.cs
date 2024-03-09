using System.Collections.Generic;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class TableCardsView : LazyMonoBehaviour
	{
		[SerializeField] private CardView[] _cardViews;

		[Dependency] private GameManager _gameManager;
		private TableState TableState => _gameManager.State.Table;

		private AnimationPlayer _dealCardsPlayer;
		
		protected override void InitInnerState()
		{
			DealCardsAnimationSetup();
		}

		private void DealCardsAnimationSetup()
		{
			var tracks = new ITrack[_cardViews.Length];
			for (int i = 0; i < tracks.Length; i++)
			{
				var transition = new TransitionStruct(600, Easing.QuintInOut, 200 * i);
				tracks[i] = new ScaleTrack(_cardViews[i].transform, ScaleTrack.KeyFramesFrom0To(Vector3.one, transition));
				_cardViews[i].transform.localScale = Vector3.zero;
			}

			_dealCardsPlayer = new AnimationPlayer(this, tracks);
		}

		protected override void Initialized()
		{
			//_gameManager.State.Table.NewCardRevealed += NewCardRevealed;
			_gameManager.Controller.Round.RoundStarted += RoundStarted;
		}

		public async UniTask DealCardsAnimation()
		{
			_dealCardsPlayer.PlayFromStart();
			await UniTask.WaitWhile(() => _dealCardsPlayer.IsPlaying);
		}

		public async UniTask RevealCardAnimation()
		{
			UniTask last = UniTask.CompletedTask;
			
			for (var i = 0; i < TableState.CardsRevealed; i++)
			{
				CardView cardView = _cardViews[i];
				if (cardView.Revealed)
					continue;
				
				last = cardView.RevealCardAnimation();
				await UniTask.Delay(100);
			}
			
			await last;
		}
		
		private void RoundStarted()
		{
			UpdateView();
		}

		private void NewCardRevealed()
		{
			UpdateView();
		}

		[EasyButtons.Button]
		public void UpdateView()
		{
			for (var i = 0; i < _cardViews.Length; i++)
			{
				CardView cardView = _cardViews[i];
				cardView.Bind(TableState.Cards[i]);
				cardView.Revealed = i < TableState.CardsRevealed;
			}
		}
	}
}
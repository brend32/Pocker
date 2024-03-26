using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			
		}

		public async UniTask HideCardsRoundEndAnimation(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			var tasks = _cardViews.Select(cardView => cardView.HideAnimation(cancellationToken));

			await UniTask.WhenAll(tasks);
		}
		
		public async UniTask DealCardsAnimation(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			HideCards();
			List<UniTask> tasks = new();

			var delay = 0;
			foreach (CardView cardView in _cardViews)
			{
				tasks.Add(_gameManager.DelayAsync(delay, cancellationToken).ContinueWith(() => cardView.ShowAnimation(cancellationToken)));
				delay += 150;
			}
			
			await UniTask.WhenAll(tasks);
		}

		public async UniTask RevealCardAnimation(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			List<UniTask> tasks = new();

			var delay = 0;
			for (var i = 0; i < TableState.CardsRevealed; i++)
			{
				CardView cardView = _cardViews[i];
				if (cardView.Revealed)
					continue;

				tasks.Add(_gameManager.DelayAsync(delay, cancellationToken).ContinueWith(() => cardView.RevealAnimation(cancellationToken)));
				delay += 100;
			}
			
			await UniTask.WhenAll(tasks);
		}

		[EasyButtons.Button]
		public void HideCards()
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			for (var i = 0; i < _cardViews.Length; i++)
			{
				CardView cardView = _cardViews[i];
				cardView.Bind(TableState.Cards[i]);
				cardView.Revealed = false;
			}
		}
	}
}
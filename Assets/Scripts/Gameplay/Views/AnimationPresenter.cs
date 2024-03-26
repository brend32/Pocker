using System.Threading;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Controllers;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class AnimationPresenter : LazyMonoBehaviour, IAnimationPresenter
	{
		[SerializeField] private TableCardsView _tableCardsView;
		[SerializeField] private PlayersView _playersView;

		[Dependency] private GameManager _gameManager;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			_gameManager.Controller.Animation.Register(this);
		}

		public UniTask DealCards(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return UniTask.CompletedTask;
			
			return _playersView.DealCardsAnimation(cancellationToken).ContinueWith(() => _tableCardsView.DealCardsAnimation(cancellationToken));
		}

		public UniTask RevealCard(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return UniTask.CompletedTask;
			
			return _tableCardsView.RevealCardAnimation(cancellationToken);
		}

		public UniTask RevealCardsRoundEnd(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return UniTask.CompletedTask;
			
			return _playersView.RevealOthersCardsRoundEndAnimation(cancellationToken);
		}

		public UniTask MakeChoice(PlayerState player, VotingResponse response, CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return UniTask.CompletedTask;
			
			return _playersView.MakeChoiceAnimation(player, response, cancellationToken);
		}

		public UniTask RoundEnd(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return UniTask.CompletedTask;
			
			return UniTask.WhenAll(_tableCardsView.HideCardsRoundEndAnimation(cancellationToken), _playersView.HideCardsRoundEndAnimation(cancellationToken));
		}
	}
}
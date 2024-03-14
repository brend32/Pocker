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

		public UniTask DealCards()
		{
			return _playersView.DealCardsAnimation().ContinueWith(_tableCardsView.DealCardsAnimation);
		}

		public UniTask RevealCard()
		{
			return _tableCardsView.RevealCardAnimation();
		}

		public UniTask RevealCardsRoundEnd()
		{
			return _playersView.RevealOthersCardsRoundEndAnimation();
		}

		public UniTask MakeChoice(PlayerState player, VotingResponse response)
		{
			return _playersView.MakeChoiceAnimation(player, response);
		}

		public UniTask RoundEnd()
		{
			return UniTask.WhenAll(_tableCardsView.HideCardsRoundEndAnimation(), _playersView.HideCardsRoundEndAnimation());
		}
	}
}
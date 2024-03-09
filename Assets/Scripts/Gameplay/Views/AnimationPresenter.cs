using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Controllers;
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
			return _tableCardsView.DealCardsAnimation();
		}

		public UniTask RevealCard()
		{
			return _tableCardsView.RevealCardAnimation();
		}

		public UniTask RevealCardsRoundEnd()
		{
			return _playersView.RevealOthersCardsRoundEndAnimation();
		}

		public UniTask MakeChoice()
		{
			return UniTask.CompletedTask;
		}
	}
}
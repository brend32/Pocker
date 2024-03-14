using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;

namespace Poker.Gameplay.Core.Controllers
{
	public class AnimationController : IAnimationPresenter
	{
		private IAnimationPresenter _presenter;
		
		public void Register(IAnimationPresenter presenter)
		{
			_presenter = presenter;
		}
		
		public UniTask DealCards()
		{
			return _presenter?.DealCards() ?? UniTask.CompletedTask;
		}

		public UniTask RevealCard()
		{
			return _presenter?.RevealCard() ?? UniTask.CompletedTask;
		}

		public UniTask MakeChoice(PlayerState player, VotingResponse response)
		{
			return _presenter?.MakeChoice(player, response) ?? UniTask.CompletedTask;
		}
		
		public UniTask RevealCardsRoundEnd()
		{
			return _presenter?.RevealCardsRoundEnd() ?? UniTask.CompletedTask;
		}
		
		public UniTask RoundEnd()
		{
			return _presenter?.RoundEnd() ?? UniTask.CompletedTask;
		}
	}

	public interface IAnimationPresenter
	{
		UniTask DealCards();
		UniTask RevealCard();
		UniTask MakeChoice(PlayerState player, VotingResponse response);
		UniTask RevealCardsRoundEnd();
		UniTask RoundEnd();
	}
}
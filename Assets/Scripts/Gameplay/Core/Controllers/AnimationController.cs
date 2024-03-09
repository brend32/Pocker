using Cysharp.Threading.Tasks;

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

		public UniTask MakeChoice()
		{
			return _presenter?.MakeChoice() ?? UniTask.CompletedTask;
		}
		
		public UniTask RevealCardsRoundEnd()
		{
			return _presenter?.RevealCardsRoundEnd() ?? UniTask.CompletedTask;
		}
	}

	public interface IAnimationPresenter
	{
		UniTask DealCards();
		UniTask RevealCard();
		UniTask MakeChoice();
		UniTask RevealCardsRoundEnd();
	}
}
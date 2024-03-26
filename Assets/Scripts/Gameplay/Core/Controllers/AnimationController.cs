using System.Threading;
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
		
		public UniTask DealCards(CancellationToken cancellationToken)
		{
			return _presenter?.DealCards(cancellationToken) ?? UniTask.CompletedTask;
		}

		public UniTask RevealCard(CancellationToken cancellationToken)
		{
			return _presenter?.RevealCard(cancellationToken) ?? UniTask.CompletedTask;
		}

		public UniTask MakeChoice(PlayerState player, VotingResponse response, CancellationToken cancellationToken)
		{
			return _presenter?.MakeChoice(player, response, cancellationToken) ?? UniTask.CompletedTask;
		}
		
		public UniTask RevealCardsRoundEnd(CancellationToken cancellationToken)
		{
			return _presenter?.RevealCardsRoundEnd(cancellationToken) ?? UniTask.CompletedTask;
		}
		
		public UniTask RoundEnd(CancellationToken cancellationToken)
		{
			return _presenter?.RoundEnd(cancellationToken) ?? UniTask.CompletedTask;
		}
	}

	public interface IAnimationPresenter
	{
		UniTask DealCards(CancellationToken cancellationToken);
		UniTask RevealCard(CancellationToken cancellationToken);
		UniTask MakeChoice(PlayerState player, VotingResponse response, CancellationToken cancellationToken);
		UniTask RevealCardsRoundEnd(CancellationToken cancellationToken);
		UniTask RoundEnd(CancellationToken cancellationToken);
	}
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models;

namespace Poker.Gameplay.Core.Contracts
{
	public enum VotingAction
	{
		CallOrCheck,
		Fold,
		Raise,
	}
	
	public interface IPlayerLogic
	{
		UniTask<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken);
	}
}
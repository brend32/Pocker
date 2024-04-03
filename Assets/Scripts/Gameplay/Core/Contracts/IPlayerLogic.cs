using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;

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
		Task<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken);
		void RoundEnded(PlayerState winner);
	}
}
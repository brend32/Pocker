using System;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models.VotingContexts;

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
		UniTask MakeVotingAction(VotingContext context);
	}
}
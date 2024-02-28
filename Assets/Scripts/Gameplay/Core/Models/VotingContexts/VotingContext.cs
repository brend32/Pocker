using System;
using Poker.Gameplay.Core.States;

namespace Poker.Gameplay.Core.Models.VotingContexts
{
	public enum VotingAction
	{
		Fold,
		Raise,
		Call
	}
	
	public struct VotingResponse
	{
		public readonly int RaiseAmount;
		public readonly VotingAction Action;

		private VotingResponse(VotingAction action, int raiseAmount = 0)
		{
			Action = action;
			RaiseAmount = raiseAmount;
		}

		public static VotingResponse Fold()
		{
			return new VotingResponse(VotingAction.Fold);
		}
		
		public static VotingResponse Call()
		{
			return new VotingResponse(VotingAction.Call);
		}
		
		public static VotingResponse Raise(int amount)
		{
			if (amount <= 0)
				throw new Exception($"Raise can't be equal or less than 0: [Amount: {amount}]");
			
			return new VotingResponse(VotingAction.Raise, amount);
		}
	}
	
	public class VotingContext
	{
		public int MinimumBet { get; set; }
		public bool IsBetRequired => MinimumBet != 0;
	}
}
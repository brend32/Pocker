using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core.Models.VotingContexts
{
	public class NoBetsPlacedContext : VotingContext
	{
		public NoBetsPlacedContext(PlayerState voter) : base(voter)
		{
		}
		
		public void Check()
		{
			Debug.Log("Check");
			MadeAction();
		}

		public void Fold()
		{
			Voter.Fold();
		}

		public void Bet(int amount)
		{
			Voter.MakeBet(amount);
			MadeAction();
		}
	}
}
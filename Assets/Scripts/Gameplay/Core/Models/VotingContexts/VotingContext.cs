using System;
using Poker.Gameplay.Core.States;

namespace Poker.Gameplay.Core.Models.VotingContexts
{
	public abstract class VotingContext
	{
		public PlayerState Voter { get; }
		public bool HasAction { get; private set; }
		
		
		protected VotingContext(PlayerState voter)
		{
			Voter = voter;
		}

		protected void MadeAction()
		{
			if (HasAction)
				throw new Exception("Multiple action not allowed");
			
			HasAction = true;
		}
	}
}
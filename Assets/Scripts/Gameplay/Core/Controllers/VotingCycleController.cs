using System.Threading;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models.VotingContexts;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core.Controllers
{
	public class VotingCycleController
	{
		private readonly GameState _state;
		private readonly TableState _table;

		public VotingCycleController(GameState state)
		{
			_state = state;
			_table = state.Table;
		}

		public async UniTask StartVotingCycle()
		{
			_table.StartNewVotingCycle();
			do
			{
				using var tokenSource = new CancellationTokenSource();
				
				var votingContext = new VotingContext();
				var thinkingTask = _table.Voter.Logic.MakeVotingAction(votingContext, tokenSource.Token).Preserve();
				var result = await UniTask.WhenAny(
					UniTask.Delay(40000, cancellationToken: tokenSource.Token),
					thinkingTask);

				if (result == 1)
				{
					VotingResponse response = thinkingTask.GetAwaiter().GetResult();
					MakeVoteAction(response);
					
					if (response.Action == VotingAction.Raise)
						_table.ResetVotingCycle();
				}
				else
				{
					_table.Fold();
					tokenSource.Cancel();
				}
				
				_table.AssignNextVoter();
				Debug.Log("New voter: " + _table.Voter.Name);
			} while (_table.IsVotingEnded() == false);
			_table.EndVotingCycle();
			
			Debug.Log("Vote cycle ended");
		}

		private void MakeVoteAction(VotingResponse response)
		{
			switch (response.Action)
			{
				case VotingAction.Call:
					_table.Call();
					break;
				
				case VotingAction.Fold:
					_table.Fold();
					break;
				
				case VotingAction.Raise:
					_table.Raise(response.RaiseAmount);
					break;
			}
		}
	}
}
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
			_table.StartVotingCycle();
			do
			{
				await UniTask.Delay(400);
				await _table.Voter.Logic.MakeVotingAction(new NoBetsPlacedContext(_table.Voter));
				_table.AssignNextVoter();
				Debug.Log("New voter: " + _table.Voter.Name);
			} while (_table.IsVotingEnded() == false);
			_table.EndVotingCycle();
			
			Debug.Log("Vote cycle ended");
		}
	}
}
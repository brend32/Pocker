using System;
using System.Threading;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core.Controllers
{
	public class VotingCycleController
	{
		public event Action VotingEnded
		{
			add => _votingEnded.Add(value);
			remove => _votingEnded.Remove(value);
		}
		
		private readonly GameState _state;
		private readonly AnimationController _animationController;
		private readonly TableState _table;
		
		private IndependentEvent _votingEnded;

		public VotingCycleController(GameState state, AnimationController animationController)
		{
			_state = state;
			_animationController = animationController;
			_table = state.Table;
		}

		public async UniTask StartVotingCycle()
		{
			if (_table.CanSkipVote())
				return;
			
			_table.StartNewVotingCycle();
			int i = 0;
			do
			{
				if (_table.Voter.CanVote())
				{
					VotingResponse response = VotingResponse.Fold();
					if (i < 2 && _table.CardsRevealed == 0)
					{
						response = VotingResponse.Raise(10);
						MakeVoteAction(response);

						await _animationController.MakeChoice(_table.Voter, response);
					}
					else
					{
						using var tokenSource = new CancellationTokenSource();

						var votingContext = new VotingContext();
						var thinkingTask = _table.Voter.Logic.MakeVotingAction(votingContext, tokenSource.Token).Preserve();
						var result = await UniTask.WhenAny(
							UniTask.Delay(40000, cancellationToken: tokenSource.Token),
							thinkingTask);

					
						if (result == 1)
						{
							response = thinkingTask.GetAwaiter().GetResult();
						}
						else
						{
							tokenSource.Cancel();
						}
						MakeVoteAction(response);

						await _animationController.MakeChoice(_table.Voter, response);
					}
				}

				_table.AssignNextVoter();
				Debug.Log("New voter: " + _table.Voter.Name);
				i++;
			} while (_table.IsVotingEnded() == false && _table.CanSkipVote() == false);
			_table.EndVotingCycle();
			_votingEnded.Invoke();
			
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
					_table.ResetVotingCycle();
					break;
			}
		}
	}
}
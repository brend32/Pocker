using System;
using System.Threading;
using System.Threading.Tasks;
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

		private readonly GameManager _gameManager;
		private readonly GameState _state;
		private readonly AnimationController _animationController;
		private readonly TableState _table;
		
		private IndependentEvent _votingEnded;

		public VotingCycleController(GameManager gameManager, GameState state, AnimationController animationController)
		{
			_gameManager = gameManager;
			_state = state;
			_animationController = animationController;
			_table = state.Table;
		}

		public async Task StartVotingCycle(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			if (_table.CanSkipVote())
				return;
			
			_table.StartNewVotingCycle();
			int i = 0;
			do
			{
				if (_gameManager.IsPlaying == false)
					return;
				
				if (_table.Voter.CanVote())
				{
					VotingResponse response = VotingResponse.Fold();
					if (i < 2 && _table.CardsRevealed == 0)
					{
						response = VotingResponse.Raise(10);
						MakeVoteAction(response);

						await _animationController.MakeChoice(_table.Voter, response, cancellationToken);
					}
					else
					{
						if (i == 2 && _table.CardsRevealed == 0)
						{
							_table.ResetVotingCycle();
						}

						response = await GetActionResponse(cancellationToken);
						MakeVoteAction(response);

						await _animationController.MakeChoice(_table.Voter, response, cancellationToken);
					}
				}

				_table.AssignNextVoter();
				//Debug.Log("New voter: " + _table.Voter.Name);
				i++;
				//await _gameManager.DelayAsync(300, cancellationToken, false);
			} while (_table.IsVotingEnded() == false);
			_table.EndVotingCycle();
			_votingEnded.Invoke();
			
			//Debug.Log("Vote cycle ended");
		}

		private async Task<VotingResponse> GetActionResponse(CancellationToken cancellationToken)
		{
			using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			var thinkingTask = _table.Voter.Logic.MakeVotingAction(_table.VotingContext, tokenSource.Token);

			if (_gameManager.BotsGame)
			{
				var response = await thinkingTask;
				tokenSource.Cancel();
				return response;
			}
			
			var result = await Task.WhenAny(
				Timeout(tokenSource.Token),
				thinkingTask);

			tokenSource.Cancel();
			return result.Result;

			async Task<VotingResponse> Timeout(CancellationToken token)
			{
				await _gameManager.DelayAsync(40000, token, false);
				return VotingResponse.Fold();
			}
		}

		private void MakeVoteAction(VotingResponse response)
		{
			//Debug.Log(response.Action);
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
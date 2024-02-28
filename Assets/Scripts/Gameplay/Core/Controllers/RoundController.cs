using System;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Controllers;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public class RoundController
	{
		public event Action RoundStarted
		{
			add => _roundStarted.Add(value);
			remove => _roundStarted.Remove(value);
		}
		
		public VotingCycleController Voting { get; }
		
		private readonly GameState _state;
		private readonly TableState _table;

		private IndependentEvent _roundStarted;

		public RoundController(GameState state)
		{
			_state = state;
			_table = state.Table;
			Voting = new VotingCycleController(state);
		}
		
		public async UniTask StartRound()
		{
			Debug.Log("Started round " + _state.Round);
			_state.StartNewRound();
			_roundStarted.Invoke();
			//TODO: Wait for animation
			while (_table.IsAllCardsRevealed() == false)
			{
				await UniTask.Delay(2000);
				await StartVotingCycle();
				_table.RevealNextCard();
			}
			
			EndRound();
		}

		public async UniTask StartVotingCycle()
		{
			Debug.Log("Started new voting cycle");
			await Voting.StartVotingCycle();
		}

		public void EndRound()
		{
			_table.EndRound();
			Debug.Log("Round ended");
		}
	}
}
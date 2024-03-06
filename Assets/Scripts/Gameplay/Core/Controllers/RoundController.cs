using System;
using System.Threading.Tasks;
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
		public event Action RoundEnded
		{
			add => _roundEnded.Add(value);
			remove => _roundEnded.Remove(value);
		}

		public event Action RevealCards
		{
			add => _revealCards.Add(value);
			remove => _revealCards.Remove(value);
		}
		
		public VotingCycleController Voting { get; }
		
		private readonly GameState _state;
		private readonly TableState _table;

		private IndependentEvent _roundStarted;
		private IndependentEvent _roundEnded;
		private IndependentEvent _revealCards;

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

			Debug.Log("Reveal cards");
			_revealCards.Invoke();
			await Task.Delay(5000);
			_table.DecideWinner();
			Debug.Log("Decide winner");
			await Task.Delay(5000);
			
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
			_roundEnded.Invoke();
			Debug.Log("Round ended");
		}
	}
}
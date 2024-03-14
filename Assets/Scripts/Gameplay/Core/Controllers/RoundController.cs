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
		private readonly AnimationController _animationController;
		private readonly TableState _table;

		private IndependentEvent _roundStarted;
		private IndependentEvent _roundEnded;
		private IndependentEvent _revealCards;

		public RoundController(GameState state, AnimationController animationController)
		{
			_state = state;
			_animationController = animationController;
			_table = state.Table;
			Voting = new VotingCycleController(state, animationController);
		}
		
		public async UniTask StartRound()
		{
			Debug.Log("Started round " + _state.Round);
			_state.StartNewRound();
			_roundStarted.Invoke();
			await _animationController.DealCards();
			while (_table.IsAllCardsRevealed() == false)
			{
				await StartVotingCycle();
				_table.RevealNextCard();
				await _animationController.RevealCard();
			}

			Debug.Log("Reveal cards");
			_revealCards.Invoke();
			await _animationController.RevealCardsRoundEnd();
			_table.DecideWinner();
			Debug.Log("Decide winner");
			await Task.Delay(5000);
			
			await EndRound();
		}

		public async UniTask StartVotingCycle()
		{
			Debug.Log("Started new voting cycle");
			await Voting.StartVotingCycle();
		}

		public async UniTask EndRound()
		{
			_table.EndRound();
			_roundEnded.Invoke();
			await _animationController.RoundEnd();
			Debug.Log("Round ended");
		}
	}
}
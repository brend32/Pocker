using System;
using System.Threading;
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

		private readonly GameManager _gameManager;
		private readonly GameState _state;
		private readonly AnimationController _animationController;
		private readonly TableState _table;

		private IndependentEvent _roundStarted;
		private IndependentEvent _roundEnded;
		private IndependentEvent _revealCards;

		public RoundController(GameManager gameManager, GameState state, AnimationController animationController)
		{
			_gameManager = gameManager;
			_state = state;
			_animationController = animationController;
			_table = state.Table;
			Voting = new VotingCycleController(gameManager, state, animationController);
		}
		
		public async Task StartRound(CancellationToken cancellationToken)
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			//Debug.Log("Started round " + _state.Round);
			_state.StartNewRound();
			_roundStarted.Invoke();
			await _animationController.DealCards(cancellationToken);
			while (_table.IsAllCardsRevealed() == false)
			{
				if (_gameManager.IsPlaying == false)
					return;
				
				await StartVotingCycle(cancellationToken);
				_table.RevealNextCard();
				await _animationController.RevealCard(cancellationToken);
			}

			//Debug.Log("Reveal cards");
			_revealCards.Invoke();
			await _animationController.RevealCardsRoundEnd(cancellationToken);
			_table.DecideWinner();
			//Debug.Log("Decide winner");
			await _gameManager.DelayAsync(5000, cancellationToken);
			
			await EndRound(cancellationToken);
		}

		public async Task StartVotingCycle(CancellationToken cancellationToken)
		{
			//Debug.Log("Started new voting cycle");
			await Voting.StartVotingCycle(cancellationToken);
		}

		public async Task EndRound(CancellationToken cancellationToken)
		{
			_table.EndRound();
			_roundEnded.Invoke();
			foreach (PlayerState state in _table.PlayersInGame)
			{
				state.Logic.RoundEnded(_table.Winner);
			}
			await _animationController.RoundEnd(cancellationToken);
			//Debug.Log("Round ended");
		}
	}
}
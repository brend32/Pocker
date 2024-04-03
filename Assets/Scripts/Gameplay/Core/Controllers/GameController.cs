using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Controllers;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public class GameController
	{
		public RoundController Round { get; }
		public AnimationController Animation { get; }
		
		private readonly GameManager _gameManager;
		private readonly GameState _state;

		public GameController(GameManager gameManager, GameState state)
		{
			_gameManager = gameManager;
			_state = state;
			Animation = new AnimationController();
			Round = new RoundController(gameManager, state, Animation);
		}

		public async Task StartGame(CancellationToken cancellationToken)
		{
			_state.StartGame();
			await _gameManager.DelayAsync(1000, cancellationToken);
			while (IsPlaying())
			{
				await Round.StartRound(cancellationToken);
			}
			await _gameManager.DelayAsync(1000, cancellationToken);
			EndGame();
		}

		private bool IsPlaying()
		{
			return _state.Me?.IsOutOfPlay != true && _state.Table.PlayersInGame.Count > 1;
		}

		public void EndGame()
		{
			_gameManager.EndGame(_state.Table.PlayersInGame.First(state => state.IsOutOfPlay == false));
		}
	}
}
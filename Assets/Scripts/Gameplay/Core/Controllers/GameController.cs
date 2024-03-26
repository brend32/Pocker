﻿using System;
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

		public async UniTaskVoid StartGame()
		{
			_state.StartGame();
			await _gameManager.DelayAsync(1000);
			while (IsPlaying())
			{
				await Round.StartRound();
			}
			await _gameManager.DelayAsync(1000);
			EndGame();
		}

		private bool IsPlaying()
		{
			return _state.Me.IsOutOfPlay == false && _state.Table.PlayersInGame.Count > 1;
		}

		public void EndGame()
		{
			if (_state.Me.IsOutOfPlay == false)
			{
				_gameManager.EndGame(true);
			}
			else
			{
				_gameManager.EndGame();
			}
		}
	}
}
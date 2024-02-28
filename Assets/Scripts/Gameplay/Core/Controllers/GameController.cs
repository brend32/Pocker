using System;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Controllers;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public class GameController
	{
		private readonly GameState _state;
		public RoundController Round { get; }
		public VotingCycleController VotingCycle { get; }

		public GameController(GameState state)
		{
			_state = state;
			Round = new RoundController(state);
		}

		public async UniTaskVoid StartGame()
		{
			_state.StartGame();
			await UniTask.Delay(1000);
			while (IsPlaying())
			{
				await Round.StartRound();
				
			}
			await EndGame();
		}

		private bool IsPlaying()
		{
			return _state.Round < 2;
		}

		public async UniTask EndGame()
		{
			Debug.Log("Game ended");
		}
	}
}
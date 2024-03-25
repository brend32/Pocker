using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Contracts;
using Poker.Gameplay.Core.Models;
using UnityEngine;

namespace Poker.Gameplay.Core.States
{
	public class BotState : PlayerState
	{
		private static readonly Dictionary<Difficulty, float> _fortuneMap = new()
		{
			{ Difficulty.Easy, 0.05f },
			{ Difficulty.Normal, 0.15f },
			{ Difficulty.Hard, 0.45f },
			{ Difficulty.Insane, 0.93f }
		};
		
		private static readonly Dictionary<Difficulty, float> _confidenceMap = new()
		{
			{ Difficulty.Easy, 0.92f },
			{ Difficulty.Normal, 1 },
			{ Difficulty.Hard, 1.3f },
			{ Difficulty.Insane, 2 }
		};
		
		private static readonly Dictionary<Difficulty, float> _dumbMap = new()
		{
			{ Difficulty.Easy, 0.25f },
			{ Difficulty.Normal, 0.05f },
			{ Difficulty.Hard, 0.09f },
			{ Difficulty.Insane, 0.02f }
		};
		
		private static readonly Dictionary<Difficulty, float> _fearMap = new()
		{
			{ Difficulty.Easy, 0.015f },
			{ Difficulty.Normal, 0.005f },
			{ Difficulty.Hard, 0.009f },
			{ Difficulty.Insane, 0.002f }
		};
		
		private static readonly Dictionary<Difficulty, float> _cashMap = new()
		{
			{ Difficulty.Easy, 1 },
			{ Difficulty.Normal, 1 },
			{ Difficulty.Hard, 1.25f },
			{ Difficulty.Insane, 2f }
		};
		
		public int Confidence { get; private set; }
		public float DumbChance { get; private set; }
		public float FortuneChance { get; private set; }
		public float FearFactor { get; private set; }

		public void Win()
		{
			Confidence += Random.Range(1, 3);
		}

		public void Lose()
		{
			Confidence -= 1;
		}

		public static BotState CreateBotPlayer(GameManager gameManager, GameSettings gameSettings, GameState state, string name)
		{
			var player = new BotState()
			{
				Balance = Mathf.CeilToInt(gameSettings.StartingCash * _cashMap[gameSettings.Difficulty]),
				Name = name,
				
				Confidence = Mathf.CeilToInt(Random.Range(3, 8) * _confidenceMap[gameSettings.Difficulty]),
				DumbChance = _dumbMap[gameSettings.Difficulty],
				FortuneChance = _fortuneMap[gameSettings.Difficulty] + Random.Range(0, 0.05f),
				FearFactor = _fearMap[gameSettings.Difficulty] + Random.Range(-0.003f, 0.03f)
			};
			var logic = new BotLogic(player, state, gameManager);
			player.Logic = logic;
			return player;
		}
	}
	
	public class BotLogic : IPlayerLogic
	{
		private readonly BotState _state;
		private readonly GameManager _gameManager;
		private readonly TableState _tableState;
		private readonly PlayerState _user;

		public BotLogic(BotState state, GameState gameState, GameManager gameManager)
		{
			_state = state;
			_gameManager = gameManager;
			_tableState = gameState.Table;
			_user = gameState.Me;

			_tableState.DecidedWinner += winner =>
			{
				if (winner == _state)
				{
					_state.Win();
				}
				else
				{
					_state.Lose();
				}
			};
		}

		private Combination GetCombination()
		{
			return new Combination(_state.Cards, _tableState.Cards.Take(_tableState.CardsRevealed));
		}

		public async UniTask<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken)
		{
			Combination combination = GetCombination();
			if (Random.value < _state.DumbChance)
			{
				Debug.Log("Dumb 1");
				return await MakeDumbAction(context);
			}

			var sum = 0;
			if (Random.value < _state.FortuneChance)
			{
				combination = new Combination(_state.Cards, _tableState.Cards);
				if (Random.value < _state.FortuneChance * _state.FortuneChance)
				{
					var userCombination = new Combination(_user.Cards, _tableState.Cards);
					if (userCombination < combination)
					{
						sum += (_tableState.CardsRevealed + 1) * 3;
					}
					else
					{
						sum -= (_tableState.CardsRevealed + 1) * 3;
					}
				}
			}
			
			sum += Mathf.CeilToInt(_state.Confidence / 3f) * combination.CombinationIndex + _state.Balance / 45;
			
			
			if (Random.value < _state.FearFactor * (_tableState.CardsRevealed + 1) / 3 && context.MinimumBet > 150)
			{
				await _gameManager.DelayAsync(Mathf.CeilToInt(Random.value * 2750) + 350, cancellationToken: cancellationToken);
				if (_state.Confidence > 6 && Random.value < _state.FearFactor)
				{
					Debug.Log("Fear 1");
					return VotingResponse.Fold();
				}
				
				if (_state.Balance > 300)
				{
					Debug.Log("Fear 2");
					return VotingResponse.Call();
				}
				
				Debug.Log("Fear 3");
				return VotingResponse.Fold();
			}

			if (_state.Confidence < 3)
			{
				Debug.Log("Dumb 2");
				return await MakeDumbAction(context);
			}
			
			Debug.Log($"Sum {sum}, Confidence: {_state.Confidence}, CombinationIndex: {combination.CombinationIndex}");
			await _gameManager.DelayAsync(Mathf.CeilToInt(Random.value * 2050) + 550, cancellationToken: cancellationToken);
			if (sum > 30)
				return VotingResponse.Raise(Random.Range(Mathf.FloorToInt(_state.Balance * 0.45f), _state.Balance));
			
			if (sum > 20)
				return VotingResponse.Raise(Random.Range(Mathf.FloorToInt(_state.Balance * 0.15f), Mathf.FloorToInt(_state.Balance * 0.45f)));
			
			if (sum > 10)
				return VotingResponse.Raise(Random.Range(Mathf.FloorToInt(_state.Balance * 0.05f),
					Mathf.FloorToInt(_state.Balance * 0.15f)));
			
			if (sum > 4)
				return VotingResponse.Call();
			
			return VotingResponse.Fold();
		}

		private async UniTask<VotingResponse> MakeDumbAction(VotingContext context)
		{
			if (context.MinimumBet / (float)_state.Balance > 0.25f)
			{
				await _gameManager.DelayAsync((int)Random.value * 700 + 150);
				if (Random.value < _state.FortuneChance == false)
				{
					return VotingResponse.Fold();
				}
			}

			await _gameManager.DelayAsync(700);
			return Random.Range(0, 3) switch
			{
				1 => VotingResponse.Fold(),
				2 => VotingResponse.Raise(Mathf.CeilToInt(Random.value * 0.5f * _state.Balance)),
				_ => VotingResponse.Call()
			};
		}
	}
}
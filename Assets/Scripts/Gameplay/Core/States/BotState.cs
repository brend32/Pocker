using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.BotLogic;
using Poker.Gameplay.Core.Contracts;
using Poker.Gameplay.Core.Models;
using Poker.Utils;
using UnityEngine;

namespace Poker.Gameplay.Core.States
{
	public class BotState : PlayerState
	{
		public int PairRank { get; private set; }
		public int RoundsWithoutFool { get; private set; }
		public int StartingCash { get; private set; }
		public Dictionary<PlayerState, double> TrustMap { get; private set; } = new();
		
		public int Wins { get; private set; }
		public int RoundsAlive { get; private set; }

		public BotSettings Settings { get; private set; } = new();

		public void CountWin()
		{
			Wins++;
		}
		
		public void CountAliveRound()
		{
			RoundsAlive++;
		}
		
		public override void Reset()
		{
			Balance = StartingCash;
			TrustMap.Clear();
			ResetRoundWithoutFoolCounter();
			base.Reset();
		}

		public void SetPairRank()
		{
			PairRank = CombinationOdds.GetPairRank(_cards);
		}

		public void IncrementRoundWithoutFoolCounter()
		{
			RoundsWithoutFool++;
		}

		public void ResetRoundWithoutFoolCounter()
		{
			RoundsWithoutFool = 0;
		}

		public void FillTrustMap(TableState table)
		{
			foreach (PlayerState state in table.PlayersInGame)
			{
				TrustMap.Add(state, 0);
			}
		}

		public void UpdateTrustMap(TableState table)
		{
			if (TrustMap.Count == 0)
			{
				FillTrustMap(table);
			}
			
			foreach (PlayerState state in table.PlayersInGame.Where(p => p.Folded == false))
			{
				var playerCardsRank = CombinationOdds.GetPairRank(state.Cards);
				if (playerCardsRank >= Settings.TrustPairRankThreshold)
				{
					TrustMap[state] += Settings.TrustGain;
				}
				else
				{
					TrustMap[state] -= Settings.TrustLoss;
				}
			}
		}

		public void ResetGoals()
		{
			Wins = 0;
			RoundsAlive = 0;
		}
		
		public override string ToString()
		{
			return $"{nameof(Wins)}: {Wins}, {nameof(RoundsAlive)}: {RoundsAlive}, {nameof(Settings)}: {Settings}";
		}
		
		public static BotState CreateBotPlayer(GameSettings gameSettings, string name)
		{
			var player = new BotState()
			{
				Balance = Mathf.CeilToInt(gameSettings.StartingCash * BotPresets.CashMultiplier[gameSettings.Difficulty]),
				StartingCash = gameSettings.StartingCash,
				Name = name,
				Logic = new BotLogic(),
			};
			player.Settings.Copy(BotPresets.Presets[gameSettings.Difficulty].RandomEntry());
			if (RandomUtils.Random.NextDouble() > 0.15f)
				player.Settings.Mutate();
			
			return player;
		}
	}
	
	public class BotLogic : IPlayerLogic
	{
		private BotState _state;
		private TableState _table;
		private VotingContext _votingContext;
		private readonly GameManager _gameManager;

		private BotSettings Settings => _state.Settings;
		private bool IsLowMoney => _state.Balance < Settings.LowMoneyThreshold * _state.StartingCash;
		private bool IsStrongPair => _state.PairRank >= Settings.StrongPairThreshold;
		private double _panicLevel;
		private double _higherEnemyCombinationChance;
		private int _combinationValue;
		private bool _isFooling;
		private bool _isSilent;

		public async Task<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken)
		{
			_votingContext = context;
			GatherInfo(context);
			var hasBets = _votingContext.MinimumBet != 20;

			// Updating required fields
			if (_table.CardsRevealed == 0)
			{
				_state.SetPairRank();
			}
			else
			{
				UpdateCombinationValue();
				UpdateHigherEnemyCombinationChance();

				if (_higherEnemyCombinationChance >= Settings.EnemyStrongerCombinationPanicThreshold)
				{
					if (_isFooling)
					{
						return VotingResponse.Call();
					}
					
					return VotingResponse.Fold();
				}
			}
			UpdatePanicLevel(hasBets);
			TrySetFoolingState();
			TrySetSilentState();

			// Choosing response based on current state
			if (hasBets == false && _table.CardsRevealed == 0)
				return BlindGuess();

			if (hasBets && _table.CardsRevealed == 0)
				return BlindGuessWithTrust();
			
			if (_panicLevel >= Settings.PanicThreshold)
				return VotingResponse.Fold();

			if (Settings.SkipTurnChance >= RandomUtils.Random.NextDouble())
				return VotingResponse.Call();
			
			if (_isSilent && _table.CardsRevealed == 3)
				return VotingResponse.Call();
			
			if (_isSilent && (_isFooling || IsStrongPair) && _table.CardsRevealed > 4)
				return VotingResponse.Raise(GetRaiseAmount());
			
			if (Settings.EnemyStrongerCombinationMiddleThreshold < _higherEnemyCombinationChance)
				return VotingResponse.Raise(GetRaiseAmount());
			
			return VotingResponse.Call();
		}

		public void RoundEnded(PlayerState winner)
		{
			if (_state.IsOutOfPlay)
				return;

			_state.CountAliveRound();
			
			if (_isFooling == false)
				_state.IncrementRoundWithoutFoolCounter();
			
			_isSilent = false;
			_isFooling = false;
			_state.UpdateTrustMap(_table);
		}

		private void TrySetFoolingState()
		{
			if (_state.RoundsWithoutFool < Settings.MinRoundsWithoutFool)
				return;
			
			_isFooling = RandomUtils.Random.NextDouble() <= Settings.FoolChance;
			_state.ResetRoundWithoutFoolCounter();
		}

		private void TrySetSilentState()
		{
			_isSilent = RandomUtils.Random.NextDouble() <= Settings.SilentChance;
		}

		private void UpdateCombinationValue()
		{
			_combinationValue = new Combination(_state.Cards, _table.Cards.Take(_table.CardsRevealed)).Value;
		}

		private void UpdateHigherEnemyCombinationChance()
		{
			_higherEnemyCombinationChance = CombinationOdds
				.GetHigherCombinationsPercent(_table.Cards.Take(_table.CardsRevealed).ToArray(), _combinationValue);
		}

		private void UpdatePanicLevel(bool includeTrust)
		{
			_panicLevel = 0;
			_panicLevel -= Settings.MoneyPanicMultiplier * _state.Balance;
			_panicLevel -= _state.PairRank * Settings.PairRankPanicMultiplier;
			_panicLevel += _votingContext.MinimumBet * Settings.BetPanicMultiplier;
			_panicLevel += _higherEnemyCombinationChance * Settings.EnemyStrongerCombinationPanicMultiplier;

			if (includeTrust)
			{
				foreach (PlayerState enemy in _table.PlayersInGame.Where(p => p.Folded == false))
				{
					if (_state.TrustMap[enemy] > Settings.EnemyTrustThreshold)
					{
						_panicLevel += _votingContext.MinimumBet * Settings.BetPanicMultiplier;
					}
				}
			}
		}

		private int GetRaiseAmount()
		{
			var raisePercent = _isFooling ? Settings.FoolRaisePercent : Settings.StrongPairRaisePercent;
			
			return Mathf.CeilToInt(
				_state.Balance *
				Mathf.Max(
					0.01f,
					(float)(raisePercent + Settings.RaiseOffset * RandomUtils.Random.NextDouble())
				)
			);
		}

		private VotingResponse BlindGuess()
		{
			if (_panicLevel >= Settings.BlindPanicThreshold)
				return VotingResponse.Fold();
			
			if (IsStrongPair && _isFooling == false)
			{
				return VotingResponse.Raise(Mathf.CeilToInt(GetRaiseAmount() * 0.5f));
			}
			
			if (IsLowMoney && _isFooling == false)
			{
				return VotingResponse.Fold();
			}

			return VotingResponse.Call();
		}
		
		private VotingResponse BlindGuessWithTrust()
		{
			if (_panicLevel >= Settings.PanicThreshold)
				return VotingResponse.Fold();
			
			if (IsStrongPair || _isFooling)
			{
				return VotingResponse.Raise(Mathf.CeilToInt(GetRaiseAmount() * 0.5f));
			}
			
			if (IsLowMoney)
			{
				return VotingResponse.Fold();
			}

			return VotingResponse.Call();
		}

		private void GatherInfo(VotingContext context)
		{
			_state ??= (BotState)context.Voter;
			_table ??= context.Table;
			
			if (_state.TrustMap.Count == 0)
				_state.FillTrustMap(_table);
		}
	}
}
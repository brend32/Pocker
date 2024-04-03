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
		public override string ToString()
		{
			return $"{nameof(Wins)}: {Wins}, {nameof(RoundsAlive)}: {RoundsAlive}, {nameof(StrongPairThreshold)}: {StrongPairThreshold}, {nameof(EnemyStrongerCombinationMiddleThreshold)}: {EnemyStrongerCombinationMiddleThreshold}, {nameof(EnemyStrongerCombinationPanicThreshold)}: {EnemyStrongerCombinationPanicThreshold}, {nameof(TrustPairRankThreshold)}: {TrustPairRankThreshold}, {nameof(TrustGain)}: {TrustGain}, {nameof(TrustLoss)}: {TrustLoss}, {nameof(EnemyTrustThreshold)}: {EnemyTrustThreshold}, {nameof(LowMoneyThreshold)}: {LowMoneyThreshold}, {nameof(MinRoundsWithoutFool)}: {MinRoundsWithoutFool}, {nameof(FoolChance)}: {FoolChance}, {nameof(SilentChance)}: {SilentChance}, {nameof(PanicThreshold)}: {PanicThreshold}, {nameof(BlindPanicThreshold)}: {BlindPanicThreshold}, {nameof(MoneyPanicMultiplier)}: {MoneyPanicMultiplier}, {nameof(PairRankPanicMultiplier)}: {PairRankPanicMultiplier}, {nameof(EnemyStrongerCombinationPanicMultiplier)}: {EnemyStrongerCombinationPanicMultiplier}, {nameof(FoolRaisePercent)}: {FoolRaisePercent}, {nameof(StrongPairRaisePercent)}: {StrongPairRaisePercent}, {nameof(RaiseOffset)}: {RaiseOffset}, {nameof(BetPanicMultiplier)}: {BetPanicMultiplier}, {nameof(SkipTurnChance)}: {SkipTurnChance}";
		}

		public int PairRank { get; private set; }
		public int RoundsWithoutFool { get; private set; }
		public int StartingCash { get; private set; }
		public Dictionary<PlayerState, double> TrustMap { get; private set; } = new();
		
		public int Wins { get; private set; }
		public int RoundsAlive { get; private set; }

		public int StrongPairThreshold { get; set; } = 19;
		public double EnemyStrongerCombinationMiddleThreshold { get; private set; } = 0.51371320977807;
		public double EnemyStrongerCombinationPanicThreshold { get; private set; } = 0.405301503837109;
		public int TrustPairRankThreshold { get; private set; } = 13;
		public double TrustGain { get; private set; } = 0.340960390865803;
		public double TrustLoss { get; private set; } = 0.523207994401455;
		public double EnemyTrustThreshold { get; private set; } = 0.441805862672627;
		public double LowMoneyThreshold { get; private set; } = 0.408863117620349;
		public int MinRoundsWithoutFool { get; private set; } = 21;
		public double FoolChance { get; private set; } = 0.432952526481822;
		public double SilentChance { get; private set; } = 0.503569041788578;
		public double PanicThreshold { get; private set; } = 0.551069484502077;
		public double BlindPanicThreshold { get; private set; } = 0.4534722879529;
		public double MoneyPanicMultiplier { get; private set; } = 0.506963741928339;
		public double PairRankPanicMultiplier { get; private set; } = 0.519698183909059;
		public double EnemyStrongerCombinationPanicMultiplier { get; private set; } = 0.571910247504711;
		public double FoolRaisePercent { get; private set; } = 0.554264077916741;
		public double StrongPairRaisePercent { get; private set; } = 0.460994556993246;
		public double RaiseOffset { get; private set; } = 0.576030493974686;
		public double BetPanicMultiplier { get; private set; } = 0.540366609245539;
		public double SkipTurnChance { get; private set; } = 0.476075193881988;

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

		public void Copy(BotState bot)
		{
			// Copy parameters from another bot
			StrongPairThreshold = bot.StrongPairThreshold; 
			EnemyStrongerCombinationMiddleThreshold = bot.EnemyStrongerCombinationMiddleThreshold; 
			EnemyStrongerCombinationPanicThreshold = bot.EnemyStrongerCombinationPanicThreshold; 
			TrustPairRankThreshold = bot.TrustPairRankThreshold; 
			TrustGain = bot.TrustGain; 
			TrustLoss = bot.TrustLoss; 
			EnemyTrustThreshold = bot.EnemyTrustThreshold; 
			LowMoneyThreshold = bot.LowMoneyThreshold; 
			MinRoundsWithoutFool = bot.MinRoundsWithoutFool; 
			FoolChance = bot.FoolChance; 
			SilentChance = bot.SilentChance; 
			PanicThreshold = bot.PanicThreshold; 
			BlindPanicThreshold = bot.BlindPanicThreshold; 
			MoneyPanicMultiplier = bot.MoneyPanicMultiplier; 
			PairRankPanicMultiplier = bot.PairRankPanicMultiplier; 
			EnemyStrongerCombinationPanicMultiplier = bot.EnemyStrongerCombinationPanicMultiplier; 
			FoolRaisePercent = bot.FoolRaisePercent; 
			StrongPairRaisePercent = bot.StrongPairRaisePercent; 
			RaiseOffset = bot.RaiseOffset; 
			BetPanicMultiplier = bot.BetPanicMultiplier; 
			SkipTurnChance = bot.SkipTurnChance; 
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
				if (playerCardsRank >= TrustPairRankThreshold)
				{
					TrustMap[state] += TrustGain;
				}
				else
				{
					TrustMap[state] -= TrustLoss;
				}
			}
		}

		public void ResetGoals()
		{
			Wins = 0;
			RoundsAlive = 0;
		}
		
		public void Mutate()
		{
			// Mutating properties to get new behaviour
			for (int i = 0; i < 5; i++)
			{
				int n = RandomUtils.Random.Next(0, 30);
				if (RandomUtils.Random.NextDouble() < 0.25)
					continue;
				
				switch (n)
				{
					case 0:
						StrongPairThreshold += RandomUtils.Random.Next(-15, 15);
						break;
					case 1:
						EnemyStrongerCombinationMiddleThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 2:
						EnemyStrongerCombinationPanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 3:
						TrustPairRankThreshold += RandomUtils.Random.Next(-15, 15);
						break;
					case 4:
						TrustGain += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 5:
						TrustLoss += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 6:
						EnemyTrustThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 7:
						LowMoneyThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 8:
						MinRoundsWithoutFool += RandomUtils.Random.Next(-15, 15);
						break;
					case 9:
						FoolChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 10:
						SilentChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 11:
						PanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 12:
						BlindPanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 13:
						MoneyPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 14:
						PairRankPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 15:
						EnemyStrongerCombinationPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 16:
						FoolRaisePercent += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 17:
						StrongPairRaisePercent += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 18:
						RaiseOffset += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 19:
						BetPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 20:
						SkipTurnChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					
					case 25:
						return;
				}
			}
		}
		
		public static BotState CreateBotPlayer(GameSettings gameSettings, string name)
		{
			var player = new BotState()
			{
				Balance = gameSettings.StartingCash,
				StartingCash = gameSettings.StartingCash,
				Name = name,
				Logic = new BotLogic(),
			};
			return player;
		}
	}
	
	public class BotLogic : IPlayerLogic
	{
		private BotState _state;
		private TableState _table;
		private VotingContext _votingContext;
		private readonly GameManager _gameManager;

		private bool IsLowMoney => _state.Balance < _state.LowMoneyThreshold * _state.StartingCash;
		private bool IsStrongPair => _state.PairRank >= _state.StrongPairThreshold;
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

				if (_higherEnemyCombinationChance >= _state.EnemyStrongerCombinationPanicThreshold)
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
			
			if (_panicLevel >= _state.PanicThreshold)
				return VotingResponse.Fold();

			if (_state.SkipTurnChance >= RandomUtils.Random.NextDouble())
				return VotingResponse.Call();
			
			if (_isSilent && _table.CardsRevealed == 3)
				return VotingResponse.Call();
			
			if (_isSilent && (_isFooling || IsStrongPair) && _table.CardsRevealed > 4)
				return VotingResponse.Raise(GetRaiseAmount());
			
			if (_state.EnemyStrongerCombinationMiddleThreshold < _higherEnemyCombinationChance)
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
			if (_state.RoundsWithoutFool < _state.MinRoundsWithoutFool)
				return;
			
			_isFooling = RandomUtils.Random.NextDouble() <= _state.FoolChance;
			_state.ResetRoundWithoutFoolCounter();
		}

		private void TrySetSilentState()
		{
			_isSilent = RandomUtils.Random.NextDouble() <= _state.SilentChance;
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
			_panicLevel -= _state.MoneyPanicMultiplier * _state.Balance;
			_panicLevel -= _state.PairRank * _state.PairRankPanicMultiplier;
			_panicLevel += _votingContext.MinimumBet * _state.BetPanicMultiplier;
			_panicLevel += _higherEnemyCombinationChance * _state.EnemyStrongerCombinationPanicMultiplier;

			if (includeTrust)
			{
				foreach (PlayerState enemy in _table.PlayersInGame.Where(p => p.Folded == false))
				{
					if (_state.TrustMap[enemy] > _state.EnemyTrustThreshold)
					{
						_panicLevel += _votingContext.MinimumBet * _state.BetPanicMultiplier;
					}
				}
			}
		}

		private int GetRaiseAmount()
		{
			var raisePercent = _isFooling ? _state.FoolRaisePercent : _state.StrongPairRaisePercent;
			
			return Mathf.CeilToInt(
				_state.Balance *
				Mathf.Max(
					0.01f,
					(float)(raisePercent + _state.RaiseOffset * RandomUtils.Random.NextDouble())
				)
			);
		}

		private VotingResponse BlindGuess()
		{
			if (_panicLevel >= _state.BlindPanicThreshold)
				return VotingResponse.Fold();
			
			if (IsStrongPair && _isFooling == false)
			{
				return VotingResponse.Raise(GetRaiseAmount());
			}
			
			if (IsLowMoney && _isFooling == false)
			{
				return VotingResponse.Fold();
			}

			return VotingResponse.Call();
		}
		
		private VotingResponse BlindGuessWithTrust()
		{
			if (_panicLevel >= _state.PanicThreshold)
				return VotingResponse.Fold();
			
			if (IsStrongPair || _isFooling)
			{
				return VotingResponse.Raise(GetRaiseAmount());
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
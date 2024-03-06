using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Contracts;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.Models.VotingContexts;
using Poker.UI.Common;
using UnityEngine;

namespace Poker.Gameplay.Core.States
{
	public class PlayerState
	{
		public event Action DataChanged
		{
			add => _dataChanged.Add(value);
			remove => _dataChanged.Remove(value);
		}
		
		public IPlayerLogic Logic { get; private set; }
		public int Balance { get; private set; }
		public string Name { get; private set; }
		public IReadOnlyList<CardModel> Cards => _cards; 
		
		public int Bet { get; private set; }
		public bool IsAllIn => Balance == 0;
		public bool Folded { get; private set; }
		public bool IsOutOfPlay { get; private set; }

		private readonly CardModel[] _cards = new CardModel[2];
		private IndependentEvent _dataChanged;

		public void GiveCards(CardModel card1, CardModel card2)
		{
			_cards[0] = card1;
			_cards[1] = card2;
			
			_dataChanged.Invoke();
		}

		public void ResetBetState()
		{
			Bet = 0;
			_dataChanged.Invoke();
		}

		public void ResetRoundState()
		{
			Bet = 0;
			Folded = false;

			IsOutOfPlay = Balance == 0;
			_dataChanged.Invoke();
		}

		public void GiveMoney(int amount)
		{
			Balance += amount;
			_dataChanged.Invoke();
		} 

		public int MakeBet(int bet)
		{
			if (Bet > bet)
				throw new Exception($"Invalid bet amount, Current: {Bet}, New bet: {bet}");

			var amount = bet - Bet; 
			
			if (Balance < amount)
			{
				amount = Balance;
			}

			Bet += amount;
			Balance -= amount;
			
			_dataChanged.Invoke();
			return amount;
		}

		public void Fold()
		{
			Folded = true;
			_dataChanged.Invoke();
		}

		public static PlayerState CreateBotPlayer(GameSettings gameSettings, string name)
		{
			var logic = new TestLogic()
			{

			};
			var player = new PlayerState()
			{
				Logic = logic,
				Balance = gameSettings.StartingCash,
				Name = name
			};
			logic.O = player;
			return player;
		}
		
		public static PlayerState CreatePlayer(GameSettings gameSettings, string name)
		{
			var player = new PlayerState()
			{
				Logic = new UserLogic(),
				Balance = gameSettings.StartingCash,
				Name = name
			};
			return player;
		}
	}

	public partial class TestLogic : IPlayerLogic
	{
		public PlayerState O;
		
		public async UniTask<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken)
		{
			if (O.Name.Contains("2"))
			{
				for (int i = 0; i < 2; i++)
				{
					Debug.Log($"Thinking long {O.Name} " + i);
					await UniTask.Delay(100, cancellationToken: cancellationToken);
				}
				return VotingResponse.Raise(10);
			}
			
			for (int i = 0; i < 3; i++)
			{
				Debug.Log($"Thinking {O.Name} " + i);
				await UniTask.Delay(100, cancellationToken: cancellationToken);
			}

			return VotingResponse.Call();
		}
	}

	public class UserLogic : IPlayerLogic
	{
		private VotingResponse? _response;

		public void MakeChoice(VotingResponse choice)
		{
			_response = choice;
		}

		public async UniTask<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken)
		{
			_response = null;
			
			while (cancellationToken.IsCancellationRequested == false)
			{
				if (_response != null)
					return _response.Value;

				await UniTask.Yield();
			}

			return default;
		}
	}
}
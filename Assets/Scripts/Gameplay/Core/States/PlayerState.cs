using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.Contracts;
using Poker.Gameplay.Core.Models;
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
		
		public IPlayerLogic Logic { get; protected set; }
		public int Balance { get; protected set; }
		public string Name { get; protected set; }
		public IReadOnlyList<CardModel> Cards => _cards; 
		
		public int Bet { get; protected set; }
		public bool IsAllIn => Balance == 0;
		public bool Folded { get; protected set; }
		public bool IsOutOfPlay { get; protected set; }

		protected readonly CardModel[] _cards = new CardModel[2];
		protected IndependentEvent _dataChanged;

		public virtual void Reset()
		{
			IsOutOfPlay = false;
			ResetBetState();
		}
		
		public void GiveCards(CardModel card1, CardModel card2)
		{
			_cards[0] = card1;
			_cards[1] = card2;
			
			_dataChanged.Invoke();
		}

		public bool CanVote()
		{
			return Balance > 0 && Folded == false && IsOutOfPlay == false;
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
				return 0;

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

	public class UserLogic : IPlayerLogic
	{
		private VotingResponse? _response;

		public void MakeChoice(VotingResponse choice)
		{
			_response = choice;
		}

		public async Task<VotingResponse> MakeVotingAction(VotingContext context, CancellationToken cancellationToken)
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

		public void RoundEnded(PlayerState winner)
		{
			
		}
	}
}
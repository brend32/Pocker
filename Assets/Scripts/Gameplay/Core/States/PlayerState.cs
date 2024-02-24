using System;
using System.Collections;
using System.Collections.Generic;
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
		public bool IsAllInBet { get; private set; }
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

		public void MakeBet(int amount)
		{
			if (Balance < amount)
				throw new InvalidOperationException("No money for bet");

			Bet = amount;
			Balance -= amount;
			
			_dataChanged.Invoke();
		}

		public void Fold()
		{
			Folded = true;
		}

		public static PlayerState CreatePlayer(Context scope, GameSettings gameSettings, string name)
		{
			return new PlayerState()
			{
				Logic = new TestLogic(),
				Balance = gameSettings.StartingCash,
				Name = name
			};
		}
	}

	public partial class TestLogic : IPlayerLogic
	{
		public async UniTask MakeVotingAction(VotingContext context)
		{
			for (int i = 0; i < 3; i++)
			{
				Debug.Log($"Thinking {context.Voter.Name} " + i);
				await UniTask.Delay(700);
			}

			if (context is NoBetsPlacedContext noBets)
			{
				noBets.Check();
			}
		}
	}
}
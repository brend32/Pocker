using System;
using System.Collections.Generic;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core.Models;
using Poker.UI.Common;

namespace Poker.Gameplay.Core.States
{
	public class PlayerState
	{
		public event Action DataChanged
		{
			add => _dataChanged.Add(value);
			remove => _dataChanged.Remove(value);
		}
		
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

		public static PlayerState CreatePlayer(GameSettings gameSettings, string name)
		{
			return new PlayerState()
			{
				Balance = gameSettings.StartingCash,
				Name = name
			};
		}
	}
}
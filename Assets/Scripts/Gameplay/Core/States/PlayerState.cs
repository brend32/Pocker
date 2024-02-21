using System;
using AurumGames.CompositeRoot;
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
		public Card[] Cards { get; private set; } = new Card[2];
		
		public int Bet { get; private set; }
		public bool IsAllInBet { get; private set; }
		public bool Folded { get; private set; }
		public bool IsOutOfPlay { get; private set; }

		private IndependentEvent _dataChanged;

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
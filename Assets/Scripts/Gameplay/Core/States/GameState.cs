using System;
using System.Collections.Generic;
using System.Linq;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.Statistics;
using Poker.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Poker.Gameplay.Core.States
{
	public class GameState
	{
		public event Action RoundEnded;
		public event Action VoteCicleEnded;
		
		public float PlayTime { get; set; }
		public int Round { get; set; }
		
		public PlayerState Me { get; private set; }
		public IReadOnlyList<PlayerState> Players => _players;
		public TableState Table { get; private set; } = new();
		public Phase Phase { get; private set; }
		
		private readonly List<PlayerState> _players = new();
		private readonly GameStatistics _statistics;

		public GameState(GameStatistics statistics)
		{
			_statistics = statistics;
		}

		public void AddMe(PlayerState player)
		{
			Me = player;
			AddPlayer(player);
		}

		public void AddPlayer(PlayerState player)
		{
			_players.Add(player);
		}

		public void StartGame()
		{
			Table.AddPlayers(_players);
			StartNewRound();
		}

		public void StartNewRound()
		{
			Round++;
			Table.StartRound();
		}

		public void MakeBet(int amount)
		{
			if (Phase != Phase.WaitingForPlayerAction)
				throw new Exception($"Unable make bet outside {nameof(Phase.WaitingForPlayerAction)} phase");
			
			Debug.Log($"Make bet {amount}");
		}
	}

	public enum Phase
	{
		WaitingForAnimationToEnd,
		WaitingForPlayerAction,
		GameEnded
	}
}
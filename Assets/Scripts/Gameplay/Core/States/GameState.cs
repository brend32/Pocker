using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Gameplay.Core.Statistics;
using UnityEngine;

namespace Poker.Gameplay.Core.States
{
	public class GameState
	{
		public event Action RoundEnded;
		public event Action VoteCicleEnded;
		
		public PlayerState Voter => VoterIndex > 0 ? _playersInGame[VoterIndex] : null;
		public float PlayTime { get; set; }
		public int Round { get; set; }
		
		public PlayerState Me { get; private set; }
		public IReadOnlyList<PlayerState> Players => _players;
		public IReadOnlyList<PlayerState> PlayersInGame => _playersInGame;
		public int VoteStartIndex { get; private set; }
		public int VoterIndex { get; private set; }
		public Phase Phase { get; private set; }
		
		private readonly List<PlayerState> _players = new();
		private readonly List<PlayerState> _playersInGame = new();
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

		public void StartNewRound()
		{
			Round++;
			_playersInGame.Clear();
			foreach (PlayerState player in _players.Where(player => player.IsOutOfPlay == false))
			{
				_playersInGame.Add(player);
			}
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
using System;
using System.Collections.Generic;
using System.Linq;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.Models.VotingContexts;
using Poker.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Poker.Gameplay.Core.States
{
	public class TableState
	{
		public IReadOnlyList<CardModel> Deck { get; } = Enumerable.Range(2, 12)
			.SelectMany(value => new CardModel[]
			{
				new(CardType.Circle, value),
				new(CardType.Flame, value),
				new(CardType.Square, value),
				new(CardType.Square, value),
			}).ToArray();
		
		public event Action PotChanged
		{
			add => _potChanged.Add(value);
			remove => _potChanged.Remove(value);
		}
		public event Action NewCardRevealed
		{
			add => _newCardRevealed.Add(value);
			remove => _newCardRevealed.Remove(value);
		}
		public event Action NewVoterAssigned
		{
			add => _newVoterAssigned.Add(value);
			remove => _newVoterAssigned.Remove(value);
		}

		public PlayerState Voter => IsVoting ? _playersInGame[VoterIndex] : null;

		public IReadOnlyList<PlayerState> PlayersInGame => _playersInGame;
		public IReadOnlyList<CardModel> Cards => _cards;
		public int CardsRevealed { get; private set; }
		public bool IsVoting { get; private set; }
		public int FirstVoterIndex
		{
			get => _firstVoterIndex;
			set => _firstVoterIndex = VoterIndex = VoteEndIndex = value;
		}
		public int VoteEndIndex { get; private set; }
		public VotingContext VotingContext { get; private set; } = new();
		public int Pot { get; private set; }
		public int VoterIndex { get; private set; }
		
		private readonly List<PlayerState> _playersInGame = new();
		private readonly CardModel[] _cards = new CardModel[5];
		private readonly List<CardModel> _deck = new();

		private IndependentEvent _newCardRevealed;
		private IndependentEvent _newVoterAssigned;
		private IndependentEvent _potChanged;
		private int _firstVoterIndex = -1;

		public void AddPlayers(IEnumerable<PlayerState> players)
		{
			_playersInGame.AddRange(players);
		}

		public void StartRound()
		{
			Pot = 0;
			
			_deck.Clear();
			_deck.AddRange(Deck);

			UpdateListOfPlayerInGame();
			DealTableCards();
			DealCardsToPlayers();
		}

		public void EndRound()
		{
			//TODO: Decide winner

			Pot = 0;

			foreach (PlayerState player in _playersInGame)
			{
				player.ResetRoundState();
			}
			
			UpdateListOfPlayerInGame();
		}

		public void StartNewVotingCycle()
		{
			IsVoting = true;
			if (FirstVoterIndex == -1)
			{
				FirstVoterIndex = Random.Range(0, _playersInGame.Count);
			}
			else
			{
				FirstVoterIndex = FirstVoterIndex.NextIndex(_playersInGame.Count);
			}
			
			_newVoterAssigned.Invoke();
			VotingContext.MinimumBet = 0;
		}

		public void Fold()
		{
			if (IsVoting == false)
				throw new Exception("Can't fold when voting is not started");
			
			Voter.Fold();
		}

		public void Call()
		{
			if (IsVoting == false)
				throw new Exception("Can't call when voting is not started");

			if (VotingContext.MinimumBet == 0)
				return;
			
			//TODO: Split pot
			var bet = Voter.MakeBet(VotingContext.MinimumBet);
			Pot += bet;
			_potChanged.Invoke();
		}

		public void Raise(int amount)
		{
			if (IsVoting == false)
				throw new Exception("Can't make bet when voting is not started");
			
			if (amount <= 0)
				throw new Exception("Can't raise amount is less or equal 0");
			
			VotingContext.MinimumBet += amount;
			var bet = Voter.MakeBet(VotingContext.MinimumBet);
			Pot += bet;
			_potChanged.Invoke();
		}

		public void ResetVotingCycle()
		{
			if (IsVoting == false)
				throw new Exception("Cannot reset voting cycle if there is no voting happening");

			VoteEndIndex = VoterIndex;
		}

		public void EndVotingCycle()
		{
			IsVoting = false;
			_newVoterAssigned.Invoke();

			foreach (PlayerState player in _playersInGame)
			{
				player.ResetBetState();
			}
		}

		public void AssignNextVoter()
		{
			if (IsVoting == false)
				throw new Exception("Can't assign next voter when voting is there");

			var current = VoterIndex;
			for (var i = 0; i < _playersInGame.Count; i++)
			{
				VoterIndex = VoterIndex.NextIndex(_playersInGame.Count);
				if (VoterIndex == current)
				{
					VoteEndIndex = VoterIndex;
					break;
				}
				
				if (Voter.Folded == false && Voter.IsAllIn == false)
					break;
			}
			
			_newVoterAssigned.Invoke();
		}

		public void RevealNextCard()
		{
			CardsRevealed++;
			_newCardRevealed.Invoke();
		}

		private void DealTableCards()
		{
			for (var i = 0; i < _cards.Length; i++)
			{
				_cards[i] = _deck.PopRandomEntry();
			}

			CardsRevealed = 0;
		}
		
		private void DealCardsToPlayers()
		{
			foreach (PlayerState player in _playersInGame)
			{
				player.GiveCards(_deck.PopRandomEntry(), _deck.PopRandomEntry());
			}
		}

		public void UpdateListOfPlayerInGame()
		{
			_playersInGame.RemoveAll(player => player.IsOutOfPlay);
		}

		public bool IsVotingEnded()
		{
			return VoteEndIndex == VoterIndex;
		}

		public bool IsAllCardsRevealed()
		{
			return CardsRevealed == _cards.Length;
		}
	}
}
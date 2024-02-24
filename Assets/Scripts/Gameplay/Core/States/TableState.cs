using System;
using System.Collections.Generic;
using System.Linq;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core.Models;
using Poker.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Poker.Gameplay.Core.States
{
	public class TableState
	{
		public IReadOnlyList<CardModel> Deck { get; } = new CardModel[]
		{
			new(CardType.Circle, 9),
			new(CardType.Flame, 9),
			new(CardType.Square, 9),
			new(CardType.Square, 9),
		};
		
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
		public int Pot { get; private set; }
		public int VoterIndex { get; private set; }
		
		private readonly List<PlayerState> _playersInGame = new();
		private readonly CardModel[] _cards = new CardModel[5];
		private readonly List<CardModel> _deck = new();

		private IndependentEvent _newCardRevealed;
		private IndependentEvent _newVoterAssigned;
		private int _firstVoterIndex = -1;

		public void AddPlayers(IEnumerable<PlayerState> players)
		{
			_playersInGame.AddRange(players);
		}

		public void StartRound()
		{
			_deck.Clear();
			_deck.AddRange(Deck);

			UpdateListOfPlayerInGame();
			DealTableCards();
			DealCardsToPlayers();
		}

		public void StartVotingCycle()
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
		}

		public void EndVotingCycle()
		{
			IsVoting = false;
			_newVoterAssigned.Invoke();
		}

		public void AssignNextVoter()
		{
			VoterIndex = VoterIndex.NextIndex(_playersInGame.Count);
			_newVoterAssigned.Invoke();
		}

		public void RevealNextCard()
		{
			CardsRevealed++;
			_newCardRevealed.Invoke();
		}

		private void DealTableCards()
		{
			//TODO: Remove dealt cards from deck
			for (var i = 0; i < _cards.Length; i++)
			{
				_cards[i] = _deck.RandomEntry();
			}

			CardsRevealed = 0;
		}
		
		private void DealCardsToPlayers()
		{
			foreach (PlayerState player in _playersInGame)
			{
				player.GiveCards(_deck.RandomEntry(), _deck.RandomEntry());
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
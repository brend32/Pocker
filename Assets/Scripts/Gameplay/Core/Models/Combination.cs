using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Gameplay.Core.Models
{
	public struct Combination : IEquatable<Combination>, IComparable<Combination>
	{
		// Use buffers to avoid allocations
		// Using thread static to prevent multi thread write 
		[ThreadStatic] private static int[] _buffer;
		[ThreadStatic] private static int[] _sameValueCards;
		[ThreadStatic] private static int[] _sameTypeCards;
		
		public string Name { get; private set; }
		public int Value { get; private set; }
		public int CombinationIndex { get; private set; }
		
		public Combination(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards) 
			: this(playerCards.Concat(tableCards).ToArray())
		{
			
		}
		
		public Combination(CardModel[] cards)
		{
			Name = string.Empty;
			Value = -1;
			CombinationIndex = -1;
			
			FormCombination(cards);
		}

		private void FormCombination(CardModel[] cards)
		{
			foreach (CardModel card in cards)
			{
				if (card.Value is < 2 or > 13)
					return;
			}
			
			_buffer ??= new int[6];
			_sameValueCards ??= new int[20];
			_sameTypeCards ??= new int[4];
			
			Array.Clear(_buffer, 0, _buffer.Length);
			Array.Clear(_sameValueCards, 0, _sameValueCards.Length);
			Array.Clear(_sameTypeCards, 0, _sameTypeCards.Length);
			Array.Sort(cards);

			var sameValueCards = _sameValueCards;
			var sameTypeCards = _sameTypeCards;
			var pairs = 0;
			var threes = 0;
			int? fourValue = null;
			CardType? flush = null;
			var straight = false;
			var streak = 1;
			var previous = 0;

			for (var i = 0; i < cards.Length; i++)
			{
				var card = cards[i];
				var sameValueCount = ++sameValueCards[card.Value];
				if (sameValueCount % 2 == 0)
					pairs++;
				if (sameValueCount % 3 == 0)
					threes++;
				if (sameValueCount >= 4)
					fourValue = card.Value;

				if (++sameTypeCards[(int)card.Type] >= 5)
					flush = card.Type;

				if (i == 0)
				{
					if (cards[^1].Value == 13 && card.Value == 2)
					{
						streak++;
					}
				}
				else if (card.Value - previous > 1)
				{
					if (streak >= 5)
					{
						straight = true;
					}

					streak = 1;
				}
				else if (card.Value - previous == 1)
				{
					streak++;
				}

				previous = card.Value;
			}

			if (streak >= 5)
			{
				straight = true;
			}

			if (flush != null && straight)
			{
				if (TryStraightFlush(cards, flush.Value, out var combinationValue))
				{
					Name = "Straight flush";
					Value = combinationValue;
					CombinationIndex = 9;
					return;
				}
			}

			if (fourValue != null)
			{
				Name = "Four of a kind";
				Value = PackFour(cards, fourValue.Value);
				CombinationIndex = 8;
				return;
			}
			
			if (pairs > 0 && threes > 0)
			{
				if (TryPackFullHouse(sameValueCards, out var combinationValue))
				{
					Name = "Full house";
					Value = combinationValue;
					CombinationIndex = 7;
					return;
				}
			}

			if (flush != null)
			{
				Name = "Flush";
				Value = PackFlush(cards, flush.Value);
				CombinationIndex = 6;
				return;
			}
			
			if (straight)
			{
				Name = "Straight";
				Value = PackStraight(cards);
				CombinationIndex = 5;
				return;
			}
			
			if (threes > 0)
			{
				Name = "Three of a kind";
				Value = PackThrees(cards, sameValueCards);
				CombinationIndex = 4;
				return;
			}
			
			if (pairs > 0)
			{
				// Pairs
				if (pairs == 1)
				{
					Name = "Pair";
					Value = PackOnePair(cards, sameValueCards);
					CombinationIndex = 2;
					return;
				}
				
				Name = "Two pairs";
				Value = PackTwoPairs(cards, sameValueCards);
				CombinationIndex = 3;
				return;
			}

			Name = "Highest card";
			Value = PackHighest(cards);
			CombinationIndex = 1;
		}
		
		private static bool TryStraightFlush(CardModel[] cards, CardType flush, out int combinationValue)
		{
			combinationValue = -1;
			var previous = 0;
			var streakStart = cards.Length - 1;
			var streak = 1;
			var hasAce = false;

			for (var i = cards.Length - 1; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Type != flush)
					continue;

				if (card.Value == 14)
					hasAce = true;

				if (streak >= 5)
					break;

				if (i == cards.Length - 1)
				{
					previous = card.Value;
					continue;
				}

				if (previous - card.Value != 1)
				{
					streak = 1;
					streakStart = i;
				}
				else
				{
					if (card.Value == 2 && hasAce)
					{
						streak++;
					}

					streak++;
				}

				previous = card.Value;
			}

			if (streak < 5)
				return false;

			var values = _buffer;
			values[0] = 9;
			for (int i = streakStart, ci = 0; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Type != flush)
					continue;

				if (ci >= 5)
					break;

				values[ci + 1] = card.Value;
				ci++;
			}

			combinationValue = Pack(values);
			return true;
		}
		
		private static int PackFour(CardModel[] cards, int fourValue)
		{
			var highestCardValue = -1;

			for (var i = cards.Length - 1; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Value == fourValue)
					continue;

				highestCardValue = card.Value;
				break;
			}

			var values = _buffer;
			values[0] = 8;
			values[1] = fourValue;
			values[2] = highestCardValue;

			return Pack(values);
		}
		
		private static bool TryPackFullHouse(int[] sameValueCount, out int combinationValue)
		{
			combinationValue = -1;
			var highestThrees = -1;
			var highestPair = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (highestThrees != -1 && highestPair != -1)
					break;

				var count = sameValueCount[i];
				if (highestThrees == -1 && count >= 3)
				{
					highestThrees = i;
				}
				else if (highestPair == -1 && count >= 2)
				{
					highestPair = i;
				}
			}

			if (highestThrees == -1 || highestPair == -1)
				return false;

			var values = _buffer;
			values[0] = 7;
			values[1] = highestThrees;
			values[2] = highestPair;

			combinationValue = Pack(values);
			return true;
		}
		
		private static int PackFlush(CardModel[] cards, CardType flush)
		{
			var values = _buffer;
			values[0] = 6;
			for (int i = cards.Length - 1, ci = 0; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Type != flush)
					continue;

				if (ci >= 5)
					break;

				values[ci + 1] = card.Value;
				ci++;
			}

			return Pack(values);
		}
		
		private static int PackStraight(CardModel[] cards)
		{
			var previous = 0;
			var streakStart = cards.Length - 1;
			var streak = 1;
			var hasAce = false;

			for (var i = cards.Length - 1; i >= 0; i--)
			{
				var card = cards[i];

				if (card.Value == 14)
					hasAce = true;

				if (streak >= 5)
					break;

				if (i == cards.Length - 1)
				{
					previous = card.Value;
					continue;
				}

				if (previous - card.Value > 1)
				{
					streak = 1;
					streakStart = i;
				}
				else if (previous - card.Value == 1)
				{
					if (card.Value == 2 && hasAce)
					{
						streak++;
					}

					streak++;
				}

				previous = card.Value;
			}

			var values = _buffer;
			values[0] = 5;
			values[1] = cards[streakStart].Value;

			return Pack(values);
		}
		
		private static int PackThrees(CardModel[] cards, int[] sameValueCount)
		{
			var highestThrees = -1;

			for (var i = 14; i >= 2; i--)
			{
				var count = sameValueCount[i];
				if (count >= 3)
				{
					highestThrees = i;
					break;
				}
			}

			var values = _buffer;
			values[0] = 4;
			values[1] = highestThrees;

			for (int i = cards.Length - 1, ci = 0; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Value == highestThrees)
					continue;

				if (ci >= 2)
					break;

				values[ci + 2] = card.Value;
				ci++;
			}

			return Pack(values);
		}
		
		private static int PackOnePair(CardModel[] cards, int[] sameValueCount)
		{
			var highestPair = -1;

			for (var i = 14; i >= 2; i--)
			{
				var count = sameValueCount[i];
				if (count >= 2)
				{
					highestPair = i;
					break;
				}
			}

			var values = _buffer;
			values[0] = 2;
			values[1] = highestPair;

			for (int i = cards.Length - 1, ci = 0; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Value == highestPair)
					continue;

				if (ci >= 3)
					break;

				values[ci + 2] = card.Value;
				ci++;
			}

			return Pack(values);
		}
		
		private static int PackTwoPairs(CardModel[] cards, int[] sameValueCount)
		{
			var highestPairFirst = -1;
			var highestPairSecond = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (highestPairFirst != -1 && highestPairSecond != -1)
					break;

				var count = sameValueCount[i];
				{
					if (count < 2)
						continue;

					if (highestPairFirst == -1)
					{
						highestPairFirst = i;
					}
					else
					{
						highestPairSecond = i;
					}
				}
			}

			var values = _buffer;
			values[0] = 3;
			values[1] = highestPairFirst;
			values[2] = highestPairSecond;

			for (int i = cards.Length - 1, ci = 0; i >= 0; i--)
			{
				var card = cards[i];
				if (card.Value == highestPairFirst || card.Value == highestPairSecond)
					continue;

				if (ci >= 1)
					break;

				values[ci + 3] = card.Value;
				ci++;
			}

			return Pack(values);
		}
		
		private static int PackHighest(CardModel[] cards)
		{
			var values = _buffer;
			values[0] = 1;

			for (int i = cards.Length - 1, ci = 0; i >= 0; i--)
			{
				var card = cards[i];

				if (ci >= 5)
					break;

				values[ci + 1] = card.Value;
				ci++;
			}

			return Pack(values);
		}
		
		private static int Pack(int[] values)
		{
			var n = 0;

			for (var i = 0; i < values.Length; i++)
			{
				n += values[i] * (int)Math.Pow(14, values.Length - i - 1);
			}

			return n;
		}

		public int CompareTo(Combination other)
		{
			return Value.CompareTo(other.Value);
		}

		public static bool operator <(Combination left, Combination right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(Combination left, Combination right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(Combination left, Combination right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >=(Combination left, Combination right)
		{
			return left.CompareTo(right) >= 0;
		}

		public bool Equals(Combination other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is Combination other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}

		public static bool operator ==(Combination left, Combination right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Combination left, Combination right)
		{
			return !left.Equals(right);
		}
	}
}
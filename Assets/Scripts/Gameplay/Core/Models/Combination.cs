using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Gameplay.Core.Models
{
	public struct Combination : IEquatable<Combination>, IComparable<Combination>
	{
		public string Name { get; private set; }
		public int Value { get; private set; }
		
		public Combination(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards)
		{
			Name = string.Empty;
			Value = -1;
			
			FormCombination(playerCards, tableCards);
		}

		private void FormCombination(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards)
		{
			var combined = playerCards.Concat(tableCards).ToArray();

			foreach (CardModel card in combined)
			{
				if (card.Value is < 2 or > 14)
					return;
			}
			
			Array.Sort(combined);

			var sameValueCards = Enumerable.Range(2, 15).ToDictionary(i => i, i => 0);
			var sameTypeCards =
				new[] { CardType.Circle, CardType.Flame, CardType.Square, CardType.Triangle }.ToDictionary(i => i,
					i => 0);
			var pairs = 0;
			var threes = 0;
			int? fourValue = null;
			CardType? flush = null;
			var straight = false;
			var streak = 1;
			var previous = 0;

			for (var i = 0; i < combined.Length; i++)
			{
				var card = combined[i];
				var sameValueCount = ++sameValueCards[card.Value];
				if (sameValueCount % 2 == 0)
					pairs++;
				if (sameValueCount % 3 == 0)
					threes++;
				if (sameValueCount >= 4)
					fourValue = card.Value;

				if (++sameTypeCards[card.Type] >= 5)
					flush = card.Type;

				if (i == 0)
				{
					if (combined[^1].Value == 14 && card.Value == 2)
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
				if (TryStraightFlush(combined, flush.Value, out var combinationValue))
				{
					Name = "Straight flush";
					Value = combinationValue;
					return;
				}
			}

			if (fourValue != null)
			{
				Name = "Four of a kind";
				Value = PackFour(combined, fourValue.Value);
				return;
			}
			
			if (pairs > 0 && threes > 0)
			{
				if (TryPackFullHouse(sameValueCards, out var combinationValue))
				{
					Name = "Full house";
					Value = combinationValue;
					return;
				}
			}

			if (flush != null)
			{
				Name = "Flush";
				Value = PackFlush(combined, flush.Value);
				return;
			}
			
			if (straight)
			{
				Name = "Straight";
				Value = PackStraight(combined);
				return;
			}
			
			if (threes > 0)
			{
				Name = "Three of a kind";
				Value = PackThrees(combined, sameValueCards);
				return;
			}
			
			if (pairs > 0)
			{
				// Pairs
				if (pairs == 1)
				{
					Name = "Pair";
					Value = PackOnePair(combined, sameValueCards);
					return;
				}
				
				Name = "Two pairs";
				Value = PackTwoPairs(combined, sameValueCards);
				return;
			}

			Name = "Highest card";
			Value = PackHighest(combined);
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

			var values = new int[6];
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

			var values = new int[6];
			values[0] = 8;
			values[1] = fourValue;
			values[2] = highestCardValue;

			return Pack(values);
		}
		
		private static bool TryPackFullHouse(Dictionary<int, int> sameValueCount, out int combinationValue)
		{
			combinationValue = -1;
			var highestThrees = -1;
			var highestPair = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (highestThrees != -1 && highestPair != -1)
					break;

				if (sameValueCount.TryGetValue(i, out var count))
				{
					if (highestThrees == -1 && count >= 3)
					{
						highestThrees = i;
					}
					else if (highestPair == -1 && count >= 2)
					{
						highestPair = i;
					}
				}
			}

			if (highestThrees == -1 || highestPair == -1)
				return false;

			var values = new int[6];
			values[0] = 7;
			values[1] = highestThrees;
			values[2] = highestPair;

			combinationValue = Pack(values);
			return true;
		}
		
		private static int PackFlush(CardModel[] cards, CardType flush)
		{
			var values = new int[6];
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

			var values = new int[6];
			values[0] = 5;
			values[1] = cards[streakStart].Value;

			return Pack(values);
		}
		
		private static int PackThrees(CardModel[] cards, Dictionary<int, int> sameValueCount)
		{
			var highestThrees = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (sameValueCount.TryGetValue(i, out var count))
				{
					if (count >= 3)
					{
						highestThrees = i;
						break;
					}
				}
			}

			var values = new int[6];
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
		
		private static int PackOnePair(CardModel[] cards, Dictionary<int, int> sameValueCount)
		{
			var highestPair = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (sameValueCount.TryGetValue(i, out var count))
				{
					if (count >= 2)
					{
						highestPair = i;
						break;
					}
				}
			}

			var values = new int[6];
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
		
		private static int PackTwoPairs(CardModel[] cards, Dictionary<int, int> sameValueCount)
		{
			var highestPairFirst = -1;
			var highestPairSecond = -1;

			for (var i = 14; i >= 2; i--)
			{
				if (highestPairFirst != -1 && highestPairSecond != -1)
					break;

				if (sameValueCount.TryGetValue(i, out var count))
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

			var values = new int[6];
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
			var values = new int[6];
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
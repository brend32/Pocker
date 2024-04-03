using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Gameplay.Core.Models
{
	public enum CardType
	{
		Hills,
		Oil,
		Balloon,
		Flame
	}
	
	public readonly struct CardModel : IEquatable<CardModel>, IComparable<CardModel>
	{
		public static CardModel[] Deck { get; } = Enumerable.Range(2, 12)
			.SelectMany(value => new CardModel[]
			{
				new(CardType.Balloon, value),
				new(CardType.Flame, value),
				new(CardType.Oil, value),
				new(CardType.Hills, value),
			}).ToArray();
		
		public readonly CardType Type;
		public readonly int Value;

		public CardModel(CardType type, int value)
		{
			Type = type;
			Value = value;
		}
		
		public bool Equals(CardModel other)
		{
			return Type == other.Type && Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is CardModel other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)Type, Value);
		}

		public static bool operator ==(CardModel left, CardModel right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CardModel left, CardModel right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return $"{nameof(Type)}: {Type}, {nameof(Value)}: {Value}";
		}

		public int CompareTo(CardModel other)
		{
			return Value.CompareTo(other.Value);
		}

		public static bool operator <(CardModel left, CardModel right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(CardModel left, CardModel right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(CardModel left, CardModel right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >=(CardModel left, CardModel right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static explicit operator int(CardModel card) => card.Value * 10 + (int)card.Type;
	}
}
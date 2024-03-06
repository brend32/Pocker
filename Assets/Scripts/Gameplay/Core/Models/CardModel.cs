using System;

namespace Poker.Gameplay.Core.Models
{
	public enum CardType
	{
		Triangle,
		Square,
		Circle,
		Flame
	}
	
	public readonly struct CardModel : IEquatable<CardModel>, IComparable<CardModel>
	{
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
	}
}
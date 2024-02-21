namespace Poker.Gameplay.Core.Models
{
	public enum CardColor
	{
		Red,
		Gray
	}
	
	public class CardModel
	{
		public CardColor Color { get; set; }
		public int Value { get; set; }
	}
}
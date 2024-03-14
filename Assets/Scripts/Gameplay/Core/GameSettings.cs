namespace Poker.Gameplay.Core
{
	public enum Difficulty
	{
		Easy,
		Normal,
		Hard,
		Insane
	}
	
	public class GameSettings
	{
		public int PlayersCount { get; set; }
		public int StartingCash { get; set; }
		public Difficulty Difficulty { get; set; }
	}
}
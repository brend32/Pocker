using AurumGames.Animation;

namespace Poker.Gameplay.Core
{
	public class GameTimeSource : ITimeSource
	{
		public float Time => _gameManager.State?.PlayTime ?? 0;
		
		private readonly GameManager _gameManager;

		public GameTimeSource(GameManager gameManager)
		{
			_gameManager = gameManager;
		}
	}
}
namespace Poker.Utils
{
	public static class IntUtils
	{
		public static int NextIndex(this int current, int count)
		{
			return ++current % count;
		}
	}
}
namespace AurumGames.Animation
{
	public interface ITimeSource
	{
		float Time { get; }
	}
	
	public static class TimeSources
	{
		public static readonly ITimeSource Scaled = new ScaledTimeSource();
		public static readonly ITimeSource Unscaled = new UnscaledTimeSource();
	}

	internal class ScaledTimeSource : ITimeSource
	{
		public float Time => UnityEngine.Time.time;
	}

	internal class UnscaledTimeSource : ITimeSource
	{
		public float Time => UnityEngine.Time.unscaledTime;
	}
}
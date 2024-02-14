namespace AurumGames.Animation
{
	internal static class Utils
	{
		public static float MaxTrackDuration(this ITrack[] tracks)
		{
			float max = 0;
			foreach (ITrack track in tracks)
			{
				var duration = track.FullDuration;
				if (duration > max)
					max = duration;
			}

			return max;
		}
	}
}
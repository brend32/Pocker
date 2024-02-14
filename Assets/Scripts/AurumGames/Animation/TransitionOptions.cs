namespace AurumGames.Animation
{
	public readonly ref struct TransitionOptions
	{
		public readonly bool? IgnoreDelay;
		public readonly Transition Transition;
		public readonly float? Delay;

		public TransitionOptions(bool? ignoreDelay = null, Transition transition = null, float? delay = null)
		{
			IgnoreDelay = ignoreDelay;
			Transition = transition;
			Delay = delay;
		}

		public static implicit operator TransitionOptions(Transition transition) => new(transition: transition);
	}
}
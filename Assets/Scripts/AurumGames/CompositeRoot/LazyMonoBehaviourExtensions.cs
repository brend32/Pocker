using UnityEngine;

namespace AurumGames.CompositeRoot
{
	public static class LazyMonoBehaviourExtensions
	{
		public static T AddComponentAndResolve<T>(this GameObject gameObject, Context context = null) where T : LazyMonoBehaviour
		{
			var component = gameObject.AddComponent<T>();
			component.InstantInit(context);

			return component;
		}
	}
}
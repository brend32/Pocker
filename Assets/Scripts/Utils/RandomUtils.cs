using System.Collections.Generic;
using UnityEngine;

namespace Poker.Utils
{
	public static class RandomUtils
	{
		public static T RandomEntry<T>(this IList<T> array)
		{
			return array[Random.Range(0, array.Count)];
		}
		
		public static T RandomEntry<T>(this T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}
	}
}
using System;
using System.Collections.Generic;
using System.Threading;

namespace Poker.Utils
{
	public static class RandomUtils
	{
		public static Random Random => _random ??= new Random(Environment.TickCount + Thread.CurrentThread.ManagedThreadId);
		[ThreadStatic] private static Random _random;
		
		public static T RandomEntry<T>(this IList<T> array)
		{
			return array[Random.Next(0, array.Count)];
		}
		
		public static T PopRandomEntry<T>(this IList<T> list)
		{
			var index = Random.Next(0, list.Count);
			T element = list[index];
			list.RemoveAt(index);
			return element;
		}
		
		public static T RandomEntry<T>(this T[] array)
		{
			return array[Random.Next(0, array.Length)];
		}
	}
}
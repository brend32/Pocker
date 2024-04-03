using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Poker.Gameplay.Core.Models;
using UnityEditor;
using UnityEngine;

namespace Poker.Gameplay.Core.BotLogic
{
	public static class CombinationOdds
	{
		public static int HighestTwoCardRank { get; private set; }
		
		private static bool _loaded;
		private static Dictionary<(int, int, int), Dictionary<int, int>> _higherCombinationMap;
		private static Dictionary<(int, int), int> _twoCardsRankMap;
		// Using thread static to prevent multi thread write 
		[ThreadStatic] private static int[] _higherCombinationIndexesBuffer;
		[ThreadStatic] private static int[] _higherCombinationBuffer;

		/// <summary>
		/// Load or create maps
		/// </summary>
		public static void Prepare()
		{
			if (_loaded == false)
				Load();
		}
		
		/// <summary>
		/// Gets 2 cards rank
		/// </summary>
		/// <param name="cards">Cards</param>
		/// <returns>Rank</returns>
		public static int GetPairRank(IReadOnlyList<CardModel> cards)
		{
			var card1 = (int)cards[0];
			var card2 = (int)cards[1];

			return _twoCardsRankMap[(Math.Max(card1, card2), Math.Min(card1, card2))];
		}

		/// <summary>
		/// Gets 2 cards rank
		/// </summary>
		/// <param name="cards">Cards</param>
		/// <returns>Rank</returns>
		public static int GetPairRank(CardModel[] cards)
		{
			var card1 = (int)cards[0];
			var card2 = (int)cards[1];

			return _twoCardsRankMap[(Math.Max(card1, card2), Math.Min(card1, card2))];
		}

		/// <summary>
		/// Finding possibility to get higher combination
		/// </summary>
		/// <param name="cards">Available cards</param>
		/// <param name="compareCombination">Combination to compare</param>
		/// <returns>Chance</returns>
		public static double GetHigherCombinationsPercent(CardModel[] cards, int compareCombination)
		{
			double overall = 0;
			double higher = 0;

			_higherCombinationIndexesBuffer ??= new int[3];
			_higherCombinationBuffer ??= new int[3];

			foreach (var length in CombinationsLoop(_higherCombinationIndexesBuffer, cards.Length))
			{
				for (int i = 0; i < length; i++)
				{
					_higherCombinationBuffer[i] = (int)cards[_higherCombinationIndexesBuffer[i]];
				}

				Array.Sort(_higherCombinationBuffer);
				var key = (_higherCombinationBuffer[2], _higherCombinationBuffer[1], _higherCombinationBuffer[0]);
				var combinations = _higherCombinationMap[key];

				foreach (var pair in combinations)
				{
					overall += pair.Value;

					if (pair.Key > compareCombination)
					{
						higher += pair.Value;
					}
				}
			}

			return higher / overall;
		}

		/// <summary>
		/// Computationally expensive function
		/// Better load computed data 
			
		/// Building maps
		/// Higher combination map - map of possible combinations that includes 3 given cards
		/// Two cards rank map - cards pair ranking
		/// </summary>
		public static void ConstructData()
		{
			// Created buffers to avoiding allocations
			var cardsBuffer = new CardModel[5];
			var numberBuffer = new int[3];
			var mainLoopIndexesBuffer = new int[5];
			var innerLoopIndexesBuffer = new int[3];
			var deck = CardModel.Deck;

			var higherCombinationMap = new Dictionary<(int, int, int), Dictionary<int, int>>(20000);
			var allCombinationsSet = new HashSet<int>();
			var twoCardsRankMapTemp = new Dictionary<(int, int), Dictionary<int, int>>(20000);
			var twoCardsRankMap = new Dictionary<(int, int), int>(20000);

			// Explores unique 5 cards combination
			foreach (var mainLoopLength in CombinationsLoop(mainLoopIndexesBuffer, deck.Length))
			{
				for (int i = 0; i < mainLoopLength; i++)
				{
					cardsBuffer[i] = deck[mainLoopIndexesBuffer[i]];
				}

				var combination = new Combination(cardsBuffer);
				allCombinationsSet.Add(combination.Value);

				// Explores all 3 cards combinations
				foreach (var innerLoopLength in CombinationsLoop(innerLoopIndexesBuffer, cardsBuffer.Length))
				{
					for (int i = 0; i < innerLoopLength; i++)
					{
						numberBuffer[i] = (int)cardsBuffer[innerLoopIndexesBuffer[i]];
					}

					Array.Sort(numberBuffer);
					var key = (numberBuffer[2], numberBuffer[1], numberBuffer[0]);
		
					if (higherCombinationMap.ContainsKey(key) == false)
						higherCombinationMap.Add(key, new Dictionary<int, int>(8));
					var temp = higherCombinationMap[key];
					temp.TryAdd(combination.Value, 0);
					temp[combination.Value]++;
				}
	
				// Explores all 2 cards combinations
				foreach (var innerLoopLength in CombinationsLoop(innerLoopIndexesBuffer, cardsBuffer.Length, 2))
				{
					for (int i = 0; i < innerLoopLength; i++)
					{
						numberBuffer[i] = (int)cardsBuffer[innerLoopIndexesBuffer[i]];
					}

					Array.Sort(numberBuffer);
					var key = (numberBuffer[1], numberBuffer[0]);
		
					if (twoCardsRankMapTemp.ContainsKey(key) == false)
						twoCardsRankMapTemp.Add(key, new Dictionary<int, int>(8));
		
					var temp = twoCardsRankMapTemp[key];
					temp.TryAdd(combination.Value, 0);
					temp[combination.Value]++;
				}
			}

			var allCombinations = allCombinationsSet.ToArray();
			Array.Sort(allCombinations);
			foreach (var pair in twoCardsRankMapTemp)
			{
				twoCardsRankMap.Add(pair.Key, 0);
				foreach (var combination in pair.Value)
				{
					var value = Array.IndexOf(allCombinations, combination.Key);
					twoCardsRankMap[pair.Key] += value * combination.Value;
				}
			}

			var twoPairRank = twoCardsRankMap.Values.Distinct().OrderBy(v => v).ToList();
			twoCardsRankMap = twoCardsRankMap.ToDictionary(p => p.Key, p => twoPairRank.IndexOf(p.Value));

			_higherCombinationMap = higherCombinationMap;
			_twoCardsRankMap = twoCardsRankMap;
			HighestTwoCardRank = _twoCardsRankMap.Values.Max();

			_loaded = true;
		}

		/// <summary>
		/// Writing maps to file
		/// </summary>
		public static void Save(string path)
		{
			if (_loaded == false)
				ConstructData();
			
			using var file = File.Create(path);
			using var binaryWriter = new BinaryWriter(file);

			binaryWriter.Write(_higherCombinationMap.Count);
			foreach (var pair in _higherCombinationMap)
			{
				var (k1, k2, k3) = pair.Key;
				binaryWriter.Write((byte)k1);
				binaryWriter.Write((byte)k2);
				binaryWriter.Write((byte)k3);
				binaryWriter.Write(pair.Value.Count);
				foreach (var combination in pair.Value)
				{
					binaryWriter.Write(combination.Key);
					binaryWriter.Write((byte)combination.Value);
				}

				binaryWriter.Flush();
			}
			
			binaryWriter.Write(_twoCardsRankMap.Count);
			foreach (var pair in _twoCardsRankMap)
			{
				var (k1, k2) = pair.Key;
				binaryWriter.Write((byte)k1);
				binaryWriter.Write((byte)k2);
				binaryWriter.Write((char)pair.Value);
				binaryWriter.Flush();
			}
		}

		/// <summary>
		/// Reading computed data from file
		/// </summary>
		public static void Load()
		{
			var asset = Resources.Load<TextAsset>("combination_odds");
			using var stream = new MemoryStream(asset.bytes);
			using var binaryReader = new BinaryReader(stream);

			var count = binaryReader.ReadInt32();
			_higherCombinationMap = new Dictionary<(int, int, int), Dictionary<int, int>>(count);
			for (int i = 0; i < count; i++)
			{
				var k1 = binaryReader.ReadByte();
				var k2 = binaryReader.ReadByte();
				var k3 = binaryReader.ReadByte();
				var combinationsCount = binaryReader.ReadInt32();

				var key = (k1, k2, k3);
				var combinations = new Dictionary<int, int>(combinationsCount);
				for (int ii = 0; ii < combinationsCount; ii++)
				{
					var value = binaryReader.ReadInt32();
					var rarity = binaryReader.ReadByte();

					combinations.Add(value, rarity);
				}
		
				_higherCombinationMap.Add(key, combinations);
			}
	
			count = binaryReader.ReadInt32();
			_twoCardsRankMap = new Dictionary<(int, int), int>(count);
			for (int i = 0; i < count; i++)
			{
				var k1 = binaryReader.ReadByte();
				var k2 = binaryReader.ReadByte();
				var value = (int)binaryReader.ReadChar();

				var key = (k1, k2);
		
				_twoCardsRankMap.Add(key, value);
			}

			HighestTwoCardRank = _twoCardsRankMap.Values.Max();

			_loaded = true;
		}

		/// <summary>
		/// Explores all combinations
		/// </summary>
		/// <param name="indexesBuffer">Indexes buffer</param>
		/// <param name="elementsAmount">Number elements in exploring collection</param>
		/// <param name="length">How many indexes should be used in buffer</param>
		/// <returns>Used indexes length</returns>
		public static IEnumerable<int> CombinationsLoop(int[] indexesBuffer, int elementsAmount, int length = -1)
		{
			length = length == -1 ? indexesBuffer.Length : length;
			
			for (int i = 0; i < length; i++)
			{
				indexesBuffer[i] = i;
			}

			yield return length;
			
			while (true)
			{
				for (int i = length - 1; i >= 0; i--)
				{
					indexesBuffer[i]++;

					if (indexesBuffer[i] < elementsAmount - (length - i - 1))
					{
						for (int ii = i + 1; ii < length; ii++)
						{
							indexesBuffer[ii] = indexesBuffer[ii - 1] + 1;
						}
						break;
					}
		
					if (i == 0)
					{
						yield break;
					}
				}

				yield return length;
			}
		}
	}
}
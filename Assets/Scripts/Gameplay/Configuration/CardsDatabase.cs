using System;
using Poker.Gameplay.Core.Models;
using UnityEngine;

namespace Poker.Gameplay.Configuration
{
	[CreateAssetMenu(menuName = "Game/Cards database", fileName = "Cards database")]
	public class CardsDatabase : ScriptableObject
	{
		public Texture2D[] Triangle;
		public Texture2D[] Square;
		public Texture2D[] Circle;
		public Texture2D[] Flame;

		public Texture2D GetTexture(CardModel model)
		{
			var array = model.Type switch
			{
				CardType.Triangle => Triangle,
				CardType.Square => Square,
				CardType.Circle => Circle,
				_ => Flame
			};

			return array[model.Value];
		} 
	}
}
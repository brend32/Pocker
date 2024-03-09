using System;
using Poker.Gameplay.Core.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Poker.Gameplay.Configuration
{
	[CreateAssetMenu(menuName = "Game/Cards database", fileName = "Cards database")]
	public class CardsDatabase : ScriptableObject
	{
		public Texture2D[] Hills;
		public Texture2D[] Oil;
		public Texture2D[] Balloon;
		public Texture2D[] Flame;

		public Texture2D GetTexture(CardModel model)
		{
			var array = model.Type switch
			{
				CardType.Hills => Hills,
				CardType.Oil => Oil,
				CardType.Balloon => Balloon,
				_ => Flame
			};

			return array[model.Value];
		}
	}
}
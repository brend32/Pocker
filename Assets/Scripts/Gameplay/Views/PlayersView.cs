using System;
using System.Linq;
using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class PlayersView : LazyMonoBehaviour
	{
		[SerializeField] private PlayerView[] _others;
		[SerializeField] private PlayerView _me;
		
		[Dependency] private GameManager _gameManager;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			
		}

		public void Bind()
		{
			PlayerState me = _gameManager.State.Me;
			_me.BindTo(me);
			var players = _gameManager.State.Players.Where(player => player != me).ToArray();

			if (players.Length > _others.Length)
				throw new Exception($"Not enough slots; Need: {players.Length}, Has: {_others.Length}");
			
			for (int i = 0; i < _others.Length; i++)
			{
				PlayerView view = _others[i];
				if (i < players.Length)
				{
					view.BindTo(players[i]);
				}
				else
				{
					view.Hide();
				}
			}
		}
	}
}
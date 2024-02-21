using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class PlayerView : LazyMonoBehaviour
	{
		[SerializeField] private TextMeshPro _balance;
		[SerializeField] private TextMeshPro _name;
		
		[Dependency] private GameManager _gameManager;

		private bool IsMe => _gameManager.State.Me == _player;
		
		private PlayerState _player;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			
		}

		public void BindTo(PlayerState player)
		{
			_player = player;
			_player.DataChanged += DataChanged;
			gameObject.SetActive(true);
			
			DataChanged();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		private void DataChanged()
		{
			_name.text = _player.Name;
			_balance.text = $"${_player.Balance}";
		}
	}
}
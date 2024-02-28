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
		[SerializeField] private CardView _card1;
		[SerializeField] private CardView _card2;
		[SerializeField] private TextMeshPro _bet;
		
		[Dependency] private GameManager _gameManager;

		private bool IsMe => _gameManager.State.Me == _player;
		private TableState TableState => _gameManager.State.Table;
		
		private PlayerState _player;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			TableState.NewVoterAssigned += NewVoterAssigned;
		}

		private void NewVoterAssigned()
		{
			DataChanged();
		}

		public void BindTo(PlayerState player)
		{
			_player = player;
			_player.DataChanged += DataChanged;
			gameObject.SetActive(true);
			
			_card1.Revealed = IsMe;
			_card2.Revealed = IsMe;
			
			DataChanged();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		private void DataChanged()
		{
			if (_player.IsOutOfPlay)
			{
				_name.text = "Out";
			}
			else
			{
				_name.text = _player.Name;
			}
			_balance.text = $"${_player.Balance}";
			_card1.Bind(_player.Cards[0]);
			_card2.Bind(_player.Cards[1]);

			_bet.text = $"Bet:\n{_player.Bet}$";

			if (TableState.IsVoting && TableState.Voter == _player)
			{
				_name.color = Color.green;
			}
			else if (_player.Folded || _player.IsOutOfPlay)
			{
				_name.color = Color.red;
			}
			else
			{
				_name.color = Color.white;
			}
		}
	}
}
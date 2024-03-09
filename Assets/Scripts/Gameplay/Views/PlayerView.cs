using System.Linq;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class PlayerView : LazyMonoBehaviour
	{
		public PlayerState PlayerState => _player;
		
		[SerializeField] private TextMeshPro _balance;
		[SerializeField] private TextMeshPro _name;
		[SerializeField] private CardView _card1;
		[SerializeField] private CardView _card2;
		[SerializeField] private TextMeshProUGUI _combination;
		[SerializeField] private BetView _bet;
		
		[Dependency] private GameManager _gameManager;

		private bool IsMe => _gameManager.State.Me == _player;
		private TableState TableState => _gameManager.State.Table;
		
		private PlayerState _player;
		private bool _revealCards;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			RoundController roundController = _gameManager.Controller.Round;
			
			TableState.NewVoterAssigned += NewVoterAssigned;
			roundController.RoundEnded += RoundEnded;
			//roundController.RevealCards += RevealCards;
			TableState.NewCardRevealed += NewCardRevealed;
		}

		private void RevealCards()
		{
			_revealCards = true;
			DataChanged();
		}

		private void NewCardRevealed()
		{
			DataChanged();
		}

		private void RoundEnded()
		{
			_revealCards = false;
			DataChanged();
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
			
			DataChanged();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public async UniTask RevealCardsRoundEndAnimation()
		{
			_revealCards = true;
			_card1.RevealCardAnimation().Forget();
			await UniTask.Delay(100);
			DataChanged();
			await _card2.RevealCardAnimation();
		}

		private void DataChanged()
		{
			var shouldShow = IsMe || _revealCards;
			_card1.Revealed = shouldShow;
			_card2.Revealed = shouldShow;

			if (shouldShow && TableState.CardsRevealed > 0)
			{
				_combination.text = new Combination(_player.Cards, TableState.Cards.Take(TableState.CardsRevealed)).Name;
			}
			else
			{
				_combination.text = string.Empty;
			}
			
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

			if (_player.Bet == 0 || TableState.IsVoting == false)
			{
				_bet.Hide();
			}
			else
			{
				_bet.SetBet(_player.Bet);
				_bet.Show();
			}

			if (TableState.Winner == _player)
			{
				_name.color = Color.yellow;
			}
			else if (TableState.IsVoting && TableState.Voter == _player)
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
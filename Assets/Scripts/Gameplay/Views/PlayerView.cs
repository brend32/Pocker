using System.Linq;
using AurumGames.Animation;
using AurumGames.Animation.Tracks;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Poker.Gameplay.Views
{
	public partial class PlayerView : LazyMonoBehaviour
	{
		public PlayerState PlayerState => _player;
		
		[SerializeField] private BalanceView _balance;
		[SerializeField] private TextMeshPro _name;
		[SerializeField] private CardView _card1;
		[SerializeField] private CardView _card2;
		[SerializeField] private StatusView Status;
		[SerializeField] private BetView _bet;
		[SerializeField] private FoldOutView _foldOut;
		
		[Dependency] private GameManager _gameManager;

		private bool IsMe => _gameManager.State.Me == _player;
		private TableState TableState => _gameManager.State.Table;
		
		private PlayerState _player;
		private bool _revealCards;
		private VotingResponse? _votingResponse;
		private StatedFluentAnimationPlayer<Color> _playerNameAnimation;
		
		protected override void InitInnerState()
		{
			var playerNameColorTrack = new FluentTextMeshProColorTrack(_name, new Transition(300, Easing.QuadOut));
			_playerNameAnimation = new StatedFluentAnimationPlayer<Color>(this, playerNameColorTrack);
			_playerNameAnimation.StateChanged += (previous, current, options) =>
			{
				playerNameColorTrack.Set(current, options);
			};
		}

		protected override void Initialized()
		{
			RoundController roundController = _gameManager.Controller.Round;
			
			TableState.NewVoterAssigned += NewVoterAssigned;
			TableState.NewCardRevealed += NewCardRevealed;
			roundController.RoundStarted += RoundStarted;
			roundController.Voting.VotingEnded += VotingEnded;

			_playerNameAnimation.TimeSource = _gameManager.TimeSource;
		}

		private void NewCardRevealed()
		{
			_votingResponse = null;
			DataChanged();
		}

		private void VotingEnded()
		{
			_votingResponse = null;
			DataChanged();
		}

		private void RoundStarted()
		{
			_revealCards = false;
			_votingResponse = null;
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
			
			_card1.Revealed = IsMe;
			_card2.Revealed = IsMe;
			
			DataChanged();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public async UniTask MakeChoiceAnimation(VotingResponse response)
		{
			_votingResponse = response;
			DataChanged();
			await _gameManager.DelayAsync(500);
		}

		public async UniTask RevealCardsRoundEndAnimation()
		{
			_revealCards = true;
			DataChanged();
			await UniTask.WhenAll(
				_gameManager.DelayAsync(100).ContinueWith(_card1.RevealAnimation),
				_card2.RevealAnimation()
			);
		}
		
		public async UniTask HideCardsRoundEndAnimation()
		{
			_votingResponse = null;
			DataChanged();
			await UniTask.WhenAll(
				_card1.HideAnimation(),
				_card2.HideAnimation()
			);
		}

		public async UniTask DealCardsAnimation()
		{
			DataChanged();
			await UniTask.WhenAll(
				_card1.ShowAnimation(),
				_card2.ShowAnimation()
			);
		}

		private void DataChanged()
		{
			var shouldShow = IsMe || _revealCards;

			if (_votingResponse.HasValue)
			{
				Status.SetText(_votingResponse.Value.Action.ToString());
				Status.Show();
			}
			else if (shouldShow && TableState.CardsRevealed > 0 && TableState.RoundEnded == false)
			{
				Status.SetText(new Combination(_player.Cards, TableState.Cards.Take(TableState.CardsRevealed)).Name);
				Status.Show();
			}
			else
			{
				Status.Hide();
			}
			
			if (_player.IsOutOfPlay)
			{
				_name.text = "Out";
			}
			else
			{
				_name.text = _player.Name;
			}
			_balance.SetValue(_player.Balance);
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
				_playerNameAnimation.SetState(new Color(1f, 0.82f, 0.34f));
			}
			else if (TableState.IsVoting && TableState.Voter == _player)
			{
				_playerNameAnimation.SetState(new Color(0.15f, 1f, 0.39f));
			}
			else if (_player.Folded || _player.IsOutOfPlay)
			{
				_playerNameAnimation.SetState(Color.red);
				if (IsMe == false)
				{
					_card1.Hide();
					_card2.Hide();
				}

				if (_player.Folded)
				{
					_foldOut.Show();
				}
			}
			else
			{
				_playerNameAnimation.SetState(Color.white);
				_foldOut.Hide();
			}
		}
	}
}
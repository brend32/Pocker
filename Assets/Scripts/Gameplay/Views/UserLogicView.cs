using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.Models;
using Poker.Gameplay.Core.States;
using TMPro;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class UserLogicView : LazyMonoBehaviour
	{
		[SerializeField] private TMP_InputField _bet;
		[SerializeField] private CanvasGroup _canvasGroup;

		[Dependency] private GameManager _gameManager;
		private TableState Table => _gameManager.State.Table;
		private PlayerState _player;
		private UserLogic _logic;
		private bool _answered;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			_player = _gameManager.State.Me;

			if (_player?.Logic is UserLogic userLogic)
			{
				_logic = userLogic;
				Table.NewVoterAssigned += NewVoterAssigned;
				UpdateView();
			}
			else
			{
				Debug.LogError("No user found");
				gameObject.SetActive(false);
			}
		}

		private void NewVoterAssigned()
		{
			_answered = false;
			UpdateView();
		}

		private void UpdateView()
		{
			if (_gameManager.IsPlaying == false)
				return;
			
			var isOurTurn = _player == Table.Voter && _answered == false;

			_canvasGroup.blocksRaycasts = isOurTurn;
			_canvasGroup.alpha = isOurTurn ? 1 : 0.7f;
		}

		public void Call()
		{
			_answered = true;
			_logic.MakeChoice(VotingResponse.Call());
			UpdateView();
		}

		public void Fold()
		{
			_answered = true;
			_logic.MakeChoice(VotingResponse.Fold());
			UpdateView();
		}

		public void Raise()
		{
			_answered = true;
			_logic.MakeChoice(VotingResponse.Raise(int.Parse(_bet.text)));
			UpdateView();
		}
	}
}
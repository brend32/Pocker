using AurumGames.CompositeRoot;
using Poker.Gameplay.Core;
using Poker.Gameplay.Core.States;
using UnityEngine;

namespace Poker.Gameplay.Views
{
	public partial class TableCardsView : LazyMonoBehaviour
	{
		[SerializeField] private CardView[] _cardViews;

		[Dependency] private GameManager _gameManager;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			_gameManager.State.Table.NewCardRevealed += NewCardRevealed;
		}

		private void NewCardRevealed()
		{
			UpdateView();
		}

		[EasyButtons.Button]
		public void UpdateView()
		{
			TableState state = _gameManager.State.Table;
			
			for (var i = 0; i < _cardViews.Length; i++)
			{
				CardView cardView = _cardViews[i];
				cardView.Bind(state.Cards[i]);
				cardView.Revealed = i < state.CardsRevealed;
			}
		}
	}
}
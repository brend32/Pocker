using System;
using AurumGames.CompositeRoot;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Poker.Gameplay.Core
{
	public partial class GameTimeUpdater : LazyMonoBehaviour
	{
		[Dependency] private GameManager _gameManager;
		
		protected override void InitInnerState()
		{
			
		}

		protected override void Initialized()
		{
			
		}

		[EasyButtons.Button]
		public void Pause()
		{
			_gameManager.PauseGame();
		}

		[EasyButtons.Button]
		public void Continue()
		{
			_gameManager.ContinueGame();
		}

		private void Update()
		{
			if (_gameManager == null || IsInit == false || _gameManager.IsPlaying == false)
				return;

			_gameManager.DeltaTime = Time.deltaTime * _gameManager.TimeScale;
			_gameManager.State.PlayTime += _gameManager.DeltaTime;
		}
	}
}
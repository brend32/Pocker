using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AurumGames.Animation;
using AurumGames.CompositeRoot;
using AurumGames.SceneManagement;
using Cysharp.Threading.Tasks;
using Poker.Gameplay.Core.BotLogic;
using Poker.Gameplay.Core.States;
using Poker.Gameplay.Core.Statistics;
using Poker.Screens;
using Poker.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace Poker.Gameplay.Core
{
	/// <summary>
	/// Bots training runner
	/// </summary>
	public partial class BotGameRunner
    {
	    private readonly Context _scope;
	    private readonly int _maxGames = 1000;
	    private readonly Thread[] _threads = new Thread[6];

	    [Dependency]
        public BotGameRunner(Context scope)
        {
	        _scope = scope;
	        
#if UNITY_EDITOR
	        EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
	        foreach (Thread thread in _threads)
	        {
		        thread.Abort();
	        }

	        EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
        }
#endif

        public void StartBotGame(GameSettings gameSettings)
        {
	        // Can't call prepare from not main thread
	        CombinationOdds.Prepare();
	        
	        for (int i = 0; i < _threads.Length; i++)
	        {
		        var threadId = i;
		        _threads[i] = new Thread(() =>
		        {
			        // Setup up game environment
			        var free = false;
			        var game = 0;
			        var gameManager = GameManager.FastCreate(_scope);
			        var bots = new BotState[gameSettings.PlayersCount];
				        	        
			        gameManager.GameEnded += GameEnded;
			        for (int i = 0; i < bots.Length; i++)
			        {
				        bots[i] = BotState.CreateBotPlayer(gameSettings, $"Bot {i}");
			        }
			        
			        // Game time loop
			        while (game <= _maxGames)
			        {
				        gameManager.StartBotGame(gameSettings, bots).Wait();
				        while (free == false)
				        {
					        Thread.Sleep(1);
				        }

				        free = false;
			        }
			        
			        gameManager.GameEnded -= GameEnded;

			        foreach (BotState bot in bots)
			        {
				        Debug.Log(bot.ToString());
			        }


			        void GameEnded(PlayerState winner)
			        {
				        if (game > _maxGames)
				        {
					        return;
				        }

				        var botWinner = (BotState)winner;
				        botWinner.CountWin();
				        //Debug.Log($"({threadId}) {game} {botWinner}");

				        foreach (BotState bot in bots)
				        {
					        bot.Reset();
				        }

				        if (game % 100 == 0)
				        {
					        // Find best candidates 
					        var maxRounds = bots.Max(b => b.RoundsAlive);
					        var maxWins = bots.Max(b => b.Wins);
					        var bestRoundBot = bots.First(b => b.RoundsAlive == maxRounds);
					        var bestWinsBot = bots.First(b => b.Wins == maxWins);

					        Debug.Log($"({threadId}) Best round, {game}, {bestRoundBot}");
					        Debug.Log($"({threadId}) Best win, {game}, {bestWinsBot}");

					        foreach (BotState bot in bots)
					        {
						        bot.ResetGoals();

						        if (bot == bestRoundBot || bot == bestRoundBot)
						        {
							        if (RandomUtils.Random.NextDouble() < 0.04f)
								        bot.Settings.Mutate();
							        continue;
						        }

						        // Copy end mutate best ones 
						        if (RandomUtils.Random.NextDouble() < 0.85f)
							        bot.Settings.Copy(RandomUtils.Random.NextDouble() < 0.5 ? bestRoundBot.Settings : bestWinsBot.Settings);
						        else
							        bot.Settings.Mutate();
						        bot.Settings.Mutate();
					        }
				        }

				        game++;
				        free = true;
			        }
		        })
		        {
			        IsBackground = true
		        };
		        _threads[i].Start();
	        }
        }

        
    }
}
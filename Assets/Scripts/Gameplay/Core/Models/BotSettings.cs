using Poker.Utils;

namespace Poker.Gameplay.Core.Models
{
	public class BotSettings
	{
		public int StrongPairThreshold { get; set; } = 19;
		public double EnemyStrongerCombinationMiddleThreshold { get; set; } = 0.51371320977807;
		public double EnemyStrongerCombinationPanicThreshold { get; set; } = 0.405301503837109;
		public int TrustPairRankThreshold { get; set; } = 13;
		public double TrustGain { get; set; } = 0.340960390865803;
		public double TrustLoss { get; set; } = 0.523207994401455;
		public double EnemyTrustThreshold { get; set; } = 0.441805862672627;
		public double LowMoneyThreshold { get; set; } = 0.408863117620349;
		public int MinRoundsWithoutFool { get; set; } = 21;
		public double FoolChance { get; set; } = 0.432952526481822;
		public double SilentChance { get; set; } = 0.503569041788578;
		public double PanicThreshold { get; set; } = 0.551069484502077;
		public double BlindPanicThreshold { get; set; } = 0.4534722879529;
		public double MoneyPanicMultiplier { get; set; } = 0.506963741928339;
		public double PairRankPanicMultiplier { get; set; } = 0.519698183909059;
		public double EnemyStrongerCombinationPanicMultiplier { get; set; } = 0.571910247504711;
		public double FoolRaisePercent { get; set; } = 0.554264077916741;
		public double StrongPairRaisePercent { get; set; } = 0.460994556993246;
		public double RaiseOffset { get; set; } = 0.576030493974686;
		public double BetPanicMultiplier { get; set; } = 0.540366609245539;
		public double SkipTurnChance { get; set; } = 0.476075193881988;


		public void Copy(BotSettings bot)
		{
			// Copy parameters from another bot
			StrongPairThreshold = bot.StrongPairThreshold; 
			EnemyStrongerCombinationMiddleThreshold = bot.EnemyStrongerCombinationMiddleThreshold; 
			EnemyStrongerCombinationPanicThreshold = bot.EnemyStrongerCombinationPanicThreshold; 
			TrustPairRankThreshold = bot.TrustPairRankThreshold; 
			TrustGain = bot.TrustGain; 
			TrustLoss = bot.TrustLoss; 
			EnemyTrustThreshold = bot.EnemyTrustThreshold; 
			LowMoneyThreshold = bot.LowMoneyThreshold; 
			MinRoundsWithoutFool = bot.MinRoundsWithoutFool; 
			FoolChance = bot.FoolChance; 
			SilentChance = bot.SilentChance; 
			PanicThreshold = bot.PanicThreshold; 
			BlindPanicThreshold = bot.BlindPanicThreshold; 
			MoneyPanicMultiplier = bot.MoneyPanicMultiplier; 
			PairRankPanicMultiplier = bot.PairRankPanicMultiplier; 
			EnemyStrongerCombinationPanicMultiplier = bot.EnemyStrongerCombinationPanicMultiplier; 
			FoolRaisePercent = bot.FoolRaisePercent; 
			StrongPairRaisePercent = bot.StrongPairRaisePercent; 
			RaiseOffset = bot.RaiseOffset; 
			BetPanicMultiplier = bot.BetPanicMultiplier; 
			SkipTurnChance = bot.SkipTurnChance; 
		}
		
		public void Mutate()
		{
			// Mutating properties to get new behaviour
			for (int i = 0; i < 5; i++)
			{
				int n = RandomUtils.Random.Next(0, 30);
				if (RandomUtils.Random.NextDouble() < 0.25)
					continue;
				
				switch (n)
				{
					case 0:
						StrongPairThreshold += RandomUtils.Random.Next(-15, 15);
						break;
					case 1:
						EnemyStrongerCombinationMiddleThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 2:
						EnemyStrongerCombinationPanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 3:
						TrustPairRankThreshold += RandomUtils.Random.Next(-15, 15);
						break;
					case 4:
						TrustGain += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 5:
						TrustLoss += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 6:
						EnemyTrustThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 7:
						LowMoneyThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 8:
						MinRoundsWithoutFool += RandomUtils.Random.Next(-15, 15);
						break;
					case 9:
						FoolChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 10:
						SilentChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 11:
						PanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 12:
						BlindPanicThreshold += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 13:
						MoneyPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 14:
						PairRankPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 15:
						EnemyStrongerCombinationPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 16:
						FoolRaisePercent += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 17:
						StrongPairRaisePercent += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 18:
						RaiseOffset += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 19:
						BetPanicMultiplier += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					case 20:
						SkipTurnChance += (RandomUtils.Random.NextDouble() - 0.5) * 0.13;
						break;
					
					case 25:
						return;
				}
			}
		}
		
		public override string ToString()
		{
			return $"{nameof(StrongPairThreshold)}: {StrongPairThreshold}, {nameof(EnemyStrongerCombinationMiddleThreshold)}: {EnemyStrongerCombinationMiddleThreshold}, {nameof(EnemyStrongerCombinationPanicThreshold)}: {EnemyStrongerCombinationPanicThreshold}, {nameof(TrustPairRankThreshold)}: {TrustPairRankThreshold}, {nameof(TrustGain)}: {TrustGain}, {nameof(TrustLoss)}: {TrustLoss}, {nameof(EnemyTrustThreshold)}: {EnemyTrustThreshold}, {nameof(LowMoneyThreshold)}: {LowMoneyThreshold}, {nameof(MinRoundsWithoutFool)}: {MinRoundsWithoutFool}, {nameof(FoolChance)}: {FoolChance}, {nameof(SilentChance)}: {SilentChance}, {nameof(PanicThreshold)}: {PanicThreshold}, {nameof(BlindPanicThreshold)}: {BlindPanicThreshold}, {nameof(MoneyPanicMultiplier)}: {MoneyPanicMultiplier}, {nameof(PairRankPanicMultiplier)}: {PairRankPanicMultiplier}, {nameof(EnemyStrongerCombinationPanicMultiplier)}: {EnemyStrongerCombinationPanicMultiplier}, {nameof(FoolRaisePercent)}: {FoolRaisePercent}, {nameof(StrongPairRaisePercent)}: {StrongPairRaisePercent}, {nameof(RaiseOffset)}: {RaiseOffset}, {nameof(BetPanicMultiplier)}: {BetPanicMultiplier}, {nameof(SkipTurnChance)}: {SkipTurnChance}";
		}
	}
}
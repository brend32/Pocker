using System.Collections.Generic;

namespace Poker.Gameplay.Core.Models
{
	public static class BotPresets
	{
		public static Dictionary<Difficulty, float> CashMultiplier { get; } = new()
		{
			{ Difficulty.Easy, 1 },
			{ Difficulty.Normal, 1 },
			{ Difficulty.Hard, 1.5f },
			{ Difficulty.Insane, 2 },
		};
		public static Dictionary<Difficulty, BotSettings[]> Presets { get; } = new()
		{
			{
				Difficulty.Easy,
				new BotSettings[]
				{
					new()
					{
						StrongPairThreshold = 19,
						EnemyStrongerCombinationMiddleThreshold = 0.51371320977807,
						EnemyStrongerCombinationPanicThreshold = 0.405301503837109,
						TrustPairRankThreshold = 13, TrustGain = 0.340960390865803,
						TrustLoss = 0.523207994401455,
						EnemyTrustThreshold = 0.441805862672627,
						LowMoneyThreshold = 0.408863117620349,
						MinRoundsWithoutFool = 21,
						FoolChance = 0.432952526481822,
						SilentChance = 0.503569041788578,
						PanicThreshold = 0.551069484502077,
						BlindPanicThreshold = 0.4534722879529,
						MoneyPanicMultiplier = 0.506963741928339,
						PairRankPanicMultiplier = 0.519698183909059,
						EnemyStrongerCombinationPanicMultiplier = 0.571910247504711,
						FoolRaisePercent = 0.554264077916741,
						StrongPairRaisePercent = 0.460994556993246,
						RaiseOffset = 0.576030493974686,
						BetPanicMultiplier = 0.540366609245539,
						SkipTurnChance = 0.476075193881988
					},
					new()
					{
						StrongPairThreshold = 19,
						EnemyStrongerCombinationMiddleThreshold = 0.51371320977807,
						EnemyStrongerCombinationPanicThreshold = 0.405301503837109,
						TrustPairRankThreshold = 13,
						TrustGain = 0.340960390865803,
						TrustLoss = 0.523207994401455,
						EnemyTrustThreshold = 0.441805862672627,
						LowMoneyThreshold = 0.408863117620349,
						MinRoundsWithoutFool = 21,
						FoolChance = 0.432952526481822,
						SilentChance = 0.503569041788578,
						PanicThreshold = 0.551069484502077,
						BlindPanicThreshold = 0.4534722879529,
						MoneyPanicMultiplier = 0.506963741928339,
						PairRankPanicMultiplier = 0.519698183909059,
						EnemyStrongerCombinationPanicMultiplier = 0.571910247504711,
						FoolRaisePercent = 0.554264077916741,
						StrongPairRaisePercent = 0.460994556993246,
						RaiseOffset = 0.576030493974686,
						BetPanicMultiplier = 0.540366609245539,
						SkipTurnChance = 0.476075193881988
					},
					new()
					{
						StrongPairThreshold = 81,
						EnemyStrongerCombinationMiddleThreshold = 0.502830162588095,
						EnemyStrongerCombinationPanicThreshold = 0.356930892377454,
						TrustPairRankThreshold = 47,
						TrustGain = 0.119556513893696,
						TrustLoss = 0.451574563131838,
						EnemyTrustThreshold = 0.0968533345265536,
						LowMoneyThreshold = 0.117478356083676,
						MinRoundsWithoutFool = 27,
						FoolChance = 0.497453593790764,
						SilentChance = 0.59090139190775,
						PanicThreshold = 0.544292233212955,
						BlindPanicThreshold = 0.504530563881182,
						MoneyPanicMultiplier = 0.3230491587385,
						PairRankPanicMultiplier = 0.395368866680968,
						EnemyStrongerCombinationPanicMultiplier = 0.696076559412371,
						FoolRaisePercent = 0.697448616080537,
						StrongPairRaisePercent = 0.456679723798616,
						RaiseOffset = 0.450995572204686,
						BetPanicMultiplier = 0.73970893354823,
						SkipTurnChance = 0.605281938919427
					}
				}
			},
			{
				Difficulty.Normal,
				new BotSettings[]
				{
					new()
					{
						StrongPairThreshold = 40,
						EnemyStrongerCombinationMiddleThreshold = 0.599044023945616,
						EnemyStrongerCombinationPanicThreshold = 0.355965936133017,
						TrustPairRankThreshold = -2,
						TrustGain = 0.155043995831667,
						TrustLoss = 0.502556539405765,
						EnemyTrustThreshold = 0.381622406886805,
						LowMoneyThreshold = 0.150200112453353,
						MinRoundsWithoutFool = 32,
						FoolChance = 0.508315404872579,
						SilentChance = 0.551263412252392,
						PanicThreshold = 0.560436618055388,
						BlindPanicThreshold = 0.489047417224187,
						MoneyPanicMultiplier = 0.484549092586425,
						PairRankPanicMultiplier = 0.478291532552612,
						EnemyStrongerCombinationPanicMultiplier = 0.492879603300695,
						FoolRaisePercent = 0.663630023379514,
						StrongPairRaisePercent = 0.464461798292337,
						RaiseOffset = 0.590421174785304,
						BetPanicMultiplier = 0.679539012757211,
						SkipTurnChance = 0.716455382914459
					},
					new()
					{
						StrongPairThreshold = 39,
						EnemyStrongerCombinationMiddleThreshold = 0.599044023945616,
						EnemyStrongerCombinationPanicThreshold = 0.355965936133017,
						TrustPairRankThreshold = -2,
						TrustGain = 0.160572478617361,
						TrustLoss = 0.502556539405765,
						EnemyTrustThreshold = 0.361355980045418,
						LowMoneyThreshold = 0.150200112453353,
						MinRoundsWithoutFool = 32,
						FoolChance = 0.508315404872579,
						SilentChance = 0.551263412252392,
						PanicThreshold = 0.560436618055388,
						BlindPanicThreshold = 0.489047417224187,
						MoneyPanicMultiplier = 0.484549092586425,
						PairRankPanicMultiplier = 0.478291532552612,
						EnemyStrongerCombinationPanicMultiplier = 0.492879603300695,
						FoolRaisePercent = 0.663630023379514,
						StrongPairRaisePercent = 0.464461798292337,
						RaiseOffset = 0.590421174785304,
						BetPanicMultiplier = 0.679539012757211,
						SkipTurnChance = 0.716455382914459
					},
					new()
					{
						StrongPairThreshold = 36,
						EnemyStrongerCombinationMiddleThreshold = 0.401886457264504,
						EnemyStrongerCombinationPanicThreshold = 0.271587253695487,
						TrustPairRankThreshold = -25,
						TrustGain = 0.339993468019661,
						TrustLoss = 0.666910406960502,
						EnemyTrustThreshold = 0.381416834580997,
						LowMoneyThreshold = 0.378454018204748,
						MinRoundsWithoutFool = 8,
						FoolChance = 0.294877005611511,
						SilentChance = 0.370357162084797,
						PanicThreshold = 0.439051829545285,
						BlindPanicThreshold = 0.563166075844641,
						MoneyPanicMultiplier = 0.547038026033004,
						PairRankPanicMultiplier = 0.470675543630951,
						EnemyStrongerCombinationPanicMultiplier = 0.446755124609845,
						FoolRaisePercent = 0.560864619783405,
						StrongPairRaisePercent = 0.557216920173365,
						RaiseOffset = 0.416549927159455,
						BetPanicMultiplier = 0.488792664398638,
						SkipTurnChance = 0.562272334858401
					}
				}
			},
			{
				Difficulty.Hard,
				new BotSettings[]
				{
					new()
					{
						StrongPairThreshold = 26,
						EnemyStrongerCombinationMiddleThreshold = 0.575552890737,
						EnemyStrongerCombinationPanicThreshold = 0.355985928045439,
						TrustPairRankThreshold = -1,
						TrustGain = 0.305317502384706,
						TrustLoss = 0.56976083449626,
						EnemyTrustThreshold = 0.459719641168096,
						LowMoneyThreshold = 0.546047132987149,
						MinRoundsWithoutFool = 9,
						FoolChance = 0.410348366842324,
						SilentChance = 0.503569041788578,
						PanicThreshold = 0.529212984951745,
						BlindPanicThreshold = 0.534087617891196,
						MoneyPanicMultiplier = 0.568770499735515,
						PairRankPanicMultiplier = 0.508718826144012,
						EnemyStrongerCombinationPanicMultiplier = 0.543645684494048,
						FoolRaisePercent = 0.492317406960322,
						StrongPairRaisePercent = 0.444832890645526,
						RaiseOffset = 0.576030493974686,
						BetPanicMultiplier = 0.777472607450144,
						SkipTurnChance = 0.410296313729705
					},
					new()
					{
						StrongPairThreshold = 55,
						EnemyStrongerCombinationMiddleThreshold = 0.634392703872024,
						EnemyStrongerCombinationPanicThreshold = 0.356912804926565,
						TrustPairRankThreshold = 6,
						TrustGain = 0.153133082283741,
						TrustLoss = 0.480129855695144,
						EnemyTrustThreshold = 0.274740661297895,
						LowMoneyThreshold = 0.168504981568382,
						MinRoundsWithoutFool = 19,
						FoolChance = 0.582114651242346,
						SilentChance = 0.596834727408488,
						PanicThreshold = 0.448943933762086,
						BlindPanicThreshold = 0.460371734391386,
						MoneyPanicMultiplier = 0.36281343651975,
						PairRankPanicMultiplier = 0.379321717584796,
						EnemyStrongerCombinationPanicMultiplier = 0.577513248098829,
						FoolRaisePercent = 0.672339387700462,
						StrongPairRaisePercent = 0.454878229738159,
						RaiseOffset = 0.519814234238018,
						BetPanicMultiplier = 0.702990432373539,
						SkipTurnChance = 0.787696435177522
					},
					new()
					{
						StrongPairThreshold = 65,
						EnemyStrongerCombinationMiddleThreshold = 0.353153817171436,
						EnemyStrongerCombinationPanicThreshold = 0.296345866953975,
						TrustPairRankThreshold = -56,
						TrustGain = 0.33851730151221,
						TrustLoss = 0.606757528587966,
						EnemyTrustThreshold = 0.363383396575962,
						LowMoneyThreshold = 0.551272398527902,
						MinRoundsWithoutFool = 15,
						FoolChance = 0.344547064349332,
						SilentChance = 0.42513535895225,
						PanicThreshold = 0.376058854565485,
						BlindPanicThreshold = 0.508719642835319,
						MoneyPanicMultiplier = 0.552256489570855,
						PairRankPanicMultiplier = 0.306109992603963,
						EnemyStrongerCombinationPanicMultiplier = 0.239682194582546,
						FoolRaisePercent = 0.611914415893913,
						StrongPairRaisePercent = 0.461605013311194,
						RaiseOffset = 0.515785989391038,
						BetPanicMultiplier = 0.479487953658273,
						SkipTurnChance = 0.50150951202979
					}
				}
			},
			{
				Difficulty.Insane,
				new BotSettings[]
				{
					new()
					{
						StrongPairThreshold = 42,
						EnemyStrongerCombinationMiddleThreshold = 0.580552901549655,
						EnemyStrongerCombinationPanicThreshold = 0.335937388099561,
						TrustPairRankThreshold = -30,
						TrustGain = 0.399745331052125,
						TrustLoss = 0.517939771448603,
						EnemyTrustThreshold = 0.37136226463111,
						LowMoneyThreshold = 0.591672573079266,
						MinRoundsWithoutFool = 20,
						FoolChance = 0.475257345567134,
						SilentChance = 0.453136176983624,
						PanicThreshold = 0.451606887993653,
						BlindPanicThreshold = 0.510109316188207,
						MoneyPanicMultiplier = 0.53098269326101,
						PairRankPanicMultiplier = 0.507247982897121,
						EnemyStrongerCombinationPanicMultiplier = 0.451529348189765,
						FoolRaisePercent = 0.491388177530432,
						StrongPairRaisePercent = 0.594146278716229,
						RaiseOffset = 0.630255063511071,
						BetPanicMultiplier = 0.869215242363908,
						SkipTurnChance = 0.384396732995902
					},
					new()
					{
						StrongPairThreshold = 64,
						EnemyStrongerCombinationMiddleThreshold = 0.462830345199498,
						EnemyStrongerCombinationPanicThreshold = 0.133812220503349,
						TrustPairRankThreshold = -20,
						TrustGain = 0.523322554597334,
						TrustLoss = 0.589388783162544,
						EnemyTrustThreshold = 0.556103363060065,
						LowMoneyThreshold = 0.466030915077854,
						MinRoundsWithoutFool = 32,
						FoolChance = 0.371689946492546,
						SilentChance = 0.65125831026942,
						PanicThreshold = 0.480921940151533,
						BlindPanicThreshold = 0.582342204269427,
						MoneyPanicMultiplier = 0.610111333482502,
						PairRankPanicMultiplier = 0.49127278137327,
						EnemyStrongerCombinationPanicMultiplier = 0.324092296125452,
						FoolRaisePercent = 0.541591528871714,
						StrongPairRaisePercent = 0.577937290948789,
						RaiseOffset = 0.705137103441687,
						BetPanicMultiplier = 0.817870173355334,
						SkipTurnChance = 0.153335857492527
					},
					new()
					{
						StrongPairThreshold = 72,
						EnemyStrongerCombinationMiddleThreshold = 0.484222037061354,
						EnemyStrongerCombinationPanicThreshold = 0.26836409335633,
						TrustPairRankThreshold = -61,
						TrustGain = 0.566883946937008,
						TrustLoss = 0.638788372162068,
						EnemyTrustThreshold = 0.718568867320085,
						LowMoneyThreshold = 0.42212603245781,
						MinRoundsWithoutFool = 15,
						FoolChance = 0.556058192547925,
						SilentChance = 0.753278214365574,
						PanicThreshold = 0.556256645012268,
						BlindPanicThreshold = 0.566985649428569,
						MoneyPanicMultiplier = 0.633519027091822,
						PairRankPanicMultiplier = 0.523960428328376,
						EnemyStrongerCombinationPanicMultiplier = 0.391395465035217,
						FoolRaisePercent = 0.640798209578047,
						StrongPairRaisePercent = 0.595612585760941,
						RaiseOffset = 0.744077647218037,
						BetPanicMultiplier = 0.629314633279084,
						SkipTurnChance = 0.165586111745569
					},
					new()
					{
						StrongPairThreshold = 72, 
						EnemyStrongerCombinationMiddleThreshold = 0.484222037061354,
						EnemyStrongerCombinationPanicThreshold = 0.26836409335633, 
						TrustPairRankThreshold = -61,
						TrustGain = 0.566883946937008, 
						TrustLoss = 0.638788372162068, 
						EnemyTrustThreshold = 0.718568867320085,
						LowMoneyThreshold = 0.42212603245781, 
						MinRoundsWithoutFool = 10, 
						FoolChance = 0.556058192547925,
						SilentChance = 0.753278214365574, 
						PanicThreshold = 0.556256645012268,
						BlindPanicThreshold = 0.566985649428569, 
						MoneyPanicMultiplier = 0.633519027091822,
						PairRankPanicMultiplier = 0.523960428328376, 
						EnemyStrongerCombinationPanicMultiplier = 0.391395465035217,
						FoolRaisePercent = 0.640798209578047, 
						StrongPairRaisePercent = 0.595612585760941,
						RaiseOffset = 0.744077647218037, 
						BetPanicMultiplier = 0.629314633279084,
						SkipTurnChance = 0.165586111745569
					}
				}
			},
		};
	}
}
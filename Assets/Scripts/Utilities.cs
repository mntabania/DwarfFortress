using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities {

	public static int lastkingdomid = 0;
	public static int lastfactionid = 0;
	public static int lastCitizenId = 0;
	public static int lastLordId = 0;
	public static int lastKingId = 0;
	public static int lastCityId = 0;
	public static int lastGeneralId = 0;

	public static int tradeCount = 0;
	public static int helpCount = 0;
	public static int giftCount = 0;
	public static int cooperate1Count = 0;
	public static int cooperate2Count = 0;


	private static System.Random rng = new System.Random();  

	public static List<T> Shuffle<T>(List<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		} 
		return list;
	}

	public static Dictionary<BIOMES, Dictionary<BIOME_PRODUCE_TYPE, int[]>> biomeResourceChances = new Dictionary<BIOMES, Dictionary<BIOME_PRODUCE_TYPE, int[]>>(){
		{BIOMES.GRASSLAND, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{20, 75, 5, 0}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{20, 0, 75, 5}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{35, 10, 50, 5}},
			}
		},

		{BIOMES.WOODLAND, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{75, 5, 20, 0}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{75, 5, 20, 0}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{35, 10, 50, 5}},
			}
		},

		{BIOMES.FOREST, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{20, 5, 75, 0}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{20, 5, 75, 0}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{20, 75, 5, 0}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{25, 0, 60, 15}},
			}
		},

		{BIOMES.DESERT, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{20, 0, 75, 5}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{5, 20, 75, 0}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{75, 25, 0, 0}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{0, 0, 5, 95}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{50, 20, 25, 5}},
			}
		},

		{BIOMES.TUNDRA, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{20, 0, 75, 5}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{20, 0, 75, 5}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{20, 5, 75, 0}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{20, 5, 75, 0}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{50, 20, 25, 5}},
			}
		},

		{BIOMES.SNOW, new Dictionary<BIOME_PRODUCE_TYPE, int[]>(){
				{BIOME_PRODUCE_TYPE.FARMING, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.HUNTING, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.WOOD, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.STONE, new int[]{5, 0, 75, 20}},
				{BIOME_PRODUCE_TYPE.MANA, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.METAL, new int[]{5, 0, 20, 75}},
				{BIOME_PRODUCE_TYPE.GOLD, new int[]{10, 0, 35, 55}},
			}
		},
	};


	public static Dictionary<LORD_PERSONALITY, Dictionary<WAR_REASONS, int>> lordWarReasons = new Dictionary<LORD_PERSONALITY, Dictionary<WAR_REASONS, int>>(){
		{LORD_PERSONALITY.TIT_FOR_TAT, new Dictionary<WAR_REASONS, int>(){
				{WAR_REASONS.DEFENDER, 200},
				{WAR_REASONS.RACIST, 100},
				{WAR_REASONS.TRADER, 400},
				{WAR_REASONS.MONEY_GRUBBER, 150},
				{WAR_REASONS.ONE_TRUE_KING, 250},
				{WAR_REASONS.TRAITOR, 50},
				{WAR_REASONS.PARANOID, 150},
				{WAR_REASONS.SCAVENGER, 100},
				{WAR_REASONS.COMPETITIVE, 200},
			}
		},

		{LORD_PERSONALITY.EMOTIONAL, new Dictionary<WAR_REASONS, int>(){
				{WAR_REASONS.DEFENDER, 150},
				{WAR_REASONS.COVETOUS, 250},
				{WAR_REASONS.RACIST, 100},
				{WAR_REASONS.TRADER, 500},
				{WAR_REASONS.SUSPICIOUS, 300},
				{WAR_REASONS.TRAITOR, 200},
				{WAR_REASONS.IMPOTENT, 300},
				{WAR_REASONS.COMPETITIVE, 100},
			}
		},


		{LORD_PERSONALITY.RATIONAL, new Dictionary<WAR_REASONS, int>(){
				{WAR_REASONS.OPPORTUNIST, 200},
				{WAR_REASONS.RACIST, 100},
				{WAR_REASONS.ONE_TRUE_KING, 150},
				{WAR_REASONS.SUSPICIOUS, 200},
				{WAR_REASONS.DEFENDER, 200},
				{WAR_REASONS.IMPOTENT, 150},
				{WAR_REASONS.PARANOID, 200},
				{WAR_REASONS.SCAVENGER, 250},
			}
		},

		{LORD_PERSONALITY.NAIVE, new Dictionary<WAR_REASONS, int>(){
				{WAR_REASONS.DEFENDER, 400},
				{WAR_REASONS.RACIST, 100},
				{WAR_REASONS.MONEY_GRUBBER, 150},
				{WAR_REASONS.OPPORTUNIST, 150},
				{WAR_REASONS.SNEAKY, 150},
				{WAR_REASONS.SCAVENGER, 250},
			}
		},

		{LORD_PERSONALITY.HATER, new Dictionary<WAR_REASONS, int>(){
				{WAR_REASONS.OPPORTUNIST, 200},
				{WAR_REASONS.RACIST, 100},
				{WAR_REASONS.COVETOUS, 200},
				{WAR_REASONS.SUSPICIOUS, 300},
				{WAR_REASONS.TRADER, 400},
				{WAR_REASONS.TRAITOR, 400},
			}
		},
	};

	public static Dictionary<LORD_PERSONALITY, Dictionary<MIGHT_TRAIT, int>> lordMightChecks = new Dictionary<LORD_PERSONALITY, Dictionary<MIGHT_TRAIT, int>> () {
		{LORD_PERSONALITY.TIT_FOR_TAT, new Dictionary<MIGHT_TRAIT, int>(){
				{MIGHT_TRAIT.BULLY, 35},
				{MIGHT_TRAIT.NORMAL, 30},
				{MIGHT_TRAIT.UNDERDOG, 35},
			}
		},

		{LORD_PERSONALITY.EMOTIONAL, new Dictionary<MIGHT_TRAIT, int>(){
				{MIGHT_TRAIT.BULLY, 35},
				{MIGHT_TRAIT.NORMAL, 30},
				{MIGHT_TRAIT.UNDERDOG, 35},
			}
		},

		{LORD_PERSONALITY.RATIONAL, new Dictionary<MIGHT_TRAIT, int>(){
				{MIGHT_TRAIT.BULLY, 40},
				{MIGHT_TRAIT.NORMAL, 60},
				{MIGHT_TRAIT.UNDERDOG, 0},
			}
		},

		{LORD_PERSONALITY.NAIVE, new Dictionary<MIGHT_TRAIT, int>(){
				{MIGHT_TRAIT.BULLY, 0},
				{MIGHT_TRAIT.NORMAL, 60},
				{MIGHT_TRAIT.UNDERDOG, 40},
			}
		},

		{LORD_PERSONALITY.HATER, new Dictionary<MIGHT_TRAIT, int>(){
				{MIGHT_TRAIT.BULLY, 50},
				{MIGHT_TRAIT.NORMAL, 40},
				{MIGHT_TRAIT.UNDERDOG, 10},
			}
		},
	};

	public static Dictionary<LORD_PERSONALITY, Dictionary<RELATIONSHIP_TRAIT, int>> lordRelationshipChecks = new Dictionary<LORD_PERSONALITY, Dictionary<RELATIONSHIP_TRAIT, int>> () {
		{LORD_PERSONALITY.TIT_FOR_TAT, new Dictionary<RELATIONSHIP_TRAIT, int>(){
				{RELATIONSHIP_TRAIT.NORMAL, 65},
				{RELATIONSHIP_TRAIT.PEACEFUL, 35},
				{RELATIONSHIP_TRAIT.WARMONGER, 0},
			}
		},

		{LORD_PERSONALITY.EMOTIONAL, new Dictionary<RELATIONSHIP_TRAIT, int>(){
				{RELATIONSHIP_TRAIT.NORMAL, 60},
				{RELATIONSHIP_TRAIT.PEACEFUL, 20},
				{RELATIONSHIP_TRAIT.WARMONGER, 20},
			}
		},

		{LORD_PERSONALITY.RATIONAL, new Dictionary<RELATIONSHIP_TRAIT, int>(){
				{RELATIONSHIP_TRAIT.NORMAL, 50},
				{RELATIONSHIP_TRAIT.PEACEFUL, 50},
				{RELATIONSHIP_TRAIT.WARMONGER, 0},
			}
		},

		{LORD_PERSONALITY.NAIVE, new Dictionary<RELATIONSHIP_TRAIT, int>(){
				{RELATIONSHIP_TRAIT.NORMAL, 30},
				{RELATIONSHIP_TRAIT.PEACEFUL, 60},
				{RELATIONSHIP_TRAIT.WARMONGER, 10},
			}
		},

		{LORD_PERSONALITY.HATER, new Dictionary<RELATIONSHIP_TRAIT, int>(){
				{RELATIONSHIP_TRAIT.NORMAL, 40},
				{RELATIONSHIP_TRAIT.PEACEFUL, 20},
				{RELATIONSHIP_TRAIT.WARMONGER, 40},
			}
		},
	};


}

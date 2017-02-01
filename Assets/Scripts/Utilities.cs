﻿using UnityEngine;
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


	public static Dictionary<LORD_PERSONALITY, Dictionary<REASONS, int>> lordReasons = new Dictionary<LORD_PERSONALITY, Dictionary<REASONS, int>>(){
		{LORD_PERSONALITY.TIT_FOR_TAT, new Dictionary<REASONS, int>(){
				{REASONS.DEFENDER, 200},
				{REASONS.RACIST, 100},
				{REASONS.TRADER, 400},
				{REASONS.MONEY_GRUBBER, 150},
				{REASONS.ONE_TRUE_KING, 250},
				{REASONS.TRAITOR, 50},
				{REASONS.PARANOID, 150},
				{REASONS.SCAVENGER, 100},
				{REASONS.COMPETITIVE, 200},
			}
		},

		{LORD_PERSONALITY.EMOTIONAL, new Dictionary<REASONS, int>(){
				{REASONS.DEFENDER, 150},
				{REASONS.COVETOUS, 250},
				{REASONS.RACIST, 100},
				{REASONS.TRADER, 500},
				{REASONS.SUSPICIOUS, 300},
				{REASONS.TRAITOR, 200},
				{REASONS.IMPOTENT, 300},
				{REASONS.COMPETITIVE, 100},
			}
		},


		{LORD_PERSONALITY.RATIONAL, new Dictionary<REASONS, int>(){
				{REASONS.OPPORTUNIST, 200},
				{REASONS.RACIST, 100},
				{REASONS.ONE_TRUE_KING, 150},
				{REASONS.SUSPICIOUS, 200},
				{REASONS.DEFENDER, 200},
				{REASONS.IMPOTENT, 150},
				{REASONS.PARANOID, 200},
				{REASONS.SCAVENGER, 250},
			}
		},

		{LORD_PERSONALITY.NAIVE, new Dictionary<REASONS, int>(){
				{REASONS.DEFENDER, 400},
				{REASONS.RACIST, 100},
				{REASONS.MONEY_GRUBBER, 150},
				{REASONS.OPPORTUNIST, 150},
				{REASONS.SNEAKY, 150},
				{REASONS.SCAVENGER, 250},
			}
		},

		{LORD_PERSONALITY.HATER, new Dictionary<REASONS, int>(){
				{REASONS.OPPORTUNIST, 200},
				{REASONS.RACIST, 100},
				{REASONS.COVETOUS, 200},
				{REASONS.SUSPICIOUS, 300},
				{REASONS.TRADER, 400},
				{REASONS.TRAITOR, 400},
			}
		},
	};

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities {

	public static int lastkingdomid = 0;
	public static int lastfactionid = 0;
	public static int lastCitizenId = 0;
	public static int lastMayorId = 0;
	public static int lastKingId = 0;
	public static int lastCityId = 0;

	public static Dictionary<BIOMES, Dictionary<JOB_TYPE, int[]>> biomeResourceChances = new Dictionary<BIOMES, Dictionary<JOB_TYPE, int[]>>(){
		{BIOMES.GRASSLAND, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{20, 75, 5}},
				{JOB_TYPE.HUNTER, new int[]{5, 0, 95}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{20, 5, 75}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},

		{BIOMES.WOODLAND, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{75, 20, 5}},
				{JOB_TYPE.HUNTER, new int[]{75, 20, 5}},
				{JOB_TYPE.WOODSMAN, new int[]{75, 20, 5}},
				{JOB_TYPE.MINER, new int[]{20, 5, 75}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},

		{BIOMES.FOREST, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{20, 75, 5}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{5, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.DESERT, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{20, 5, 75}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{75, 20, 5}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.TUNDRA, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{20, 5, 75}},
				{JOB_TYPE.HUNTER, new int[]{20, 5, 75}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{50, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.SNOW, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{5, 0, 95}},
				{JOB_TYPE.WOODSMAN, new int[]{5, 0, 95}},
				{JOB_TYPE.MINER, new int[]{5, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},
	};
}

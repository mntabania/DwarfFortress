using UnityEngine;
using System.Collections;

public class Miner {

	public static CitizenUpgradeRequirements req;

	static Miner(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
		req.resource.Add (new Resource(RESOURCE.MANA_STONE, 0));
	}

	public static int GetProduction(int level, int mayorLikeRating){
		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f)) + mayorLikeRating;
	}
}

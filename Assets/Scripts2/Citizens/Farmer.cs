using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer {

	public static CitizenUpgradeRequirements req;
	public static int chance;

	static Farmer(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.FOOD, 0));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 0));
	}

	public static int GetProduction(int level, int mayorLikeRating){
		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f)) + mayorLikeRating;
	}
}

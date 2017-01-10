using UnityEngine;
using System.Collections;

public class Warrior: Roles {

	public static CitizenUpgradeRequirements req;

	static Warrior(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 0));
	}

//	public static int GetProduction(int level, int mayorLikeRating){
//		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f)) + mayorLikeRating;
//	}
}

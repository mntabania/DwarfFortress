using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer: Job {

	public Farmer(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 100));
		this.upgradeRequirements = req;
		this.resourcesProduced = new RESOURCE[]{ RESOURCE.MANA_STONE };
	}

	public static int GetProduction(int level, int hexValue, int mayorLikeRating){
		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f))+ hexValue + mayorLikeRating;
	}

	public int GetDailyProduction(RESOURCE resourceType){
		if (resourceType == RESOURCE.MANA_STONE) {
			return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.manaStoneValue;
		}
		return 0;
	}

	public int[] GetAllDailyProduction(){
		int manaStones = (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.manaStoneValue;
		return new int[]{0,0,0,0,manaStones};
	}
}

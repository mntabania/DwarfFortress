using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Alchemist : Job {

	public Alchemist(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.MANA_STONE, 100));
		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.MANA_STONE };
		this._jobType = JOB_TYPE.ALCHEMIST;
	}

//	public static int GetProduction(int level, int hexValue, int mayorLikeRating){
//		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f))+ hexValue + mayorLikeRating;
//	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.MANA_STONE) {
			return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.manaStoneValue;
		}
		return 0;
	}

	override public int GetDailyProduction(){
		if(this.citizen == null){
			return 0;
		}
		return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.manaStoneValue;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0 };
		}
		int manaStones = (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.manaStoneValue;
		return new int[]{0,0,0,0,manaStones};
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.manaStoneValue);
		return hexTiles.Where(x => x.manaStoneValue == maxValue).ToList();
//		return hexTiles[0].ToList();
	}
}

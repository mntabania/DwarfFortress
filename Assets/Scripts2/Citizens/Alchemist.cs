using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Alchemist : Job {

	public Alchemist(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.MANA_STONE, 200));
//		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
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
			int halfHexValue = (int)((float)this.citizen.assignedTile.manaStoneValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range (1, halfHexValue))) * 2f);
		}
		return 0;
	}

	override public int GetDailyProduction(){
		if(this.citizen == null){
			return 0;
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.manaStoneValue / 2f);
		return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue))) * 2f);
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0 };
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.manaStoneValue / 2f);
		int manaStones =  (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range (1, halfHexValue))) * 2f);
		return new int[]{0,0,0,0,manaStones};
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.manaStoneValue);
		return hexTiles.Where(x => x.manaStoneValue == maxValue).ToList();
//		return hexTiles[0].ToList();
	}
}

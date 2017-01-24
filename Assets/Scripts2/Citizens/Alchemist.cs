using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Alchemist : Job {

	public Alchemist(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.MANA, 200));
//		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.MANA };
		this._jobType = JOB_TYPE.ALCHEMIST;
		this._residence = RESIDENCE.OUTSIDE;

	}

//	public static int GetProduction(int level, int hexValue, int mayorLikeRating){
//		return (int)((float)(5 + (5 * level)) * Random.Range(1f, 1.4f))+ hexValue + mayorLikeRating;
//	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.MANA) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.manaValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range (1, halfHexValue + 1))) * this.citizen.city.alchemistMultiplier);
		}
		return 0;
	}

	override public int GetAveDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.MANA) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.manaValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * (halfHexValue / 2f))) * this.citizen.city.alchemistMultiplier);
		}
		return 0;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0, 0};
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.manaValue / 2f);
		int manaStones =  (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range (1, halfHexValue + 1))) * this.citizen.city.alchemistMultiplier);
		return new int[]{0,0,0,0,manaStones, 0};
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.manaValue);
		return hexTiles.Where(x => x.manaValue == maxValue).ToList();
//		return hexTiles[0].ToList();
	}
}

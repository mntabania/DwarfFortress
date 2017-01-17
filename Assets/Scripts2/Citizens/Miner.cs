using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Miner: Job {

	public Miner(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.METAL, 200));
//		req.resource.Add (new Resource(RESOURCE.MANA, 100));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.METAL };
		this._jobType = JOB_TYPE.MINER;
		this._residence = RESIDENCE.OUTSIDE;

	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.METAL) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.metalValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * (halfHexValue/2f))) * 2f);
		}
		return 0;
	}

	override public int GetAveDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.METAL) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.metalValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * (halfHexValue / 2f))) * 2f);
		}
		return 0;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0, 0 };
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.metalValue / 2f);
		int metals =  (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue + 1))) * 2f);
		return new int[]{0,0,0,0,0,metals}; //gold, food, lumber, stone, manastone, metal
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.metalValue);
		return hexTiles.Where(x => x.metalValue == maxValue).ToList();
	}
}

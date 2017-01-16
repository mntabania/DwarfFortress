using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Quarryman: Job {

	public Quarryman(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		//		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		req.resource.Add (new Resource(RESOURCE.STONE, 200));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.STONE };
		this._jobType = JOB_TYPE.QUARRYMAN;
	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.STONE) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.stoneValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue))) * 2f);
		}
		return 0;
	}

	override public int GetDailyProduction(){
		if(this.citizen == null){
			return 0;
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.stoneValue / 2f);
		return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue))) * 2f);
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0, 0 };
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.stoneValue / 2f);
		int stones = (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue))) * 2f);
		return new int[]{0,0,0,stones,0,0}; //gold, food, lumber, stone, manastone, metal
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.stoneValue);
		return hexTiles.Where(x => x.stoneValue == maxValue).ToList();
	}
}

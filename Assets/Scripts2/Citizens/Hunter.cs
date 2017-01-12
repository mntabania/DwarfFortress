using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Hunter: Job {

	public Hunter(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		req.resource.Add (new Resource(RESOURCE.STONE, 100));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.FOOD };
		this._jobType = JOB_TYPE.HUNTER;
	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.FOOD) {
			return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.huntingValue;
		}
		return 0;
	}

	override public int GetDailyProduction(){
		if(this.citizen == null){
			return 0;
		}
		return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.huntingValue;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0 };
		}
		int food = (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.huntingValue;
		return new int[]{0,food,0,0,0}; //gold, food, lumber, stone, manastone
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.huntingValue);
		return hexTiles.Where(x => x.huntingValue == maxValue).ToList();
	}
}

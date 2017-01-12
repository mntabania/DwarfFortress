using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Woodsman: Job {

	public static CitizenUpgradeRequirements req;

	public Woodsman(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.LUMBER, 100));
		req.resource.Add (new Resource(RESOURCE.STONE, 100));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.LUMBER };
		this._jobType = JOB_TYPE.WOODSMAN;
	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.LUMBER) {
			return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.woodValue;
		}
		return 0;
	}

	override public int GetDailyProduction(){
		if(this.citizen == null){
			return 0;
		}
		return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.woodValue;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0 };
		}
		int lumbers = (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.woodValue;
		return new int[]{0,0,lumbers,0,0}; //gold, food, lumber, stone, manastone
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.woodValue);
		return hexTiles.Where(x => x.woodValue == maxValue).ToList();
	}
}

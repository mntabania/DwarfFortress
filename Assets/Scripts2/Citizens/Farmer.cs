﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Farmer: Job {

	public Farmer(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
//		req.resource.Add (new Resource(RESOURCE.FOOD, 100));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 200));
		this._upgradeRequirements = req;
		this._resourcesProduced = new RESOURCE[]{ RESOURCE.FOOD };
		this._jobType = JOB_TYPE.FARMER;
		this._residence = RESIDENCE.OUTSIDE;
	}

	override public int GetDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.FOOD) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.farmingValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range(1, halfHexValue + 1))) * this.citizen.city.farmerMultiplier);
		}
		return 0;
	}

	override public int GetAveDailyProduction(RESOURCE resourceType){
		if(this.citizen == null){
			return 0;
		}
		if (resourceType == RESOURCE.FOOD) {
			int halfHexValue = (int)((float)this.citizen.assignedTile.farmingValue / 2f);
			return (int)((halfHexValue + (this.citizen.level * (halfHexValue / 2f))) * this.citizen.city.farmerMultiplier);
		}
		return 0;
	}

	override public int[] GetAllDailyProduction(){
		if(this.citizen == null){
			return new int[]{ 0, 0, 0, 0, 0, 0 };
		}
		int halfHexValue = (int)((float)this.citizen.assignedTile.farmingValue / 2f);
		int food = (int)((halfHexValue + (this.citizen.level * UnityEngine.Random.Range (1, halfHexValue + 1))) * this.citizen.city.farmerMultiplier);
		return new int[]{0,food,0,0,0,0}; //gold, food, lumber, stone, manastone, metal
	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		int maxValue = hexTiles.Max(x => x.farmingValue);
		return hexTiles.Where(x => x.farmingValue == maxValue).ToList();
	}
}

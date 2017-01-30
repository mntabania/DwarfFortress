using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Mage: Job {

	public Mage(){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements();
		//		req.resource.Add (new Resource(RESOURCE.LUMBER, 100));
		req.resource.Add (new Resource(RESOURCE.MANA, 200));
		this._upgradeRequirements = req;
		this._resourcesProduced = null;
//		this._jobType = JOB_TYPE.MAGE;		
		this._residence = RESIDENCE.INSIDE;

	}

	//	override public int GetDailyProduction(RESOURCE resourceType){
	//		return 0;
	//	}
	//
	//	override public int GetDailyProduction(){
	//		return (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.stoneValue;
	//	}

	//	override public int[] GetAllDailyProduction(){
	//		int stones = (int)((float)(5 + (5 * this.citizen.level)) * Random.Range(1f, 1.4f))+ this.citizen.assignedTile.stoneValue;
	//		return new int[]{0,0,0,stones,0}; //gold, food, lumber, stone, manastone
	//	}

	override internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		List<HexTile> newList = new List<HexTile> ();
		newList.Add (this.citizen.city.hexTile);
		return newList;
	}
}

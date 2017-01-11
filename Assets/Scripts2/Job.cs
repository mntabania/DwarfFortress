using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Job {

	protected JOB_TYPE _jobType;
	protected Citizen citizen;
	protected CitizenUpgradeRequirements _upgradeRequirements;
	protected RESOURCE[] resourcesProduced; //gold, food, wood, stone, mana stone

	public JOB_TYPE jobType{
		get{ return _jobType; }
	}

	public CitizenUpgradeRequirements upgradeRequirements{
		get { return _upgradeRequirements; }
		set { _upgradeRequirements = value; }
	}

	public int GetDailyProduction(RESOURCE resourceType){
		return 0;
	}

	public int[] GetAllDailyProduction(){
		return new int[]{0,0,0,0,0};
	}

	internal List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		return hexTiles;
	}


}

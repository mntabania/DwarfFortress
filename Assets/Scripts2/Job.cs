using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Job {

	[SerializeField]protected JOB_TYPE _jobType;
	[SerializeField]protected Citizen _citizen;
	[SerializeField]protected CitizenUpgradeRequirements _upgradeRequirements;
	[SerializeField]protected RESOURCE[] _resourcesProduced; //gold, food, wood, stone, mana stone

	public Job(){
		
	}

	public JOB_TYPE jobType{
		get{ return _jobType; }
	}
	public Citizen citizen{
		get { return _citizen; }
		set { _citizen = value; }
	}
	public RESOURCE[] resourcesProduced{
		get{ return _resourcesProduced; }
	}
	public CitizenUpgradeRequirements upgradeRequirements{
		get { return _upgradeRequirements; }
		set { _upgradeRequirements = value; }
	}

	public virtual int GetDailyProduction(RESOURCE resourceType){
		return 0;
	}
	public virtual int GetDailyProduction(){
		return 0;
	}
	public virtual int[] GetAllDailyProduction(){
		return new int[]{0,0,0,0,0};
	}

	internal virtual List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		return hexTiles;
	}


}

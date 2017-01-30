using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Job {

	[SerializeField]protected JOB_TYPE _jobType;
	[SerializeField]protected Citizen _citizen;
//	[SerializeField]protected int _citizenLevel;
//	[SerializeField]protected HexTile _assignedTile;
	[SerializeField]protected CitizenUpgradeRequirements _upgradeRequirements;
	[SerializeField]protected RESOURCE[] _resourcesProduced; //gold, food, wood, stone, mana stone
	[SerializeField]protected RESIDENCE _residence;
	[SerializeField]protected Army _army;

	public Job(){
		this._army = null;
	}
	public Army army{
		get{return _army;}
		set { _army = value; }
	}
	public RESIDENCE residence{
		get{return _residence;}
	}
	public JOB_TYPE jobType{
		get{ return _jobType; }
	}
	public Citizen citizen{
		get { return _citizen; }
		set { _citizen = value; }
	}
//	public int citizenLevel{
//		get { return _citizenLevel; }
//		set { _citizenLevel = value; }
//	}
//	public HexTile assignedTile{
//		get { return _assignedTile; }
//		set { _assignedTile = value; }
//	}
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
	public virtual int GetAveDailyProduction(RESOURCE resourceType){
		return 0;
	}
	public virtual int[] GetAllDailyProduction(){
		return new int[]{0,0,0,0,0,0};
	}

	internal virtual List<HexTile> GetViableNeighborTiles (List<HexTile> hexTiles){
		return hexTiles;
	}

	public void CopyData(Job job){
		this._jobType = job.jobType;
		this._citizen = job.citizen;
		this._upgradeRequirements = job.upgradeRequirements;
		this._resourcesProduced = job.resourcesProduced;
		this._residence = job.residence;
	}
}

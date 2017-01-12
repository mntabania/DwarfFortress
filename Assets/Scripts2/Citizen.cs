using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Citizen {
	[SerializeField] protected int _id;
	[SerializeField] protected string _name;
	[SerializeField] protected int _level;
	[SerializeField] protected const int baseFoodConsumption = 3;
	[SerializeField] protected HexTile _assignedTile;
	[SerializeField] protected int _upgradeChance;
	[SerializeField] protected Job _job;
	protected CityTest _city;


	#region getters and setters
	public int id{
		get{ return _id; }
		set{ _id = value; }
	}
	public string name{
		get{ return _name; }
		set{ _name = value; }
	}
	public int level{
		get{ return _level; }
	}

	public HexTile assignedTile{
		get{ return _assignedTile; }
	}

	public int upgradeChance{
		get{ return _upgradeChance; }
		set{ _upgradeChance = value; }
	}

	public Job job{
		get{ return _job; }
	}

	public CityTest city{
		get{ return _city; }
	}
	#endregion

	public Citizen(JOB_TYPE jobType, CityTest city){
		this._id = GetID()+1;
		this.name = "CITIZEN" + this._id;
		this._job = GetJob(jobType);
		this._city = city;
		this._level = 1;
		this._job.citizen = this;
		SetLastID (this._id);
	}

	public Job GetJob(JOB_TYPE jobType){
		for(int i = 0; i < Lookup.JOB_REF.Length; i++){
			if(Lookup.GetJobInfo(i).jobType == jobType){
				return Lookup.GetJobInfo (i);
			}
		}
		return null;
	}
	public int GetDailyProduction(RESOURCE resourceType) {
		return _job.GetDailyProduction (resourceType);
	}

	public int[] GetAllDailyProduction() {
		return _job.GetAllDailyProduction();
	}

	public CitizenUpgradeRequirements GetUpgradeRequirements(){
		return _job.upgradeRequirements;
	}

	internal int FoodConsumption(){
		return baseFoodConsumption * _level;
	}

	internal void UpgradeCitizen(){
		this._level += 1;
		UpdateUpgradeRequirements ();
	}

	internal void ChangeJob(JOB_TYPE jobType){
		for(int i = 0; i < Lookup.JOB_REF.Length; i++){
			if(Lookup.GetJobInfo(i).jobType == jobType){
				this._job = Lookup.GetJobInfo (i);
				this._job.citizen = this;
				break;
			}
		}
		UpdateUpgradeRequirements ();
	}

	internal void AssignCitizenToTile(List<HexTile> hexTiles){
		HexTile[] viableHexTiles = _job.GetViableNeighborTiles (hexTiles).ToArray ();
		int randomNeighbour = UnityEngine.Random.Range (0, viableHexTiles.Length);
		this._city.ownedBiomeTiles.Add(viableHexTiles [randomNeighbour]);
		viableHexTiles [randomNeighbour].isOccupied = true;
		this._assignedTile = viableHexTiles[randomNeighbour];

		if(this._city.kingdomTile){
			viableHexTiles [randomNeighbour].SetTileColor (this._city.kingdomTile.kingdom.tileColor);
		}
	}

	private void UpdateUpgradeRequirements(){
		for (int i = 0; i < _job.upgradeRequirements.resource.Count; i++) {
			_job.upgradeRequirements.resource [i].resourceQuantity *= this._level;
		}
	}
	int GetID(){
		return Utilities.lastCitizenId;
	}

	void SetLastID(int id){
		Utilities.lastCitizenId = id;
	}

}

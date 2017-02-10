using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Citizen {
	[SerializeField] protected int _id;
	[SerializeField] protected string _name;
	[SerializeField] protected int _level;
	[SerializeField] protected const int baseFoodConsumption = 10;
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
		this._job = GetJob (jobType);
//		this._job = new Job();
//		this._job.CopyData(GetJob (jobType));
		this._city = city;
		this._level = 1;
		AssignJobToCitizen ();
		SetLastID (this._id);
	}
	private void AssignJobToCitizen(){
		this._job.citizen = this;
		if(this._job.army != null && this.city.kingdomTile != null){ // meaning that this job has an army or is illegible for an army, if not, army stats will not be assigned
			this._job.army.armyCount = this.city.kingdomTile.kingdom.armyBaseUnits;
			this._job.army.CopyArmyStatsData (this.city.kingdomTile.kingdom.armyBaseStats);
		}

	}
	public Job GetJob(JOB_TYPE jobType){
		switch(jobType){
		case JOB_TYPE.ALCHEMIST:
			return new Alchemist ();
//		case JOB_TYPE.ARCHER:
//			return new Archer ();
//		case JOB_TYPE.BRAWLER:
//			return new Brawler ();
		case JOB_TYPE.FARMER:
			return new Farmer ();
		case JOB_TYPE.HUNTER:
			return new Hunter ();

		case JOB_TYPE.MINER:
			return new Miner ();
		case JOB_TYPE.QUARRYMAN:
			return new Quarryman ();
		case JOB_TYPE.DEFENSE_GENERAL:
			return new DefenseGeneral ();
		case JOB_TYPE.OFFENSE_GENERAL:
			return new OffenseGeneral ();
		case JOB_TYPE.WOODSMAN:
			return new Woodsman ();
		case JOB_TYPE.PIONEER:
			return new Pioneer ();
		default:
			return new Job ();
		}
	}
//	public Job GetJob(JOB_TYPE jobType){
//		for(int i = 0; i < Lookup.JOB_REF.Length; i++){
//			if(Lookup.GetJobInfo(i).jobType == jobType){
//				return Lookup.GetJobInfo (i);
//			}
//		}
//		return null;
//	}
	public int GetDailyProduction(RESOURCE resourceType) {
		return _job.GetDailyProduction (resourceType);
	}

	public int GetAveDailyProduction(RESOURCE resourceType) {
		return _job.GetAveDailyProduction (resourceType);
	}

	public int[] GetAllDailyProduction() {
		return _job.GetAllDailyProduction();
	}
		
	public CitizenUpgradeRequirements GetUpgradeRequirements(){
		return _job.upgradeRequirements;
	}
		
	internal int FoodConsumption(){
		return baseFoodConsumption;
	}

	internal void UpgradeCitizen(){
		this._level += 1;
		UpdateUpgradeRequirements ();
	}

	internal void ChangeJob(JOB_TYPE jobType){
		this._job = GetJob(jobType);
		this._job.citizen = this;
		if (jobType != JOB_TYPE.PIONEER) {
			UpdateUpgradeRequirements ();
		}
	}

	internal void AssignCitizenToTile(List<HexTile> hexTiles){
		if(this.job.residence == RESIDENCE.OUTSIDE){
			HexTile[] viableHexTiles = _job.GetViableNeighborTiles (hexTiles).ToArray ();
			int randomNeighbour = UnityEngine.Random.Range (0, viableHexTiles.Length);
			this._city.ownedBiomeTiles.Add(viableHexTiles [randomNeighbour]);
			viableHexTiles [randomNeighbour].isOccupied = true;
			this._assignedTile = viableHexTiles[randomNeighbour];
			if(this._city.kingdomTile){
				if (!viableHexTiles [randomNeighbour].isCity) {
					viableHexTiles [randomNeighbour].SetTileColor (Color.blue);
				}
			}
			this._city.goldValue = this._city.GetGoldValue ();
		}else{
			this._assignedTile = this.city.hexTile;
		}
	}

	internal void SetCitizenTile(HexTile hexTile){
		this._assignedTile = hexTile;
		if (!hexTile.isCity) {
			hexTile.isOccupied = true;
			hexTile.SetTileColor (Color.blue);
		}
	}

	internal void ResetLevel(){
		this._level = 1;
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

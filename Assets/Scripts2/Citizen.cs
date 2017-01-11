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
	[SerializeField] protected CityTest _city;


	#region getters and setters
	public int id{
		get{ return _id; }
		set{ _id = value; }
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
	#endregion

	public Citizen(Job job, CityTest city){
		this._id = GetID()+1;
		this._job = job;
		this._city = city;
		SetLastID (this._id);
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
		for (int i = 0; i < _job.upgradeRequirements.resource.Count; i++) {
			_job.upgradeRequirements.resource [i].resourceQuantity *= this._level;
		}
	}

	internal void ChangeJob(Job job){
		this._job = job;
	}

	internal void AssignCitizenToTile(List<HexTile> hexTiles){
		HexTile[] viableHexTiles = _job.GetViableNeighborTiles (hexTiles).ToArray ();
		int randomNeighbour = UnityEngine.Random.Range (0, viableHexTiles.Length);
		this._city.ownedBiomeTiles.Add(viableHexTiles [randomNeighbour]);
		viableHexTiles [randomNeighbour].isOccupied = true;
		this.assignedTile = viableHexTiles[randomNeighbour];

		if(this._city.kingdomTile){
			viableHexTiles [randomNeighbour].SetTileColor (this._city.kingdomTile.kingdom.tileColor);
		}
	}
		
	int GetID(){
		return Utilities.lastkingdomid;
	}

	void SetLastID(int id){
		Utilities.lastkingdomid = id;
	}

}

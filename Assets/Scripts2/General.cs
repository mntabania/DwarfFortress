using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class General {
	public int id;
	public string name;
	public CityTest city;
	public CityTest targetCity;
	public HexTile location;
	public HexTile targetLocation;
	public List<Tile> roads;
	public Army army;
	public int taskID;
	public GENERAL_TASK task;
	public int daysBeforeArrival;
	public int daysBeforeReleaseTask;
	public bool isOnTheWay;
	public GameObject generalAvatar;

	public General(CityTest city){
		this.id = GetID() + 1;
		this.name = "GENERAL" + this.id;
		this.city = city;
		this.location = city.hexTile;
		this.targetLocation = null;
		this.targetCity = null;
		this.army = new Army (city);
		this.taskID = 0;
		this.task = GENERAL_TASK.NONE;
		this.isOnTheWay = false;
		this.daysBeforeArrival = 0;
		this.daysBeforeReleaseTask = 0;
		this.roads = new List<Tile> ();
		GeneralAI.onMove += Move;
		GeneralAI.onCheckTask += CheckTask;
		SetLastID (this.id);
	}

	private void Move(){
//		if(this.onAttack || this.onHelp){
		if (this.targetLocation != null) {
			if(this.location != this.targetLocation){
				if(this.daysBeforeArrival > 0){
					this.daysBeforeArrival -= 1;
				}else if(this.daysBeforeArrival < 0){
					this.daysBeforeArrival = 0;
				}
				if(this.roads.Count > 0){
					this.generalAvatar.GetComponent<CitizenAvatar>().MakeCitizenMove (this.location, this.roads [0].hexTile);
					this.location = this.roads[0].hexTile;
					this.roads.RemoveAt (0);
					if(this.location.isCity && this.location.isOccupied){
						if(this.location.GetCityTileTest().cityAttributes.id != this.city.id){
							bool isDead = false;
							int armyNumber = this.army.armyCount;
							this.location.GetCityTileTest ().cityAttributes.CheckBattleMidwayCity (this, ref armyNumber, ref isDead);
							if(!isDead){
								this.army.armyCount = armyNumber;
							}
						}
					}
				}
//				Debug.Log (GameManager.Instance.currentDay + ": " + this.name + " HAS MOVED TO " + this.location.name);
			}else{
				Debug.Log (GameManager.Instance.currentDay + ": " + this.name + " HAS ARRIVED AT " + this.targetLocation.name);
				this.isOnTheWay = false;
				this.targetLocation = null;
				GameObject.Destroy(this.generalAvatar);
			}
//			if (this.location == this.targetLocation) {
//				if (this.location != this.city.hexTile) {
//					this.targetLocation.GetCityTileTest ().cityAttributes.visitingGenerals.Add (this);
//				}
//				switch (this.task) {
//				case GENERAL_TASK.NONE:
//					
//					break;
//				case GENERAL_TASK.ON_ATTACK:
//					//BATTLE
//					break;
//				case GENERAL_TASK.ON_DEFEND:
//					break;
//				case GENERAL_TASK.ON_HELP:
//					break;
//				}
//
//				this.targetLocation = null; 
//			}
		}
	}
	private void CheckTask(){
//		Debug.Log (this.name + " CHECKING TASK....");
		if(this.task == GENERAL_TASK.ON_ATTACK){
			if(this.targetCity.id == 0){
				this.taskID = 0;
				this.task = GENERAL_TASK.NONE;
			}
		}
		if(this.task == GENERAL_TASK.NONE){
			if (!this.isOnTheWay) {
				this.city.GiveMeTask (this);
			}
		}
		if(this.task == GENERAL_TASK.ON_HELP || this.task == GENERAL_TASK.ON_DEFEND){
			this.daysBeforeReleaseTask -= 1;
			if(this.daysBeforeReleaseTask <= 0){
				this.task = GENERAL_TASK.NONE;
				this.taskID = 0;
				this.daysBeforeReleaseTask = 0;
			}
		}

	}
	internal int ArmyStrength(){
		return this.army.armyCount * this.army.armyStats.hp;
	}
	internal void AskForCommands(){
		GeneralAI.SendInstructions (GENERAL_STATUS.BATTLE_DONE, this);
	}
	private int GetID(){
		return Utilities.lastGeneralId;
	}

	private void SetLastID(int id){
		Utilities.lastGeneralId = id;
	}
}

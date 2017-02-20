using UnityEngine;
using System.Collections;

[System.Serializable]
public class General {
	public int id;
	public string name;
	public CityTest city;
	public CityTest targetCity;
	public HexTile location;
	public HexTile targetLocation;
	public Army army;
	public GENERAL_TASK task;
	public int daysBeforeArrival;
	public int daysBeforeReleaseTask;

	public General(CityTest city){
		this.id = GetID() + 1;
		this.name = "GENERAL" + this.id;
		this.city = city;
		this.location = city.hexTile;
		this.targetLocation = null;
		this.targetCity = null;
		this.army = new Army (city);
		this.task = GENERAL_TASK.NONE;
		this.daysBeforeArrival = 0;
		this.daysBeforeReleaseTask = 0;
		GeneralAI.onMove += Move;
		GeneralAI.onCheckTask += CheckTask;
		SetLastID (this.id);
	}

	private void Move(){
//		if(this.onAttack || this.onHelp){
		if (this.targetLocation != null) {
			if(this.location != this.targetLocation){
				this.location = this.targetLocation;
				Debug.Log (this.name + " HAS MOVED TO " + this.location.name);
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
		Debug.Log (this.name + " CHECKING TASK....");
		if(this.task == GENERAL_TASK.NONE){
			this.city.GiveMeTask (this);
		}
		if(this.task == GENERAL_TASK.ON_HELP || this.task == GENERAL_TASK.ON_DEFEND){
			this.daysBeforeReleaseTask -= 1;
			if(this.daysBeforeReleaseTask <= 0){
				this.task = GENERAL_TASK.NONE;
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

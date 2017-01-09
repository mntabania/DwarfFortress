using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Citizen {
	public int id;
	public string name;
	public CITIZEN_TYPE type;
	public int level;
	public int strength;
	public int productionValue;
	public int foodConsumption;
	public RESIDENCE residence;
	public HexTile assignedTile;
	public int upgradeChance;
	public int createChance;
	public CitizenUpgradeRequirements citizenUpgradeRequirements;

	public Citizen(string dummy){
		this.id = 1 + GetID ();
		this.name = "CITIZEN" + this.id;
		this.type = (CITIZEN_TYPE)(UnityEngine.Random.Range (0, Enum.GetNames (typeof(CITIZEN_TYPE)).Length));
		this.level = 1;
		this.strength = 0;
		this.productionValue = GetProductionValue(this.type);
		this.foodConsumption = 3;
		this.residence = GetCitizenResidence (this.type);
		this.assignedTile = null;
		this.upgradeChance = 0;
		this.createChance = 0;
		UpdateUpgradeRequirements ();
		SetLastID (this.id);
	}
	public Citizen(CITIZEN_TYPE citizenType){
		this.id = 1 + GetID ();
		this.name = "CITIZEN" + this.id;
		this.type = citizenType;
		this.level = 1;
		this.strength = 0;
		this.productionValue = GetProductionValue(this.type);
		this.foodConsumption = 3;
		this.residence = GetCitizenResidence (this.type);
		this.assignedTile = null;
		this.upgradeChance = 0;
		this.createChance = 0;
		UpdateUpgradeRequirements ();
		SetLastID (this.id);
	}
	private int GetProductionValue(CITIZEN_TYPE citizenType){
		if(citizenType == CITIZEN_TYPE.DIPLOMAT || citizenType == CITIZEN_TYPE.SPY || citizenType == CITIZEN_TYPE.WARRIOR || citizenType == CITIZEN_TYPE.ARCHER || citizenType == CITIZEN_TYPE.MAGE
			|| citizenType == CITIZEN_TYPE.HUNTER || citizenType == CITIZEN_TYPE.HEALER || citizenType == CITIZEN_TYPE.BUILDER || citizenType == CITIZEN_TYPE.ENCHANTER){

			return 0;
		}
		return 10;
	}
	private RESIDENCE GetCitizenResidence(CITIZEN_TYPE citizenType){
		if(citizenType == CITIZEN_TYPE.HUNTER || citizenType == CITIZEN_TYPE.FARMER || citizenType == CITIZEN_TYPE.MINER || citizenType == CITIZEN_TYPE.WOODSMAN
			|| citizenType == CITIZEN_TYPE.ALCHEMIST){

			return RESIDENCE.OUTSIDE;
		}
		return RESIDENCE.INSIDE;
	}
	internal void UpdateUpgradeRequirements(){
		this.citizenUpgradeRequirements = AccessRequirements.AccessCitizenUpgradeRequirements (this.type, this.level);
	}
	private int GetID(){
		return Utilities.lastCitizenId;
	}
	private void SetLastID(int id){
		Utilities.lastCitizenId = id;
	}
}

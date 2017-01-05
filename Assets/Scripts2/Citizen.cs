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

	public Citizen(){
		this.id = 1 + GetID ();
		this.name = "CITIZEN" + this.id;
		this.type = (CITIZEN_TYPE)(UnityEngine.Random.Range (0, Enum.GetNames (typeof(CITIZEN_TYPE)).Length));
		this.level = 1;
		this.strength = 0;
		this.productionValue = GetProductionValue(this.type);

		SetLastID (this.id);
	}


	private int GetProductionValue(CITIZEN_TYPE citizenType){
		if(citizenType == CITIZEN_TYPE.DIPLOMAT || citizenType == CITIZEN_TYPE.SPY || citizenType == CITIZEN_TYPE.WARRIOR || citizenType == CITIZEN_TYPE.ARCHER || citizenType == CITIZEN_TYPE.MAGE
			|| citizenType == CITIZEN_TYPE.HUNTER || citizenType == CITIZEN_TYPE.HEALER || citizenType == CITIZEN_TYPE.BUILDER || citizenType == CITIZEN_TYPE.ENCHANTER){

			return 0;
		}
		return 10;
	}
	private int GetID(){
		return Utilities.lastCitizenId;
	}
	private void SetLastID(int id){
		Utilities.lastCitizenId = id;
	}
}

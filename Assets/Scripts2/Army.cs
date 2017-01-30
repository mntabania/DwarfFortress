using UnityEngine;
using System.Collections;

[System.Serializable]
public class Army {

//	public int id;
//	public string name;
	public GENERAL_CLASSIFICATION classification;
	public int armyLevel;
	public int armyCount;
	public int armyExperience;

	public Army(GENERAL_CLASSIFICATION classification){
//		this.id = GetID () + 1;
//		this.name = "GENERAL" + this.id;
		this.classification = classification;
		this.armyCount = 0;
		this.armyLevel = 1;
		this.armyExperience = 0;
	}
	
//	int GetID(){
//		return Utilities.lastGeneralId;
//	}
//
//	void SetLastID(int id){
//		Utilities.lastGeneralId = id;
//	}
}

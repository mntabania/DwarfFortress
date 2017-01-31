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
	public ArmyStats armyStats;

	public Army(GENERAL_CLASSIFICATION classification){
//		this.id = GetID () + 1;
//		this.name = "GENERAL" + this.id;
		this.classification = classification;
		this.armyCount = 0;
		this.armyLevel = 1;
		this.armyExperience = 0;
		this.armyStats = new ArmyStats(0,0);
	}
	public void CopyArmyStatsData(ArmyStats armyStats){
		this.armyStats.hp = armyStats.hp;
		this.armyStats.attack = armyStats.attack;

	}
//	int GetID(){
//		return Utilities.lastGeneralId;
//	}
//
//	void SetLastID(int id){
//		Utilities.lastGeneralId = id;
//	}
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Army {

//	public int id;
//	public string name;
	public int armyLevel;
	public int armyCount;
	public int armyExperience;
	public ArmyStats armyStats;

//comment
//comment 2	
	public Army(CityTest city){
//		this.id = GetID () + 1;
//		this.name = "GENERAL" + this.id;
		this.armyCount = city.kingdomTile.kingdom.armyBaseUnits;
		this.armyLevel = 1;
		this.armyExperience = 0;
		this.armyStats = new ArmyStats(city.kingdomTile.kingdom.armyBaseStats.hp,city.kingdomTile.kingdom.armyBaseStats.attack);
	}
	public void CopyArmyStatsData(ArmyStats armyStats){
		this.armyStats.hp = armyStats.hp;
		this.armyStats.attack = armyStats.attack;

	}

}

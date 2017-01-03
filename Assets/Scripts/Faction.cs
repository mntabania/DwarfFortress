using UnityEngine;
using System.Collections;

[System.Serializable]
public class Faction {
	public int id;
	public string factionName;
	public RACE race;
	public Religion religion;
	public Culture culture;

//	public Faction(){
//		this.id = 0;
//		this.factionName = string.Empty;
//		this.race = RACE.HUMANS;
//		this.religion = null;
//		this.culture = null;
//	}
//	public Faction(Faction faction){
//		this.id = faction.id;
//		this.factionName = faction.factionName;
//		this.race = faction.race;
//		this.religion = faction.religion;
//		this.culture = faction.culture;
//	}
//	public object Clone(){
//		return new Faction {
//			id = this.id,
//			factionName = this.factionName,
//			race = this.race,
//			religion = this.religion,
//			culture = this.culture
//
//		};
//	}
	public void CopyData(Faction faction){
		this.id = faction.id;
		this.factionName = faction.factionName;
		this.race = faction.race;
		this.religion = faction.religion;
		this.culture = faction.culture;
	}
}

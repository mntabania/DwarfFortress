using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactionStorage: MonoBehaviour {
	public static FactionStorage Instance;
	public List<Faction> allFactions = new List<Faction>();

	void Awake(){
		Instance = this;
	}
	internal Faction CheckFaction(Faction faction){
		for(int i = 0; i < allFactions.Count; i++){
			if(faction.religion == allFactions[i].religion && faction.culture == allFactions[i].culture && faction.race == allFactions[i].race){
				return allFactions[i];
				break;
			}
		}
		return null;
	}
	internal void AddFaction(Faction faction){
		Faction newFaction = new Faction ();
		newFaction.id = faction.id;
		newFaction.factionName = faction.factionName;
		newFaction.race = faction.race;
		newFaction.religion = faction.religion;
		newFaction.culture = faction.culture;

		allFactions.Add (newFaction);
	}
}

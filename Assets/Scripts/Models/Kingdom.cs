using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Kingdom : IKingdom {
	public int id;
	public List<CityTile> cities;
	public RACE race;
	public float populationGrowth;
	public int altruism;
	public int ambition;
	public int performance;
	public int army;
	public string kingdomName;

	public Kingdom(float populationGrowth, RACE race, List<CityTile> cities, string kingdomName){
		int[] traits = GenerateTraits ();
		this.id = 1 + GetID();
		this.kingdomName = kingdomName;
		this.populationGrowth = populationGrowth;
		this.army = GenerateArmyPopulation ();
		this.race = race;
		this.cities = cities;
		this.altruism = traits [0];
		this.ambition = traits [1];
		this.performance = traits [2];

		SetLastID (this.id);

	}
	public int GenerateArmyPopulation(){
		return UnityEngine.Random.Range (100, 501);
	}
	public int[] GenerateTraits(){
		int altruism = UnityEngine.Random.Range (0, 10);
		int ambition = UnityEngine.Random.Range (0, 10);
		int performance = UnityEngine.Random.Range (0, 10);

		return new int[]{ altruism, ambition, performance };
	}
	public int GetID(){
		return Utilities.lastkingdomid;
	}
	public void SetLastID(int id){
		Utilities.lastkingdomid = id;
	}
}

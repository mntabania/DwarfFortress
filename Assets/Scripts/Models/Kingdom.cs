using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Kingdom : IKingdom {
	public int id;
	public string kingdomName;
	public RACE race;
	public float populationGrowth;
	public int altruism;
	public int ambition;
	public int performance;
	public int army;

	public int darkAgeCounter;
	public int darkAgeChance;

	public int goldenAgeCounter;
	public int goldenAgeIncreaseCounter;
	public int goldenAgeChance;

	public bool isDead;
	public bool isInDarkAge;
	public bool isInGoldenAge;

	public Color tileColor;

	public List<CityTile> cities;
	public List<KingdomTile> adjacentKingdoms;
	public List<KingdomTile> enemyKingdoms;
	public List<int> citiesGained;
	public List<int> citiesLost;
	public List<Faction> factions;
	public List<KingdomRelations> kingdomRelations;


	internal int defaultDarkAgeChance = 3;
	internal int defaultGoldenAgeChance = 3;
	internal int performanceStorage;

	public Kingdom(float populationGrowth, RACE race, List<CityTile> cities, string kingdomName, Color tileColor){
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
		this.performanceStorage = this.performance;
		this.darkAgeCounter = 0;
		this.darkAgeChance = this.defaultDarkAgeChance;
		this.goldenAgeCounter = 0;
		this.goldenAgeIncreaseCounter = 0;
		this.goldenAgeChance = this.defaultGoldenAgeChance;
		this.tileColor = tileColor;
		this.isDead = false;
		this.isInDarkAge = false;
		this.isInGoldenAge = false;
		this.adjacentKingdoms = new List<KingdomTile> ();
		this.enemyKingdoms = new List<KingdomTile> ();
		this.citiesGained = new List<int> ();
		this.citiesLost = new List<int> ();
		this.factions = new List<Faction> ();
		this.kingdomRelations = new List<KingdomRelations> ();
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

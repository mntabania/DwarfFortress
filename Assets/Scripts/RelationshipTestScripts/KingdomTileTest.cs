using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingdomTileTest : MonoBehaviour {
	public KingdomTest kingdom;

	//	public string kingdomName;
	//	public RACE race;
	//	public float populationGrowth;
	//	public float cityPopulation;
	//	public int altruism;
	//	public int ambition;
	//	public int performance;
	//	public List<CityTile> cities;


	public void CreateKingdom(float populationGrowth, RACE race, List<CityTileTest> cities, Color tileColor){
		this.kingdom = new KingdomTest (populationGrowth, race, cities, tileColor);
		for (int i = 0; i < this.kingdom.cities.Count; i++) {
			this.kingdom.cities [i].cityAttributes.kingdomTile = this;
			this.kingdom.cities [i].GetComponent<SpriteRenderer> ().color = this.kingdom.cities [i].cityAttributes.kingdomTile.kingdom.tileColor;
			//			this.kingdom.cities [i].cityAttributes.faction = this.kingdom.factions [0];
		}
		//		this.kingdom = kingdom;
		//		this.cities = kingdom.cities;
		//		this.race = kingdom.race;
		//		this.populationGrowth = kingdom.populationGrowth;
		//		this.cityPopulation = kingdom.cityPopulation;
		//		this.altruism = kingdom.altruism;
		//		this.ambition = kingdom.ambition;
		//		this.performance = kingdom.performance;
		//		this.kingdomName = kingdom.kingdomName;
	}

}

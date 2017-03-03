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

	void Start(){
		GameManager.Instance.turnEnded += TurnActions;
	}

	public void CreateKingdom(float populationGrowth, RACE race, List<CityTileTest> cities, Color tileColor){
		this.kingdom = new KingdomTest (populationGrowth, race, cities, tileColor);
		for (int i = 0; i < this.kingdom.cities.Count; i++) {
			this.kingdom.cities [i].cityAttributes = new CityTest (this.kingdom.cities[i].hexTile, this);
			this.kingdom.cities [i].cityAttributes.OccupyCity();
//			this.kingdom.cities [i].cityAttributes.kingdomTile = this;
//			this.kingdom.cities [i].cityAttributes.cityLord = new Lord (this.kingdom.cities [i].cityAttributes);
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

	[ContextMenu("Compute Military Strength")]
	public void ComputeMilitaryStrength(){
		Debug.Log(this.kingdom.ComputeMilitaryStrength().ToString());
	}

	[ContextMenu("Go To War")]
	public void GoToWarForTest(){
		this.kingdom.lord.GoToWarWith(GameManager.Instance.kingdoms[1].kingdom.lord);
		GameManager.Instance.kingdoms [1].kingdom.lord.GoToWarWith (this.kingdom.lord);
	}
		
	public void AddCityToKingdom(CityTileTest city){
		this.kingdom.AddCityToKingdom (city);
//		city.cityAttributes = new CityTest (city.GetComponent<HexTile>(), this);
		city.cityAttributes.kingdomTile = this;
		city.GetComponent<HexTile> ().SetTileColor (kingdom.tileColor);
	}

	internal void TurnActions(int currentDay){
//		kingdom.CheckForExpansion();
		kingdom.lord.AutoUpdateMilitaryData(currentDay);
		if (currentDay % 5 == 0) {
			kingdom.CheckForRevolution();
		}
		kingdom.lord.CheckForWars();

		if (kingdom.lord.hasExperiencedWar) {
			if (kingdom.lord.currentWars.Count > 0) {
				kingdom.lord.daysWithoutWar = 0;
			} else {
				kingdom.lord.daysWithoutWar += 1;
			}
		}
	}



}

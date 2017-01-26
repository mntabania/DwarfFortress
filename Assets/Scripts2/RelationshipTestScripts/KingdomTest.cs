using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class KingdomTest{

	public int id;
	public string kingdomName;
	public Lord lord;
	public List<CityTileTest> cities;
	public RACE kingdomRace;
	public bool isDead;
	public Color tileColor;
	public List<Relationship> relationshipKingdoms;

	public RESOURCE primaryRaceResource;
	public RESOURCE secondaryRaceResource;

	protected int expansionChance;
	protected const int defaultExpansionChance = 2;

	public KingdomTest(float populationGrowth, RACE race, List<CityTileTest> cities, Color tileColor){
		this.id = GetID() + 1;
		this.kingdomName = "KINGDOM" + this.id;
		this.lord = new Lord(this);
		this.cities = cities;
		this.kingdomRace = race;
		this.isDead = false;
		this.tileColor = tileColor;
		this.relationshipKingdoms = new List<Relationship>();
		this.expansionChance = defaultExpansionChance;
		SetLastID (this.id);
		DetermineCityUpgradeResourceType();
	}

	private int GetID(){
		return Utilities.lastkingdomid;
	}
	private void SetLastID(int id){
		Utilities.lastkingdomid = id;
	}

	internal void AddCityToKingdom(CityTileTest city){
		cities.Add (city);
		city.GetComponent<HexTile> ().SetTileColor (tileColor);
	}

	internal void CheckForExpansion(){
		//Check cities if there is no pioneer already set as needed role
		//also check if the kingdom has an adjacent unoccupied city
		int adjacentUnoccupiedCitiesCount = 0;
		for (int i = 0; i < cities.Count; i++) {
			if (cities [i].cityAttributes.neededRole == JOB_TYPE.PIONEER || cities[i].cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.PIONEER) > 0) {
				//a city in this kingdom already has a pioneer set as a needed role or already has a pioneer
				return;
			}
				
			for (int j = 0; j < cities [i].cityAttributes.connectedCities.Count; j++) {
				if (!cities [i].cityAttributes.connectedCities [j].hexTile.isOccupied) {
					adjacentUnoccupiedCitiesCount++;
				}
			}
		}

		if (adjacentUnoccupiedCitiesCount == 0) {
			//there is no adjacent unoccupied city for the kingdom to expand to
			return;
		}

		int chance = Random.Range (0, 100);
		if (chance < this.expansionChance) {
			List<CityTileTest> citiesWithoutNeededRole = cities.Where (x => x.cityAttributes.neededRole == JOB_TYPE.NONE).ToList ();
			if (citiesWithoutNeededRole.Count > 0) {
				citiesWithoutNeededRole [Random.Range (0, citiesWithoutNeededRole.Count)].cityAttributes.neededRole = JOB_TYPE.PIONEER;
				this.expansionChance = defaultExpansionChance;
			}
		} else {
			this.expansionChance += 1;
		}
	}

	internal CityTileTest NearestUnoccupiedCity(){
		List<CityTileTest> unoccupiedCities = new List<CityTileTest>();
		for (int i = 0; i < cities.Count; i++) {
			unoccupiedCities.AddRange (cities [i].cityAttributes.unoccupiedConnectedCities);
		}
		unoccupiedCities = unoccupiedCities.OrderBy(x => Vector2.Distance (this.cities[0].transform.position, x.transform.position)).ToList();
		return unoccupiedCities [0];
	}
  
	

	void DetermineCityUpgradeResourceType(){
		switch (this.kingdomRace) {
		case RACE.HUMANS:
			this.primaryRaceResource = RESOURCE.STONE;
			this.secondaryRaceResource = RESOURCE.METAL;
			break;
		case RACE.ELVES:
			this.primaryRaceResource = RESOURCE.LUMBER;
			this.secondaryRaceResource = RESOURCE.MANA;
			break;
		case RACE.MINGONS:
			this.primaryRaceResource = RESOURCE.LUMBER;
			this.secondaryRaceResource = RESOURCE.METAL;
			break;
		case RACE.CROMADS:
			this.primaryRaceResource = RESOURCE.STONE;
			this.secondaryRaceResource = RESOURCE.MANA;
			break;
		}
	}


}

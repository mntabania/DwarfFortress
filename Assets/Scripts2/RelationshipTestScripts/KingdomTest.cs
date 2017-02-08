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

	public int armyBaseUnits;
	public int armyIncreaseUnits;

	public ArmyStats armyBaseStats;
	public ArmyStats armyIncreaseStats;

	public List<Resource> armyIncreaseUnitResource;

	protected int expansionChance;
	protected const int defaultExpansionChance = 3;

	protected List<CityTileTest> citiesOrderedByUnrest{
		get{ return cities.OrderByDescending(x => x.cityAttributes.unrest).ToList(); }
	}

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
		this.armyBaseUnits = GetArmyBaseUnits ();
		this.armyIncreaseUnits = GetArmyIncreaseUnits ();
		this.armyBaseStats = GetArmyBaseStats ();
		this.armyIncreaseStats = GetArmyIncreaseStats ();
		this.armyIncreaseUnitResource = GetArmyIncreaseUnitResource ();

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
		} 
//		else {
//			this.expansionChance += 1;
//		}
	}

	internal void CheckForRevolution(){
		for (int i = 0; i < cities.Count; i++) {
			cities [i].cityAttributes.unrest = 50;
			int chanceToRevolt = (int)Mathf.Abs((float)cities[i].cityAttributes.unrest / 4f);
			int choice = Random.Range (0,1000);
			if (choice < chanceToRevolt) {
				//A city has revolted!
				if (this.cities.Count == 1) {
					//Replace Lord
					GameManager.Instance.RemoveRelationshipToOtherLords(this.lord);
					this.lord = new Lord(this);
					this.lord.CreateInitialRelationshipsToLords();
					GameManager.Instance.AddRelationshipToOtherLords(this.lord);
					GameManager.Instance.UpdateLordAdjacency();
					return;
				} else if(this.cities.Count >= 2) {
					int numOfCitiesToJoinRevolt = 0;
					int averageUnrest = (TotalUnrestInKingdom() / 8) / this.cities.Count;
					int x = this.cities.Count - 1;
					while (x > 0) {
						int chance = averageUnrest * x;
						choice = Random.Range(0,100);
						if (choice < chance) {
							numOfCitiesToJoinRevolt++;
						}
						x--;
					}

					List<CityTileTest> citiesForNewKingdom = new List<CityTileTest>();
					citiesForNewKingdom.Add(cities[i]);

//					Debug.Log("Number of cities to join revolt: " + numOfCitiesToJoinRevolt.ToString () + "/" + this.cities.Count.ToString());
					if (numOfCitiesToJoinRevolt == (this.cities.Count - 1)) {
						GameManager.Instance.RemoveRelationshipToOtherLords(this.lord);
						this.lord = new Lord (this);
						this.lord.CreateInitialRelationshipsToLords();
						GameManager.Instance.AddRelationshipToOtherLords(this.lord);
						GameManager.Instance.UpdateLordAdjacency();
						return;
					} else if (numOfCitiesToJoinRevolt > 0) {
						for (int j = 0; j < citiesOrderedByUnrest.Count; j++) {
							if (citiesOrderedByUnrest [j] == cities [i]) {
								continue;
							}

							citiesForNewKingdom.Add(citiesOrderedByUnrest[j]);
							if ((citiesForNewKingdom.Count-1) == numOfCitiesToJoinRevolt) {
								break;
							}
						}
					}

					this.RemoveCitiesFromKingdom (citiesForNewKingdom);
					KingdomTileTest newKingdom = GameManager.Instance.CreateNewKingdom(this.kingdomRace, citiesForNewKingdom);
					//Set this kingdom's lord to dislike the new lord of the new kingdom
					for (int j = 0; j < this.lord.relationshipLords.Count; j++) {
						if (this.lord.relationshipLords[j].id == newKingdom.kingdom.lord.id) {
							this.lord.relationshipLords[j].like = -50;
							this.lord.relationshipLords[j].lordRelationship = this.lord.GetLordRelationship(this.lord.relationshipLords[j].like);
						}
					}
					break;
				}
			}
		}
	}

	public void RemoveCitiesFromKingdom(List<CityTileTest> cities){
		for (int i = 0; i < cities.Count; i++) {
			this.cities.Remove (cities[i]);
		}
//		Debug.Log ("Remove City From Kingdom!: " + this.kingdomName + "/" + city.name);
//		this.cities.Remove(city);
	}

	int TotalUnrestInKingdom(){
		int totalUnrest = 0;
		for (int i = 0; i < cities.Count; i++) {
			totalUnrest += cities[i].cityAttributes.unrest;
		}
		return totalUnrest;
	}

	internal CityTileTest NearestUnoccupiedCity(){
		List<CityTileTest> unoccupiedCities = new List<CityTileTest>();
		for (int i = 0; i < cities.Count; i++) {
			unoccupiedCities.AddRange (cities [i].cityAttributes.unoccupiedConnectedCities);
		}
		if (unoccupiedCities.Count > 0) {
			unoccupiedCities = unoccupiedCities.OrderBy (x => Vector2.Distance (this.cities [0].transform.position, x.transform.position)).ToList ();
			return unoccupiedCities [0];
		}
		return null;
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
	internal ArmyStats GetArmyBaseStats(){
		switch(this.kingdomRace){
		case RACE.HUMANS:
			return new ArmyStats (250, 6);
		case RACE.ELVES:
			return new ArmyStats (200, 8);
		case RACE.MINGONS:
			return new ArmyStats (150, 6);
		case RACE.CROMADS:
			return new ArmyStats (400, 12);
		}
		return null;
	}
	internal ArmyStats GetArmyIncreaseStats(){
		switch(this.kingdomRace){
		case RACE.HUMANS:
			return new ArmyStats (5, 1);
		case RACE.ELVES:
			return new ArmyStats (3, 2);
		case RACE.MINGONS:
			return new ArmyStats (2, 1);
		case RACE.CROMADS:
			return new ArmyStats (20, 3);
		}
		return null;
	}
	internal int GetArmyBaseUnits(){
		switch(this.kingdomRace){
		case RACE.HUMANS:
			return 80;
		case RACE.ELVES:
			return 60;
		case RACE.MINGONS:
			return 100;
		case RACE.CROMADS:
			return 40;
		}
		return 0;
	}
	internal int GetArmyIncreaseUnits(){
		switch(this.kingdomRace){
		case RACE.HUMANS:
			return 20;
		case RACE.ELVES:
			return 15;
		case RACE.MINGONS:
			return 25;
		case RACE.CROMADS:
			return 10;
		}
		return 0;
	}

	internal List<Resource> GetArmyIncreaseUnitResource(){
		switch(this.kingdomRace){
		case RACE.HUMANS:
			return new List<Resource>(){new Resource(RESOURCE.GOLD, 500), new Resource(RESOURCE.STONE, 50)};
		case RACE.ELVES:
			return new List<Resource>(){new Resource(RESOURCE.GOLD, 200), new Resource(RESOURCE.LUMBER, 80)};
		case RACE.MINGONS:
			return new List<Resource>(){new Resource(RESOURCE.GOLD, 400), new Resource(RESOURCE.LUMBER, 150)};
		case RACE.CROMADS:
			return new List<Resource>(){new Resource(RESOURCE.GOLD, 300), new Resource(RESOURCE.STONE, 30)};
		}
		return null;
	}

	internal int ComputeMilitaryStrength(){
		int totalStrength = 0;
		for (int i = 0; i < this.cities.Count; i++) {
			for (int j = 0; j < this.cities[i].cityAttributes.citizens.Count; j++) {
				if (this.cities [i].cityAttributes.citizens [j].job.jobType == JOB_TYPE.OFFENSE_GENERAL ||
					this.cities [i].cityAttributes.citizens [j].job.jobType == JOB_TYPE.DEFENSE_GENERAL) {
					totalStrength += (this.cities [i].cityAttributes.citizens [j].job.army.armyCount * this.cities [i].cityAttributes.citizens [j].job.army.armyStats.hp);
				}
			}
		}
		return totalStrength;
	}

	internal int ComputeTotalCitizenCount(){
		int totalCitizenCount = 0;
		for (int i = 0; i < this.cities.Count; i++) {
			totalCitizenCount += this.cities[i].cityAttributes.citizens.Count;
		}
		return totalCitizenCount;
	}

 }

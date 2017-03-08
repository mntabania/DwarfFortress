using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class KingdomTest{

	public int id;
	public string kingdomName;
	public KingdomTileTest kingdomTile;
	internal Lord lord;
	public Royalty assignedLord;
	public RoyaltyList royaltyList;

	public List<CityTileTest> cities;
	public RACE kingdomRace;
	public bool isDead;
	public Color tileColor;
	public List<RelationshipKingdoms> relationshipKingdoms;

	public RESOURCE primaryRaceResource;
	public RESOURCE secondaryRaceResource;

	public int armyBaseUnits;
	public int armyIncreaseUnits;

	public ArmyStats armyBaseStats;
	public ArmyStats armyIncreaseStats;

	public List<Resource> armyIncreaseUnitResource;

	public List<MarriedCouple> marriedCouples;

	public List<Royalty> elligibleBachelors{
		get{ return this.royaltyList.allRoyalties.Where(x => x.age >= 16 && x.gender == GENDER.MALE && !x.isMarried).ToList();}
	}
	public List<Royalty> elligibleBachelorettes{
		get{ return this.royaltyList.allRoyalties.Where(x => x.age >= 16 && x.gender == GENDER.FEMALE && !x.isMarried).ToList();}
	}

	protected int expansionChance;
	protected const int defaultExpansionChance = 4;

	protected List<CityTileTest> citiesOrderedByUnrest{
		get{ return cities.OrderByDescending(x => x.cityAttributes.unrest).ToList(); }
	}

	public KingdomTest(float populationGrowth, RACE race, List<CityTileTest> cities, Color tileColor, KingdomTileTest kingdomTile){
		this.id = GetID() + 1;
		this.kingdomName = "KINGDOM" + this.id;
		this.kingdomTile = kingdomTile;
		this.lord = null;
//		this.lord = new Lord(this);
//		this.assignedLord = new Royalty (this, true);
		this.cities = cities;
		this.kingdomRace = race;
		this.isDead = false;
		this.tileColor = tileColor;
		this.relationshipKingdoms = new List<RelationshipKingdoms>();
		this.expansionChance = defaultExpansionChance;
		this.armyBaseUnits = GetArmyBaseUnits ();
		this.armyIncreaseUnits = GetArmyIncreaseUnits ();
		this.armyBaseStats = GetArmyBaseStats ();
		this.armyIncreaseStats = GetArmyIncreaseStats ();
		this.armyIncreaseUnitResource = GetArmyIncreaseUnitResource ();
		this.royaltyList = new RoyaltyList ();
		this.marriedCouples = new List<MarriedCouple>();

		SetLastID (this.id);
		DetermineCityUpgradeResourceType();
		CreateInitialRoyalties ();
	}

	private int GetID(){
		return Utilities.lastkingdomid;
	}
	private void SetLastID(int id){
		Utilities.lastkingdomid = id;
	}

	internal void CreateInitialRoyalties(){
		GENDER gender = GENDER.MALE;
		int randomGender = UnityEngine.Random.Range (0, 100);
		if(randomGender < 20){
			gender = GENDER.FEMALE;
		}
		this.assignedLord = new Royalty (this, UnityEngine.Random.Range (16, 36), gender, 2);
		Royalty father = new Royalty (this, UnityEngine.Random.Range (60, 81), GENDER.MALE, 1);
		Royalty mother = new Royalty (this, UnityEngine.Random.Range (60, 81), GENDER.FEMALE, 1);

		this.assignedLord.isDirectDescendant = true;

		father.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), father.age);
		mother.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), mother.age);
		this.assignedLord.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (mother.age + this.assignedLord.age));

		father.AddChild (this.assignedLord);
		mother.AddChild (this.assignedLord);
		this.assignedLord.AddParents(father, mother);
		MarriageManager.Instance.Marry(father, mother);


		int siblingsChance = UnityEngine.Random.Range (0, 100);
		if(siblingsChance < 50){
			Royalty sibling = MarriageManager.Instance.MakeBaby (father, mother, UnityEngine.Random.Range(0,this.assignedLord.age), false);
			Royalty sibling2 = MarriageManager.Instance.MakeBaby (father, mother, UnityEngine.Random.Range(0,this.assignedLord.age), false);

			sibling.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (mother.age + sibling.age));
			sibling2.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (mother.age + sibling2.age));

		}else if(siblingsChance >= 50 && siblingsChance < 75){
			Royalty sibling = MarriageManager.Instance.MakeBaby (father, mother, UnityEngine.Random.Range(0,this.assignedLord.age), false);
			sibling.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (mother.age + sibling.age));
		}

		int spouseChance = UnityEngine.Random.Range (0, 100);
		if (spouseChance < 80) {
			Royalty spouse = MarriageManager.Instance.CreateSpouse (this.assignedLord);
			spouse.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (mother.age + spouse.age));

			int childChance = UnityEngine.Random.Range (0, 100);
			if (childChance < 50) {
				if(this.assignedLord.spouse.age == 16){
					//NO CHILD
				}else if(this.assignedLord.spouse.age == 17){
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));

				}else if(this.assignedLord.spouse.age == 18){
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					Royalty child2 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 1, false);

					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));
					child2.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child2.age));

				}else{
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					Royalty child2 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 1, false);
					Royalty child3 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 2, false);

					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));
					child2.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child2.age));
					child3.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child3.age));

				}

			} else if (childChance >= 50 && childChance < 70) {
				if(this.assignedLord.spouse.age == 16){
					//NO CHILD
				}else if(this.assignedLord.spouse.age == 17){
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));

				}else{
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					Royalty child2 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 1, false);

					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));
					child2.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child2.age));
				}

			} else if (childChance >= 70 && childChance < 90) {
				if(this.assignedLord.spouse.age == 16){
					//NO CHILD
				}else{
					Royalty child1 = MarriageManager.Instance.MakeBaby (this.assignedLord, this.assignedLord.spouse, 0, false);
					child1.AssignBirthday ((MONTH)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(MONTH)).Length)), UnityEngine.Random.Range (1, 5), (spouse.age + child1.age));

				}
			}
		}
	}

	internal void UpdateLordSuccession(){
		List<Royalty> orderedMaleRoyalties = this.royaltyList.allRoyalties.Where (x => x.gender == GENDER.MALE && x.generation > this.assignedLord.generation && x.isDirectDescendant == true).OrderBy(x => x.generation).ThenByDescending(x => x.age).ToList();
		List<Royalty> orderedFemaleRoyalties = this.royaltyList.allRoyalties.Where (x => x.gender == GENDER.FEMALE && x.generation > this.assignedLord.generation && x.isDirectDescendant == true).OrderBy(x => x.generation).ThenByDescending(x => x.age).ToList();
		List<Royalty> orderedBrotherRoyalties = this.royaltyList.allRoyalties.Where (x => x.gender == GENDER.MALE && x.father == this.assignedLord.father && x.id != this.assignedLord.id).OrderByDescending(x => x.age).ToList();
		List<Royalty> orderedSisterRoyalties = this.royaltyList.allRoyalties.Where (x => x.gender == GENDER.FEMALE && x.father == this.assignedLord.father && x.id != this.assignedLord.id).OrderByDescending(x => x.age).ToList();

		List<Royalty> orderedRoyalties = orderedMaleRoyalties.Concat (orderedFemaleRoyalties).Concat(orderedBrotherRoyalties).Concat(orderedSisterRoyalties).ToList();

		this.royaltyList.successionRoyalties.Clear ();
		this.royaltyList.successionRoyalties = orderedRoyalties;
	}

	internal void AssignNewLord(Royalty newLord){
		if(newLord == null){
			CreateInitialRoyalties ();
		}else{
			if(newLord.loyalLord.id != this.assignedLord.id){
				int assimilateChance = UnityEngine.Random.Range (0, 100);
				if(assimilateChance < 35){
					AssimilateKingdom (newLord.loyalLord.kingdom);
					return;
				}else{
					newLord.kingdom.SearchRelationshipKingdomsById (newLord.loyalLord.kingdom.id).isAtWar = false;
					newLord.loyalLord.kingdom.SearchRelationshipKingdomsById (newLord.kingdom.id).isAtWar = false;
				}
			}
			this.assignedLord = newLord;
			if(!this.assignedLord.isDirectDescendant){
				RoyaltyEventDelegate.TriggerChangeIsDirectDescendant (false);
				Utilities.ChangeDescendantsRecursively (this.assignedLord, true);
			}
			UpdateLordSuccession ();
		}

	}

	internal void AssimilateKingdom(KingdomTest newKingdom){
		for(int i = 0; i < this.cities.Count; i++){
			newKingdom.AddCityToKingdom (this.cities [i]);
		}
		for (int i = 0; i < this.royaltyList.allRoyalties.Count; i++) {
			newKingdom.AddRoyaltyToKingdom (this.royaltyList.allRoyalties [i]);
		}
		PoliticsPrototypeManager.Instance.kingdoms.Remove (this.kingdomTile);
	}
	internal void AddRoyaltyToKingdom(Royalty royalty){
		this.royaltyList.allRoyalties.Add (royalty);
		royalty.kingdom = this;
	}
	internal void CreateInitialRelationshipsToKingdoms(){
		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			KingdomTest otherKingdom = PoliticsPrototypeManager.Instance.kingdoms[i].kingdom;
			if (otherKingdom.id != this.id) {
				this.relationshipKingdoms.Add (new RelationshipKingdoms (otherKingdom, DECISION.NEUTRAL, 0));
			}
		}
	}
//-----------------------------------------------------------------------------------------------
	internal void AddCityToKingdom(CityTileTest city){
		this.cities.Add (city);
		city.cityAttributes.kingdomTile = this.kingdomTile;
		city.GetComponent<HexTile> ().SetTileColor (this.tileColor);
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

	void PassOnWarsToOtherLord(List<Relationship> warsOfPreviousLord, Lord newLord){
		if (warsOfPreviousLord.Count <= 0) {
			return;
		}
		for (int i = 0; i < newLord.relationshipLords.Count; i++) {
			for (int j = 0; j < warsOfPreviousLord.Count; j++) {
				if (newLord.relationshipLords[i].lord.id == warsOfPreviousLord[j].lord.id) {
					newLord.GoToWarWith (newLord.relationshipLords [i].lord);
					newLord.relationshipLords [i].lord.GoToWarWith(newLord);
				}
			}
		}
		Debug.LogError("Passed on wars to: " + newLord.id.ToString() + " - " + newLord.name);
	}

	internal void CheckForRevolution(){
		for (int i = 0; i < cities.Count; i++) {
//			cities [i].cityAttributes.unrest = 0;
			List<Relationship> previousLordsWars = this.lord.currentWars;
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
					PassOnWarsToOtherLord(previousLordsWars, this.lord);
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
						PassOnWarsToOtherLord(previousLordsWars, this.lord);
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
						if (this.lord.relationshipLords[j].lord.id == newKingdom.kingdom.lord.id) {
							this.lord.relationshipLords[j].like = -50;
							this.lord.relationshipLords[j].lordRelationship = this.lord.GetLordRelationship(this.lord.relationshipLords[j].like);
							//Set both new lord and this kingdom's lord to war
							this.lord.GoToWarWith(newKingdom.kingdom.lord);
							newKingdom.kingdom.lord.GoToWarWith(this.lord);
							Debug.Log ("Lord that rebelled: " + newKingdom.kingdom.lord.id.ToString() + " - " + newKingdom.kingdom.lord.name + "is now at war with: "
								+ this.lord.id.ToString() + " - " + this.lord.name);
							break;
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
			for (int j = 0; j < this.cities[i].cityAttributes.generals.Count; j++) {
				totalStrength += this.cities [i].cityAttributes.generals [j].ArmyStrength();
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

	internal bool SuccessionRoyaltiesHasLoyaltyTo (Royalty lord){
		for(int i = 0; i < this.royaltyList.successionRoyalties.Count; i++){
			if(this.royaltyList.successionRoyalties[i].loyalLord.id == lord.id){
				return true;
			}
		}
		return false;
	}
	internal RelationshipKingdoms SearchRelationshipKingdomsById(int id){
		if(this.relationshipKingdoms.Count > 0){
			for(int i = 0; i < this.relationshipKingdoms.Count; i++){
				if(this.relationshipKingdoms[i].kingdom.id == id){
					return this.relationshipKingdoms [i];
				}
			}
		}
		return null;
	}
 }

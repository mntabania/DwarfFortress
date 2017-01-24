using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	public delegate void TurnEndedDelegate();
	public TurnEndedDelegate turnEnded;

	public Sprite grasslandSprite;
	public Sprite woodlandSprite;
	public Sprite forestSprite;
	public Sprite desertSprite;
	public Sprite tundraSprite;
	public Sprite snowSprite;

	public List<GameObject> cities;
	public GameObject kingdomTilePrefab;
	public List<KingdomTileTest> kingdoms;

	public int currentDay = 0;

	public bool isDayPaused = false;

	public int daysUntilNextHarvest = 30;
	public bool harvestTime;

	void Awake(){
		Instance = this;
		this.cities = new List<GameObject>();
		turnEnded += IncrementDaysOnTurn;
		turnEnded += WaitForHarvest;
	}

	void Start(){
		MapGenerator();
		GenerateCities();
		GenerateBiomes ();
		GenerateInitialKingdoms();
		CreateInitialRelationshipsToLords ();
//		AssignCitiesToKingdoms();
		GenerateCityConnections ();
//		GenerateInitialCitizens ();
		StartResourceProductions ();
		UserInterfaceManager.Instance.SetCityInfoToShow (this.cities [0].GetComponent<CityTileTest> ());
	}

	void MapGenerator(){
		GridMap.Instance.GenerateGrid();
	}

	void GenerateCities(){
		CreateCity(GridMap.Instance.listHexes [293].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [1244].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [2094].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [2127].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [1222].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [276].GetComponent<HexTile>());
//		CreateCity(GridMap.Instance.listHexes [705].GetComponent<HexTile>());
//		CreateCity(GridMap.Instance.listHexes [1706].GetComponent<HexTile>());
	}

	void GenerateCityConnections(){
		for (int i = 0; i < this.cities.Count; i++) {
			CityTileTest currentCityTile = this.cities[i].GetComponent<CityTileTest>();
			int nextCityIndex = i + 1;
			int previousCityIndex = i - 1;
			if (nextCityIndex >= this.cities.Count) {
				nextCityIndex = 0;
			}
			if (previousCityIndex < 0) {
				previousCityIndex = 5;
			}
			currentCityTile.cityAttributes.connectedCities.Add(this.cities[nextCityIndex].GetComponent<CityTileTest>());
			currentCityTile.cityAttributes.connectedCities.Add(this.cities[previousCityIndex].GetComponent<CityTileTest>());
		}
	}

	void GenerateBiomes(){
		for (int i = 0; i < this.cities.Count; i++) {
			HexTile currentHexTile = this.cities [i].GetComponent<HexTile> ();
			HexTile[] neighbours = currentHexTile.GetTilesInRange(5);
			BIOMES targetBiomeType = BIOMES.BARE;
			RESOURCE primaryResource = RESOURCE.FOOD;
			RESOURCE secondaryResource = RESOURCE.FOOD;
			Sprite biomeSprite = null;
			if (i == 0) {
				targetBiomeType = BIOMES.GRASSLAND;
				biomeSprite = grasslandSprite;
			} else if (i == 1) {
				targetBiomeType = BIOMES.DESERT;
				biomeSprite = desertSprite;
			} else if (i == 2) {
				targetBiomeType = BIOMES.WOODLAND;
				biomeSprite = woodlandSprite;
			} else if (i == 3) {
				targetBiomeType = BIOMES.GRASSLAND;
				biomeSprite = grasslandSprite;
			} else if (i == 4) {
				targetBiomeType = BIOMES.FOREST;
				biomeSprite = forestSprite;
			} else if (i == 5) {
				targetBiomeType = BIOMES.WOODLAND;
				biomeSprite = woodlandSprite;
			}else if (i == 6) {
				targetBiomeType = BIOMES.SNOW;
				biomeSprite = snowSprite;
			}else if (i == 7) {
				targetBiomeType = BIOMES.TUNDRA;
				biomeSprite = tundraSprite;
			}

			for (int j = 0; j < neighbours.Length; j++) {
				neighbours[j].SetTileBiomeType(targetBiomeType);
				neighbours[j].GenerateResourceValues ();
				neighbours[j].GetComponent<SpriteRenderer> ().sprite = biomeSprite;

				switch (targetBiomeType) {
				case BIOMES.GRASSLAND:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.WOODLAND:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				case BIOMES.FOREST:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.DESERT:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				case BIOMES.TUNDRA:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.SNOW:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				}
			}
		}
	}

	void GenerateInitialKingdoms(){
		List<GameObject> tempCities = new List<GameObject> ();
		tempCities.AddRange (cities);
		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){this.cities[0].GetComponent<CityTileTest>()}, new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		this.kingdoms.Add (goKingdom1.GetComponent<KingdomTileTest>());

		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom2.transform.parent = this.transform;
		goKingdom2.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){this.cities[1].GetComponent<CityTileTest>()}, new Color(40f/255f, 255f/255f, 0f/255f));
		goKingdom2.name = goKingdom2.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom2.GetComponent<KingdomTileTest>());

		GameObject goKingdom3 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom3.transform.parent = this.transform;
		goKingdom3.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.MINGONS, new List<CityTileTest>(){this.cities[2].GetComponent<CityTileTest>()}, new Color(0f/255f, 234f/255f, 255f/255f));
		goKingdom3.name = goKingdom3.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom3.GetComponent<KingdomTileTest>());


		GameObject goKingdom4 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom4.transform.parent = this.transform;
		goKingdom4.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.CROMADS, new List<CityTileTest>(){this.cities[3].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
		goKingdom4.name = goKingdom4.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom4.GetComponent<KingdomTileTest>());

		GameObject goKingdom5 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom5.transform.parent = this.transform;
		goKingdom5.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){this.cities[4].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
		goKingdom5.name = goKingdom5.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom5.GetComponent<KingdomTileTest>());

		GameObject goKingdom6 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom6.transform.parent = this.transform;
		goKingdom6.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){this.cities[5].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
		goKingdom6.name = goKingdom6.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom6.GetComponent<KingdomTileTest>());
	}
	internal void CreateInitialRelationshipsToLords(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			this.kingdoms [i].kingdom.lord.CreateInitialRelationshipsToLords ();
//			CityTest otherCity = this.city.kingdomTile.kingdom.cities [i].cityAttributes;
//			if (otherCity.id != this.city.id) {
//				this.relationshipLords.Add (new Relationship (otherCity.cityLord.id, otherCity.cityLord.name, DECISION.NEUTRAL, 0));
//			}
		}
	}
	void AssignCitiesToKingdoms(){
		for (int i = 0; i < cities.Count; i++) {
			KingdomTileTest randomKingdom = kingdoms [Random.Range (0, kingdoms.Count)];
			randomKingdom.AddCityToKingdom (cities [i].GetComponent<CityTileTest> ());
		}
	}

	void CreateCity(HexTile tile){
		tile.SetTileColor(Color.black);
		tile.isCity = true;
		tile.isOccupied = true;
		tile.tile.canPass = false;
		tile.gameObject.AddComponent<CityTileTest>();
		tile.gameObject.GetComponent<CityTileTest>().hexTile = tile;
//		tile.gameObject.GetComponent<CityTileTest>().cityAttributes = new CityTest(tile, tile.biomeType);
		this.cities.Add(tile.gameObject);
	}

	void StartResourceProductions(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				this.kingdoms [i].kingdom.cities [j].SetCityAsActiveAndSetProduction ();
			}
		}
		ActivateProducationCycle();
	}

	public void ActivateProducationCycle(){
		InvokeRepeating("EndTurn", 0f, 0.5f);
	}
		
	public void EndTurn(){
		if (isDayPaused) {
			return;
		}
		turnEnded();

		for (int i = 0; i < this.kingdoms.Count; i++) {
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				if (currentDay % 7 == 0) { //Select a new Citizen to create(Only occurs every 7 days)
					this.kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenForCreation(true); //assign citizen type to the City's newCitizenTarget
				}

//				if(kingdoms [i].kingdom.cities [j].cityAttributes.upgradeCitizenTarget == null){
//					kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenToUpgrade();
//				}

			}
		}
	}

	public void TogglePause(){
		isDayPaused = !isDayPaused;
	}

	void IncrementDaysOnTurn(){
		currentDay++;
		UserInterfaceManager.Instance.UpdateDayCounter(currentDay);
	}

	void WaitForHarvest(){
		if (daysUntilNextHarvest <= 1) {
			//TODO: Put Harvest Code Execution here
			for (int i = 0; i < this.cities.Count; i++) {
				this.cities [i].GetComponent<CityTileTest> ().cityAttributes.TriggerFoodHarvest();
			}
			daysUntilNextHarvest = 30;
			harvestTime = true;
		} else {
			daysUntilNextHarvest--;
			harvestTime = false;
		}

	}

	internal void CooperateEvent1(){
		List<KingdomTileTest> kingdoms = new List<KingdomTileTest> ();
		kingdoms.AddRange (this.kingdoms);
		kingdoms = Utilities.Shuffle (kingdoms);

		if(kingdoms.Count > 1){

			/*
			A huge monster guarding some treasure is discovered. 
			The Lords must cooperate to defeat it. If they cooperate, the monster is defeated and they are able to split the treasure. 
			One can be betrayed by unleashing the monster onto the other Lord's realm while the other takes all the treasure. 
			If both attempt to do this, the monster stays and the treasure remains guarded.
			*/

			Lord lord1 = kingdoms [0].kingdom.lord;
			Lord lord2 = kingdoms [1].kingdom.lord;

			DECISION lord1Decision = lord1.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE1, lord2);
			DECISION lord2Decision = lord2.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE1, lord1);

			if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.NICE){
				
			}else if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.RUDE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.RUDE){

			}

			lord1.AdjustLikeness (lord2, lord1Decision, lord2Decision, LORD_EVENTS.COOPERATE1);
			lord2.AdjustLikeness (lord1, lord2Decision, lord1Decision, LORD_EVENTS.COOPERATE1);

		}else{
			Debug.Log ("THERE IS NOT ENOUGH KINGDOMS! CAN'T COOPERATE (1)");
		}
	}

	internal void CooperateEvent2(){
		List<KingdomTileTest> kingdoms = new List<KingdomTileTest> ();
		kingdoms.AddRange (this.kingdoms);
		kingdoms = Utilities.Shuffle (kingdoms);

		if(kingdoms.Count > 1){

			/*
			A huge monster guarding some treasure is discovered. 
			The Lords must cooperate to defeat it. If they cooperate, the monster is defeated and they are able to split the treasure. 
			One can be betrayed by unleashing the monster onto the other Lord's realm while the other takes all the treasure. 
			If both attempt to do this, the monster stays and the treasure remains guarded.
			*/

			Lord lord1 = kingdoms [0].kingdom.lord;
			Lord lord2 = kingdoms [1].kingdom.lord;

			DECISION lord1Decision = lord1.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE2, lord2);
			DECISION lord2Decision = lord2.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE2, lord1);

			if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.RUDE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.RUDE){

			}

			lord1.AdjustLikeness (lord2, lord1Decision, lord2Decision, LORD_EVENTS.COOPERATE2);
			lord2.AdjustLikeness (lord1, lord2Decision, lord1Decision, LORD_EVENTS.COOPERATE2);

		}else{
			Debug.Log ("THERE IS NOT ENOUGH KINGDOMS! CAN'T COOPERATE (1)");
		}
	}


}

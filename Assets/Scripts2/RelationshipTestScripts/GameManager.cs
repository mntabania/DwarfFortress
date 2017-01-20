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
//		GenerateCityConnections ();
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
		} else {
			daysUntilNextHarvest--;
		}

	}






}

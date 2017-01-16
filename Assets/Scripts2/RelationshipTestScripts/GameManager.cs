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

	public GameObject[] hexTiles;
	public GameObject kingdomTilePrefab;
	public List<KingdomTileTest> kingdoms;

	public int currentDay = 0;

	public bool isDayPaused = false;

	void Awake(){
		Instance = this;
		turnEnded += IncrementDaysOnTurn;
	}

	void Start(){
		MapGenerator();
		GenerateCities();
		GenerateCityConnections ();
		GenerateBiomes ();
		GenerateInitialKingdoms();
		GenerateInitialCitizens ();
		StartResourceProductions ();
		UserInterfaceManager.Instance.SetCityInfoToShow (hexTiles [0].GetComponent<CityTileTest> ());
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
	}

	void GenerateCityConnections(){
		for (int i = 0; i < hexTiles.Length; i++) {
			CityTileTest currentCityTile = hexTiles[i].GetComponent<CityTileTest>();
			int nextCityIndex = i + 1;
			int previousCityIndex = i - 1;
			if (nextCityIndex >= hexTiles.Length) {
				nextCityIndex = 0;
			}
			if (previousCityIndex < 0) {
				previousCityIndex = 5;
			}
			currentCityTile.cityAttributes.connectedCities.Add(hexTiles[nextCityIndex].GetComponent<CityTileTest>());
			currentCityTile.cityAttributes.connectedCities.Add(hexTiles[previousCityIndex].GetComponent<CityTileTest>());
		}
	}

	void GenerateBiomes(){
		for (int i = 0; i < hexTiles.Length; i++) {
			HexTile currentHexTile = hexTiles [i].GetComponent<HexTile> ();
			HexTile[] neighbours = currentHexTile.GetTilesInRange(5);
			BIOMES targetBiomeType = BIOMES.BARE;
			Sprite biomeSprite = null;
			if (i == 0) {
				targetBiomeType = BIOMES.GRASSLAND;
				biomeSprite = grasslandSprite;
			} else if (i == 1) {
				targetBiomeType = BIOMES.WOODLAND;
				biomeSprite = woodlandSprite;
			} else if (i == 2) {
				targetBiomeType = BIOMES.FOREST;
				biomeSprite = forestSprite;
			} else if (i == 3) {
				targetBiomeType = BIOMES.DESERT;
				biomeSprite = desertSprite;
			} else if (i == 4) {
				targetBiomeType = BIOMES.TUNDRA;
				biomeSprite = tundraSprite;
			} else if (i == 5) {
				targetBiomeType = BIOMES.SNOW;
				biomeSprite = snowSprite;
			}
			for (int j = 0; j < neighbours.Length; j++) {
				neighbours[j].SetTileBiomeType(targetBiomeType);
				neighbours[j].GenerateResourceValues ();
				neighbours[j].GetComponent<SpriteRenderer> ().sprite = biomeSprite;
			}
		}
	}

	void GenerateInitialKingdoms(){
		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){
			hexTiles[0].GetComponent<CityTileTest>(), hexTiles[1].GetComponent<CityTileTest>(), hexTiles[2].GetComponent<CityTileTest>(),
			hexTiles[3].GetComponent<CityTileTest>(), hexTiles[4].GetComponent<CityTileTest>(), hexTiles[5].GetComponent<CityTileTest>(),
		}, new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom1.GetComponent<KingdomTileTest>());

		//		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		//		goKingdom2.transform.parent = this.transform;
		//		goKingdom2.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){hexTiles[3].GetComponent<CityTileTest>()}, new Color(40f/255f, 255f/255f, 0f/255f));
		//		goKingdom2.name = goKingdom2.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		//		kingdoms.Add (goKingdom2.GetComponent<KingdomTileTest>());
	}

	private void GenerateInitialCitizens(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				this.kingdoms [i].kingdom.cities [j].cityAttributes.citizens = InitialCitizens (this.kingdoms [i].kingdom.cities [j]);
				this.kingdoms [i].kingdom.cities [j].cityAttributes.AssignInitialCitizens();
			}
		}

	}
	private List<Citizen> InitialCitizens(CityTileTest cityTile){
		List<Citizen> citizens = new List<Citizen> ();
		citizens.Add(new Citizen (JOB_TYPE.FARMER, cityTile.cityAttributes));
		citizens.Add(new Citizen (JOB_TYPE.MINER, cityTile.cityAttributes));
		citizens.Add(new Citizen (JOB_TYPE.WOODSMAN, cityTile.cityAttributes));
		citizens.Add(new Citizen (JOB_TYPE.WARRIOR, cityTile.cityAttributes));

		return citizens;
	}

	void CreateCity(HexTile tile){
		tile.SetTileColor(Color.black);
		tile.isCity = true;
		tile.isOccupied = true;
		tile.tile.canPass = false;
		tile.gameObject.AddComponent<CityTileTest>();
		tile.gameObject.GetComponent<CityTileTest>().cityAttributes = new CityTest(tile, tile.biomeType);

		for (int i = 0; i < this.hexTiles.Length; i++) {
			if (this.hexTiles [i] == null) {
				this.hexTiles [i] = tile.gameObject;
				break;
			}
		}
		tile.gameObject.GetComponent<CityTileTest> ().cityAttributes.AssignInitialCitizens();
	}

	void StartResourceProductions(){
		for (int i = 0; i < kingdoms.Count; i++) {
			for (int j = 0; j < kingdoms [i].kingdom.cities.Count; j++) {
				kingdoms [i].kingdom.cities [j].SetCityAsActiveAndSetProduction ();
			}
		}
		ActivateProducationCycle();
	}

	public void ActivateProducationCycle(){
		InvokeRepeating("EndTurn", 0f, 1f);
	}
		
	public void EndTurn(){
		if (isDayPaused) {
			return;
		}
		turnEnded();

		for (int i = 0; i < kingdoms.Count; i++) {
			for (int j = 0; j < kingdoms [i].kingdom.cities.Count; j++) {
				if (currentDay % 7 == 0) { //Select a new Citizen to create(Only occurs every 7 days)
					kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenForCreation(true); //assign citizen type to the City's newCitizenTarget
				}

				if(kingdoms [i].kingdom.cities [j].cityAttributes.upgradeCitizenTarget == null){
					kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenToUpgrade();
				}

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






}

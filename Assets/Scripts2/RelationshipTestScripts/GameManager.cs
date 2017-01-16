using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	public delegate void TurnEndedDelegate();
	public TurnEndedDelegate turnEnded;

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
		CreateCity(GridMap.Instance.listHexes [1025].GetComponent<HexTile>());
		GenerateInitialKingdoms();
//		GenerateInitialCitizens ();
		StartResourceProductions ();
		UserInterfaceManager.Instance.SetCityInfoToShow (hexTiles [0].GetComponent<CityTileTest> ());
	}

	void MapGenerator(){
		GridMap.Instance.GenerateGrid();
	}

	void GenerateInitialKingdoms(){
		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){hexTiles[0].GetComponent<CityTileTest>()}, new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom1.GetComponent<KingdomTileTest>());
		//		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		//		goKingdom2.transform.parent = this.transform;
		//		goKingdom2.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){hexTiles[3].GetComponent<CityTileTest>()}, new Color(40f/255f, 255f/255f, 0f/255f));
		//		goKingdom2.name = goKingdom2.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		//		kingdoms.Add (goKingdom2.GetComponent<KingdomTileTest>());
	}
	private void GenerateInitialCitizens(){
//		for(int i = 0; i < this.kingdoms.Count; i++){
//			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
//				this.kingdoms [i].kingdom.cities [j].cityAttributes.citizens = InitialCitizens (this.kingdoms [i].kingdom.cities [j]);
//				this.kingdoms [i].kingdom.cities [j].cityAttributes.AssignInitialCitizens();
//			}
//		}

	}


	void CreateCity(HexTile tile){
		tile.SetTileColor(Color.black);
		tile.isCity = true;
		tile.isOccupied = true;
		tile.tile.canPass = false;
		tile.gameObject.AddComponent<CityTileTest>();
		tile.gameObject.GetComponent<CityTileTest>().hexTile = tile;
//		tile.gameObject.GetComponent<CityTileTest>().cityAttributes = new CityTest(tile, tile.biomeType);

		this.hexTiles [0] = tile.gameObject;
//		tile.gameObject.GetComponent<CityTileTest> ().cityAttributes.AssignInitialCitizens();
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






}

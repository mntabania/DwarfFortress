using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PoliticsPrototypeManager : MonoBehaviour {

	public static PoliticsPrototypeManager Instance = null;

	public delegate void TurnEndedDelegate();
	public TurnEndedDelegate turnEnded;

	public int month;
	public int week;
	public int year;

	public GameObject kingdomTilePrefab;
	public GameObject kingdomsParent;
	public List<KingdomTileTest> kingdoms;

	public static Dictionary<int, List<Royalty>> fullGenealogy;

	public bool isDayPaused = false;

	void Awake(){
		Instance = this;
		fullGenealogy = new Dictionary<int, List<Royalty>>();
	}

	void Start(){
		GridMap.Instance.GenerateGrid();
		EquatorGenerator.Instance.GenerateEquator();
		Biomes.Instance.GenerateElevation();
		Biomes.Instance.GenerateBiome();
		Biomes.Instance.GenerateTileDetails ();
//		CityGenerator.Instance.GenerateCities();
		CityGenerator.Instance.GenerateCitiesForTest();

		for (int i = 0; i < CityGenerator.Instance.cities.Count; i++) {
			CityGenerator.Instance.cities[i].GetCityTileTest().hexTile = CityGenerator.Instance.cities[i];
		}

		this.GenerateKingdoms();
		this.GenerateConnections();
		CreateInitialRelationshipsToKingdoms ();
		PoliticsPrototypeUIManager.Instance.LoadKingdoms();
		StartWeekProgression();
	}

	public void GenerateConnections(){
		//Kingdom 1
		GridMap.Instance.GameBoard [21, 30].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[18,33].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[14,32].hexTile.GetCityTileTest(), 
			GridMap.Instance.GameBoard[29,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[21,23].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [18, 33].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[21,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[14,32].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[14,37].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [14, 32].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[21,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[18,33].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [14, 37].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[18,33].hexTile.GetCityTileTest()};

		//Kingdom 2
		GridMap.Instance.GameBoard [29, 30].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[32,33].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[35,31].hexTile.GetCityTileTest(),
			GridMap.Instance.GameBoard[21,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[29,23].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [32, 33].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[29,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[36,37].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [35, 31].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[29,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[36,37].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [36, 37].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[32,33].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[35,31].hexTile.GetCityTileTest()};

		//Kingdom 3
		GridMap.Instance.GameBoard [29, 23].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[29,16].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[29,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[21,23].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [29, 16].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[29,23].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[33,14].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [33, 14].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[29,16].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[37,14].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [37, 14].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[33,14].hexTile.GetCityTileTest()};

		//Kingdom 4
		GridMap.Instance.GameBoard [21, 23].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[16,18].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[20,18].hexTile.GetCityTileTest(),
			GridMap.Instance.GameBoard[21,30].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[29,23].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [16, 18].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[21,23].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[20,18].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [20, 18].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[21,23].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[16,18].hexTile.GetCityTileTest(), GridMap.Instance.GameBoard[15,15].hexTile.GetCityTileTest()};
		GridMap.Instance.GameBoard [15, 15].hexTile.GetCityTileTest ().cityAttributes.connectedCities = new List<CityTileTest> (){
			GridMap.Instance.GameBoard[20,18].hexTile.GetCityTileTest()};

		for (int i = 0; i < CityGenerator.Instance.cities.Count; i++) {
			HexTile currentTile = CityGenerator.Instance.cities [i];
			for (int j = 0; j < currentTile.GetCityTileTest().cityAttributes.connectedCities.Count; j++) {
				Tile connectedCity = currentTile.GetCityTileTest().cityAttributes.connectedCities[j].hexTile.tile;
				currentTile.tile.canPass = true;
				connectedCity.canPass = true;
				List<Tile> roads = GetPath(currentTile.tile, connectedCity, PATHFINDING_MODE.ROAD_CREATION).ToList();
				for (int k = 0; k < roads.Count; k++) {
					roads [k].hexTile.isRoad = true;
					if (!roads [k].hexTile.isCity && !roads [k].hexTile.isOccupied) {
						roads [k].hexTile.SetTileColor (Color.gray);
					}
				}
				currentTile.tile.canPass = false;
				connectedCity.canPass = false;
			}
		}

	}

	public IEnumerable<Tile> GetPath(Tile startingTile, Tile destinationTile, PATHFINDING_MODE pathfindingMode){
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destinationTile.X, 2) + Math.Pow(t.Y - destinationTile.Y, 2));
		var path = PathFind.PathFind.FindPath(startingTile, destinationTile, distance, estimate, pathfindingMode);
		return path;
	}

	void StartWeekProgression(){
		InvokeRepeating("StartTime", 0f, 0.2f);
	}

	void StartTime(){
		if (isDayPaused) {
			return;
		}
		this.week += 1;
		if (week > 4) {
			this.week = 1;
			this.month += 1;
			if (this.month > 12) {
				this.month = 1;
				this.year += 1;
				RoyaltyEventDelegate.TriggerIncreaseIllnessAndAccidentChance ();
			}
		}
		if (turnEnded != null) {
			turnEnded ();
		}

		if(this.kingdoms.Count > 0){
//			for (int i = 0; i < this.kingdoms.Count; i++) {
//				for (int j = 0; j < this.kingdoms[i].kingdom.lord.relationshipLords.Count; j++) {
//					if(this.kingdoms[i].kingdom.lord.relationshipLords[j].lord.kingdom.cities.Count <= 0){
//						this.kingdoms [i].kingdom.lord.relationshipLords.RemoveAt (j);
//						j--;
//					}	
//				}
//			}
			for (int i = 0; i < this.kingdoms.Count; i++) {
				if(this.kingdoms[i].kingdom.cities.Count <= 0){
					this.kingdoms [i].kingdom.isDead = true;
//					turnEnded -= this.kingdoms [i].TurnActions;
//					Destroy (this.kingdoms [i].gameObject);
					this.kingdoms.RemoveAt (i);
					i--;
				}
			}
		}

	}

	void GenerateKingdoms(){

		CreateNewKingdom (new List<CityTileTest> () {
			CityGenerator.Instance.cities [0].GetCityTileTest(),
			CityGenerator.Instance.cities [1].GetCityTileTest(),
			CityGenerator.Instance.cities [2].GetCityTileTest(),
			CityGenerator.Instance.cities [3].GetCityTileTest()
		}, Color.black);

		CreateNewKingdom (new List<CityTileTest> () {
			CityGenerator.Instance.cities [4].GetCityTileTest(),
			CityGenerator.Instance.cities [5].GetCityTileTest(),
			CityGenerator.Instance.cities [6].GetCityTileTest(),
			CityGenerator.Instance.cities [7].GetCityTileTest()
		}, Color.magenta);

		CreateNewKingdom (new List<CityTileTest> () {
			CityGenerator.Instance.cities [8].GetCityTileTest(),
			CityGenerator.Instance.cities [9].GetCityTileTest(),
			CityGenerator.Instance.cities [10].GetCityTileTest(),
			CityGenerator.Instance.cities [11].GetCityTileTest()
		}, Color.blue);

		CreateNewKingdom (new List<CityTileTest> () {
			CityGenerator.Instance.cities [12].GetCityTileTest(),
			CityGenerator.Instance.cities [13].GetCityTileTest(),
			CityGenerator.Instance.cities [14].GetCityTileTest(),
			CityGenerator.Instance.cities [15].GetCityTileTest()
		}, Color.magenta);

//		List<HexTile> elligibleCities = new List<HexTile>(CityGenerator.Instance.cities);
//
//		int numOfKingdomsToCreate = 4;
//		for (int i = 0; i < numOfKingdomsToCreate; i++) {
//			List<CityTileTest> citiesForKingdom = new List<CityTileTest>();
//			HexTile kingdomParentTile = elligibleCities[Random.Range (0, elligibleCities.Count)];
//
//			elligibleCities.Remove(kingdomParentTile);
//
//			elligibleCities = elligibleCities.OrderBy(x => Vector3.Distance(kingdomParentTile.transform.position, x.transform.position)).ToList();
//
//			for (int j = 0; j < 3; j++) {
//				citiesForKingdom.Add(elligibleCities[j].GetComponent<CityTileTest>());
//			}
//
//			for (int j = 0; j < citiesForKingdom.Count; j++) {
//				elligibleCities.Remove(citiesForKingdom[j].hexTile);
//			}
//
//			citiesForKingdom.Add(kingdomParentTile.GetCityTileTest ());
//
//
//			CreateNewKingdom(citiesForKingdom);
//		}
	}

	public KingdomTest CreateNewKingdom(List<CityTileTest> cities, Color kingdomColor){
		GameObject goKingdom = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom.transform.parent = kingdomsParent.transform;
		goKingdom.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, cities,  kingdomColor);
		goKingdom.name = goKingdom.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		this.kingdoms.Add (goKingdom.GetComponent<KingdomTileTest>());
		this.turnEnded += goKingdom.GetComponent<KingdomTileTest> ().kingdom.CheckForWars;
		return goKingdom.GetComponent<KingdomTileTest>().kingdom;
	}

	#region family tree
	public void RegisterRoyalty(Royalty royalty){
		if (fullGenealogy.ContainsKey (royalty.generation)) {
			List<Royalty> allRoyaltiesInGeneration = fullGenealogy [royalty.generation];
			allRoyaltiesInGeneration.Add(royalty);
		} else {
			fullGenealogy.Add (royalty.generation, new List<Royalty> (){ royalty });
		}
	}
	#endregion

	internal void CreateInitialHatred(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.royaltyList.allRoyalties.Count; j++){
				this.kingdoms [i].kingdom.royaltyList.allRoyalties [j].ChangeHatred ();
			}
		}
	}
	public void AddRelationshipToOtherKingdoms(KingdomTest newKingdom){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if (this.kingdoms[i].kingdom.id != newKingdom.id) {
				this.kingdoms[i].kingdom.relationshipKingdoms.Add (new RelationshipKingdoms(newKingdom, DECISION.NEUTRAL, 0));
			}
		}
	}

	public KingdomTest GetKingdomByName(string name){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if (this.kingdoms [i].kingdom.kingdomName == name) {
				return this.kingdoms[i].kingdom;
			}
		}
		return null;
	}

	internal void CreateInitialRelationshipsToKingdoms(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			this.kingdoms [i].kingdom.CreateInitialRelationshipsToKingdoms ();
		}
	}

	internal void TogglePause(){
		isDayPaused = !isDayPaused;
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CityGenerator : MonoBehaviour {

	public static CityGenerator Instance = null;

	public int cityCount;
	public int capitalCityCount;
	public float minCityDistance;

	public List<HexTile> tilesElligibleForCities;

	public List<HexTile> cities;
	public List<CityTile> capitalCities;
	public List<CityTile> pendingCityTiles;
	public List<Line> allLines;

	int latestIndex = 0;
	Line lastLine = null;
	Dictionary<BIOMES, int> elligibleBiomes;


	void Awake(){
		Instance = this;
	}

	/*
	 * Generate Cities then generate
	 * each cities' connections
	 * */
	public void GenerateCities(){
		cities = new List<HexTile>();
		allLines = new List<Line>();
		pendingCityTiles = new List<CityTile>();
		bool forTesting = false;

		if (forTesting) {
			SetTileAsCity(GridMap.Instance.listHexes[50].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[55].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[150].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[155].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[210].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[460].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[360].GetComponent<HexTile>());
			SetTileAsCity(GridMap.Instance.listHexes[660].GetComponent<HexTile>());

			ConnectCities(GridMap.Instance.listHexes[50].GetComponent<HexTile>(), GridMap.Instance.listHexes[55].GetComponent<HexTile>());
			ConnectCities(GridMap.Instance.listHexes[150].GetComponent<HexTile>(), GridMap.Instance.listHexes[155].GetComponent<HexTile>());
			ConnectCities(GridMap.Instance.listHexes[210].GetComponent<HexTile>(), GridMap.Instance.listHexes[460].GetComponent<HexTile>());
			ConnectCities(GridMap.Instance.listHexes[360].GetComponent<HexTile>(), GridMap.Instance.listHexes[660].GetComponent<HexTile>());

			for (int i = 0; i < allLines.Count; i++) {
				Line currentLine = allLines[i];
				Debug.Log ("===== LINE: " + currentLine.startPoint + "," + currentLine.endPoint + "=====");
				IsCollinear(currentLine.startPoint, currentLine.endPoint);
			}
//			SetCapitalCities();
//			pendingCityTiles.Add(capitalCities[0]);
//			GenerateCityConnections();
		} else {
			CheckForUnelligibleBiomes ();
			for (int i = 0; i < cityCount; i++) {
				BIOMES chosenBiome = RollForCityBiome();
				Debug.Log ("Chosen Biome-" + i + ": " + chosenBiome);
				if (chosenBiome == BIOMES.SNOW) {
					PlaceCityOnBiomeTile (Biomes.Instance.snowHexTiles);
				} else if (chosenBiome == BIOMES.TUNDRA) {
					PlaceCityOnBiomeTile (Biomes.Instance.tundraHexTiles);
				} else if (chosenBiome == BIOMES.GRASSLAND) {
					PlaceCityOnBiomeTile (Biomes.Instance.grasslandHexTiles);
				} else if (chosenBiome == BIOMES.WOODLAND) {
					PlaceCityOnBiomeTile (Biomes.Instance.woodsHexTiles);
				} else if (chosenBiome == BIOMES.FOREST) {
					PlaceCityOnBiomeTile (Biomes.Instance.forestHexTiles);
				} else if (chosenBiome == BIOMES.DESERT) {
					PlaceCityOnBiomeTile (Biomes.Instance.desertHexTiles);
				}
			}
			SetCapitalCities();
			pendingCityTiles.Add(capitalCities[0]);
			GenerateCityConnections();
		}

	}

	void GenerateRoads(){
		for (int i = 0; i < cities.Count; i++) {
			CityTile currentCityTile = pendingCityTiles[i];
			int randomNumberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();

//			cities[i]
		}
	}
		

	void GenerateCityConnections(){
		for (int i = latestIndex; i < pendingCityTiles.Count; i++) {
			CityTile currentCityTile = pendingCityTiles[i];
			HexTile currentHexTile = currentCityTile.cityAttributes.hexTile;
			int numberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();
			List<HexTile> listOrderedByDistance = currentCityTile.GetAllCitiesByDistance();
			Debug.Log ("======TILE: " + currentHexTile.name + "======");

			GenerateRoads(i, numberOfRoads, currentCityTile, listOrderedByDistance, currentHexTile);

			latestIndex = i;

			if (pendingCityTiles.Count != cityCount && (i + 1) == pendingCityTiles.Count) {
				Debug.Log ("MISSED SOME CITIES!");
				Debug.Log ("Create Lines for missed out cities");
				for (int x = 0; x < cities.Count; x++) {
					if (!pendingCityTiles.Contains (cities[x].GetCityTile())) {
						Debug.Log("======Missed out city: " + cities[x].name + " ======");

						HexTile possibleConnectionTile = FindNearestCityWithConnection(cities [x].GetCityTile());
						if (possibleConnectionTile != null) {
							ConnectCities (cities [x], possibleConnectionTile);
							if (!pendingCityTiles.Contains (cities [x].GetCityTile ())) {
								pendingCityTiles.Add (cities [x].GetCityTile ());
							}
							if (!pendingCityTiles.Contains (possibleConnectionTile.GetCityTile ())) {
								pendingCityTiles.Add (possibleConnectionTile.GetCityTile ());
							}
						}
					}

				}
			}
		}
		Debug.Log("PENDING CITY TILES: " + pendingCityTiles.Count);

		//Check for missed out cities
		Debug.Log("Missed Cities: " + (cities.Count - pendingCityTiles.Count).ToString());
	}

	public HexTile FindNearestCityWithConnection(CityTile tile){
		List<HexTile> cityTilesByDistance = tile.GetAllCitiesByDistance();
		for (int i = 0; i < cityTilesByDistance.Count; i++) {
			if (cityTilesByDistance[i].GetCityTile().cityAttributes.numOfRoads > 0) {
				if (i == cityTilesByDistance.Count) {
					Debug.LogError ("No more possible connectionssssss");
					break;
				}
				if (IsLineIntersecting (tile.transform.position, cityTilesByDistance [i].transform.position) ||
				    IsCollinear (tile.transform.position, cityTilesByDistance [i].transform.position)) {
					continue;
				} else {
					return cityTilesByDistance [i];
				}
			}
		}
		return null;
	}

	void GenerateRoads(int i, int numberOfRoads, CityTile currentCityTile, List<HexTile> listOrderedByDistance, HexTile currentHexTile){
		if (numberOfRoads > currentCityTile.cityAttributes.connectedCities.Count) { 
			/*
			 * if generated number of roads is greater than the current connected cities, add connections
			 * */
			numberOfRoads = numberOfRoads - currentCityTile.cityAttributes.connectedCities.Count;
			int roadsCreated = 0;
			Debug.Log ("Create " + numberOfRoads + " roads");
			for (int j = 0; roadsCreated < numberOfRoads; j++) {
				if (j >= listOrderedByDistance.Count) {
					Debug.Log ("No more possible connections");
					break;
				}
				if (currentCityTile.cityAttributes.connectedCities.Contains (listOrderedByDistance [j].GetCityTile ()) ||
					listOrderedByDistance [j].GetCityTile ().cityAttributes.connectedCities.Count >= 3) {
					Debug.Log ("Cannot connect: ");
					Debug.Log ("Because that city is already connected?: " + currentCityTile.cityAttributes.connectedCities.Contains (listOrderedByDistance [j].GetCityTile ()).ToString ());
					Debug.Log ("Because that city alread has 4 or more cities connected to it?: " + (listOrderedByDistance [j].GetCityTile ().cityAttributes.connectedCities.Count >= 4).ToString ());
					continue;
				} else {
					if (IsLineIntersecting (currentCityTile.transform.position, listOrderedByDistance[j].transform.position) ||
						IsCollinear (currentCityTile.transform.position, listOrderedByDistance[j].transform.position)) {
						Debug.Log ("Cannot connect: ");
						Debug.Log ("Because Line Intersects?: " + IsLineIntersecting (currentCityTile.transform.position, listOrderedByDistance [j].transform.position).ToString ());
						Debug.Log ("Because Line is Collinear?: " + IsCollinear(currentCityTile.transform.position, listOrderedByDistance[j].transform.position));
						continue;
					} else {
						ConnectCities(currentHexTile, listOrderedByDistance[j]);
						roadsCreated++;
						/*
						 * if List<CityTile> pendingCityTiles does not contain the 
						 * connected city, add it to the list
						 * */
						if (pendingCityTiles.Contains (listOrderedByDistance [j].GetCityTile ())) {
							if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
								if (numberOfRoads >= 4) {
									break;
								}
								numberOfRoads++;

							}
						} else {
							pendingCityTiles.Add (listOrderedByDistance [j].GetCityTile ());
						}
					}

				}
			}
			Debug.Log ("Created " + roadsCreated + " roads");
		} else if (numberOfRoads < currentCityTile.cityAttributes.connectedCities.Count) { 
			/*
				 * if generated number of roads is less than current connected cities, 
				 * set number of cities to be equal the number of connected cities
				 * */
			Debug.Log ("Set number of roads to current connected cities count");
			currentCityTile.cityAttributes.numOfRoads = currentCityTile.cityAttributes.connectedCities.Count;
			if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
				if (currentCityTile.cityAttributes.numOfRoads >= 4) {
					return;
				}
				currentCityTile.cityAttributes.numOfRoads++;
				i--;
			}
		} else if (numberOfRoads == currentCityTile.cityAttributes.connectedCities.Count) {
			Debug.Log ("Number of Roads is already met, not creating any more");
			if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
				if (currentCityTile.cityAttributes.numOfRoads >= 4) {
					return;
				}
				currentCityTile.cityAttributes.numOfRoads++;
				i--;
			}
		} else {
			Debug.Log ("Did not create anything, because meh");
		}

	}

	void ConnectCities(HexTile originTile, HexTile targetTile){
//		GLDebug.DrawLine (currentCityTile.transform.position, listOrderedByDistance [j].transform.position, Color.black, 10000f); //Draw Line Between 2 Cities
		Debug.Log ("Connected to: " + targetTile.name);
		GameObject lineGO = LineDrawer.Instance.DrawLine (originTile.transform, originTile.transform.position, targetTile.transform.position);
		lastLine = lineGO.GetComponent<Line> ();
		lineGO.SetActive (false);
		RefinePaths (originTile, targetTile);
		PathManager.Instance.DeterminePath (originTile.tile, targetTile.tile);
		allLines.Add (lineGO.GetComponent<Line> ());
		originTile.GetCityTile().cityAttributes.AddCityAsConnected (targetTile.GetCityTile ());
		targetTile.GetCityTile ().cityAttributes.AddCityAsConnected (originTile.GetCityTile());

		ResetPassableTiles();
	}

	public void RefinePaths(HexTile startTile, HexTile destinationTile){
		for (int i = 0; i < cities.Count; i++) {
			HexTile currentTile = cities[i];
			if (currentTile != startTile && currentTile != destinationTile) {
				for (int j = 0; j < currentTile.neighbours.Count; j++) {
					currentTile.neighbours [j].tile.canPass = false;
				}
			}
		}
//		List<Road> allRoads = PathManager.Instance.allRoads;
//		if (allRoads.Count > 0) {
//			for (int i = 0; i < allRoads.Count; i++) {
//				Road currentRoad = allRoads[i];
//				if (!currentRoad.connectingCities.Contains(startTile) && !currentRoad.connectingCities.Contains(destinationTile)) {
//					for (int j = 0; j < currentRoad.path.Length; j++) {
//						if (!currentRoad.path [j].hexTile.isCity) {
//							currentRoad.path [j].canPass = false;
//						}
//					}
//				}
//			}
//		}
	}


	void ResetPassableTiles(){
		List<GameObject> allHexes = GridMap.Instance.listHexes;
		for (int i = 0; i < allHexes.Count; i++) {
			HexTile currentHexTile = allHexes[i].GetComponent<HexTile>();
			if (!currentHexTile.isCity) {
				currentHexTile.tile.canPass = true;
			}
		}
	}

	bool IsLineIntersecting(Vector3 startPoint, Vector3 endPoint){
		for (int i = 0; i < allLines.Count; i++) {
			if(allLines[i].startPoint != startPoint && allLines[i].endPoint != endPoint){
				bool intersection = FasterLineSegmentIntersection (startPoint, endPoint, allLines[i].startPoint, allLines[i].endPoint);

				if (intersection == true) {
					return true;
				}
			}
		}
		return false;
	}

	bool IsCollinear(Vector3 startPoint, Vector3 endPoint){
		Vector2 A = startPoint;
		Vector2 B = endPoint;
		for (int i = 0; i < allLines.Count; i++) {
			Vector2 C = allLines[i].startPoint;
			Vector2 D = allLines [i].endPoint;
			if(allLines[i].startPoint != startPoint && allLines[i].endPoint != endPoint){
				float line1Slope = (A.y - B.y)/(A.x - B.x);
				float line2Slope = (C.y - D.y) /(C.x - D.x);
				if(line1Slope == line2Slope){
					if (PointOnLineSegment (A, B, C) || PointOnLineSegment (A, B, D) || 
						PointOnLineSegment (C, D, A) || PointOnLineSegment (C, D, B)) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool PointOnLineSegment(Vector2 pt1, Vector2 pt2, Vector2 pt, double epsilon = 0.001) {
		if (pt.x - Math.Max(pt1.x, pt2.x) > epsilon || 
			Math.Min(pt1.x, pt2.x) - pt.x > epsilon || 
			pt.y - Math.Max(pt1.y, pt2.y) > epsilon || 
			Math.Min(pt1.y, pt2.y) - pt.y > epsilon)
			return false;

		if (Math.Abs(pt2.x - pt1.x) < epsilon)
			return Math.Abs(pt1.x - pt.x) < epsilon || Math.Abs(pt2.x - pt.x) < epsilon;
		if (Math.Abs(pt2.y - pt1.y) < epsilon)
			return Math.Abs(pt1.y - pt.y) < epsilon || Math.Abs(pt2.y - pt.y) < epsilon;

		double x = pt1.x + (pt.y - pt1.y) * (pt2.x - pt1.x) / (pt2.y - pt1.y);
		double y = pt1.y + (pt.x - pt1.x) * (pt2.y - pt1.y) / (pt2.x - pt1.x);

		return Math.Abs(pt.x - x) < epsilon || Math.Abs(pt.y - y) < epsilon;
	}

	static bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {

		Vector2 a = p2 - p1;
		Vector2 b = p3 - p4;
		Vector2 c = p1 - p3;

		float alphaNumerator = b.y*c.x - b.x*c.y;
		float alphaDenominator = a.y*b.x - a.x*b.y;
		float betaNumerator  = a.x*c.y - a.y*c.x;
		float betaDenominator  = a.y*b.x - a.x*b.y;

		bool doIntersect = true;

		if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4) {
			return false;
		}

		if (alphaDenominator == 0 || betaDenominator == 0) {
			doIntersect = false;
		} else {

			if (alphaDenominator > 0) {
				if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
					doIntersect = false;

				}
			} else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
				doIntersect = false;
			}

			if (doIntersect && betaDenominator > 0) {
				if (betaNumerator < 0 || betaNumerator > betaDenominator) {
					doIntersect = false;
				}
			} else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
				doIntersect = false;
			}
		}

		return doIntersect;
	}

	/*
	 * Choose Capital Cities, they are the 4 cities
	 * with richnessLevel closest to 60
	 * */
	void SetCapitalCities(){
		int lastIndex = 0;
		capitalCities = new List<CityTile>();
		cities.Sort (delegate(HexTile a, HexTile b) {
			return (a.gameObject.GetComponent<CityTile>().cityAttributes.richnessLevel.CompareTo(b.gameObject.GetComponent<CityTile>().cityAttributes.richnessLevel));
		});

		for (int i = 0; i < cities.Count; i++) {
			CityTile currentCity = cities [i].gameObject.GetComponent<CityTile> ();
			int richnessLevel = currentCity.cityAttributes.richnessLevel;
			if (richnessLevel < 60) {
				continue;
			} else {
				lastIndex = i-1;
				break;
			}
		}

		for (int j = 0; j < capitalCityCount; j++) {
			int capitalCityIndex = lastIndex - j;
			cities[capitalCityIndex].GetComponent<CityTile>().cityAttributes.cityType = CITY_TYPE.CAPITAL;
			cities [capitalCityIndex].GetCityTile().cityAttributes.population = cities [capitalCityIndex].GetCityTile ().cityAttributes.GeneratePopulation ();
//			cities[capitalCityIndex].SetTileColor(Color.yellow);
			capitalCities.Add(cities[capitalCityIndex].GetComponent<CityTile>());
		}
	}

	/*
	 * Check if there is at least 1 tile from 
	 * each biome present, eliminate all biomes that don't have tiles
	 * from city selection
	 * */
	void CheckForUnelligibleBiomes(){
		elligibleBiomes = new Dictionary<BIOMES, int>();
		if(Biomes.Instance.snowHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.SNOW, 5);
		}
		if(Biomes.Instance.tundraHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.TUNDRA, 10);
		}
		if(Biomes.Instance.grasslandHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.GRASSLAND, 40);
		}
		if(Biomes.Instance.woodsHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.WOODLAND, 30);
		}
		if(Biomes.Instance.forestHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.FOREST, 10);
		}
		if(Biomes.Instance.desertHexTiles.Count > 0){
			elligibleBiomes.Add(BIOMES.DESERT, 5);
		}
	}


	/*
	 * Randomly choose which biome a 
	 * city will spawn in, current chances are the ff:
	 * Snow - 5% - 0~4
	 * Tundra - 10% - 5~14
	 * Grassland - 40% - 15~54
	 * Woods - 30% - 55~84
	 * Forest - 10% - 85~94
	 * Desert - 5% - 95~99
	 * */
	BIOMES RollForCityBiome(){
		//Get total percentage
		int total = 0;
		foreach (int value in elligibleBiomes.Values) {
			total += value;
		}
		int roll = UnityEngine.Random.Range (0, total);
		int lowerBound = 0;
		int upperBound = 0;

		foreach(KeyValuePair<BIOMES,int> keyValue in elligibleBiomes){
			BIOMES biome = keyValue.Key;
			int biomeChance = keyValue.Value;
			upperBound += biomeChance;
			if (roll >= lowerBound && roll < upperBound ) {
				return biome;
			} else {
				lowerBound += biomeChance;
			}
		}
		return BIOMES.GRASSLAND;
	}


	/*
	 * Randomly choose a tile from a specific
	 * biome, and mark it as a city
	 * */
	void PlaceCityOnBiomeTile(List<HexTile> biomeTiles){
		List<HexTile> cityPlacableBiomeTile = new List<HexTile>();

		if (tilesElligibleForCities.Count <= 0) {
			cityCount = cities.Count;
			Debug.LogError ("There are no more elligible tiles to put a city on!");
		}

		//check if this biome still has city placable tiles
		for (int i = 0; i < biomeTiles.Count; i++) {
			if (tilesElligibleForCities.Contains(biomeTiles[i])) {
				cityPlacableBiomeTile.Add (biomeTiles [i]);
			}
		}

		if (cityPlacableBiomeTile.Count <= 0) {
			//if this point is reached, it means no more tiles in the current biome are available, place a random one anywhere
			int randomTileIndex = UnityEngine.Random.Range (0, tilesElligibleForCities.Count);
			HexTile chosenTile = tilesElligibleForCities [randomTileIndex];
			SetTileAsCity (chosenTile);
			HexTile[] neighborTiles = chosenTile.GetTilesInRange (minCityDistance);
			for (int i = 0; i < neighborTiles.Length; i++) {
				if (tilesElligibleForCities.Contains (neighborTiles [i])) {
					//Remove all neighbor tiles from elligible tiles
					tilesElligibleForCities.Remove (neighborTiles [i]);
				}
			}
			tilesElligibleForCities.Remove (chosenTile);
			return;
		}

		bool choosingCity = true;
		while (choosingCity) {
			//Choose random tile from city placable biome tiles
			int randomTileIndex = UnityEngine.Random.Range (0, cityPlacableBiomeTile.Count);
			HexTile chosenTile = cityPlacableBiomeTile[randomTileIndex];
			Debug.Log ("Tiles Elligible For Cities: " + tilesElligibleForCities.Count);

			//check if chosen tile is elligible for city placement
			if (tilesElligibleForCities.Contains(chosenTile)) {
				choosingCity = false;
				SetTileAsCity (chosenTile);

				HexTile[] neighborTiles = chosenTile.GetTilesInRange (minCityDistance);
				for (int i = 0; i < neighborTiles.Length; i++) {
					if (tilesElligibleForCities.Contains (neighborTiles [i])) {
						//Remove all neighbor tiles from elligible tiles
						tilesElligibleForCities.Remove (neighborTiles [i]);
					}
				}
				tilesElligibleForCities.Remove (chosenTile);
			}
		}
	}

	void SetTileAsCity(HexTile tile){
		tile.SetTileColor(Color.black);
		tile.isCity = true;
		tile.gameObject.AddComponent<CityTile>();
		tile.gameObject.GetComponent<CityTile>().cityAttributes = new City(tile, tile.biomeType);
		cities.Add(tile);
	}

	/*
	 * Returns true if tile in question,
	 * is within 2 tiles of a city
	 * */
	bool IsTileNearCity(HexTile tile){
		//Change 1.5f to change radius of distance
		Collider2D[] nearHexes = Physics2D.OverlapCircleAll (new Vector2(tile.transform.position.x, tile.transform.position.y), minCityDistance);
		for (int i = 0; i < nearHexes.Length; i++) {
			nearHexes[i].gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
			if (nearHexes[i].gameObject.GetComponent<HexTile>().isCity) {
				return true;
			}
		}
		return false;
	}
}

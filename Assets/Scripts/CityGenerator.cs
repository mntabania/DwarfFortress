using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityGenerator : MonoBehaviour {

	public static CityGenerator Instance = null;

	public int cityCount;
	public int capitalCityCount;
	public float minCityDistance;

	[HideInInspector]
	public List<HexTile> tilesElligibleForCities;

	public List<HexTile> cities;
	public List<CityTile> capitalCities;
	public List<CityTile> pendingCityTiles;
	public List<Line> allLines;

	int latestIndex = 0;

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
		CheckForUnelligibleBiomes ();
		for (int i = 0; i < cityCount; i++) {
			BIOMES chosenBiome = RollForCityBiome();
			//Debug.Log ("Chosen Biome-" + i + ": " + chosenBiome);
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
		allLines = new List<Line> ();
		pendingCityTiles = new List<CityTile>();
		pendingCityTiles.Add(capitalCities[0]);
		GenerateCityConnections();
	}
		

	void GenerateCityConnections(){
		Line lastLine = null;
		for (int i = latestIndex; i < pendingCityTiles.Count; i++) {
			CityTile currentCityTile = pendingCityTiles[i];
			HexTile currentHexTile = currentCityTile.cityAttributes.hexTile;
			int numberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();
			List<HexTile> listOrderedByDistance = currentCityTile.GetAllCitiesByDistance();
//			Debug.Log ("======TILE: " + currentHexTile.name + "======");

			if (numberOfRoads > currentCityTile.cityAttributes.connectedCities.Count) { 
				/*
				 * if generated number of roads is greater than the current connected cities, add connections
				 * */
				numberOfRoads = numberOfRoads - currentCityTile.cityAttributes.connectedCities.Count;
				int roadsCreated = 0;
//				Debug.Log ("Create " + numberOfRoads + " roads");
				for (int j = 0; roadsCreated < numberOfRoads; j++) {
//					Debug.Log (j);
					if (j >= listOrderedByDistance.Count) {
//						Debug.Log ("No more possible connections");
						break;
					}
					if (currentCityTile.cityAttributes.connectedCities.Contains (listOrderedByDistance [j].GetCityTile ()) ||
						listOrderedByDistance [j].GetCityTile ().cityAttributes.connectedCities.Count >= 3) {
//						Debug.Log ("Cannot connect: ");
//						Debug.Log ("Because that city is already connected?: " + currentCityTile.cityAttributes.connectedCities.Contains (listOrderedByDistance [j].GetCityTile ()).ToString ());
//						Debug.Log ("Because that city alread has 4 or more cities connected to it?: " + (listOrderedByDistance [j].GetCityTile ().cityAttributes.connectedCities.Count >= 4).ToString ());
						continue;
					} else {
						if (IsLineIntersecting (currentCityTile.transform.position, listOrderedByDistance [j].transform.position)) {
//							Debug.Log ("Cannot connect: ");
//							Debug.Log ("Because Line Intersects?: " + IsLineIntersecting (currentCityTile.transform.position, listOrderedByDistance [j].transform.position).ToString ());
							continue;
						} else {
							IsCollinear (currentCityTile.transform.position, listOrderedByDistance [j].transform.position);
//							GLDebug.DrawLine (currentCityTile.transform.position, listOrderedByDistance [j].transform.position, Color.black, 10000f); //Draw Line Between 2 Cities
							GameObject lineGO = LineDrawer.Instance.DrawLine (currentCityTile.transform, currentCityTile.transform.position, listOrderedByDistance [j].transform.position);
							lastLine = lineGO.GetComponent<Line> ();
							allLines.Add (lineGO.GetComponent<Line> ());
							currentCityTile.cityAttributes.AddCityAsConnected (listOrderedByDistance [j].GetCityTile ());
							listOrderedByDistance [j].GetCityTile ().cityAttributes.AddCityAsConnected (currentCityTile);
//							Debug.Log ("Connected to: " + listOrderedByDistance [j].name);
							roadsCreated++;
							/*
							 * if List<CityTile> pendingCityTiles does not contain the 
							 * connected city, add it to the list
							 * */
							if (pendingCityTiles.Contains (listOrderedByDistance [j].GetCityTile ())) {
								if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
									numberOfRoads++;
								}
							} else {
								pendingCityTiles.Add (listOrderedByDistance [j].GetCityTile ());
							}
						}

					}
				}
//				Debug.Log ("Created " + roadsCreated + " roads");
			} else if (numberOfRoads < currentCityTile.cityAttributes.connectedCities.Count) { 
				/*
				 * if generated number of roads is less than current connected cities, 
				 * set number of cities to be equal the number of connected cities
				 * */
//				Debug.Log ("Set number of roads to current connected cities count");
				currentCityTile.cityAttributes.numOfRoads = currentCityTile.cityAttributes.connectedCities.Count;
				if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
					currentCityTile.cityAttributes.numOfRoads++;
					i--;
				}
			} else if (numberOfRoads == currentCityTile.cityAttributes.connectedCities.Count) {
//				Debug.Log ("Number of Roads is already met, not creating any more");
				if (pendingCityTiles.Count != cityCount && (i+1) == pendingCityTiles.Count) {
					currentCityTile.cityAttributes.numOfRoads++;
					i--;
				}
			} else {
//				Debug.Log ("Did not create anything, because meh");
			}

			latestIndex = i;
			if (pendingCityTiles.Count != cityCount && (i + 1) == pendingCityTiles.Count) {
//				Debug.Log ("Create Lines for missed out cities");
				for (int x = 0; x < cities.Count; x++) {
					if (!pendingCityTiles.Contains (cities [x].GetCityTile())) {
//						Debug.Log("======Missed out city: " + cities[x].name + " ======");
						HexTile possibleConnectionTile = cities [x].GetCityTile ().FindNearestCityWithConnection();
//						Debug.Log ("Possible Connection: " + possibleConnectionTile.name);
//						if (IsLineIntersecting (currentCityTile.transform.position, possibleConnectionTile.transform.position)) {
//							possibleConnectionTile = cities [x].GetCityTile ().cityTilesByDistance [0];
//						}
						GameObject lineGO = LineDrawer.Instance.DrawLine (cities[x].transform, cities[x].transform.position, possibleConnectionTile.transform.position);
						lastLine = lineGO.GetComponent<Line> ();
						allLines.Add (lineGO.GetComponent<Line> ());
						cities[x].GetCityTile().cityAttributes.AddCityAsConnected (possibleConnectionTile.GetCityTile ());
						possibleConnectionTile.GetCityTile ().cityAttributes.AddCityAsConnected (cities[x].GetCityTile());
						if (!pendingCityTiles.Contains (cities[x].GetCityTile ())) {
							pendingCityTiles.Add (cities[x].GetCityTile ());
						}
						if (!pendingCityTiles.Contains (possibleConnectionTile.GetCityTile ())) {
							pendingCityTiles.Add (possibleConnectionTile.GetCityTile ());
						}
					}

				}
			}


		}
//		Debug.Log("PENDING CITY TILES: " + pendingCityTiles.Count);

		//Check for missed out cities
//		Debug.Log("Missed Cities: " + (cities.Count - pendingCityTiles.Count).ToString());

//		if (pendingCityTiles.Count < cityCount) {
//			CityTile latestTile = pendingCityTiles.Last();
//			List<HexTile> listOrderedByDistance = latestTile.GetAllCitiesByDistance();
//			for (int x = 0; x < listOrderedByDistance.Count; x++) {
//				if (listOrderedByDistance[x].GetCityTile().cityAttributes.connectedCities.Count <= 0) {
//					if (!IsLineIntersecting (latestTile.transform.position, listOrderedByDistance [x].transform.position)) {
//						GameObject lineGO = LineDrawer.Instance.DrawLine (latestTile.transform, latestTile.transform.position, listOrderedByDistance [x].transform.position);
//						lastLine = lineGO.GetComponent<Line> ();
//						allLines.Add (lineGO.GetComponent<Line> ());
//						latestTile.cityAttributes.AddCityAsConnected (listOrderedByDistance [0].GetCityTile ());
//						listOrderedByDistance [x].GetCityTile ().cityAttributes.AddCityAsConnected (latestTile);
//						pendingCityTiles.Add (listOrderedByDistance [x].GetCityTile());
//						Debug.Log ("Connected to: " + listOrderedByDistance [0].name);
//						GenerateCityConnections();
//						break;
//					}
//				}
//			}
//		}
	}

	void RegenerateLines(){
		for (int i = 0; i < allLines.Count; i++) {
			Destroy (allLines [i].gameObject);
		}
		latestIndex = 0;
		GenerateCityConnections();
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

//	bool IsCollinear(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2){
//		bool condition1 = (((s1.x - e1.x) * (s2.y - e2.y)) - ((s2.x - e2.x) * (s1.y - e1.y))) == 0;
//		bool condition2 = (((s1.x - s2.x)*(e1.y - e2.y)) - ((e1.x - e2.x)* (s1.y - s2.y))) == 0;
//		bool condition3 = (s1.x - e2.x)
//	}

	bool IsCollinear(Vector3 startPoint, Vector3 endPoint){
//		Vector2 A, Vector2 B, Vector2 C, Vector2 D
		Vector2 A = startPoint;
		Vector2 B = endPoint;
		for (int i = 0; i < allLines.Count; i++) {
			Vector2 C = allLines[i].startPoint;
			Vector2 D = allLines [i].endPoint;
			if(allLines[i].startPoint != startPoint && allLines[i].endPoint != endPoint){
				Vector2 CmP = new Vector2(C.x - A.x, C.y - A.y);
				Vector2 r = new Vector2(B.x - A.x, B.y - A.y);
				Vector2 s = new Vector2(D.x - C.x, D.y - C.y);

				float CmPxr = CmP.x * r.y - CmP.y * r.x;
				float CmPxs = CmP.x * s.y - CmP.y * s.x;
				float rxs = r.x * s.y - r.y * s.x;

				if (CmPxr == 0f) {
					// Lines are collinear, and so intersect if they have any overlap

					bool collinear = ((C.x - A.x < 0f) != (C.x - B.x < 0f))
						|| ((C.y - A.y < 0f) != (C.y - B.y < 0f));
					Debug.Log (startPoint + "," + endPoint + "," + allLines [i].startPoint + "," + allLines [i].endPoint);
					if (collinear) {
						Debug.Log ("Lines are collinear");
					} else {
						Debug.Log ("Lines are not collinear");
					}
					return collinear;
				}

				if (rxs == 0f) {
//					Debug.Log ("Lines are parallel");
					return false; // Lines are parallel.
				}
			}
		}
		return false;
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




	// Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
	// intersect the intersection point may be stored in the floats i_x and i_y.
	int get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y, 
		float p2_x, float p2_y, float p3_x, float p3_y)
	{
		float s1_x, s1_y, s2_x, s2_y;
		s1_x = p1_x - p0_x;     s1_y = p1_y - p0_y;
		s2_x = p3_x - p2_x;     s2_y = p3_y - p2_y;

		float s, t;
		s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
		t = ( s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

		if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
		{
			// Collision detected
			Debug.Log("Collision Detected");
			return 1;
		}
		Debug.Log("No Collision");
		return 0; // No collision
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
			//Debug.Log ("City: " + cities [i].gameObject.name + " richness: " + cities[i].gameObject.GetComponent<CityTile>().cityAttributes.richnessLevel.ToString());
			CityTile currentCity = cities [i].gameObject.GetComponent<CityTile> ();
			int richnessLevel = currentCity.cityAttributes.richnessLevel;
			if (richnessLevel < 60) {
				continue;
			} else {
				lastIndex = i-1;
				//Debug.Log ("Last Index: " + lastIndex);
				break;
			}
		}

		for (int j = 0; j < capitalCityCount; j++) {
			int capitalCityIndex = lastIndex - j;
			//Debug.Log ("Capital City is: " + cities [capitalCityIndex].gameObject.name + " richness: " + cities [capitalCityIndex].gameObject.GetComponent<CityTile> ().cityAttributes.richnessLevel.ToString ());
			cities[capitalCityIndex].GetComponent<CityTile>().cityAttributes.cityType = CITY_TYPE.CAPITAL;
			cities [capitalCityIndex].GetCityTile().cityAttributes.population = cities [capitalCityIndex].GetCityTile ().cityAttributes.GeneratePopulation ();
			cities[capitalCityIndex].SetTileColor(Color.yellow);
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
		int roll = Random.Range (0, total);
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
		bool choosingCity = true;

		while (choosingCity) {
			int randomTileIndex = Random.Range (0, biomeTiles.Count);
			HexTile chosenTile = biomeTiles [randomTileIndex];

			if (tilesElligibleForCities.Contains (chosenTile)) {
				choosingCity = false;
				chosenTile.SetTileColor(Color.black);
				chosenTile.isCity = true;
				chosenTile.gameObject.AddComponent<CityTile>();
				chosenTile.gameObject.GetComponent<CityTile>().cityAttributes = new City(chosenTile, chosenTile.biomeType);
				cities.Add(chosenTile);

				HexTile[] neighborTiles = chosenTile.GetTilesInRange(minCityDistance);
				for (int i = 0; i < neighborTiles.Length; i++) {
					if (tilesElligibleForCities.Contains (neighborTiles [i])) {
						tilesElligibleForCities.Remove (neighborTiles [i]);
					}
				}
				tilesElligibleForCities.Remove(chosenTile);
			}
		}
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

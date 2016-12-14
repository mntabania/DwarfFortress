using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		GenerateCityConnections();
	}

	void GenerateCityConnections(){
		pendingCityTiles = new List<CityTile>();
		pendingCityTiles.Add(capitalCities[0]);
		for (int i = 0; i < pendingCityTiles.Count; i++) {
			
			CityTile currentCityTile = pendingCityTiles[i];
			HexTile currentHexTile = currentCityTile.cityAttributes.hexTile;
			int numberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();
			List<HexTile> listOrderedByDistance = currentCityTile.GetAllCitiesByDistance();

			if (numberOfRoads > currentCityTile.cityAttributes.connectedCities.Count) { 
				/*
				 * if generated number of roads is greater than the current connected cities, add connections
				 * */
				numberOfRoads = numberOfRoads - currentCityTile.cityAttributes.connectedCities.Count;

				for (int j = 0; j < numberOfRoads; j++) {
					if (currentCityTile.cityAttributes.connectedCities.Contains(listOrderedByDistance[j].GetCityTile()) ||
						listOrderedByDistance[j].GetCityTile().cityAttributes.connectedCities.Count == 3) {
						listOrderedByDistance.RemoveAt(j);
						j--;
						continue;
					} else {
						GLDebug.DrawLine (currentCityTile.transform.position, listOrderedByDistance [j].transform.position, Color.black, 10000f); //Draw Line Between 2 Cities
						currentCityTile.cityAttributes.AddCityAsConnected (listOrderedByDistance [j].GetCityTile ());
						listOrderedByDistance [j].GetCityTile ().cityAttributes.AddCityAsConnected (currentCityTile);
						/*
						 * if List<CityTile> pendingCityTiles does not contain the 
						 * connected city, add it to the list
						 * */
						if (!pendingCityTiles.Contains (listOrderedByDistance [j].GetCityTile ())) {
							pendingCityTiles.Add (listOrderedByDistance [j].GetCityTile ());
						}
					}
				}
			} else if (numberOfRoads < currentCityTile.cityAttributes.connectedCities.Count) { 
				/*
				 * if generated number of roads is less than current connected cities, 
				 * set number of cities to be equal the number of connected cities
				 * */
				currentCityTile.cityAttributes.numOfRoads = currentCityTile.cityAttributes.connectedCities.Count;
			}
		}
		Debug.Log("PENDING CITY TILES: " + pendingCityTiles.Count);
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour {

	public static CityGenerator Instance = null;

	public int intCityCount;
	public float fltMinCityDistance;

	[HideInInspector]
	public List<HexTile> tilesElligibleForCities;

	public List<HexTile> cities;
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
		for (int i = 0; i < intCityCount; i++) {
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
		GenerateCityConnections();
	}

	void GenerateCityConnections(){
		int randomCityIndex = Random.Range(0, cities.Count);
		HexTile currentCity = cities [randomCityIndex];
		HexTile nextCity;
		CityTile cityTile = currentCity.gameObject.GetComponent<CityTile>();

		int linesRandomizer = Random.Range (0, 101);
		int numberOfLines = 0;
		if (linesRandomizer >= 0 && linesRandomizer < 11) {
			numberOfLines = 1;
		} else if (linesRandomizer >= 11 && linesRandomizer < 81) {
			numberOfLines = 2;
		} else if (linesRandomizer >= 81 && linesRandomizer < 101) {
			numberOfLines = 3;
		}

		cityTile.cityAttributes.numOfRoads = numberOfLines;

		for (int i = 0; i < numberOfLines; i++) {
			nextCity = cities [Random.Range(0, cities.Count)];
			if (nextCity != currentCity) {
				GLDebug.DrawLine (currentCity.transform.position, nextCity.transform.position, Color.black, 1000f);
				cityTile.cityAttributes.connectedCities.Add(nextCity.gameObject.GetComponent<CityTile>());
			} else {
				i--;
			}
		}


//		GLDebug.DrawLine (cities [0].transform.position, cities [1].transform.position, Color.black, 10f);
//		GLDebug.DrawLine (cities [1].transform.position, cities [2].transform.position, Color.black, 10f);
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
	 * Desert - 5% - 95~100
	 * */
	BIOMES RollForCityBiome(){
		//Get total percentage
		int total = 0;
		foreach (int value in elligibleBiomes.Values) {
			total += value;
		}
		int roll = Random.Range (0, (total+1));
		int lowerBound = 0;
		int upperBound = 0;

		foreach(KeyValuePair<BIOMES,int> keyValue in elligibleBiomes){
			BIOMES biome = keyValue.Key;
			int biomeChance = keyValue.Value;
			upperBound += biomeChance;
			if (roll >= lowerBound && roll < (upperBound + 1)) {
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
				chosenTile.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
				chosenTile.isCity = true;
				chosenTile.gameObject.AddComponent<CityTile>();
				chosenTile.gameObject.GetComponent<CityTile>().cityAttributes = new City(chosenTile, chosenTile.biomeType);
				cities.Add(chosenTile);

				HexTile[] neighborTiles = GetTilesInRange (chosenTile, fltMinCityDistance);
				for (int i = 0; i < neighborTiles.Length; i++) {
					if (tilesElligibleForCities.Contains (neighborTiles [i])) {
						tilesElligibleForCities.Remove (neighborTiles [i]);
					}
				}
			}
		}
	}	 

	/*
	 * Returns true if tile in question,
	 * is within 2 tiles of a city
	 * */
	bool IsTileNearCity(HexTile tile){
		//Change 1.5f to change radius of distance
		Collider2D[] nearHexes = Physics2D.OverlapCircleAll (new Vector2(tile.transform.position.x, tile.transform.position.y), fltMinCityDistance);
		for (int i = 0; i < nearHexes.Length; i++) {
			nearHexes[i].gameObject.GetComponent<SpriteRenderer> ().color = Color.yellow;
			if (nearHexes[i].gameObject.GetComponent<HexTile> ().isCity) {
				return true;
			}
		}
		return false;
	}

	/*
	 * Returns all Hex tiles within a radius
	 * */
	HexTile[] GetTilesInRange(HexTile centerTile, float radius){
		Collider2D[] nearHexes = Physics2D.OverlapCircleAll (new Vector2(centerTile.transform.position.x, centerTile.transform.position.y), radius);
		HexTile[] nearTiles = new HexTile[nearHexes.Length];
		for (int i = 0; i < nearTiles.Length; i++) {
			nearTiles [i] = nearHexes [i].gameObject.GetComponent<HexTile> ();
		}
		return nearTiles;
	}





}
